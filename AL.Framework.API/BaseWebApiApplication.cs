using AL.Common.Extention;
using AL.Common.Models;
using AL.Framework.API.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace AL.Framework.API
{
    /// <summary>
    /// WebApi 全局配置基类
    /// </summary>
    public class BaseWebApiApplication : HttpApplication
    {
        /// <summary>
        /// 程序启动
        /// </summary>
        protected void Application_Start()
        {
            //注册WebApi
            GlobalConfiguration.Configure(new Action<HttpConfiguration>(Register));
        }
        
        /// <summary>
        /// 注册WebApi配置
        /// </summary>
        /// <param name="config"></param>
        protected void Register(HttpConfiguration config)
        {
            //映射应用程序的特性定义路由
            config.MapHttpAttributeRoutes();
            //移除XML媒体类型格式化
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            //注册URL路由配置
            RegisterRoutes(config.Routes);
            //注册过滤器配置
            RegisterFilters(config.Filters);

            //绑定授权效验和错误返回消息
            Filter.AuthorizeAttribute.AuthorizeVerifyProvider = AuthorizeVerify;
        }

        /// <summary>
        /// 注册URL路由配置
        /// </summary>
        /// <param name="routes"></param>
        public virtual void RegisterRoutes(HttpRouteCollection routes)
        {
            routes.MapHttpRoute("DefaultApi", "{controller}/{action}/{id}", new { id = RouteParameter.Optional });
        }

        /// <summary>
        /// 注册过滤器配置
        /// </summary>
        /// <param name="filters"></param>
        public virtual void RegisterFilters(HttpFilterCollection filters)
        {
            //添加筛选器
            filters.Add(new FilterException());//错误日志筛选器
            filters.Add(new FilterOperation());  //访问采集筛选器
            filters.Add(new FilterMonitor());  //性能监控筛选器
        }

        /// <summary>
        /// 授权权限验证
        /// </summary>
        /// <param name="Token">授权Token</param>
        /// <returns></returns>
        public virtual ResultId AuthorizeVerify(string Token)
        {
            throw new ArgumentNullException("未配置授权效验提供者");
        }
       
      

    }
}