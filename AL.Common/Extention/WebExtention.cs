#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：Web上下文 Request,Response,Url 扩展类
*
*　　	创建人： 阿凌
*       创建人Email：513845034@qq.com
*       
*****************************************************************************/

#endregion

using System;
using System.Web;

namespace AL.Common.Extention
{
    /// <summary>
    /// Web上下文 Request,Response,Url 扩展类
    /// </summary>
    public static class WebExtention
    {
        #region Request
        /// <summary>
        /// 判断是否为Ajax提交
        /// </summary>
        /// <returns></returns>
        public static bool IsAjax()
        {
            return HttpContext.Current.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
        /// <summary>
        /// 获取客户端地址IP
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            string ip = HttpContext.Current.Request.UserHostAddress;
            if (ip.IndexOf("localhost") > -1 || ip.IndexOf("::1") > -1)
            {
                ip = "127.0.0.1";
                return ip;
            }
            else
            {
                if ((string.IsNullOrEmpty(ip) ? 0 : (ip.IsIP() ? 1 : 0)) == 0)
                    ip = "127.0.0.1";
                return ip;
            }

        }
        /// <summary>
        /// 获取当前域名
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDomainUrl()
        {
            int port = HttpContext.Current.Request.Url.Port ;
            if (port == 80 || port == 443)
                port = 0;
            return HttpContext.Current.Request .Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + ((port == 0) ? "" : (":" + HttpContext.Current.Request.Url.Port.ToString()));
        }

        /// <summary>
        /// 获取游客终端信息
        /// </summary>
        public static VisitorTerminal GetVisitorTerminal()
        {
            string userAgent = HttpContext.Current.Request.UserAgent;
            VisitorTerminal VT = new VisitorTerminal();
            VT.Time = DateTime.Now;

            if (string.IsNullOrWhiteSpace(userAgent))
                userAgent = "";
            VT.UserAgent = userAgent;

            string str2 = userAgent.ToLower();
            bool flag1 = str2.Contains("ipad");
            bool flag2 = str2.Contains("iphone os");
            bool flag3 = str2.Contains("midp");
            bool flag4 = str2.Contains("rv:1.2.3.4");
            bool flag5 = flag4 ? flag4 : str2.Contains("ucweb");
            bool flag6 = str2.Contains("android");
            bool flag7 = str2.Contains("windows ce");
            bool flag8 = str2.Contains("windows mobile");
            bool flag9 = str2.Contains("micromessenger");
            bool flag10 = str2.Contains("windows phone ");
            bool flag11 = str2.Contains("appwebview(ios)");
            VT.Terminal = EnumVisitorTerminal.PC;
            if (flag1 || flag2 || (flag3 || flag5) || (flag6 || flag7 || (flag8 || flag10)))
                VT.Terminal = EnumVisitorTerminal.Moblie;
            if (flag1 || flag2)
            {
                VT.OperaSystem = EnumVisitorOperaSystem.IOS;
                VT.Terminal = EnumVisitorTerminal.Moblie;
                if (flag1)
                    VT.Terminal = EnumVisitorTerminal.PAD;
                if (flag11)
                    VT.Terminal = EnumVisitorTerminal.IOS;
            }
            if (flag6)
            {
                VT.OperaSystem = EnumVisitorOperaSystem.Android;
                VT.Terminal = EnumVisitorTerminal.Moblie;
            }
            if (flag9)
                VT.Terminal = EnumVisitorTerminal.WeiXin;

            //判断是否为手机端
            if (VT.Terminal == EnumVisitorTerminal.Moblie || VT.Terminal == EnumVisitorTerminal.PAD || (VT.Terminal == EnumVisitorTerminal.WeiXin || VT.Terminal == EnumVisitorTerminal.IOS))
                VT.IsMobileTerminal = true;

            //获取客户端IP
            VT.ClientIP = WebExtention.GetIP();
            //获取访问地址
            VT.Uri = HttpContext.Current.Request.Url.AbsolutePath;
            return VT;
        }

        #endregion

        #region Response

        #region Cookie 方法
        /// 删除Cookie
        ///  <summary></summary>
        /// <param name="name">cookie名称</param>
        public static void DeleteCookie( string name)
        {
            HttpContext.Current.Response.AppendCookie(new HttpCookie(name)
            {
                Expires = DateTime.Now.AddYears(-1)
            });
        }
        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="name">cookie名称</param>
        /// <returns></returns>
        public static string GetCookie(string name)
        {
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies[name];
            return httpCookie == null ? string.Empty : httpCookie.Value;
        }
        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="name">cookie集合名称</param>
        /// <param name="key">cookie key</param>
        /// <returns></returns>
        public static string GetCookie(string name, string key)
        {
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies[name];
            string str1;
            if ((httpCookie == null ? 1 : (!httpCookie.HasKeys ? 1 : 0)) == 0)
            {
                string str2 = httpCookie[key];
                if (str2 != null)
                {
                    str1 = str2;
                    goto label_4;
                }
            }
            str1 = string.Empty;
            label_4:
            return str1;
        }
        /// <summary>
        /// 设置Cookie 
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        public static void SetCookie(string name, string value)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie != null)
                cookie.Value = value;
            else
                cookie = new HttpCookie(name, value);
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        /// <param name="dt">有效时间</param>
        public static void SetCookie(string name, string value, DateTime dt)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name] ?? new HttpCookie(name);
            cookie.Value = value;
            cookie.Expires = dt;
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="name">Cookie名字</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetCookie(string name, string key, string value)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name] ?? new HttpCookie(name);
            cookie[key] = value;
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="name">Cookie名字</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="dt">有效时间</param>
        public static void SetCookie(string name, string key, string value, DateTime dt)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name] ?? new HttpCookie(name);
            cookie[key] = value;
            cookie.Expires = dt;
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        #endregion

        #region Session 方法

        /// <summary>
        /// 设置Session
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetSession(string key, object value)
        {
            HttpContext.Current.Session[key] = value;
        }
        /// <summary>
        /// 获取Session值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetSession<T>(string key)
        {
            try
            {
                if (HttpContext.Current.Session[key] == null)
                    return default(T);
                return (T)HttpContext.Current.Session[key];
            }
            catch (Exception)
            {
                return default(T);
            }
        }


        #endregion

        #endregion

        #region URL
        /// <summary>
        /// Html 解码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string s)
        {
            return HttpUtility.HtmlDecode(s);
        }
        /// <summary>
        /// Html 编码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string HtmlEncode(this string s)
        {
            return HttpUtility.HtmlEncode(s);
        }
        /// <summary>
        /// URL 解码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string UrlDecode(this string s)
        {
            return HttpUtility.UrlDecode(s);
        }
        /// <summary>
        /// URL 编码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string UrlEncode(this string s)
        {
            return HttpUtility.UrlEncode(s);
        }
        #endregion
    }


    /// <summary>
    /// 终端类型
    /// </summary>
    public enum EnumVisitorTerminal
    {
        Android = 5,
        IOS = 4,
        Moblie = 1,
        Other = 0x63,
        PAD = 2,
        PC = 0,
        WeiXin = 3
    }
    /// <summary>
    /// 终端系统类型
    /// </summary>
    public enum EnumVisitorOperaSystem
    {
        Android = 1,
        IOS = 2,
        Linux = 3,
        MacOS = 5,
        Other = 0x63,
        UNIX = 4,
        Windows = 0,
        WindowsCE = 7,
        WindowsMobile = 8,
        WindowsPhone = 6
    }

    /// <summary>
    /// 访问终端信息
    /// </summary>
    public class VisitorTerminal
    {
        /// <summary>
        /// 是否手机端
        /// </summary>
        public bool IsMobileTerminal { get; set; }
        /// <summary>
        /// 控制器
        /// </summary>
        public string Controller { get; set; }
        /// <summary>
        /// 请问地址
        /// </summary>
        public string Uri { get; set; }
        /// <summary>
        /// 终端系统类型
        /// </summary>
        public EnumVisitorOperaSystem OperaSystem { get; set; }
        /// <summary>
        /// 终端类型
        /// </summary>
        public EnumVisitorTerminal Terminal { get; set; }
        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIP { get; set; }
        /// <summary>
        /// HTTP 请求的 User-Agent 标头的值
        /// </summary>
        public string UserAgent { get; set; }
        /// <summary>
        /// 访问时间
        /// </summary>
        public DateTime Time { get; set; }
    }
}
