using System.Text;

namespace FortLibrary.Encoders
{
    public class Generate
    {
        private static Random random = new Random();

        /// <summary>
        /// Random generated string
        /// </summary>
        /// <param name="length">The length of the string.</param>
        /// <returns>The random string.</returns>
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder randomString = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                randomString.Append(chars[random.Next(chars.Length)]);
            }
            return randomString.ToString();
        }
    }
}
