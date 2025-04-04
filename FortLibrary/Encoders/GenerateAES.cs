using System.Security.Cryptography;
using System.Text;

namespace FortLibrary.Encoders
{
    public class GenerateAES
    {
        /// <summary>
        /// Encrypts a string using AES-256 with CBC mode and PKCS7 padding.
        /// </summary>
        /// <param name="input">The string to encrypt.</param>
        /// <param name="key">The encryption key.</param>
        /// <returns>A base64-encoded string of the encrypted data.</returns>
        public static string EncryptAES256(string input, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            using (SHA256 sha256 = SHA256.Create())
            {
                keyBytes = sha256.ComputeHash(keyBytes);
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                aes.GenerateIV();

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] encryptedData = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

                byte[] combinedData = new byte[aes.IV.Length + encryptedData.Length];
                Array.Copy(aes.IV, 0, combinedData, 0, aes.IV.Length);
                Array.Copy(encryptedData, 0, combinedData, aes.IV.Length, encryptedData.Length);

                string encryptedBase64 = Convert.ToBase64String(combinedData);
                return encryptedBase64;
            }
        }

        /// <summary>
        /// Decrypts a base64-encoded string using AES-256 with CBC mode and PKCS7 padding.
        /// </summary>
        /// <param name="encryptedBase64">The base64-encoded string to decrypt.</param>
        /// <param name="key">The encryption key.</param>
        /// <returns>The decrypted string.</returns>
        public static string DecryptAES256(string encryptedBase64, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            using (SHA256 sha256 = SHA256.Create())
            {
                keyBytes = sha256.ComputeHash(keyBytes);
            }

            byte[] encryptedData = Convert.FromBase64String(encryptedBase64);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] iv = new byte[aes.BlockSize / 8];
                Array.Copy(encryptedData, 0, iv, 0, iv.Length);
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedData, iv.Length, encryptedData.Length - iv.Length))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
