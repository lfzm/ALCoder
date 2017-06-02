using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AL.Common.Extention;
using AL.Common.Modules.LogModule;

namespace AL.Framework.WebMvc.Filter
{
    /// <summary>
    /// 操作日志过滤器(属性 配置)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OperationLogAttribute : ActionFilterAttribute
    {
        #region 属性
        /// <summary>
        /// 需要记录参数
        /// </summary>
        public string ParameterNameList { get; set; }
        /// <summary>
        /// 操作消息
        /// </summary>
        public string Message { get; set; }
        #endregion

        public OperationLogAttribute()
        {
        }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="message">操作消息</param>
        /// <param name="parameterNameList">需要记录参数</param>
        public OperationLogAttribute(string message, string parameterNameList = "")
        {
            this.Message = message;
            this.ParameterNameList = parameterNameList;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception == null)
            {
                //获取方法上下文
                string controller = filterContext.RouteData.Values["controller"].ToString();
                string action = filterContext.RouteData.Values["action"].ToString();
                object area = filterContext.RouteData.Values["area"];
                //返回消息
                string message = filterContext.Controller.ViewBag.Message;

                //获取请求Form和QueryString参数
                NameValueCollection formcoll = filterContext.Controller.ControllerContext.HttpContext.Request.Form;
                NameValueCollection querycoll =  filterContext.Controller.ControllerContext.HttpContext.Request.QueryString;


                StringBuilder builder = new StringBuilder();
                if (!string.IsNullOrEmpty(this.ParameterNameList))
                {
                    //记录设置的参数
                    foreach (string key in this.ParameterNameList.Split(new char[] { ',', '|' }))
                    {
                        string value = formcoll[key];
                        if (string.IsNullOrEmpty(value))
                            value = querycoll[key];
                        if (!string.IsNullOrEmpty(value))
                        {
                            builder.AppendFormat("{0}={1}&", key, value);
                        }
                    }
                }
                else
                {
                    //未设置记录参数，就默认记录全部Form和QueryString参数
                    foreach (string key in formcoll.Keys)
                    {
                        builder.AppendFormat("{0}={1}&", key, formcoll[key]);
                    }
                    foreach (string key in querycoll.Keys)
                    {
                        builder.AppendFormat("{0}={1}&", key, querycoll[key]);
                    }
                }

                if (string.IsNullOrEmpty(message))
                    message = "";
                if (string.IsNullOrEmpty(this.Message))
                    this.Message = "";
                dynamic log = new
                {
                    action_name = action,//请求方法
                    controller_name = area + "/" + controller,//请求控制器
                    action_data = builder.ToString(),//请求参数
                    action_ip = filterContext.RequestContext.HttpContext.Request.UserHostAddress,//请求客户端IP
                    action_time = DateTime.Now,
                    action_title = this.Message,
                    remark = message,
                    user_id = (filterContext.Controller is BaseOAuthController)? (filterContext.Controller as BaseOAuthController).UserId:0
                };
            
                //记录日志
                message = JsonExtention .ToJsonString(log);
                LogHelper.Info(message);

                base.OnActionExecuted(filterContext);
            }
        }
    }

    /// <summary>
    /// (过滤器)操作日志过滤器(FilterConfig 配置)
    /// </summary>
    public class FilterOperation : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            OperationLogAttribute log = new OperationLogAttribute();
            log.OnActionExecuted(filterContext);
        }
    }
}
