using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FoodExpress.Helpers
{
    public static class DataHelper
    {
        public static string EncryptPasswordSalt(string password)
        {
            HashAlgorithm algorithm = new SHA1Managed();
            var saltBytes = GenerateSalt(50);
            var plainTextBytes = Encoding.ASCII.GetBytes(password);

            var plainTextWithSaltBytes = AppendByteArray(plainTextBytes, saltBytes);
            var saltedSHA1Bytes = algorithm.ComputeHash(plainTextWithSaltBytes);
            var saltedSHA1WithAppendedSaltBytes = AppendByteArray(saltedSHA1Bytes, saltBytes);

            return Convert.ToBase64String(saltedSHA1WithAppendedSaltBytes);
        }
        private static byte[] GenerateSalt(int size)
        {
            var buff = new byte[size];
            for (int i = 0; i < size; i++)
            {
                buff[i] = Convert.ToByte(size - i);
            }
            return SHA1.Create().ComputeHash(buff);
        }
        private static byte[] AppendByteArray(byte[] byteArray1, byte[] byteArray2)
        {
            var byteArrayResult = new byte[byteArray1.Length + byteArray2.Length];
            Array.Copy(byteArray1, byteArrayResult, byteArray1.Length);
            Array.Copy(byteArray2, 0, byteArrayResult, byteArray1.Length, byteArray2.Length);
            return byteArrayResult;
        }
    }
}
