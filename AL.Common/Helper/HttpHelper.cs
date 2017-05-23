#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：Http请求(简易)
*
*　　	创建人： 阿凌
*       创建人Email：513845034@qq.com
*       
*****************************************************************************/

#endregion
using AL.Common.Helper.Http;
using AL.Common.Modules.LogModule;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AL.Common.Helper
{
    /// <summary>
    /// Http 请求方法
    /// </summary>
    public class HttpHelper
    {
        #region Form-Html
        const string FormFormat = "<form id=\"payform\" name=\"payform\" action=\"{0}\" method=\"POST\">{1}</form>";
        const string InputFormat = "<input type=\"hidden\" id=\"{0}\" name=\"{0}\" value=\"{1}\">";
        /// <summary>
        /// 请求(POST方式)
        /// </summary>
        /// <param name="formContent"></param>
        public static void SubmitForm(string formContent)
        {
            string submitscr = formContent + "<script>document.forms['payform'].submit();</script>";
            HttpContext.Current.Response.Write(submitscr);
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 创建FORM
        /// </summary>
        /// <param name="content">字段集合</param>
        /// <param name="action">提交地址</param>
        /// <returns></returns>
        public static string CreateForm(string content, string action)
        {
            content += "<input type=\"submit\" value=\"确定\" style=\"display:none;\">";
            return String.Format(CultureInfo.InvariantCulture, FormFormat, action, content);
        }
        /// <summary>
        /// 创建FORM
        /// </summary>
        /// <param name="content">字段集合</param>
        /// <param name="action">提交地址</param>
        /// <returns></returns>
        public static string CreateForm(Dictionary<string, string> list, string action)
        {
            string content = "";
            if (list == null) return "";
            foreach (string key in list.Keys)
            {
                content += CreateField(key, list[key].Trim());
            }
            content += "<input id=\"form_submit\" type=\"submit\" value=\"在线入金\" style=\"display:none;\">";
            return String.Format(CultureInfo.InvariantCulture, FormFormat, action, content);
        }
        /// <summary>
        /// 创建字段
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="strValue">参数值</param>
        /// <returns></returns>
        public static string CreateField(string name, string strValue)
        {
            return String.Format(CultureInfo.InvariantCulture, InputFormat, name, strValue);
        }
        #endregion

        #region Form-Data
        const string sUserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        const string sContentType = "application/x-www-form-urlencoded";
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="data">请求参数  格式：a=1&b=2&...</param>
        /// <param name="url">访问地址</param>
        /// <param name="requestEncoding">请求编码</param>
        /// <param name="responseEncoding">响应编码</param>
        /// <returns>服务器响应</returns>
        public static string CreatePostHttpResponse(string data, string url, Encoding requestEncoding, Encoding responseEncoding)
        {
            StreamReader stream = CreateHttpResponse(data, url, requestEncoding, responseEncoding, HttpMethod.POST);
            string stringResponse = stream.ReadToEnd();
            stream.Close();
            return stringResponse;
        }
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="data">请求参数  格式：a=1&b=2&...</param>
        /// <param name="url">访问地址</param>
        public static async Task<string> CreatePostHttpResponseAsync(string data, string url)
        {
            StreamReader stream = CreateHttpResponse(data, url, Encoding.UTF8, Encoding.UTF8, HttpMethod.POST);
            string stringResponse = await stream.ReadToEndAsync();
            stream.Close();
            return stringResponse;
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="data">请求参数  格式：a=1&b=2&...</param>
        /// <param name="url">访问地址</param>
        /// <param name="responseEncoding">响应编码</param>
        /// <returns></returns>
        public static string CreateGetHttpResponse(string data, string url, Encoding responseEncoding)
        {
            if (url.Contains("?"))
                url += data;
            else
                url += "?" + data;
            StreamReader stream = CreateHttpResponse("", url, responseEncoding, responseEncoding, HttpMethod.GET);
            string stringResponse = stream.ReadToEnd();
            stream.Close();
            return stringResponse;
        }
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="data">请求参数  格式：a=1&b=2&...</param>
        /// <param name="url">访问地址</param>
        /// <param name="responseEncoding">响应编码</param>
        /// <returns></returns>
        public static async Task<string> CreateGetHttpResponseAsync(string data, string url, Encoding responseEncoding)
        {
            if (url.Contains("?"))
                url += data;
            else
                url += "?" + data;
            StreamReader stream = CreateHttpResponse("", url, responseEncoding, responseEncoding, HttpMethod.GET);
            string stringResponse = await stream.ReadToEndAsync();
            stream.Close();
            return stringResponse;
        }
        /// <summary>
        /// Http 请求
        /// </summary>
        /// <param name="data">请求的数据</param>
        /// <param name="url">目标url</param>
        /// <param name="url">响应编码</param>
        /// <returns>服务器响应</returns>
        public static StreamReader CreateHttpResponse(string data, string url, Encoding requestEncoding, Encoding responseEncoding, HttpMethod method)
        {
            try
            {
                #region 创建httpWebRequest对象
                //HttpClientCertificate
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                HttpWebRequest httpRequest = WebRequest.Create(url) as HttpWebRequest;
                if (httpRequest == null)
                    throw new HttpRequestException(string.Format("Invalid url string: {0}", url), null);

                httpRequest.Accept = "*/*";
                httpRequest.UserAgent = sUserAgent;
                httpRequest.ContentType = sContentType;
                httpRequest.Method = method.ToString();

                //设置发送内容
                if (method != HttpMethod.GET)
                {
                    if (data == null)
                        data = "";
                    byte[] bytes = requestEncoding.GetBytes(data);
                    httpRequest.ContentLength = bytes.Length;
                    Stream requestStream = httpRequest.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
                #endregion

                #region 获取请求返回数据流
                Stream responseStream = httpRequest.GetResponse().GetResponseStream();
                return new StreamReader(responseStream, responseEncoding);
                #endregion

            }
            catch (Exception ex)
            {
                throw new HttpRequestException(ex.Message+ url, ex);
            }

        }
        /// <summary>
        /// 验证Https SSL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {   // 总是接受    
            return true;
        }


        #endregion

     
    }

  
}
