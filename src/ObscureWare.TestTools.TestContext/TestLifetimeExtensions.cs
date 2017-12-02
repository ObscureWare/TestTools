namespace ObscureWare.TestTools.TestContext
{
    using Conditions;

    using JetBrains.Annotations;

    public static class TestLifetimeExtensions
    {
        /// <summary>
        /// Registers object to be released with defined strategy when test is finished
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="testLifetimeKeeper"></param>
        /// <returns></returns>
        [PublicAPI]
        public static T HoldLifetimeDuringTest<T>(this T obj, TestLifetimeContext testLifetimeKeeper) where T : class
        {
            obj.Requires(nameof(obj)).IsNotNull();
            testLifetimeKeeper.Requires(nameof(testLifetimeKeeper)).IsNotNull();

            testLifetimeKeeper.RegisterLifetimeObject(obj, null);
            return obj;
        }

        /// <summary>
        /// Registers object to be released with defined strategy and given parent object when test is finished 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TParent"></typeparam>
        /// <param name="obj"></param>
        /// <param name="testLifetimeKeeper"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        [PublicAPI]
        public static T HoldLifetimeDuringTestWith<T, TParent>(this T obj, TestLifetimeContext testLifetimeKeeper, TParent parent) where T : class
        {
            obj.Requires(nameof(obj)).IsNotNull();
            testLifetimeKeeper.Requires(nameof(testLifetimeKeeper)).IsNotNull();

            testLifetimeKeeper.RegisterLifetimeObject(obj, parent);
            return obj;
        }
    }
}
