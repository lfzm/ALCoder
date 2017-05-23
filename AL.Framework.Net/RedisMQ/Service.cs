using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zzll.Net.Framework.Helper;

namespace Zzll.Net.Framework.RedisMQ
{
    /// <summary>
    /// RedisMQ监控服务
    /// </summary>
    public class Service : BaseHandle<Config>
    {
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="func"></param>
        public override void Start(Func<Request, Response> func)
        {
            if (base.config == null)
                throw new Exception("未设置RedisMQ监控服务配置");

            base.Handle_Event = func;
            foreach (Redis item in config.redis)
            {
                RedisClient client = new RedisClient(item.dbNum, item.connectString);
                client.Subscribe(item.redisKey, (channel, value) =>
                {
                    Request req = new Request();
                    req.channel = RequestChannel.RedisMQ;
                    req.code = item.func;
                    req.id = Utils.GetReqId();
                    req.para = value.ToString();
                    LogHelper.Add(req.id+ "->Request", req, LogType.RedisMQ);
                    Response resp = func(req);
                    LogHelper.Add(req.id + "->Response", resp, LogType.RedisMQ);
                });
            }
            Console.WriteLine(DateTime.Now.ToString("MM-dd HH:mm:ss:fff") + " RedisMQ监控启动：" + config.redis.Count);
            LogHelper.Add("启动RedisMQ监控->RedisMQ", config.redis.Count, LogType.RedisMQ);

        }
    }
}
