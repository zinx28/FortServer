namespace FortBackend.src.App.Utilities.Helpers
{
    public class Base64
    {
        public static string GenerateRandomString(int length)
        {
            byte[] RandomBytes = new byte[length];
            new Random().NextBytes(RandomBytes);
            return Convert.ToBase64String(RandomBytes);
        }
    }
}
