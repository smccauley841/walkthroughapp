using System.Security.Cryptography;
using System.Text;

namespace WalkthroughApp_API.Helpers
{
    public interface IEncodePassword
    {
        Tuple<byte[], byte[]> EncodePassword(string password);
        byte[] EncodePasswordWithKey(byte[] encodeKey, string password);
    }

    public class Decryption : IEncodePassword
    {
        public Tuple<byte[], byte[]> EncodePassword(string password)
        {
            using var hmac = new HMACSHA512();

            var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            var passwordSalt = hmac.Key;

            return Tuple.Create(passwordHash, passwordSalt);
        }

        public byte[] EncodePasswordWithKey(byte[] encodeKey, string password)
        {
            using var hmac = new HMACSHA512();
            hmac.Key = encodeKey;

            return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}

