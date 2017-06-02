using AL.Common.Extention;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AL.Framework.WebMvc.Filter
{
    /// <summary>
    /// 是否开启Https超文本传输安全协议 过滤器
    /// </summary>
    public class FilterSwitchHttps : RequireHttpsAttribute
    {
        /// <summary>
        /// 重写验证方法,判断是否需要https,如果需要https,就交给父类的方法处理,如果不需要,就自己处理
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            SwitchHttpsAttribute https = new SwitchHttpsAttribute();
            https.RequireSecure = true;
            https.OnAuthorization(filterContext);
        }
    }

    /// <summary>
    /// (属性)开启Https超文本传输安全协议
    /// </summary>
    public class SwitchHttpsAttribute : RequireHttpsAttribute
    {
        /// <summary>
        /// 字段表示是否需要安全的https链接,默认不需要
        /// </summary>
        public bool RequireSecure = false;

        /// <summary>
        /// 重写验证方法,判断是否需要https,如果需要https,就交给父类的方法处理,如果不需要,就自己处理
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (RequireSecure)
            {
                //需要https,执行父类方法,转化为https
                base.OnAuthorization(filterContext);
            }
            else
            {
                //如果设置为非安全链接,即http,进入该区块
                //判断链接,如果为https,这转换为http
                if (filterContext.HttpContext.Request.IsSecureConnection)
                {

                    HandleNonHttpRequest(filterContext);
                }
            }
        }

        /// <summary>
        /// 重写处理链接方法,处理https请求,使其重定向http
        /// </summary>
        /// <param name="filterContext"></param>
        protected virtual void HandleNonHttpRequest(AuthorizationContext filterContext)
        {
            if (String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                string url = WebExtention.GetCurrentDomainUrl() + filterContext.HttpContext.Request.RawUrl.ToString();
                //重定向
                filterContext.Result = new RedirectResult(url);
            }
        }

    }
}
