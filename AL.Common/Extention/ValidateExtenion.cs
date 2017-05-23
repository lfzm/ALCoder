#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：字符串验证 扩展类
*
*　　	创建人： 阿凌
*       创建人Email：513845034@qq.com
*       
*****************************************************************************/

#endregion

using System.Text.RegularExpressions;

namespace AL.Common.Extention
{
    /// <summary>
    /// 字符验证扩展类
    /// </summary>
   public static class ValidateExtenion
    {
        #region 正则表达式
        /// <summary>
        /// Ip 正则表达式
        /// </summary>
        public const string _ipregex = "^(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])$";//Ip
        /// <summary>
        /// 邮箱 正则表达式
        /// </summary>
        public const string _emailregex = "^(\\w)+(\\.\\w+)*@(\\w)+((\\.\\w+)+)$";
        /// <summary>
        /// 手机号码  正则表达式
        /// </summary>
        public static string _mobileregex = "^(13|14|15|16|17|18|19)[0-9]{9}$";
        /// <summary>
        /// 电话号码 正则表达式
        /// </summary>
        private static string _phoneregex = "^(\\d{3,4}-?)?\\d{7,8}$";
        #endregion

        #region 验证方法
        /// <summary>
        /// 验证IP
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsIP(this string s)
        {
            Regex regex = new Regex(_ipregex);
            return regex.IsMatch(s);
        }
        /// <summary>
        /// 验证IP
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsEmail(this string s)
        {
            Regex regex = new Regex(_emailregex, RegexOptions.IgnoreCase);//邮箱地址
            return regex.IsMatch(s);
        }
        /// <summary>
        /// 验证手机号码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsMobile(this string s)
        {
            Regex regex = new Regex(_mobileregex);
            return regex.IsMatch(s);
        }

        /// <summary>
        /// 验证电话号码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsPhone(this string s)
        {
            Regex regex = new Regex(_phoneregex);
            return regex.IsMatch(s);
        }

        #endregion
    }
}
