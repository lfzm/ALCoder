using AL.Common.Models;
using AL.Common.Modules.LogModule;
using AL.Common.Extention;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace AL.Framework.API.Filter
{
    /// <summary>
    /// 授权筛选器
    /// </summary>
    public class AuthorizeAttribute : AuthorizationFilterAttribute
    {
        /// <summary>
        /// 授权验证提供者
        /// </summary>
        public static Func<string, ResultId> AuthorizeVerifyProvider { get; set; }
        /// <summary>
        /// 是否必须使用Https协议
        /// </summary>
        public static bool IsUriSchemeHttps { get; set; }

        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var request = actionContext.Request;
            //判断必须使用https访问接口
            if (request.RequestUri.Scheme != Uri.UriSchemeHttps && IsUriSchemeHttps)
                return HandleUnauthorizedRequest(actionContext, new Result("Https is required", false, ResultTypes.NoRight));
            //判断是否需要授权判断
            if (actionContext.ActionDescriptor.GetCustomAttributes<UnAuthorize>().Count > 0)
                return base.OnAuthorizationAsync(actionContext, cancellationToken);

            //获取token 判断用户是否授权
            NameValueCollection list = request.RequestUri.ParseQueryString();
            string token = list["token"];
            if (string.IsNullOrEmpty(token))
                return HandleUnauthorizedRequest(actionContext, new Result("Token is required", false, ResultTypes.ParaError) );

            //调用提供者验证是否授权
            ResultId vret = AuthorizeVerifyProvider(token);
            if (!vret.Success)
                return HandleUnauthorizedRequest(actionContext, vret);
            //把访问用户存储
            actionContext.RequestContext.RouteData.Values.Add("USERID", vret.Id);
            actionContext.RequestContext.RouteData.Values.Add("TOKEN", token);
            return base.OnAuthorizationAsync(actionContext, cancellationToken);
        }

        /// <summary>
        /// 授权失败处理
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="vret">返回消息</param>
        private Task HandleUnauthorizedRequest(HttpActionContext actionContext, Result vret)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            Result res = new Result(vret.Message,vret.Success);
            res.Status = vret.Status;
            
            response.Content = new StringContent(JsonExtention.ToJsonString(res), Encoding.GetEncoding("UTF-8"), "application/json");
            response.StatusCode = HttpStatusCode.OK;
            actionContext.Response = response;

            //授权失败，收集访问信息
            VisitorTerminal vt = AL.Common.Extention.WebExtention.GetVisitorTerminal();
            LogHelper.Info(vt);//存储
            return Task.FromResult<object>(null);
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
