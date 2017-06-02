namespace AL.Framework.WebMvc
{
    using System.Web.Mvc;
    using System.Web.SessionState;

    /// <summary>
    /// 异步控制器基类
    /// </summary>
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class BaseAsyncController : AsyncController
    {
    }
}

