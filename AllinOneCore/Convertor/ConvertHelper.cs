using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AllinOneCore.Convertor
{
    public class ConvertHelper
    {
        private ConvertHelper instance = new ConvertHelper();

        public ConvertHelper Current
        {
            get
            {
                return instance == null ? instance = new ConvertHelper() : instance;
            }
        }

        private static Dictionary<string, string> vn_str_pt = new Dictionary<string, string>();
        /// <summary>
        /// 
        /// </summary>
        static ConvertHelper()
        {
            vn_str_pt.Add("a", "á|à|ả|ã|ạ|ă|ắ|ặ|ằ|ẳ|ẵ|â|ấ|ầ|ẩ|ẫ|ậ");
            vn_str_pt.Add("d", "đ");
            vn_str_pt.Add("e", "é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ");
            vn_str_pt.Add("i", "í|ì|ỉ|ĩ|ị");
            vn_str_pt.Add("o", "ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ");
            vn_str_pt.Add("u", "ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự");
            vn_str_pt.Add("y", "ý|ỳ|ỷ|ỹ|ỵ");
            vn_str_pt.Add("A", "Á|À|Ả|Ã|Ạ|Ă|Ắ|Ặ|Ằ|Ẳ|Ẵ|Â|Ấ|Ầ|Ẩ|Ẫ|Ậ");
            vn_str_pt.Add("D", "Đ");
            vn_str_pt.Add("E", "É|È|Ẻ|Ẽ|Ẹ|Ê|Ế|Ề|Ể|Ễ|Ệ");
            vn_str_pt.Add("I", "Í|Ì|Ỉ|Ĩ|Ị");
            vn_str_pt.Add("O", "Ó|Ò|Ỏ|Õ|Ọ|Ô|Ố|Ồ|Ổ|Ỗ|Ộ|Ơ|Ớ|Ờ|Ở|Ỡ|Ợ");
            vn_str_pt.Add("U", "Ú|Ù|Ủ|Ũ|Ụ|Ư|Ứ|Ừ|Ử|Ữ|Ự");
            vn_str_pt.Add("Y", "Ý|Ỳ|Ỷ|Ỹ|Ỵ");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="vl">Value to parsing</param>
        /// <returns></returns>
        public static T ParseEnum<T>(string vl)
        {
            return (T)Enum.Parse(typeof(T), vl, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string DeUnicodeConvert(string input)
        {
            if (!String.IsNullOrEmpty(input))
            {
                foreach (var item in vn_str_pt)
                {
                    input = Regex.Replace(input, item.Value, item.Key);
                }
            }
            return input;
        }

        public static string UrlFormatConvert(string input)
        {
            if (!String.IsNullOrEmpty(input))
            {
                return Regex.Replace(Regex.Replace(Regex.Replace(DeUnicodeConvert(input), "[^a-zA-Z0-9]", "-"), "-+", "-"), " ", "-").ToLower().TrimStart('-').TrimEnd('-');
            }
            return input;
        }

        public static string FormatDateTime(DateTime? date, string strFormatString)
        {
            return date != null ? Convert.ToDateTime(date).ToString(strFormatString) : "";//"HH:mm, dd/MM/yyyy"
        }

        public static string CollapseString(string strString, int length)
        {
            if (!string.IsNullOrEmpty(strString))
            {
                if (strString.Length <= length)
                {
                    return strString;
                }
                else
                {
                    return strString.Substring(0, length) + "...";
                }
            }
            else
                return "";

        }
        public static T ConvertObjectToObject<T>(T objConvert, Object obj)
        {
            PropertyInfo[] propObject = obj.GetType().GetProperties();
            PropertyInfo[] propConvertor = objConvert.GetType().GetProperties();
            foreach (var fieldConvert in propConvertor)
            {
                foreach (var fieldObject in propObject)
                {
                    var valueField = fieldObject.GetValue(obj);
                    if (fieldConvert.Name == fieldObject.Name)
                    {
                        if (valueField != null)
                        {
                            fieldConvert.SetValue(objConvert, valueField);
                        }
                        break;
                    }
                }
            }
            return objConvert;
        }

    }
}
