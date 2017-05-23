using AL.Common.Extention;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace AL.Framework.API
{
    /// <summary>
    /// (需要授权)
    /// </summary>
    [Filter.Authorize]
    public class BaseOAuthApiController : BaseApiController
    {
        /// <summary>
        /// 授权Uid
        /// </summary>
        public int UserId
        {
            get
            {
                if (RequestContext.RouteData.Values.ContainsKey("USERID"))
                {
                    int uid = RequestContext.RouteData.Values["USERID"].ToString().ToInt32();
                    return uid;
                }
                return 0;
            }
        }

        /// <summary>
        /// 授权Token
        /// </summary>
        public string Token
        {
            get
            {
                if (RequestContext.RouteData.Values.ContainsKey("TOKEN"))
                    return RequestContext.RouteData.Values["TOKEN"].ToString();
                else
                    return null;
            }
        }

    }
}
