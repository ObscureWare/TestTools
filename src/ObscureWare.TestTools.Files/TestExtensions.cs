namespace ObscureWare.TestTools.Files
{
    using System;

    /// <summary>
    /// Some handy naming generation extension methods. For more complex solution look for "Bogus" Nugget. Or something I'm about yet to create in planned future.
    /// </summary>
    public static class TestExtensions
    {
        private static readonly Random Rnd = new Random();

        /// <summary>
        /// Builds string of required length concatenating random characters from given string.
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string BuildRandomString(this string sourceString, uint length)
        {
            char[] array = new char[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = sourceString[Rnd.Next(0, sourceString.Length)];
            }

            return new string(array);
        }

        public static string Numeric => @"0123456789";

        public static string UpperAlpha => @"ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string LowerAlpha => @"abcdefghijklmnopqrstuvwxyz";

        public static string UpperAlphanumeric => UpperAlpha + Numeric;

        public static string LowerAlphanumeric => LowerAlpha + Numeric;

        public static string MixedAlphanumeric => UpperAlphanumeric + LowerAlphanumeric;

        public static string AlphanumericIdentifier => UpperAlphanumeric + LowerAlphanumeric + @"______"; // increased probability ;-)
    }
}