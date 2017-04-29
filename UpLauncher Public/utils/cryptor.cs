using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace UpLauncher_Public.utils
{
    class cryptor
    {
        public static string fileCryptor_md5(string fileName)
        {
            FileStream fs = File.OpenRead(fileName);
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(fs);
            fs.Close();
            StringBuilder str = new StringBuilder();
            foreach (byte b in hash)
                str.AppendFormat("{0:X2}", b);
            return (str.ToString().ToLower());
        }
    }
}
