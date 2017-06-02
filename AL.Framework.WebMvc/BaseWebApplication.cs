using AL.Common.Extention;
using AL.Common.Models;
using AL.Framework.WebMvc.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AL.Framework.WebMvc
{
    /// <summary>
    /// 全局配置基类
    /// </summary>
    public class BaseWebApplication : HttpApplication
    {
        /// <summary>
        /// 程序启动
        /// </summary>
        protected void Application_Start()
        {
            //注册区域控制器
            BaseAreaRegistration.RegisterAreas();
            //注册URL路由
            RegisterRoutes(RouteTable.Routes);
            //注册资源URL路由
            RegisterBundles(BundleTable.Bundles);
            //注册全局过滤器
            RegisterFilters(GlobalFilters.Filters);

            //绑定授权效验和错误返回消息
            Filter.AuthorizeAttribute.AuthorizeVerifyFailProvider = AuthorizeVerifyFail;
            Filter.AuthorizeAttribute.AuthorizeVerifyProvider = AuthorizeVerify;
        }

        /// <summary>
        /// 注册资源配置
        /// </summary>
        /// <param name="bundles"></param>
        public virtual void RegisterBundles(BundleCollection bundles)
        {
           
        }
        /// <summary>
        /// 注册全局过滤器配置
        /// </summary>
        /// <param name="filters"></param>
        public virtual void RegisterFilters(GlobalFilterCollection filters)
        {
            filters.Add(new FilterException());//错误处理过滤器
           // filters.Add(new FilterOperation());//操作日志记录过滤器
        }
        /// <summary>
        /// 注册URL路由配置
        /// </summary>
        /// <param name="routes"></param>
        public virtual void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("Areas/");
            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
          
        }
        /// <summary>
        /// 授权权限验证
        /// </summary>
        /// <returns></returns>
        public virtual ResultId AuthorizeVerify()
        {
            throw new ArgumentNullException("未配置授权效验提供者");
        }
        /// <summary>
        /// 授权失败返回处理
        /// </summary>
        /// <param name="vret"></param>
        /// <returns></returns>
        public virtual ActionResult AuthorizeVerifyFail(Result vret)
        {
            //调用默认授权失败返回消息处理
            string url = WebExtention.GetCurrentDomainUrl() + WebExtention.UrlEncode(HttpContext.Current.Request.RawUrl.ToString());
            //判断是否为Ajax请求
            if (WebExtention.IsAjax())
            {
                if (string.IsNullOrEmpty(vret.Message))
                    vret.Message = "登录超时,请重新登录！";
                //Ajax请求返回json数据
                Result ret = new Result(vret.Message, ResultTypes.UnAuthorize);
                JsonResult result = new JsonResult();
                result.Data = ret;
                return (ActionResult)result;
            }
            else
            {
                //没有登录跳转请问登录
                RouteValueDictionary route = new RouteValueDictionary();
                route.Add("action", "Index");
                route.Add("controller", "OAuth");
                route.Add("return_url", url);
                RedirectToRouteResult result = new RedirectToRouteResult(route);
                return result;
            }
        }

    }
}
