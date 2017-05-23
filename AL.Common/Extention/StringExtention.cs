#region Copyright (C) 2017 AL系列开源项目

/*       
　　	文件功能描述：String类型扩展类

　　	创建人：阿凌
        创建人Email：513845034@qq.com

*/
#endregion
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AL.Common.Extention
{
    /// <summary>
    /// String类型扩展类
    /// </summary>
    public static class StringExtention
    {
        /// <summary>
        /// 获取字符长度
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int GetStringLength(this string s)
        {
            return string.IsNullOrEmpty(s) ? 0 : Encoding.Default.GetBytes(s).Length;
        }
        /// <summary>
        /// 字符串拆分
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="splitStr">拆分间隔字符</param>
        /// <returns></returns>
        public static string[] SplitString(this string sourceStr, string splitStr)
        {
            string[] strArray;
            if ((string.IsNullOrEmpty(sourceStr) ? 0 : (!string.IsNullOrEmpty(splitStr) ? 1 : 0)) == 0)
                strArray = new string[0];
            else if (sourceStr.IndexOf(splitStr) == -1)
                strArray = new string[1]
                {
          sourceStr
                };
            else if (splitStr.Length == 1)
                strArray = sourceStr.Split(splitStr[0]);
            else
                strArray = Regex.Split(sourceStr, Regex.Escape(splitStr), RegexOptions.IgnoreCase);
            return strArray;
        }
        /// <summary>
        /// 字符串拆分
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        public static string[] SplitString(this string sourceStr)
        {
            return StringExtention.SplitString(sourceStr, ",");
        }
        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="length">截取长度</param>
        /// <returns></returns>
        public static string SubString(this string sourceStr, int startIndex, int length)
        {
            return string.IsNullOrEmpty(sourceStr) ? "" : (sourceStr.Length < startIndex + length ? sourceStr.Substring(startIndex) : sourceStr.Substring(startIndex, length));
        }
        /// <summary> 
        /// 截取字符串 默认0开始
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="length">截取长度</param>
        public static string SubString(this string sourceStr, int length)
        {
            return StringExtention.SubString(sourceStr, 0, length);
        }
        /// <summary> 
        /// 从尾部截取字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="length">截取长度</param>
        public static string SubStringEnd(this string sourceStr, int length)
        {
            if (sourceStr.Length <= length)
                return sourceStr;
            return StringExtention.SubString(sourceStr, sourceStr.Length - length, length);
        }
        /// <summary>
        /// 删除头部字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="trimStr">删除的字符</param>
        /// <returns></returns>
        public static string TrimStart(this string sourceStr, string trimStr)
        {
            return StringExtention.TrimStart(sourceStr, trimStr, true);
        }
        /// <summary>
        /// 删除头部字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="trimStr">删除的字符</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static string TrimStart(this string sourceStr, string trimStr, bool ignoreCase)
        {
            return !string.IsNullOrEmpty(sourceStr) ? ((string.IsNullOrEmpty(trimStr) ? 0 : (sourceStr.StartsWith(trimStr, ignoreCase, CultureInfo.CurrentCulture) ? 1 : 0)) != 0 ? sourceStr.Remove(0, trimStr.Length) : sourceStr) : string.Empty;
        }
        /// <summary>
        /// 删除尾部字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="trimStr">删除的字符</param>
        /// <returns></returns>
        public static string TrimEnd(this string sourceStr, string trimStr)
        {
            return StringExtention.TrimEnd(sourceStr, trimStr, true);
        }
        /// <summary>
        /// 删除尾部字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="trimStr">删除的字符</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static string TrimEnd(this string sourceStr, string trimStr, bool ignoreCase)
        {
            return !string.IsNullOrEmpty(sourceStr) ? ((string.IsNullOrEmpty(trimStr) ? 0 : (sourceStr.EndsWith(trimStr, ignoreCase, CultureInfo.CurrentCulture) ? 1 : 0)) != 0 ? sourceStr.Substring(0, sourceStr.Length - trimStr.Length) : sourceStr) : string.Empty;
        }
        /// <summary>
        /// 删除字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="trimStr">删除的字符</param>
        /// <returns></returns>
        public static string Trim(this string sourceStr, string trimStr)
        {
            return StringExtention.Trim(sourceStr, trimStr, true);
        }
        /// <summary>
        /// 删除字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="trimStr">删除的字符</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static string Trim(this string sourceStr, string trimStr, bool ignoreCase)
        {
            string str;
            if (string.IsNullOrEmpty(sourceStr))
                str = string.Empty;
            else if (string.IsNullOrEmpty(trimStr))
            {
                str = sourceStr;
            }
            else
            {
                if (sourceStr.StartsWith(trimStr, ignoreCase, CultureInfo.CurrentCulture))
                    sourceStr = sourceStr.Remove(0, trimStr.Length);
                if (sourceStr.EndsWith(trimStr, ignoreCase, CultureInfo.CurrentCulture))
                    sourceStr = sourceStr.Substring(0, sourceStr.Length - trimStr.Length);
                str = sourceStr;
            }
            return str;
        }
        /// <summary>
        /// 字符串替换
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="newStr">新字符串</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="length">截取长度</param>
        /// <returns></returns>
        public static string Replace(this string sourceStr, string newStr, int startIndex, int length)
        {
            string oldStr = sourceStr.SubString(startIndex,length);
            return sourceStr.Replace(oldStr, newStr);
        }
        public static int IndexOf(this string sourceStr, int order)
        {
            return StringExtention.IndexOf(sourceStr, '-', order);
        }

        public static int IndexOf(this string sourceStr, char c, int order)
        {
            int length = sourceStr.Length;
            int num;
            for (int index = 0; index < length; ++index)
            {
                if ((int)c == (int)sourceStr[index])
                {
                    if (order != 1)
                    {
                        --order;
                    }
                    else
                    {
                        num = index;
                        goto label_8;
                    }
                }
            }
            num = -1;
            label_8:
            return num;
        }

    }
}
