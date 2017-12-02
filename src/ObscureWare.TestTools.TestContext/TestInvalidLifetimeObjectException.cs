namespace ObscureWare.TestTools.TestContext
{
    using System;

    internal class TestInvalidLifetimeObjectException : ApplicationException
    {
        public TestInvalidLifetimeObjectException(string message) : base(message)
        {

        }
    }
}