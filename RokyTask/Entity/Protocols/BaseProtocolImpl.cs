using CommonUtils;
using Roky.SerialPortHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols
{
    public abstract class BaseProtocolImpl<Entity> : IEntityProtocol where Entity : class, IEntityProtocol, new()
    {
        //解码
        public IEntityProtocol Decode(byte[] args)
        {
            List<ProtocolAttribute> mProtocolAttributeList = DisplaySelfAttribute<Entity>();
            //
            Entity mEntity = new Entity();

            foreach (ProtocolAttribute item in mProtocolAttributeList)
            {
                byte[] temp = SplitArray(args, item.Index, item.Index + item.Length);

                SetModelValue(item.PropertyName, temp, mEntity);
            }

            return mEntity;
        }
        //编码 拼包
        public byte[] Encode()
        {
            List<ProtocolAttribute> mProtocolAttributeList = DisplaySelfAttribute<Entity>();
            byte[] returnValue = null;
            foreach (ProtocolAttribute item in mProtocolAttributeList)
            {
                byte[] current = GetModelValue(item.PropertyName, this);

                byte[] valueTemp = SplitArray(current, current.Length - item.Length, current.Length);

                if (returnValue != null)
                {
                    returnValue = MergerArray(returnValue, valueTemp);
                }
                else
                {
                    returnValue = valueTemp;
                }

            }

            if (returnValue == null)
            {
                returnValue = new byte[0];
            }

            return returnValue;
        }

        public abstract int GetCommand();

        /// <summary>
        /// 通过反射取自定义属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private static List<ProtocolAttribute> DisplaySelfAttribute<T>() where T : class, new()
        {

            string tableName = string.Empty;

            List<ProtocolAttribute> list = new List<ProtocolAttribute>();

            Type objType = typeof(T);

            //取属性上的自定义特性

            foreach (PropertyInfo propInfo in objType.GetProperties())
            {
                object[] objAttrs = propInfo.GetCustomAttributes(typeof(ProtocolAttribute), true);
                if (objAttrs.Length > 0)
                {

                    ProtocolAttribute attr = objAttrs[0] as ProtocolAttribute;

                    if (attr != null)
                    {
                        list.Add(attr);
                    }

                }

            }
            list.Sort(new ColumnIndexComparer());
            return list;

        }

        /// 
        /// 获取类中的属性值
        /// 
        /// 
        public byte[] GetModelValue(string FieldName, object obj)
        {
            try
            {
                Type Ts = obj.GetType();
                //                object o = Ts.GetProperty(FieldName).GetValue(obj, null);
                string typeName = Ts.GetProperty(FieldName).PropertyType.Name;
                object o = Ts.GetProperty(FieldName).GetValue(obj, null);
                if (typeName == "string") //string
                {
                    byte[] Value = ByteProcess.stringToByteArray((string)o);
                    return Value;
                }
                else if (typeName == "Int32") //int
                {
                    long tempVlaue = (int)o;
                    byte[] Value = ByteProcess.longToByteArray(tempVlaue);
                    return Value;
                }
                else if (typeName == "Int64") //long
                {
                    byte[] Value = ByteProcess.longToByteArray((long)o);
                    return Value;
                }
                else
                {
                    return (o as byte[]);
                }

                /*
                                if (o is string)
                                {
                                    //string 转 byte[]
                                    byte[] Value = ByteProcess.stringToByteArray((string)o);
                                    return Value;
                                }
                                else if (o is byte[])
                                {
                                    return (o as byte[]);
                                }
                                else
                                {
                                    //当成long 处理 转 byte[]
                                    byte[] Value = ByteProcess.longToByteArray((long)o);
                                    return Value;
                                }
                */
            }
            catch
            {
                return null;
            }
        }

        /// 
        /// 设置类中的属性值
        /// 
        /// 
        /// 
        /// 
        public bool SetModelValue(string FieldName, byte[] Value, object obj)
        {
            try
            {
                Type Ts = obj.GetType();
                string typeName = Ts.GetProperty(FieldName).PropertyType.Name;
                if (typeName == "string")
                {
                    //byte 转 string
                    string _value = ByteProcess.byteArrayToString(Value);
                    Ts.GetProperty(FieldName).SetValue(obj, _value, null);

                }
                else if (typeName == "Int32")
                {
                    //byte 转 int
                    int _value = ByteProcess.byteArrayToInt(MergerArray(Value, 4), 0);
                    Ts.GetProperty(FieldName).SetValue(obj, _value, null);
                }
                else
                    Ts.GetProperty(FieldName).SetValue(obj, Value, null);

                return true;

            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 从数组中截取一部分成新的数组
        /// </summary>
        /// <param name="Source">原数组</param>
        /// <param name="StartIndex">原数组的起始位置</param>
        /// <param name="EndIndex">原数组的截止位置</param>
        /// <returns></returns>
        public byte[] SplitArray(byte[] Source, int StartIndex, int EndIndex)
        {
            try
            {
                byte[] result = new byte[EndIndex - StartIndex];
                for (int i = 0; i < EndIndex - StartIndex; i++)
                    result[i] = Source[i + StartIndex];
                return result;
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 不足长度的前面补空格,超出长度的前面部分去掉
        /// </summary>
        /// <param name="First">要处理的数组</param>
        /// <param name="byteLen">数组长度</param>
        /// <returns></returns>
        public byte[] MergerArray(byte[] First, int byteLen)
        {
            byte[] result;
            if (First.Length > byteLen)
            {
                result = new byte[byteLen];
                for (int i = 0; i < byteLen; i++) result[i] = First[i + First.Length - byteLen];
                return result;
            }
            else
            {
                result = new byte[byteLen];
                for (int i = 0; i < byteLen; i++) result[i] = 0;
                First.CopyTo(result, byteLen - First.Length);
                return result;
            }
        }

        /// <summary>
        /// 合并数组
        /// </summary>
        /// <param name="First">第一个数组</param>
        /// <param name="Second">第二个数组</param>
        /// <returns>合并后的数组(第一个数组+第二个数组，长度为两个数组的长度)</returns>
        public byte[] MergerArray(byte[] First, byte[] Second)
        {
            byte[] result = new byte[First.Length + Second.Length];
            First.CopyTo(result, 0);
            Second.CopyTo(result, First.Length);
            return result;
        }
    }

    class ColumnIndexComparer : IComparer<ProtocolAttribute>
    {
        #region IComparer 成员
        public int Compare(ProtocolAttribute x, ProtocolAttribute y)
        {

            return x.Index - y.Index;

        }
        #endregion

    }
}
