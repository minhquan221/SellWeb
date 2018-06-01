using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using Lib.Data.DataSetting;

namespace Lib.Data.DataConnect
{
    internal class ConfigSystem
    {
        // Fields
        protected string strPasswordConnect = ConfigurationManager.AppSettings["DataEncryptKey"].ToString();
        private static string strConnect = string.Empty;

        // Properties
        public static string ConnectionString
        {
            get
            {
                return strConnect;
            }
            set
            {
                strConnect = value;
            }
        }

        // Methods
        protected static string Encrypt(string strText, string strPassword)
        {
            byte[] buffer = Encoding.Unicode.GetBytes(strText);
            byte[] rgbSalt = new byte[] { 80, 0x76, 0x61, 110, 0x21, 0x4d, 0x65, 100, 0x76, 0x15, 100, 0x65, 0x76 };
            PasswordDeriveBytes bytes = new PasswordDeriveBytes(strPassword, rgbSalt);
            MemoryStream stream = new MemoryStream();
            Rijndael rijndael = Rijndael.Create();
            rijndael.Key = bytes.GetBytes(0x20);
            rijndael.IV = bytes.GetBytes(0x10);
            CryptoStream stream2 = new CryptoStream(stream, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
            stream2.Write(buffer, 0, buffer.Length);
            stream2.Close();
            return Convert.ToBase64String(stream.ToArray());
        }

        public static bool LoadConfig()
        {
            string strText = string.Empty;
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null)
            {
                strText = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
            bool flag = false;
            if (ConfigurationManager.AppSettings["IsConnectionStringEncrypted"] != null)
            {
                flag = Convert.ToBoolean(ConfigurationManager.AppSettings["IsConnectionStringEncrypted"]);
            }
            if (!flag)
            {
                strText = Encrypt(strText, ConfigurationManager.AppSettings["DataEncryptKey"]);
            }
            ConnectionString = strText;
            Settings.Default.Save();
            return true;
        }


    }

}