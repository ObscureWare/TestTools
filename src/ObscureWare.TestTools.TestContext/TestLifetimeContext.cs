namespace ObscureWare.TestTools.TestContext
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Files;
    using JetBrains.Annotations;

    /// <summary>
    /// Used to keep track of disposable objects and resources during test lifetime
    /// </summary>
    [PublicAPI]
    public class TestLifetimeContext : IDisposable
    {
        private readonly TestMode _mode;
        private readonly string _testName;
        private readonly Stack<IDisposable> _stackedDisposables = new Stack<IDisposable>();
        private bool _disposed = false;

        private static readonly Dictionary<Type, Type> KnownTestWrapperTypes = new Dictionary<Type, Type>();
        private static readonly List<Assembly> AlreadyScannedAssemblies = new List<Assembly>();
        

        public TestLifetimeContext(TestMode mode = TestMode.Clean, string testName = null)
        {
            if (string.IsNullOrWhiteSpace(testName))
            {
                testName = this.BuildtestName();
            }

            this._mode = mode;
            this._testName = testName;
        }

        /// <summary>
        /// Custom or automatic name of the test
        /// </summary>
        public string TestName => this._testName;

        /// <summary>
        /// Selected testing mode
        /// </summary>
        public TestMode Mode => this._mode;


        private string BuildtestName()
        {
            // TODO: use stack trace to find it - 2 steps back
            return TestExtensions.AlphanumericIdentifier.BuildRandomString(20);
        }

        internal void RegisterLifetimeObject(object o, object parent)
        {
            if (o == null) throw
                new ArgumentNullException(nameof(o));

            if (this._mode == TestMode.LeaveDirty)
                return; // just do not track objects for cleanup... They will just remain - until erased otherwise...

            Type t = o.GetType();
            IDisposable wrapperObject = this.GetWrapperFor(t, o, parent);
            if (wrapperObject == null)
            {
                if (o is IDisposable disposable)
                {
                    wrapperObject = disposable;
                }
                else
                {
                    throw new TestInvalidLifetimeObjectException($"Type {o.GetType().FullName} neither has dedicated LifetimeWrapper nor is IDisposable itself.");
                }
            }
            this._stackedDisposables.Push(wrapperObject);
        }

        private IDisposable GetWrapperFor(Type type, object obj, object parent)
        {
            Type dedicatedWrapeprType = this.GetWrapperTypeFor(type);
            if (dedicatedWrapeprType != null)
            {
                return this.TryInstatianteWrapper(dedicatedWrapeprType, obj, parent);
            }
            else
            {
                return null;
            }
        }

        private IDisposable TryInstatianteWrapper(Type dedicatedWrapeprType, object o, object parent)
        {
            if (parent != null)
            {
                var twoParamsCtor = dedicatedWrapeprType.GetConstructor(new Type[] { o.GetType(), parent.GetType() });
                if (twoParamsCtor != null)
                {
                    return (IDisposable)Activator.CreateInstance(dedicatedWrapeprType, new object[] { o, parent });
                }

                throw new TestInvalidLifetimeObjectException(
                    $"LifetimeWrapper object {dedicatedWrapeprType.FullName} for type {o.GetType().FullName} does not support Parent parameter.");
            }

            var onParamCtor = dedicatedWrapeprType.GetConstructor(new Type[] { o.GetType() });
            if (onParamCtor != null)
            {
                return (IDisposable)Activator.CreateInstance(dedicatedWrapeprType, new object[] { o });
            }

            throw new TestInvalidLifetimeObjectException(
                $"LifetimeWrapper object {dedicatedWrapeprType.FullName} for type {o.GetType().FullName} does not exposes proper public constructor.");
        }

        private Type GetWrapperTypeFor(Type type)
        {
            if (!KnownTestWrapperTypes.Any())
            {
                this.RegisterKnownWrappers();
            }

            Type t = null;
            KnownTestWrapperTypes.TryGetValue(type, out t);

            return t ?? (t = this.TryIndividualWrapper(type));
        }

        /// <summary>
        /// This will check assembly of the type - if wasn't yet scanned
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Type TryIndividualWrapper(Type type)
        {
            if (AlreadyScannedAssemblies.Contains(type.Assembly))
            {
                return null;
            }

            this.ScanAssemblyForWrappers(type.Assembly);

            Type t = null;
            KnownTestWrapperTypes.TryGetValue(type, out t);
            return t;
        }

        /// <summary>
        /// This will read wrappers from all assemblies already loaded into AppDomain
        /// </summary>
        private void RegisterKnownWrappers()
        {
            lock (KnownTestWrapperTypes)
            {
                if (!KnownTestWrapperTypes.Any())
                {
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        this.ScanAssemblyForWrappers(assembly);
                    }
                }
            }
        }

        private void ScanAssemblyForWrappers(Assembly assembly)
        {
            foreach (var t in assembly.GetTypes())
            {
                var attrib = t.GetCustomAttributes(typeof(LifetimeGuardianForAttribute), inherit: true).SingleOrDefault();
                if (attrib != null)
                {
                    KnownTestWrapperTypes.Add(((LifetimeGuardianForAttribute)attrib).WrappedType, t);
                }
            }

            AlreadyScannedAssemblies.Add(assembly);
        }

        ~TestLifetimeContext()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            if (!this._disposed)
            {
                while (this._stackedDisposables.Any())
                {
                    var releaseObj = this._stackedDisposables.Pop();

                    try
                    {
                        releaseObj.Dispose();
                    }
                    catch (Exception ex)
                    {
                        throw new TestDisposingException(this.TestName, ex);
                    }
                }

                this._disposed = true;
            }
        }
    }
}