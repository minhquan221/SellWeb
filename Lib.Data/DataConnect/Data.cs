using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;

namespace Lib.Data.DataConnect
{
    public class Data
    {
        // Fields
        protected const string DEFAULT_OUT_PARAMETER = "v_Out";
        protected const int DEFAULT_OUT_PARAMETER_LENGTH = 0xfa0;
        protected string strPasswordConnect = ConfigurationManager.AppSettings["DataEncryptKey"];
        protected string strConnect = string.Empty;

        // Methods
        public static IData CreateData()
        {
            if (string.IsNullOrEmpty(ConfigSystem.ConnectionString))
            {
                ConfigSystem.LoadConfig();
            }
            return CreateData(ConfigSystem.ConnectionString);
        }

        public static IData CreateData(string strConnect)
        {
            if (string.IsNullOrEmpty(strConnect))
            {
                return CreateData();
            }
            return CreateData(strConnect, true);
        }

        public static IData CreateData(string strConnect, bool bolIsCrypt)
        {
            
            switch (RegconizeStringConnect(bolIsCrypt ? Decrypt(strConnect,  ConfigurationManager.AppSettings["DataEncryptKey"]) : strConnect))
            {
                case DATABASETYPE.SQLSERVER:
                    return new SQLConnect(bolIsCrypt ? strConnect : Encrypt(strConnect, ConfigurationManager.AppSettings["DataEncryptKey"]));

                case DATABASETYPE.ORACLE:
                    return new OracleConnect(bolIsCrypt ? strConnect : Encrypt(strConnect, ConfigurationManager.AppSettings["DataEncryptKey"]));

                case DATABASETYPE.MySQL:
                    return new MySQLConnect(bolIsCrypt ? strConnect : Encrypt(strConnect, ConfigurationManager.AppSettings["DataEncryptKey"]));

                case DATABASETYPE.MsAccess:
                    return new MsAccessConnect(bolIsCrypt ? strConnect : Encrypt(strConnect, ConfigurationManager.AppSettings["DataEncryptKey"]));
            }
            return null;
        }

        public static string Decrypt(string strText, string strPassword)
        {
            if (strText.Trim().Length == 0)
            {
                return string.Empty;
            }
            byte[] buffer = Convert.FromBase64String(strText);
            byte[] rgbSalt = new byte[] { 80, 0x76, 0x61, 110, 0x21, 0x4d, 0x65, 100, 0x76, 0x15, 100, 0x65, 0x76 };
            PasswordDeriveBytes bytes = new PasswordDeriveBytes(strPassword, rgbSalt);
            MemoryStream stream = new MemoryStream();
            Rijndael rijndael = Rijndael.Create();
            rijndael.Key = bytes.GetBytes(0x20);
            rijndael.IV = bytes.GetBytes(0x10);
            CryptoStream stream2 = new CryptoStream(stream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
            stream2.Write(buffer, 0, buffer.Length);
            stream2.Close();
            return Encoding.Unicode.GetString(stream.ToArray());
        }

        public static string Encrypt(string strText, string strPassword)
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

        protected static DATABASETYPE RegconizeStringConnect(string strConnect)
        {
            int num2;
            string[] strArray = new string[] { "Data Source", "User ID", "Password", "Unicode" };
            string[] strArray2 = new string[] { "Server", "DataBase", "UID", "Pwd", "Data Source", "User ID", "Password", "Initial Catalog" };
            string[] strArray3 = new string[] { "Server", "User ID", "Password", "DataBase" };
            string[] strArray4 = new string[] { "Provider", "Microsoft", "Jet", "OLEDB", "Data Source" };
            int num = 0;
            num += strConnect.ToUpper().Split(new string[] { "ORA" }, StringSplitOptions.None).Length;
            for (num2 = 0; num2 < strArray.Length; num2++)
            {
                if (strConnect.ToUpper().Contains(strArray[num2].ToUpper()))
                {
                    num++;
                }
            }
            int num3 = 0;
            num3 += strConnect.ToUpper().Split(new string[] { "SQL" }, StringSplitOptions.None).Length;
            for (num2 = 0; num2 < strArray2.Length; num2++)
            {
                if (strConnect.ToUpper().Contains(strArray2[num2].ToUpper()))
                {
                    num3++;
                }
            }
            int num4 = 0;
            for (num2 = 0; num2 < strArray3.Length; num2++)
            {
                if (strConnect.ToUpper().Contains(strArray3[num2].ToUpper()))
                {
                    num4++;
                }
            }
            int num5 = 0;
            for (num2 = 0; num2 < strArray4.Length; num2++)
            {
                if (strConnect.ToUpper().Contains(strArray4[num2].ToUpper()))
                {
                    num5++;
                }
            }
            if (num4 >= 4)
            {
                return DATABASETYPE.MySQL;
            }
            if (num5 >= 5)
            {
                return DATABASETYPE.MsAccess;
            }
            return ((num >= num3) ? DATABASETYPE.ORACLE : DATABASETYPE.SQLSERVER);
        }

        // Nested Types
        public enum DATABASETYPE
        {
            NONE,
            SQLSERVER,
            ORACLE,
            MySQL,
            MsAccess
        }
    }
}


