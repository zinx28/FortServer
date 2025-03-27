using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.Encoders
{
    public class RandomHash
    {
        /// <summary>
        /// Calculates the MD5 hash of a given string.
        /// </summary>
        /// <param name="input">The input string to hash.</param>
        /// <returns>The MD5 hash as a hexadecimal string.</returns>
        public static string CalculateMd5Hash(string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
