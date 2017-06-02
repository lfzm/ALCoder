
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using AL.Common.Extention;
using AL.Common.Modules.LogModule;

namespace AL.Framework.WebApi.Filter
{
    /// <summary>
    /// 异常过滤器
    /// </summary>
    public class FilterException : ExceptionFilterAttribute
    {
        public override Task OnExceptionAsync(HttpActionExecutedContext context, CancellationToken cancellationToken)
        {
            dynamic reqMessage = new
            {
                Success = false,
                Status = 500,
                Message = "系统繁忙"
            };
            string msg = JsonExtention.ToJsonString(reqMessage);
            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StringContent(msg, Encoding.GetEncoding("UTF-8"), "application/json");
            response.StatusCode = HttpStatusCode.OK;
            context.Response = response;

            //文本日志记录
            LogHelper.Error(context.Exception.Message, "", context.Exception);

            return Task.FromResult<object>(null);
        }

    }
}
