using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace DotNetTool
{
    public static class FileHelper
    {
        public static void WriteTxtFileWithLock(string path, string content)
        {
            using FileStream fs = new(path, FileMode.Create,
                    FileAccess.ReadWrite, FileShare.ReadWrite);

            using StreamWriter file = new(fs, new System.Text.UTF8Encoding(false));

            file.Write(content);
            file.Flush();

        }

        public static void WriteBinFileWithLock(string path, byte[] bin_arr)
        {
            using FileStream fs = new(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

            fs.Write(bin_arr, 0, bin_arr.Length);
        }

        public static void CreateDirIfNotExists(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        public static List<string> GetAllFiles(string dir)
        {
            return Directory.EnumerateFiles(dir).ToList();
        }

        public static string ReadFile(string file)
        {
            return File.ReadAllText(file, System.Text.Encoding.UTF8);
        }

        public static byte[] ReadFile2Bytes(string file)
        {
            using FileStream fs = new(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            int length = Convert.ToInt32(fs.Length);
            byte[] bytes = new byte[length];
            fs.Read(bytes, 0, length);  //由于文件已经按照UTF-8编码保存，所以直接读取到内存即可

            return bytes;

        }

        public static string CalculateMD5(string filename)
        {
            using MD5 md5 = MD5.Create();
            using FileStream stream = File.OpenRead(filename);

            byte[] hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        public static string GetUUID()
        {
            return Guid.NewGuid().ToString().ToUpper();
        }


    }
}
