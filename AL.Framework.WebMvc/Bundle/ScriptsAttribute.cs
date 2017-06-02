using System;
using System.Web.Mvc;
using System.Web.Optimization;
using AL.Common.Helper.Encrypt;
using AL.Framework.WebMvc.Bundle;

namespace AL.Framework.WebMvc
{
    /// <summary>
    /// JS 资源注册属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method| AttributeTargets.Assembly, AllowMultiple = true)]
    public class ScriptsAttribute : ActionFilterAttribute
    {

        /// <summary>
        /// Js脚本
        /// </summary>
        private string[] scripts { get; set; }
        /// <summary>
        /// Js脚本分组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="scripts">js脚本路径数组</param>
        public ScriptsAttribute(params string[] scripts)
        {
            this.scripts = scripts;
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

            if (ALScripts.BundleScripts.ContainsKey(key))
                return;

            //注册资源
            if (!string.IsNullOrEmpty(this.GroupName))
                ALScripts.BundlesGroup(key, this.GroupName, this.scripts);
            else
                ALScripts.BundlesAdd(key, scripts);

            base.OnActionExecuting(filterContext);
        }
    }



}
