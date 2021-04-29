using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DotNetTool
{
    public static class EncryptHelper
    {
        private static readonly string _key = "beta";

        public static string Encrypt(string str)
        {
            byte[] key = Encoding.Unicode.GetBytes(_key);
            byte[] data = Encoding.Unicode.GetBytes(str);

            using DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();

            using MemoryStream MStream = new MemoryStream();

            using CryptoStream CStream = new CryptoStream(MStream,
                descsp.CreateEncryptor(key, key), CryptoStreamMode.Write);

            CStream.Write(data, 0, data.Length);
            CStream.FlushFinalBlock();
            byte[] temp = MStream.ToArray();

            return Convert.ToBase64String(temp);
        }

        public static string Decrypt(string str)
        {
            byte[] key = Encoding.Unicode.GetBytes(_key);
            byte[] data = Convert.FromBase64String(str);

            using DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();

            using MemoryStream MStream = new MemoryStream();

            using CryptoStream CStream = new CryptoStream(MStream,
                descsp.CreateDecryptor(key, key), CryptoStreamMode.Write);

            CStream.Write(data, 0, data.Length);
            CStream.FlushFinalBlock();
            byte[] temp = MStream.ToArray();

            return Encoding.Unicode.GetString(temp);
        }
    }
}

