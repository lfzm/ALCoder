#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：工具辅助类
*
*　　	创建人： 阿凌
*       创建人Email：513845034@qq.com
*       
*****************************************************************************/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AL.Common.Helper
{
    /// <summary>
    /// 工具辅助类
    /// </summary>
    public static class UtilsHelper
    {
        #region SQL Filter
        /// <summary>
        /// 过滤 Sql 语句字符串中的注入脚本
        /// </summary>
        /// <param name="source">传入的字符串</param>
        /// <returns>过滤后的字符串</returns>
        public static string SqlFilter(string source)
        {
            source = source.Replace("\"", "");
            source = source.Replace("&", "&amp");
            source = source.Replace("<", "&lt");
            source = source.Replace(">", "&gt");
            source = source.Replace("delete", "");
            source = source.Replace("update", "");
            source = source.Replace("insert", "");
            source = source.Replace("'", "''");
            source = source.Replace(";", "；");
            source = source.Replace("(", "（");
            source = source.Replace(")", "）");
            source = source.Replace("Exec", "");
            source = source.Replace("Execute", "");
            source = source.Replace("xp_", "x p_");
            source = source.Replace("sp_", "s p_");
            source = source.Replace("0x", "0 x");
            return source;
        }

        #endregion

        #region 生成随机字母或数字
        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <returns></returns>
        public static string Number(int Length)
        {
            return Number(Length, true);
        }

        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns></returns>
        public static string Number(int Length, bool Sleep)
        {
            if (Sleep)
                System.Threading.Thread.Sleep(1);
            string result = "";
            System.Random random = new Random();
            for (int i = 0; i < Length; i++)
            {
                result += random.Next(0, 9).ToString();
            }
            return result;
        }
        /// <summary>
        /// 生成随机字母字符串(数字字母混和)
        /// </summary>
        /// <param name="codeCount">待生成的位数</param>
        public static string GetCheckCode(int codeCount, bool Sleep)
        {
            if (Sleep)
                System.Threading.Thread.Sleep(1);
            string str = string.Empty;
            int rep = 0;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }
        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="numSeeds"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static int Next(int numSeeds, int length)
        {
            byte[] buffer = new byte[length];
            System.Security.Cryptography.RNGCryptoServiceProvider Gen = new System.Security.Cryptography.RNGCryptoServiceProvider();
            Gen.GetBytes(buffer);
            uint randomResult = 0x0;//这里用uint作为生成的随机数  
            for (int i = 0; i < length; i++)
            {
                randomResult |= ((uint)buffer[i] << ((length - 1 - i) * 8));
            }
            return (int)(randomResult % numSeeds);
        }
        #endregion

        #region 字符拼接
        /// <summary>
        /// 构造参数 拼接分隔符“&”
        /// </summary>
        /// <param name="list">参数集合</param>
        /// <returns></returns>
        public static string StringStructure(Dictionary<string, string> list)
        {
          return   StringStructure(list, '&');
        }
        /// <summary>
        /// 字符拼接
        /// </summary>
        /// <param name="list">参数集合</param>
        /// <param name="Delimiter">分隔符</param>
        /// <returns></returns>
        public static string StringStructure(Dictionary<string, string> list, char Delimiter)
        {
            string parstr = "";

            if (list == null)
                return "";
            list = (from entry in list
                    orderby entry.Key ascending
                    select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach (string key in list.Keys)
            {
                parstr += key + "=" + list[key].Trim() + Delimiter;
            }
            return (parstr.TrimEnd(Delimiter));
        }
        #endregion
    }
}
