using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtils
{
    public class Util
    {
        public static string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }

        /// <summary>
        /// 清除所有绑定的事件
        /// </summary>
        /// <param name="objectHasEvents"></param>
        /// <param name="eventName"></param>
        public static void ClearAllEvents(object objectHasEvents, string eventName)
        {
            if (objectHasEvents == null)
            {
                return;
            }

            try
            {
                EventInfo[] events = objectHasEvents.GetType().GetEvents(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (events == null || events.Length < 1)
                {
                    return;
                }

                for (int i = 0; i < events.Length; i++)
                {
                    EventInfo ei = events[i];

                    if (ei.Name == eventName)
                    {
                        FieldInfo fi = ei.DeclaringType.GetField("printPageHandler", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (fi != null)
                        {
                            fi.SetValue(objectHasEvents, null);
                        }

                        break;
                    }
                }
            }
            catch
            {
            }
        }

        #region 提取字符串中的数字
        public static int GetDigits(string _str)
        {
            int ConvertDigits = 0;
            string num = null;
            foreach (char item in _str)
            {
                if (item >= 48 && item <= 58)
                {
                    num += item;
                }
            }

            ConvertDigits = int.Parse(num);

            return ConvertDigits;
        }
        #endregion

    }
}
