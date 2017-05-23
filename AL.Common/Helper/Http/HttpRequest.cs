#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：Http请求方法
*
*　　	创建人： 阿凌
*       创建人Email：513845034@qq.com
*       
*****************************************************************************/

#endregion
using AL.Common.Extention;
using AL.Common.Modules.LogModule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AL.Common.Helper.Http
{
    /// <summary>
    /// Http请求
    /// </summary>
    public class HttpRequest
    {
        #region 属性 构造函数
        const string sUserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        /// <summary>
        /// 构造函数 编码默认为Utf-8   请求内容默认为x_www_form_urlencoded
        /// </summary>
        /// <param name="url">请求地址</param>
        public HttpRequest(string url) : this(url, ContentType.x_www_form_urlencoded) { }
        /// <summary>
        /// 构造函数 编码默认为Utf-8
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="type">请求内容类型</param>
        public HttpRequest(string url, ContentType type) : this(url, type, Encoding.UTF8, Encoding.UTF8)
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="type">请求内容类型</param>
        /// <param name="reqEncode">请求编码</param>
        /// <param name="respEncode">响应编码</param>
        public HttpRequest(string url, ContentType type, Encoding reqEncode, Encoding respEncode)
        {
            requestEncoding = reqEncode;
            responseEncoding = respEncode;
            addressUrl = url;
            method = HttpMethod.POST;
            contentType = type;
        }
        /// <summary>
        /// 请求编码
        /// </summary>
        public Encoding requestEncoding { get; set; }
        /// <summary>
        /// 响应编码
        /// </summary>
        public Encoding responseEncoding { get; set; }
        /// <summary>
        ///  请求地址
        /// </summary>
        public string addressUrl { get; set; }
        /// <summary>
        /// 请求方式
        /// </summary>
        public HttpMethod method { get; set; }
        /// <summary>
        /// 内容编码
        /// </summary>
        public ContentType contentType { get; set; }
        #endregion

        #region Post请求
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="data">请求参数</param>
        /// <returns></returns>
        public string PostRequest(string data)
        {
            HttpWebRequest request = PostWebRequest(data);

            Stream responseStream = request.GetResponse().GetResponseStream();
            StreamReader stream = new StreamReader(responseStream, responseEncoding);
            string stringResponse = stream.ReadToEnd();
            stream.Close();
            responseStream.Close();

            return stringResponse;
        }
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="data">请求参数</param>
        /// <returns></returns>
        public async Task<string> PostRequestAsync(string data)
        {
            HttpWebRequest request = PostWebRequest(data);
            WebResponse resp = await request.GetResponseAsync();
            Stream responseStream = resp.GetResponseStream();
            StreamReader stream = new StreamReader(responseStream, responseEncoding);
            string stringResponse = stream.ReadToEnd();
            stream.Close();
            responseStream.Close();

            return stringResponse;
        }
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="data">请求参数</param>
        /// <returns></returns>
        public HttpWebRequest PostWebRequest(string data)
        {
            try
            {
                this.method = HttpMethod.POST;
                HttpWebRequest httpRequest = WebRequest();
                if (data == null) data = "";
                //发送消息内容
                byte[] bytes = requestEncoding.GetBytes(data);
                httpRequest.ContentLength = bytes.Length;
                if (!string.IsNullOrEmpty(data))
                {
                    Stream requestStream = httpRequest.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
                return httpRequest;
            }
            catch (Exception ex)
            {
                throw new HttpRequestException(ex.Message + addressUrl, ex);
            }
        }

        #endregion

        #region Get请求
      
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="data">请求参数</param>
        /// <returns></returns>
        public string GetRequest(string data)
        {
            this.SetGetParam(data);
            HttpWebRequest request = WebRequest();
            Stream responseStream = request.GetResponse().GetResponseStream();
            StreamReader stream = new StreamReader(responseStream, responseEncoding);
            string stringResponse = stream.ReadToEnd();
            stream.Close();
            responseStream.Close();
            return stringResponse;
        }
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="data">请求参数</param>
        /// <returns></returns>
        public async Task<string> GetRequestAsync(string data)
        {
            this.SetGetParam(data);
            HttpWebRequest request = WebRequest();
            WebResponse resp = await request.GetResponseAsync();
            Stream responseStream = resp.GetResponseStream();
            StreamReader stream = new StreamReader(responseStream, responseEncoding);
            string stringResponse = stream.ReadToEnd();
            stream.Close();
            responseStream.Close();
            return stringResponse;
        }
        /// <summary>
        /// 设置Get请求参数
        /// </summary>
        /// <param name="data"></param>
        private void SetGetParam(string data)
        {
            this.method = HttpMethod.GET;
            if (string.IsNullOrEmpty(data))
                return;
            if (data.First().Equals("&"))
                data= data.Remove(0, 1);
            if (this.addressUrl.Contains("?"))
                this.addressUrl = this.addressUrl + "&" +data;
            else
                this.addressUrl = this.addressUrl + "?"+ data;
         
        }
        #endregion

        /// <summary>
        /// Http请求
        /// </summary>
        /// <returns></returns>
        public HttpWebRequest WebRequest()
        {
            if (string.IsNullOrEmpty(this.addressUrl))
                throw new HttpRequestException("请设置请求地址");
            //HttpClientCertificate
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
            HttpWebRequest httpRequest = System.Net.WebRequest.Create(addressUrl) as HttpWebRequest;
            if (httpRequest == null)
                throw new HttpRequestException("Invalid url string");

            httpRequest.Accept = "*/*";
            httpRequest.UserAgent = sUserAgent;
            httpRequest.Method = method.ToString();
            if (contentType == ContentType.json)
                httpRequest.ContentType = "application/json";
            else
                httpRequest.ContentType = "application/x-www-form-urlencoded";
            return httpRequest;
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

    }


}
