using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtils
{
    public class ByteProcess
    {
        /**
	 * Creates a 16bit byte array from a 16bit character
	 * 
	 * @param cChar
	 *            Character to convert
	 */
        public static byte[] charToByteArray(char cChar)
        {
            return new byte[] { (byte)((cChar >> 8) & 0xff), (byte)((cChar >> 0) & 0xff), };
        }

        /**
         * Creates an 8bit byte array from a boolean
         * 
         * @param bBoolean
         *            Boolean to convert
         */
        public static byte[] booleanToByteArray(bool bBoolean)
        {
            return new byte[] { (byte)(bBoolean ? 0x01 : 0x00) };
        }

        /**
         * Creates a 32bit byte array from an integer
         * 
         * @param int32
         *            Integer to convert
         */
        public static byte[] intToByteArray(int int32)
        {
            return new byte[] { (byte)((int32 >> 24) & 0xff), (byte)((int32 >> 16) & 0xff), (byte)((int32 >> 8) & 0xff), (byte)((int32 >> 0) & 0xff), };
        }

        /**
         * Creates a 64bit byte array from a long
         * 
         * @param int64
         *            Long to convert
         */
        public static byte[] longToByteArray(long int64)
        {
            return new byte[] { (byte) ((int64 >> 56) & 0xff), (byte) ((int64 >> 48) & 0xff), (byte) ((int64 >> 40) & 0xff), (byte) ((int64 >> 32) & 0xff), (byte) ((int64 >> 24) & 0xff),
                (byte) ((int64 >> 16) & 0xff), (byte) ((int64 >> 8) & 0xff), (byte) ((int64 >> 0) & 0xff) };
        }

        /**
         * Creates a byte array from a String
         * 
         * @param sString
         *            String to convert
         */
        public static byte[] stringToByteArray(String sString)
        {
            return (sString == null) ? null : System.Text.Encoding.Default.GetBytes(sString);
        }

        /**
         * Creates a Char from a byte array
         * 
         * @param byteArray
         *            Array of bytes to convert
         */
        public static char byteArrayToChar(byte[] byteArray)
        {
            return (char)((0xff & byteArray[0]) << 8 | (0xff & byteArray[1]) << 0);
        }

        /**
         * Creates a Char array from a byte array
         * 
         * @param byteArray
         *            Array of bytes to convert
         */
        public static char[] byteArrayToCharArray(byte[] byteArray)
        {
            int size = byteArray.Length / 2;
            char[] charArray = new char[size];
            byte[] tmpArray;
            for (int s = 0; s < size; s++)
            {
                tmpArray = ByteProcess.getSubByteArray(byteArray, s * 2, (s + 1) * 2);
                charArray[s] = byteArrayToChar(tmpArray);
            }
            return charArray;
        }

        /**
         * Creates a byte array from a Char array
         * 
         * @param charArray
         *            Array of characters to convert
         */
        public static byte[] charArrayToByteArray(char[] charArray)
        {
            byte[] byteArray = new byte[charArray.Length * 2];
            byte[] tmpArray;
            for (int s = 0; s < charArray.Length; s++)
            {
                tmpArray = ByteProcess.charToByteArray(charArray[s]);
                byteArray = ByteProcess.appendToByteArray(byteArray, tmpArray, s * 2);
            }
            return byteArray;
        }

        public static short byteArrayToShort(byte[] byteArray, int offset)
        {
            return (short)((0xff & byteArray[offset]) << 8 | (0xff & byteArray[offset + 1]) << 0);
        }

        /**
         * Creates an Integer from a byte array
         * 
         * @param byteArray
         *            Byte array to convert
         * @param offset
         *            Offset to start parsing
         */
        public static int byteArrayToInt(byte[] byteArray, int offset)
        {
            return (0xff & byteArray[offset]) << 24 | (0xff & byteArray[offset + 1]) << 16 | (0xff & byteArray[offset + 2]) << 8 | (0xff & byteArray[offset + 3]) << 0;
        }

        /**
         * Creates a long from a byte array
         * 
         * @param byteArray
         *            Byte array to convert
         * @param offset
         *            Offset to start parsing
         */
        public static long byteArrayToLong(byte[] byteArray, int offset)
        {
            // return (0xff & byteArray[offset]) << 56 | (0xff & byteArray[offset +
            // 1]) << 48 | (0xff & byteArray[offset + 2]) << 40 | (0xff &
            // byteArray[offset + 3]) << 32
            // | (0xff & byteArray[offset + 4]) << 24 | (0xff & byteArray[offset +
            // 5]) << 16 | (0xff & byteArray[offset + 6]) << 8 | (0xff &
            // byteArray[offset + 7]) << 0;
            long i = 0;
            int len = byteArray.Length;
            for (int m = 0; m < len - 1; m++)
            {
                i += byteArray[m] & 0xff;
                i <<= 8;
            }

            i += byteArray[len - 1] & 0xff;
            return i;

        }

        /**
         * Creates a String from a byte array
         * 
         * @param byteArray
         *            Byte array to convert
         */
        public static String byteArrayToString(byte[] byteArray)
        {
            return System.Text.Encoding.Default.GetString(byteArray);
        }

        /**
         * Adds a byte array to another byte array
         * 
         * @param dArray
         *            Array to append to
         * @param sArray
         *            Array to append
         * @param offset
         *            Offset to start appending
         */
        public static byte[] appendToByteArray(byte[] dArray, byte[] sArray, int offset)
        {
            for (int i = 0; i < sArray.Length; i++)
                dArray[offset + i] = sArray[i];
            return dArray;
        }

        /**
         * Returns a byte array from another byte array
         * 
         * @param byteArray
         *            Byte array to parse
         * @param offset_START
         *            Starting location
         * @param offset_END
         *            Ending location
         */
        public static byte[] getSubByteArray(byte[] byteArray, int offset_START, int offset_END)
        {
            byte[] byteBuffer = new byte[offset_END - offset_START];
            for (int i = offset_START; i < offset_END; i++)
                byteBuffer[i - offset_START] = byteArray[i];
            return byteBuffer;
        }

        /**
         * Compares to byte arrays
         * 
         * @param array1
         *            Array 1
         * @param array2
         *            Array 2
         */
        public static bool compareByteArray(byte[] array1, byte[] array2)
        {
            if (array1 == array2)
                return true;
            else
                return false;
        }

        /**
         * Converts a String to an array of Char
         * 
         * @param sVal
         *            String to convert
         */
        public static char[] stringToCharArray(String sVal)
        {
            return sVal.ToCharArray();
        }

        /**
         * Returns an Integer from a String
         * 
         * @param val
         *            String to parse
         * @param offset
         *            Offset to start parsing
         */
        public static int getIntFromString(String val, int offset)
        {
            char[] chrCode = new char[CHARSIZE];
            byte[] comCode;

            chrCode = val.ToCharArray(offset, offset + CHARSIZE);
            comCode = ByteProcess.charArrayToByteArray(chrCode);
            return ByteProcess.byteArrayToInt(comCode, 0);
        }

        public static int CHARSIZE = 2;

        public void _ize()
        {
            // TODO Auto-generated method stub

        }

        /// <summary>
        /// 获取bit位值
        /// </summary>
        /// <param name="num"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int get(int num, int index)
        {
            return (num & (0x1 << index)) >> index;
        }

        /// <summary>
        /// 把带有“:”的字符串，转换成byte[]
        /// </summary>
        /// <param name="sString"></param>
        /// <returns></returns>
        public static byte[] stringToByteArrayNoColon(string sString)
        {
            //把带有":"给去掉
            if (sString != null)
            {
                sString = sString.Replace(":", "");
                return HexStringToBytes(sString);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 
        /// hex string 转 Byte
        /// 
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(string data)
        {
            int c;
            List<byte> result = new List<byte>();

            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(data);
            sw.Flush();
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);

            StringBuilder number = new StringBuilder();

            while ((c = sr.Read()) > 0)
            {
                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
                {
                    number.Append((char)c);

                    if (number.Length >= 2)
                    {
                        result.Add(Convert.ToByte(number.ToString(), 16));
                        number.Length = 0;
                    }
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// 
        /// 
        /// 
        /// </summary>
        /// <param name="sString"> 输入string </param>
        /// <param name="Len"> 总长度 </param>
        /// <returns></returns>
        public static byte[] base64ToByteArray(string sString, int Len)
        {
            byte[] mCode = new byte[Len];

            byte[] code = Convert.FromBase64String(sString);
            code.CopyTo(mCode, 0);
            return mCode;
        }
        // List数组改成 json数组
        public static string listToJsonString(List<string> mList)
        {
            string szJson = "";
            DataContractJsonSerializer json = new DataContractJsonSerializer(mList.GetType());

            using (MemoryStream stream = new MemoryStream())
            {
                json.WriteObject(stream, mList);

                szJson = Encoding.UTF8.GetString(stream.ToArray());
            }

            return szJson;
        }

        public static int server2ue(int index)
        {
            if (index < 0xf0)
            {
                return index + 1;
            }
            else if (index == 0xff)
            {
                return 19;
            }
            else
            {
                // 蓝牙物理钥匙
                int btKeyIndex = index - 0xf0 + 13;

                if (btKeyIndex > 18)
                {
                    return 18;
                }
                else
                {
                    return btKeyIndex;
                }
            }
        }

    }
}
