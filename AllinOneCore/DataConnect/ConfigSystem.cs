using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllinOneCore.DataConnect
{
    class ConfigSystem
    {
        private static string strConnect;
        protected const string strPasswordConnect = "25251325@gmail.com";

        // Methods
        static ConfigSystem();
        public ConfigSystem();
        protected static string Encrypt(string strText, string strPassword);
        public static bool LoadConfig();

        // Properties
        public static string ConnectionString { get; set; }

    }
}
