using AL.Common.Helper.Encrypt;
using AL.Framework.MVC.Bundle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Optimization;

namespace AL.Framework.MVC
{
    /// <summary>
    /// CSS 资源注册属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class StylesAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Js脚本分组名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// Js脚本
        /// </summary>
        private string[] styles { get; set; }
 
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="styles">CSS脚本路径数组</param>
        public StylesAttribute(params string[] styles) 
        {
            this.styles = styles;
        }

     

        /// <summary>
        /// 方法执行时调用
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //获取页面定位
            string controller = filterContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RouteData.Values["action"].ToString();
            object area = filterContext.RouteData.Values["area"];
            string key = string.Format("{0}/{1}/{2}", area, controller, action);

            if (ALStyles.BundleStyles.ContainsKey(key))
                return;

            //注册资源
            if (!string.IsNullOrEmpty(this.GroupName))
                ALStyles.BundlesGroup(key, this.GroupName);
            else
                ALStyles.BundlesAdd(key, styles);
            base.OnActionExecuting(filterContext);
        }
    }
}
