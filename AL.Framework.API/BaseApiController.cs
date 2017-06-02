using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Text;
using AL.Common.Models;

namespace AL.Framework.WebApi
{
    /// <summary>
    /// WebApi 基类 
    /// </summary>
    public abstract class BaseApiController : ApiController
    {
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
