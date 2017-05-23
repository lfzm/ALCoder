
using AL.Common.Extention;


namespace AL.Framework.MVC
{
    /// <summary>
    /// 授权基类
    /// </summary>
    [Filter.Authorize]
    public class BaseOAuthController : BaseController
    {

        /// <summary>
        /// 授权Uid
        /// </summary>
        public int UserId
        {
            get
            {
                if (Request.RequestContext.RouteData.Values.ContainsKey("USERID"))
                {
                    int uid = Request. RequestContext.RouteData.Values["USERID"].ToString().ToInt32();
                    return uid;
                }
                return 0;
            }
        }



    }
}
