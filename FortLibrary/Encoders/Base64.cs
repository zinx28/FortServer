namespace FortLibrary.Encoders
{
    public class Base64
    {

        /// <summary>
        /// Generates a random string
        /// </summary>
        /// <param name="length">The given legth of the string</param>
        /// <returns>The random generated string</returns>
        public static string GenerateRandomString(int length)
        {
            byte[] RandomBytes = new byte[length];
            new Random().NextBytes(RandomBytes);
            return Convert.ToBase64String(RandomBytes);
        }
    }
}
