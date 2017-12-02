namespace ObscureWare.TestTools.TestContext
{
    using System;

    public class TestDisposingException : ApplicationException
    {
        public TestDisposingException(string testName, Exception exception) : base(testName, exception)
        {

        }
    }
}