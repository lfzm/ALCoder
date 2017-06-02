using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace AL.Framework.WebApi.Filter
{
    /// <summary>
    /// 性能监控
    /// </summary>
    public class FilterMonitor: ActionFilterAttribute
    {
        /// <summary>
        /// 在调用操作方法之后发生
        /// </summary>
        /// <returns></returns>
        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }

        /// <summary>
        /// 在调用操作方法之前发生
        /// </summary>
        /// <returns></returns>
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }
    }
}
