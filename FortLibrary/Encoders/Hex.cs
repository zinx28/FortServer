using System.Security.Cryptography;
using System.Text;

namespace FortLibrary.Encoders
{
    public class Hex
    {
        /// <summary>
        /// Generates a random hex string with a given length
        /// </summary>
        /// <param name="length">The given length of the string</param>
        /// <returns>A 4andom byte converted to a string without '-'</returns>
        public static string GenerateRandomHexString(int length)
        {
            byte[] RandomBytes = new byte[length / 2];
            new Random().NextBytes(RandomBytes);
            return BitConverter.ToString(RandomBytes).Replace("-", string.Empty);
        }

        /// <summary>
        /// Converts a string to a hexadecimal hash using SHA-1
        /// </summary>
        /// <param name="hexString">The string to hash.</param>
        /// <returns>A hexadecimal SHA-1 hash of the input string</returns>
        public static string ConvStringToSHA1(string hexString)
        {
            byte[] data = Encoding.UTF8.GetBytes(hexString);
            string hexHash = "";
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(data);
                hexHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
            return hexHash;
        }

        /// <summary>
        /// Converts a string to a hexadecimal hash using SHA-256
        /// </summary>
        /// <param name="hexString">The string to hash.</param>
        /// <returns>A hexadecimal SHA-256 hash of the input string</returns>
        public static string ConvStringToSHA256(string hexString)
        {
            byte[] data = Encoding.UTF8.GetBytes(hexString);
            string hexHash = "";
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(data);
                hexHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
            return hexHash;
        }
    }
}
