using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zzll.Net.Framework.Helper;
using Zzll.Net.Framework.TcpService;

namespace Zzll.Net.Framework
{
    /// <summary>
    /// 通讯中间件
    /// </summary>
    public class NetService
    {
        public delegate Response RequestHandle(Request req);
        /// <summary>
        /// 消息处理事件
        /// </summary>
        public event RequestHandle RequestHandle_Event;
        private TcpService.Service _tcpService;
        /// <summary>
        /// Tcp服务端
        /// </summary>
        public TcpService.Service TcpService { get { return _tcpService; } }

        /// <summary>
        /// 启动监听服务
        /// </summary>
        public void Start()
        {
            //启动日志记录
            Helper.LogHelper.Start();

            #region TcpService
            string path = ConfigurationManager.AppSettings["TcpServiceConfig"];
            if (!string.IsNullOrEmpty(path))
            {
                _tcpService = new TcpService.Service();
                _tcpService.GetConfig(path);
                _tcpService.Start(requestHandle);
            }
            #endregion

            #region RedisMQ
            path = ConfigurationManager.AppSettings["RedisMQConfig"];
            if (!string.IsNullOrEmpty(path))
            {
                RedisMQ.Service rsvr = new RedisMQ.Service();
                rsvr.GetConfig(path);
                if (rsvr.config != null)
                    rsvr.Start(requestHandle);
            }
            #endregion

            #region Timing
            path = ConfigurationManager.AppSettings["TimingConfig"]; if (!string.IsNullOrEmpty(path))
            {
                Timing.TimingService tsvr = new Timing.TimingService();
                tsvr.GetConfig(path);
                if (tsvr.config != null)
                    tsvr.Start(requestHandle);
            }
            #endregion
        }
        /// <summary>
        /// 消息处理方法
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        private Response requestHandle(Request req)
        {
            if (RequestHandle_Event == null) return null;
            return RequestHandle_Event(req);
        }
    }
}
