using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using AL.Common.Extention;
using AL.Common.Modules.LogModule;

namespace AL.Framework.WebApi.Filter
{
    /// <summary>
    /// 访问信息采集
    /// </summary>
    public class FilterOperation : ActionFilterAttribute
    {
        /// <summary>
        /// 在调用操作方法之前发生
        /// </summary>
        /// <returns></returns>
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            //手续信息
            Collection();
            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        /// <summary>
        /// 收集信息
        /// </summary>
        /// <param name="Request"></param>
        public void Collection()
        {
            VisitorTerminal vt = WebExtention .GetVisitorTerminal();
            LogHelper.Info(vt);//存储
        }



    }
}
