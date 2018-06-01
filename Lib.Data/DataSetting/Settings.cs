using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Data.DataSetting
{
    internal sealed class Settings : ApplicationSettingsBase
    {
        // Fields
        private static Settings defaultInstance = ((Settings)SettingsBase.Synchronized(new Settings()));

        // Properties
        public static Settings Default =>
            defaultInstance;
    }
}
