using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using AL.Common.Extention;
using AL.Common.Models;

namespace AL.Framework.MVC.Filter
{
    /// <summary>
    /// 授权过滤器
    /// </summary>
    public class AuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// 授权验证提供者
        /// </summary>
        public static Func<ResultId> AuthorizeVerifyProvider { get; set; }
        /// <summary>
        /// 授权验证失败返回消息提供者
        /// </summary>
        public static Func<Result, ActionResult> AuthorizeVerifyFailProvider { get; set; }
        /// <summary>
        /// 授权登录处理
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;
            //如果有该属性就不需授权
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(UnAuthorize), false).Length > 0)
                return;

            //调用提供者验证是否授权
            ResultId vret = AuthorizeVerifyProvider();
            //判断访问是否授权
            if (vret.Success)
            {
                filterContext.RouteData.Values.Add("USERID", vret.Id);
                return;
            }

            filterContext.Result = AuthorizeVerifyFailProvider((Result)vret);

            
        }
    }


    /// <summary>
    /// 取消授权
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UnAuthorize : Attribute
    {
    }

}


