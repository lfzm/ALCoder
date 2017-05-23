#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：加盐sha1的加解密实现
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
    /// HmacSha1 算法
    /// </summary>
    public static class HmacSha1
    {
        /// <summary>
        /// 获取HmacSha1加密值 
        /// </summary>
        /// <param name="data">加密明文</param>
        /// <param name="key">秘钥</param>
        /// <param name="encoding">如果为空，则默认Utf-8</param>
        /// <returns> 加密后的字节流通过Base64转化 </returns>
        public static string EncryptBase64(string data, string key, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            var bytes = encoding.GetBytes(data);
            var keyBytes = encoding.GetBytes(key);

            byte[] resultbytes = Encrypt(keyBytes, bytes);

            return Convert.ToBase64String(resultbytes);
        }

        /// <summary>
        /// 返回加密后的密文
        /// </summary>
        /// <param name="data">加密明文</param>
        /// <param name="key">秘钥</param>
        /// <param name="encoding">如果为空，则默认Utf-8</param>
        /// <returns>加密后的字节流通过UTF-8转化</returns>
        public static string EncryptUtf8(string data, string key, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            var bytes = encoding.GetBytes(data);
            var keyBytes = encoding.GetBytes(key);

            byte[] resultbytes = Encrypt(keyBytes,  bytes);

            return Encoding.UTF8.GetString(resultbytes);
        }


        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static byte[] Encrypt(byte[] key, byte[] bytes)
        {
            byte[] resultbytes;
            using (var hmac = new HMACSHA1(key))
            {
                resultbytes = hmac.ComputeHash(bytes);
            }
            return resultbytes;
        }
    }
}
