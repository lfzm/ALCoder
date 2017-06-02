using AL.Common.Helper.Encrypt;
using AL.Framework.WebMvc.Bundle;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

namespace AL.Framework.WebMvc
{
    /// <summary>
    ///Css资源
    /// </summary>
    public static class ALStyles
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        static ALStyles()
        {
            BundleStyles = new Dictionary<string, Bundles>();
        }
        /// <summary>
        /// Css资源绑定缓存
        /// </summary>
        public static Dictionary<string, Bundles> BundleStyles {get; set; }
        /// <summary>
        /// Css资源基础分组名称
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
            if (!BundleStyles.ContainsKey(name))
                name = BasisName;
              Bundles b = BundleStyles[name];
            IHtmlString html = Styles.Render(b.virtualPath);
            return html;
        }

        /// <summary>
        /// CSS脚本资源绑定添加
        /// </summary>
        /// <param name="name">CSS脚本资源绑定名称</param>
        /// <param name="styles">CSS脚本路径数组</param>
        public static void BundlesAdd(string name, params string[] styles)
        {
            if (!BundleStyles.ContainsKey(name))
            {
                string virtualPath = "~/" + Md5.HalfEncryptHexString(name + "/Style");
                BundleTable.Bundles.Add(new StyleBundle(virtualPath).Include(styles));
                Bundles b = new Bundles()
                {
                    item = styles,
                    name = name,
                    virtualPath = virtualPath
                };
                if (BundleStyles.Count == 0 && string.IsNullOrEmpty(BasisName))
                    BasisName = name;
                BundleStyles.Add(name, b);
            }
        }

        /// <summary>
        /// CSS脚本资源分组绑定
        /// </summary>
        /// <param name="name">CSS脚本资源绑定名称</param>
        /// <param name="groupName">分组名称</param>
        /// <param name="styles">CSS脚本路径数组 如果为空就绑定分组，如果有知道资源就重新生成资源绑定</param>
        public static void BundlesGroup(string name, string groupName, params string[] styles)
        {
            if (BundleStyles.ContainsKey(name))
                return;
            //获取绑定分组对象
            Bundles gb = null;
            if (BundleStyles.ContainsKey(groupName))
                gb = BundleStyles[groupName];

            if (styles.Count() == 0)
            {
                //未设置其他资源
                if (gb != null)
                    BundleStyles.Add(name, gb);
            }
            else
            {
                //重新生成资源绑定
                if (gb != null)
                    styles = gb.item.Concat(styles).ToArray();
                BundlesAdd(name, styles);
            }
        }
    }
}
