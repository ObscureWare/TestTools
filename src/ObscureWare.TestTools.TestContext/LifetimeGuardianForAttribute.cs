namespace ObscureWare.TestTools.TestContext
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Denotes lifetime guardian class for specific type
    /// </summary>
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class LifetimeGuardianForAttribute : Attribute
    {
        /// <summary>
        /// The type this Wrapper is dedicated for. No inheritance supported.
        /// </summary>
        public Type WrappedType { get; private set; }

        public LifetimeGuardianForAttribute(Type wrappedType)
        {
            this.WrappedType = wrappedType;
        }
    }
}