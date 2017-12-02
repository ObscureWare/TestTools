namespace ObscureWare.TestTools.TestContext
{
    /// <summary>
    /// Mode at which given test is being run
    /// </summary>
    public enum TestMode
    {
        /// <summary>
        /// Will erase all objects registered during test using IDisposable calls and / or Guardian classes
        /// </summary>
        Clean,

        /// <summary>
        /// Will not erase registered objects (However, this cannot prevent any custom IDisposable operations)
        /// </summary>
        LeaveDirty
    }
}