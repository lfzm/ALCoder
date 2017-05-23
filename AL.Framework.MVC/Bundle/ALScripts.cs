using AL.Common.Helper.Encrypt;
using AL.Framework.MVC.Bundle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

namespace AL.Framework.MVC
{
    /// <summary>
    /// Js脚本资源
    /// </summary>
    public static class ALScripts
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        static ALScripts()
        {
            BundleScripts = new Dictionary<string, Bundles>();
        }
        /// <summary>
        /// JS脚本资源缓存
        /// </summary>
        public static Dictionary<string, Bundles> BundleScripts { get; set; }
        /// <summary>
        /// JS资源基础分组名称
        /// </summary>
        public static string BasisName { get; set; }
        public static IHtmlString Render()
        {
            RequestContext filterContext = HttpContext.Current.Request.RequestContext;
            string controller = filterContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RouteData.Values["action"].ToString();
            object area = filterContext.RouteData.Values["area"];
            string name = string.Format("{0}/{1}/{2}", area, controller, action);

            //获取页面定位
            if (!BundleScripts.ContainsKey(name))
                name = BasisName;
            Bundles b = BundleScripts[name];

            IHtmlString html = Scripts.Render(b.virtualPath);
            return html;
        }


        /// <summary>
        /// JS脚本资源绑定添加
        /// </summary>
        /// <param name="name">JS脚本资源绑定名称</param>
        /// <param name="scripts">js脚本路径数组</param>
        public static void BundlesAdd(string name, params string[] scripts)
        {
            if (!BundleScripts.ContainsKey(name))
            {
                string virtualPath = "~/" + Md5.HalfEncryptHexString(name + "/Script");
                BundleTable.Bundles.Add(new ScriptBundle(virtualPath).Include(scripts));
                Bundles b = new Bundles()
                {
                    item = scripts,
                    name = name,
                    virtualPath = virtualPath
                };
                if (BundleScripts.Count == 0 && string.IsNullOrEmpty(BasisName))
                    BasisName = name;
                BundleScripts.Add(name, b);
            }
        }

        /// <summary>
        /// JS脚本资源分组绑定
        /// </summary>
        /// <param name="name">JS脚本资源绑定名称</param>
        /// <param name="groupName">分组名称</param>
        /// <param name="scripts">JS脚本路径数组 如果为空就绑定分组，如果有知道资源就重新生成资源绑定</param>
        public static void BundlesGroup(string name, string groupName, params string[] scripts)
        {
            if (BundleScripts.ContainsKey(name))
                return;
            //获取绑定分组对象
            Bundles gb = null;
            if (BundleScripts.ContainsKey(groupName))
                gb = BundleScripts[groupName];

            if (scripts.Count() == 0)
            {
                //未设置其他资源
                if (gb != null)
                    BundleScripts.Add(name, gb);
            }
            else
            {
                //重新生成资源绑定
                if (gb != null)
                {
                    //获取分组的资源
                    scripts = gb.item.Concat(scripts).ToArray();
                }
                BundlesAdd(name, scripts);
            }
        }
    }
}
