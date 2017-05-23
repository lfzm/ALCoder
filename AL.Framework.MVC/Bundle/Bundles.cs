using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AL.Framework.MVC.Bundle
{
    /// <summary>
    /// 捆绑Css、Js对象
    /// </summary>
    public class Bundles
    {
        /// <summary>
        /// 资源绑定名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 虚拟路径
        /// </summary>
        public string virtualPath { get; set; }
        /// <summary>
        /// 捆绑资源项
        /// </summary>
        public string[] item { get; set; }
    }
}
