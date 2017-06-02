using System;
using System.Web;
using System.Web.Mvc;
using AL.Common.Modules.LogModule;
using AL.Common.Extention;

namespace AL.Framework.WebMvc.Filter
{
    /// <summary>
    /// 错误处理过滤器
    /// </summary>
    public class FilterException : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            Exception(filterContext);
        }

        public void Exception(ExceptionContext filterContext)
        {
            Exception exception = this.GetInnerException(filterContext.Exception);

            string Message = exception.Message;
            //获取报错信息
            string controller = filterContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RouteData.Values["action"].ToString();

            //判断是否提交非法参数请求
            if (exception is HttpRequestValidationException)
                filterContext.Result = (ActionResult)GetErrorResult("您提交了非法字符");
            else
            {
                //记录日志
                LogHelper.Error(exception,controller + "/"+ action+"-->"+Message);

                //返回错误信息
                filterContext.Result = (ActionResult)GetErrorResult("系统繁忙，请重试");
            }
            filterContext.HttpContext.Response.StatusCode = 200;
            filterContext.ExceptionHandled = true;
        }
        /// <summary>
        /// 获取内部异常
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private Exception GetInnerException(Exception ex)
        {
            if (ex.InnerException != null)
                return GetInnerException(ex.InnerException);
            else
                return ex;
        }

        /// <summary>
        /// 返回错误信息
        /// </summary>
        /// <param name="Message">错误消息</param>
        /// <param name="ViewName">页面名字</param>
        /// <returns></returns>
        private ActionResult GetErrorResult(string Message, string ViewName = "Error")
        {
            if (WebExtention.IsAjax())//判断是否是ajax请求
            {
                JsonResult result = new JsonResult();
                result.Data = new
                {
                    Success = false,
                    Status = 500,
                    Message = Message
                };
                return (ActionResult)result;
            }
            else
            {
                ViewResult view = new ViewResult();
                view.ViewName = ViewName;
                view.ViewBag.Message= Message;
                return (ActionResult)view;
            }
        }
    }
}
