using System.Web.Mvc;
using AL.Common.Extention;
using AL.Common.Models;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AL.Framework.WebMvc
{
    /// <summary>
    /// MVC基类
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// 行为执行事件
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            VisitorTerminal vt = WebExtention .GetVisitorTerminal();
            if (vt.IsMobileTerminal)
            {
                //this.JumpMobileUrl(filterContext.RouteData, "");//跳转至移动网站
            }
            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 错误事件
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {

            base.OnException(filterContext);
            //FilterException ex = new FilterException();
            //ex.Exception(filterContext);
       
        }
        /// <summary>
        /// 返回至错误页面
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public ActionResult Error(string Message)
        {
            ViewBag.Message = Message;
            return base.View("Error");
        }

        /// <summary>
        /// 验证请求数据
        /// </summary>
        /// <returns></returns>
        public Result IsValid(object data)
        {
            if (data == null)
                return new Result("参数不能为空", ResultTypes.ParaError);
            if (!ModelState.IsValid)
            {
                List<ModelState> msList = ModelState.Values.ToList();
                StringBuilder Message = new StringBuilder();
                for (int i = 0; i < msList.Count; i++)
                {
                    ModelState ms = msList[i];
                    foreach (ModelError error in ms.Errors)
                    {
                        if (error.Exception != null || string.IsNullOrEmpty(error.ErrorMessage))
                            continue;
                        Message.Append(error.ErrorMessage + "\r");
                    }
                }
                if (Message.Length == 0)
                    Message.Append("参数验证失败");
                return new Result(Message.ToString(), ResultTypes.ParaError);
            }
            else return new Result("success", true);
        }

    }
}
