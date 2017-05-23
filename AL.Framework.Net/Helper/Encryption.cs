using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Zzll.Net.Framework.Helper
{
    /// <summary>
    /// 报文加密
    /// </summary>
    public class Encryption
    {
        #region 加密方法
        /// <summary>
        /// 密钥
        /// </summary>
        private const string Key = "a1b2c3d4e5f6g7h8i9j0k101";
        // 格式化md5 hash 字节数组所用的格式（两位小写16进制数字）
        private static readonly string m_strHexFormat = "x2";

        /// <summary>
        /// 取动态密钥
        /// </summary>
        /// <returns></returns>
        public static string getKey(string key = Key)
        {
            //为了防止出现请求端与响应端时间有差别而导致密钥不一致的问题，凌晨1点以前的日期取前一天
            DateTime now = DateTime.Now;
            if (now.Hour < 1) now = now.AddDays(-1);
            int bgi = now.Year + now.Month * 31 + now.Day;
            bgi = bgi % key.Length;
            return key.Substring(bgi) + key.Substring(0, bgi);
        }

        #region DES ECB模式
        /// <summary>
        /// DES3 3DES加密  ECB模式解密
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV(当模式为ECB时，IV无用)</param>
        /// <param name="str">密文的byte数组</param>
        /// <returns>明文的byte数组</returns>
        private static byte[] Des3DecodeECB(byte[] key, byte[] iv, byte[] data)
        {
            try
            {
                // Create a new MemoryStream using the passed 
                // array of encrypted data.
                MemoryStream msDecrypt = new MemoryStream(data);

                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.ECB;
                tdsp.Padding = PaddingMode.PKCS7;

                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                    tdsp.CreateDecryptor(key, iv),
                    CryptoStreamMode.Read);

                // Create buffer to hold the decrypted data.
                byte[] fromEncrypt = new byte[data.Length];

                // Read the decrypted data out of the crypto stream
                // and place it into the temporary buffer.
                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

                //Convert the buffer into a string and return it.
                return fromEncrypt;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }

        /// <summary>
        /// DES3 3DES加密 ECB模式加密
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV(当模式为ECB时，IV无用)</param>
        /// <param name="str">明文的byte数组</param>
        /// <returns>密文的byte数组</returns>
        private static byte[] Des3EncodeECB(byte[] key, byte[] iv, byte[] data)
        {
            try
            {
                // Create a MemoryStream.
                MemoryStream mStream = new MemoryStream();

                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.ECB;
                tdsp.Padding = PaddingMode.PKCS7;
                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                CryptoStream cStream = new CryptoStream(mStream,
                    tdsp.CreateEncryptor(key, iv),
                    CryptoStreamMode.Write);

                // Write the byte array to the crypto stream and flush it.
                cStream.Write(data, 0, data.Length);
                cStream.FlushFinalBlock();

                // Get an array of bytes from the 
                // MemoryStream that holds the 
                // encrypted data.
                byte[] ret = mStream.ToArray();

                // Close the streams.
                cStream.Close();
                mStream.Close();

                // Return the encrypted buffer.
                return ret;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }

        }


        #endregion

        #region DES模式
        /// <summary>
        /// 3重DES加密/Desede加密， 与java语言中使用的3DES加密一致
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="key">密钥</param>
        /// <returns>密文(16进制字符串)</returns>
        public static string Encrypt3ECB(string data, string key, Encoding Encode)
        {

            //首先将密钥转成字节数组，双方都采用相同编码
            byte[] keybytes = Encode.GetBytes(key);
            //加密数据也同样的采用GBK编码，转成字节数组
            byte[] databytes = Encode.GetBytes(data);

            //使用C#的 ECB Mode, PKCS7 Padding  3DES加密
            byte[] iv = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };      //当模式为ECB时，IV无用
            byte[] encryptbytes = Des3EncodeECB(keybytes, iv, databytes);
            //将加密得到的字节数组转成16进制字符串作为密文返回
            StringBuilder ciphertext = new StringBuilder();
            foreach (byte b in encryptbytes)
            {
                //转化为16进制字符小写字母加数字
                ciphertext.AppendFormat("{0:x2}", b);
            }
            //返回加密后的密文字符串
            return ciphertext.ToString();
        }

        /// <summary>
        /// 3重DES解密/Desede解密， 与java语言中使用的3DES解密一致
        /// </summary>
        /// <param name="ciphertext">密文(16进制字符串)</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string Decrypt3ECB(string ciphertext, string key, Encoding Encode)
        {
            //首先将密钥转成字节数组
            byte[] keybytes = Encode.GetBytes(key);

            byte[] encryptbytes = new byte[ciphertext.Length / 2];    //与转成16进制字符串相反，两个16进制字符串转成一个字节，所以字节数组的长度为字符串长度的一半.
            for (int i = 0; i < (ciphertext.Length / 2); i++)
            {
                int num2 = Convert.ToInt32(ciphertext.Substring(i * 2, 2), 0x10);      // 0x10转换(转成1位16进制数)， 两个字符转成一个字节
                encryptbytes[i] = (byte)num2;
            }
            //这样就得到了密文的字节数组

            //接下来就是解密
            //使用C#的 ECB Mode, PKCS7 Padding  3DES加密
            byte[] iv = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };      //当模式为ECB时，IV无用
            byte[] databytes = Des3DecodeECB(keybytes, iv, encryptbytes);                     //因为C++在字符串末尾加\0的问题，解密出来的字符串后面往往会有一些\0  这是byte数组中值为0的byte元素编码成字符的结果 

            //解密失败的情况
            if (databytes == null) return null;

            //将得到的明文数据的字节数组使用GBK编码
            string texts = Encode.GetString(databytes);

            //使用StringBuilder的替换，解决字符中有\0的问题     
            StringBuilder sb = new StringBuilder(texts);
            sb.Replace("\0", "");
            return sb.ToString();
        }


        /// <summary>
        /// 进行DES解密。
        /// </summary>
        /// <param name="pToDecrypt">要解密的以Base64</param>
        /// <param name="sKey">密钥，且必须为8位。</param>
        /// <returns>已解密的字符串。</returns>
        public static string DecryptDES(string pToDecrypt, Encoding Encode)
        {
            try
            {
                string sKey = "xmxmdcmz";
                byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                    des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        cs.Close();
                    }
                    string str = Encode.GetString(ms.ToArray());
                    ms.Close();
                    return str;
                }
            }
            catch (Exception)
            {
                return pToDecrypt;
            }
        }
        /// <summary>
        /// 进行DES加密
        /// </summary>
        /// <param name="strPwd"></param>
        /// <returns></returns>
        public static string EncryptDES(string strPwd, Encoding Encode)
        {
            //获取加密服务  
            System.Security.Cryptography.MD5CryptoServiceProvider md5CSP = new System.Security.Cryptography.MD5CryptoServiceProvider();

            //获取要加密的字段，并转化为Byte[]数组  
            byte[] testEncrypt = Encode.GetBytes(strPwd);

            //加密Byte[]数组  
            byte[] resultEncrypt = md5CSP.ComputeHash(testEncrypt);

            //将加密后的数组转化为字段(普通加密)  
            string testResult = Encode.GetString(resultEncrypt);

            //作为密码方式加密   
            string pwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(strPwd, "MD5");
            return pwd;
        }
        #endregion

        #region MD5模式
        /// <summary>
        /// 对字符串进行md5加密
        /// </summary>
        /// <param name="str">需要进行md5演算的字符串</param>
        /// <param name="charEncoder">
        /// 指定对输入字符串进行编解码的 Encoding 类型
        /// </param>
        /// <returns>用小写字母表示的32位16进制数字字符串</returns>
        public static string EncryptMD5(string str, Encoding encoder)
        {
            byte[] bytesOfStr = EncryptMD5s(str, encoder);
            int bLen = bytesOfStr.Length;
            StringBuilder pwdBuilder = new StringBuilder(32);
            for (int i = 0; i < bLen; i++)
            {
                pwdBuilder.Append(bytesOfStr[i].ToString(m_strHexFormat));
            }
            return pwdBuilder.ToString();
        }

        /// <summary>
        /// 对字符串进行md5加密
        /// </summary>
        /// <param name="str">需要进行md5演算的字符串</param>
        /// <param name="charEncoder">
        /// 指定对输入字符串进行编解码的 Encoding 类型
        /// </param>
        /// <returns>长度16 的 byte[] 数组</returns>
        public static byte[] EncryptMD5s(string str, Encoding encoder)
        {
            System.Security.Cryptography.MD5 md5 =
                System.Security.Cryptography.MD5.Create();
            return md5.ComputeHash(encoder.GetBytes(str));
        }


        /// <summary>
        ///  MD5 16位加密
        /// </summary>
        /// <param name="str">需要进行md5演算的字符串</param>
        /// <returns></returns>
        public static string EncryptMD5_ServiceProvider(string str, Encoding encoder)
        {
            byte[] oriBt = encoder.GetBytes(str);
            System.Security.Cryptography.MD5 CryptSrv = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] cryBt = CryptSrv.ComputeHash(oriBt);
            return BitConverter.ToString(cryBt);
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// 加密方式
    /// </summary>
    public enum EncryptType
    {
        /// <summary>
        /// 正常不加密
        /// </summary>
        Normal,
        /// <summary>
        /// DES3层加密 (推荐)
        /// </summary>
        DES3,
        /// <summary>
        /// MD5 签名
        /// </summary>
        MD5
    }
}
