using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zzll.Net.Framework.Helper;

namespace Zzll.Net.Framework.Timing
{
    /// <summary>
    /// 定时处理服务
    /// </summary>
    public class TimingService : BaseHandle<Config>
    {
        /// <summary>
        /// 开启定时处理监控
        /// </summary>
        /// <param name="func"></param>
        public override void Start(Func<Request, Response> func)
        {
            base.Handle_Event = func;
            if (base.config == null)
                throw new Exception("未设置定时监控服务配置");
            Thread Timing = new Thread(TimingMessageNotice);
            Timing.IsBackground = true;
            Timing.Start();
            Console.WriteLine(DateTime.Now.ToString("MM-dd HH:mm:ss:fff") + " 定时监控服务启动：" + config.timing.Count);
            LogHelper.Add("定时监控服务启动->TimingService", base.config.timing.Count, LogType.Timing);
        }

        private void TimingMessageNotice()
        {
            //循环处理操作
            while (true)
            {
                Thread.Sleep(5);//休眠5秒之后只需监视
                for (int i = 0; i < config.timing.Count; i++)
                {
                    var item = config.timing[i];
                    try
                    {

                        string funCode = "";
                        if (item.type == TimingExec.TimingExecType.Interval)
                        {
                            if (item.lastExevTime.AddMilliseconds(item.Interval) > DateTime.Now)
                                continue;
                            funCode = item.funCode;
                        }
                        else if (item.type == TimingExec.TimingExecType.Day)
                        {
                            DateTime time = Convert.ToDateTime(item.execTime.ToString("HH:mm:ss"));
                            if (time > DateTime.Now || time < item.lastExevTime)
                                continue;
                            funCode = item.funCode;
                        }
                        else if (item.type == TimingExec.TimingExecType.month)
                        {
                            int month = (int)DateTime.Now.Month;
                            int lastmonth = (int)item.lastExevTime.Month;
                            if (month < lastmonth)
                                continue;
                            DateTime time = Convert.ToDateTime(item.execTime.ToString("dd HH:mm:ss"));
                            if (time > DateTime.Now || time < item.lastExevTime)
                                continue;
                            funCode = item.funCode;
                        }
                        else if (item.type == TimingExec.TimingExecType.week)
                        {
                            int week = (int)DateTime.Now.DayOfWeek;
                            int lastweek = (int)item.lastExevTime.DayOfWeek;
                            if (lastweek != week)
                                continue;
                            DateTime time = Convert.ToDateTime(item.execTime.ToString("HH:mm:ss"));
                            if (time > DateTime.Now || time < item.lastExevTime)
                                continue;
                            funCode = item.funCode;
                        }
                        if (string.IsNullOrEmpty(funCode))
                            continue;
                        //判断功能是否在运行中
                        if (base.Handle_Event == null || item.IsRun)
                            continue;

                        item.IsRun = true;
                        Request req = new Request();
                        req.channel = RequestChannel.Timing;
                        req.code = funCode;
                        req.id = Utils.GetReqId();
                        req.para = "";
                        LogHelper.Add(req.id + "->Request", req, LogType.Timing);
                        //异步调用处理方法
                        IAsyncResult iar = base.Handle_Event.BeginInvoke(req,
                            (iars) =>
                            {
                                #region 异步方法完成后执行回调
                                Response resp = ((Func<Request, Response>)((System.Runtime.Remoting.Messaging.AsyncResult)iars).AsyncDelegate).EndInvoke(iars);
                                string func = iars.AsyncState.ToString();

                                LogHelper.Add(req.id + "->Response", resp, LogType.Timing);
                                try
                                {
                                    //修改执行事件
                                    TimingExec timing = config.timing.Find(f => f.funCode == resp.code);
                                    if (timing == null)
                                        return;
                                    lock (config)
                                    {
                                        //保存最后执行时间
                                        timing.IsRun = false;
                                        timing.lastExevTime = DateTime.Now;
                                        this.SetConfig();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.Add("定时处理 异步回调" + resp.code, ex.Message, LogType.Error);
                                }
                                #endregion
                            },
                            funCode);


                    }
                    catch (Exception ex)
                    {
                        LogHelper.Add("定时处理 " + item.funCode, ex.Message, LogType.Error);
                    }
                }
            }
        }
    }
}
