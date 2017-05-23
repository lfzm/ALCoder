#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：md5加密实现
*
*　　	创建人： 阿凌
*       创建人Email：513845034@qq.com
*       
*****************************************************************************/

#endregion
using System;
using System.Security.Cryptography;
using System.Text;

namespace AL.Common.Helper.Encrypt
{
    /// <summary>
    /// MD5加密类
    /// </summary>
    public static class Md5
    {
        /// <summary>
        /// 获取16位MD5加密值
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HalfEncryptHexString(string input, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            byte[] result = Encrypt(encoding.GetBytes(input));
            return BitConverter.ToString(result, 4, 8).Replace("-", "");
        }


        /// <summary>
        /// 获取32位MD5加密值
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string EncryptHexString(string input, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input", "MD5加密的字符串不能为空！");

            if (encoding == null)
                encoding = Encoding.UTF8;

            var data = encoding.GetBytes(input);
            var encryData = Encrypt(data);

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < encryData.Length; i++)
            {
                sBuilder.Append(encryData[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();

        }


        /// <summary>
        /// 获取MD5加密值
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                throw new ArgumentNullException("bytes", "MD5加密的字节不能为空！");

            using (MD5 md5Hash = MD5.Create())
            {
                return md5Hash.ComputeHash(bytes);
            }
        }




    }
}
