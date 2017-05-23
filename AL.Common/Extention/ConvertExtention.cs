#region Copyright (C) 2017 AL系列开源项目

/*       
　　	文件功能描述：类型转换扩展类

　　	创建人：阿凌
        创建人Email：513845034@qq.com

*/
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace AL.Common.Extention
{
    /// <summary>
    /// Convert 扩展器 
    /// </summary>
    public static class ConvertExtention
    {
        /// <summary>
        /// 字符串转化成 UInt32
        /// </summary>
        /// <param name="obj">要转化的值</param>
        /// <param name="defaultValue">如果转化失败，返回的默认值</param>
        /// <returns></returns>
        public static UInt32 ToUInt32(this string obj, UInt32 defaultValue = 0)
        {
            try
            {
                UInt32 returnValue = 0;
                if (UInt32.TryParse(obj, out returnValue))
                {
                    return returnValue;
                }
            }
            catch (Exception)
            {

            }
            return defaultValue;
        }
        /// <summary>
        /// 字符串转化成 Int32
        /// </summary>
        /// <param name="obj">要转化的值</param>
        /// <param name="defaultValue">如果转化失败，返回的默认值</param>
        /// <returns></returns>
        public static int ToInt32(this string obj, int defaultValue = 0)
        {
            try
            {
                int returnValue = 0;
                if (int.TryParse(obj, out returnValue))
                {
                    return returnValue;
                }
            }
            catch (Exception)
            {

            }
            return defaultValue;
        }
        /// <summary>
        /// object 转化成 Int32
        /// </summary>
        /// <param name="obj">要转化的值</param>
        /// <param name="defaultValue">如果转化失败，返回的默认值</param>
        /// <returns></returns>
        public static int ToInt32(this object obj, int defaultValue = 0) 
        {
            if (obj == null)
            {
                return defaultValue;
            }
            return obj.ToString().ToInt32();
        }
        /// <summary>
        /// 字符串转化成 long
        /// </summary>
        /// <param name="obj">要转化的值</param>
        /// <param name="defaultValue">如果转化失败，返回的默认值</param>
        /// <returns></returns>
        public static long ToInt64(this string obj, int defaultValue = 0)
        {
            try
            {
                long returnValue = 0;
                if (long.TryParse(obj, out returnValue))
                {
                    return returnValue;
                }
            }
            catch (Exception)
            {

            }
            return defaultValue;
        }
        /// <summary>
        /// 字符串转化成 Decimal
        /// </summary>
        /// <param name="obj">要转化的值</param>
        /// <param name="defaultValue">如果转化失败，返回的默认值</param>
        /// <returns></returns>
        public static decimal ToDecimal(this string obj, decimal defaultValue = 0)
        {
            try
            {
                decimal returnValue = 0;
                if (decimal.TryParse(obj, out returnValue))
                {
                    return returnValue;
                }
            }
            catch (Exception)
            {

            }
            return defaultValue;
        }
        /// <summary>
        /// object 转化成 Decimal
        /// </summary>
        /// <param name="obj">要转化的值</param>
        /// <param name="defaultValue">如果转化失败，返回的默认值</param>
        /// <returns></returns>
        public static decimal ToDecimal(this object obj, decimal defaultValue = 0)
        {
            if (obj == null)
            {
                return defaultValue;
            }
            return obj.ToString().ToInt32();
        }
        /// <summary>
        /// 字符串转化成 Double
        /// </summary>
        /// <param name="obj">要转化的值</param>
        /// <param name="defaultValue">如果转化失败，返回的默认值</param>
        /// <returns></returns>
        public static double ToDouble(this string obj, double defaultValue = 0)
        {
            try
            {
                double returnValue = 0;
                if (double.TryParse(obj, out returnValue))
                {
                    return returnValue;
                }
            }
            catch (Exception)
            {

            }
            return defaultValue;
        }
        /// <summary>
        /// object 转化成 Double
        /// </summary>
        /// <param name="obj">要转化的值</param>
        /// <param name="defaultValue">如果转化失败，返回的默认值</param>
        /// <returns></returns>
        public static double ToDouble(this object obj, double defaultValue = 0)
        {
            if (obj == null)
            {
                return defaultValue;
            }
            return obj.ToString().ToDouble();
        }
        /// <summary>
        /// 字符串转化成 float
        /// </summary>
        /// <param name="obj">要转化的值</param>
        /// <param name="defaultValue">如果转化失败，返回的默认值</param>
        /// <returns></returns>
        public static float ToFloat(this string obj, float defaultValue = 0)
        {
            try
            {
                float returnValue = 0;
                if (float.TryParse(obj, out returnValue))
                {
                    return returnValue;
                }
            }
            catch (Exception)
            {

            }
            return defaultValue;
        }
        /// <summary>
        /// object 转化成 float
        /// </summary>
        /// <param name="obj">要转化的值</param>
        /// <param name="defaultValue">如果转化失败，返回的默认值</param>
        /// <returns></returns>
        public static float ToFloat(this object obj, float defaultValue = 0)
        {
            if (obj == null)
            {
                return defaultValue;
            }
            return obj.ToString().ToFloat();
        }
        /// <summary>
        /// 字符串转化成 DateTime
        /// </summary>
        /// <param name="obj">要转化的值</param>
        /// <returns>返回转换后值 可空</returns>
        public static DateTime? ToDateTime(this string obj)
        {
            if (string.IsNullOrEmpty(obj))
            {
                return null;
            }
            DateTime date;
            return DateTime.TryParse(obj, out date) ? date : default(DateTime?);
        }
        /// <summary>
        /// 字符串转化成 DateTime
        /// </summary>
        /// <param name="obj">要转化的值</param>
        /// <param name="defaultValue">如果转化失败，返回的默认值</param>
        /// <returns>返回转换后值 可空</returns>
        public static DateTime ToDateTime(this string s, DateTime defaultValue)
        {
            DateTime result;
            return string.IsNullOrWhiteSpace(s) || !DateTime.TryParse(s, out result) ? defaultValue : result;
        }
        /// <summary>
        /// 转化成布尔类型
        /// </summary>
        /// <param name="str">要转化的值</param>
        /// <returns></returns>
        public static bool ToBoolean(this string str)
        {
            bool isOkay = false;
            Boolean.TryParse(str, out isOkay);
            return isOkay;
        }
        /// <summary>
        /// 根据指定编码转化成对应的64位编码
        /// </summary>
        /// <param name="source">要转化的值</param>
        /// <param name="encoding">编码类型</param>
        /// <returns></returns>
        public static string ToBase64(this string source, Encoding encoding)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException("source", "转化Base64字符串不能为空");
            }
            byte[] bytes = encoding.GetBytes(source);
            return Convert.ToBase64String(bytes);
        }
        /// <summary>
        ///  从base64编码解码出正常的值
        /// </summary>
        /// <param name="baseString">base64编码</param>
        /// <param name="encoding">编码类型</param>
        /// <returns></returns>
        public static string FromBase64(this string baseString, Encoding encoding)
        {
            if (string.IsNullOrEmpty(baseString))
            {
                throw new ArgumentNullException("baseString", "解码Base64字符串不能为空");
            }
            byte[] bytes = Convert.FromBase64String(baseString);
            return encoding.GetString(bytes);
        }

   
        
    }
}
