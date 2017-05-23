using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zzll.Net.Framework.Helper
{
    /// <summary>
    /// 日志记录工具
    /// </summary>
    public class LogHelper
    {
        #region 属性
        private static string LogPath { get; set; }
        /// <summary>
        ///开启数据日志记录 配合WriteDBLog方法使用
        /// </summary>
        public static bool StartWriteDBLog { get; set; }
        /// <summary>
        /// 操作日志寄存器
        /// </summary>
        private static List<Log> LogRegister = new List<Log>();

        #endregion

        /// <summary>
        /// 启动日志记录（默认在运行目录）
        /// </summary>
        public static void Start()
        {
            LogPath = Directory.GetCurrentDirectory()+"//Log//";
            System.Threading.Thread log = new System.Threading.Thread(StartLogWrite);
            log.IsBackground = true;
            log.Start();
        }
        /// <summary>
        /// 启动日志记录服务
        /// </summary>
        /// <param name="logPath">日志存储目录</param>
        private static void Start(string logPath)
        {
            LogPath = logPath;
            Start();
        }
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="title">日志标题</param>
        /// <param name="content">日志内容</param>
        /// <param name="dirs">日志类型</param>
        public static void Add(string title, string content, LogType dirs)
        {
            Log log = new Log();
            log.Title = title;
            log.Content = content;
            log.Time = DateTime.Now;
            log.logDir = dirs;
            Add(log);
        }
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="title">日志标题</param>
        /// <param name="content">日志内容</param>
        /// <param name="dirs">日志类型</param>
        public static void Add(string title, object content, LogType dirs)
        {
            Log log = new Log();
            log.Title = title;
            log.Content = content;
            log.Time = DateTime.Now;
            log.logDir = dirs;
            Add(log);
        }
        /// <summary>
        /// 记录数据库日志记录
        /// </summary>
        /// <param name="obj"></param>
        public static void WriteDBLog(string obj)
        {
            if (string.IsNullOrEmpty(obj.Trim()))
                return;
            if (StartWriteDBLog)
            {
                Add("", obj.Trim(), LogType.DataBase);
            }
        }
        /// <summary>
        /// 记录当前操作日志
        /// </summary>
        /// <param name="log"></param>
        public static void Add(Log log)
        {
            lock (LogRegister)
            {
                LogRegister.Add(log);
            }
        }
        /// <summary>
        /// 启动日志记录线程
        /// </summary>
        private static void StartLogWrite()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                if (LogRegister.Count == 0)
                    continue;
                for (int i = 0; i < LogRegister.Count; i++)
                {
                    Log log = null;
                    try
                    {
                        log = LogRegister[i];
                        Logging(log);
                        //此操作结束需要移除该操作日志
                        lock (LogRegister)
                        {
                            LogRegister.Remove(log);
                        }
                        i--;
                    }
                    catch { }
                }
            }
        }
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="log">日志对象</param>
        private static void Logging(Log log)
        {
            try
            {
                string Filepath = string.Empty;
                Filepath = LogPath + log.logDir + "\\";

                if (!Directory.Exists(Filepath))
                    Directory.CreateDirectory(Filepath);
                string filename = DateTime.Now.ToString("yyyy-MM-dd");
                string filePath = Filepath + filename + ".txt";
                FileInfo file = new FileInfo(filePath);
                if (!file.Exists)
                    file.Create().Close();
                StreamWriter sw = new StreamWriter(filePath, true);
                string content = "";
                if (log.Content.GetType() == typeof(string))
                    content = log.Content.ToString();
                else
                {
                    try
                    {
                        if (log.Content == null)
                            content = "null";
                        else
                            content = JsonConvert.SerializeObject(log.Content);
                    }
                    catch (Exception ex)
                    {
                        log.Content = "错误";
                    }
                }
                sw.WriteLine(log.Time.ToString("HH:mm:ss:fff") + "  " + log.Title + "  " + content);
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {

            }
        }

    }

    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 调试日志
        /// </summary>
        Debug,
        /// <summary>
        /// 处理日志
        /// </summary>
        Handle,
        /// <summary>
        /// 错误日志
        /// </summary>
        Error,
        /// <summary>
        /// 性能监控
        /// </summary>
        Monitor,

        /// <summary>
        /// 数据库
        /// </summary>
        DataBase,
        /// <summary>
        /// 定时任务日志
        /// </summary>
        Timing,
        /// <summary>
        /// TcpService 通讯日志
        /// </summary>
        TcpService,
        /// <summary>
        /// TcpClient  通讯日志
        /// </summary>
        TcpClient,
        /// <summary>
        /// Redis 消息队列
        /// </summary>
        RedisMQ
     
    }
    /// <summary>
    /// 日志对象
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 日志时间
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// 保存路径
        /// </summary>
        public LogType logDir { get; set; }
        /// <summary>
        /// 日志标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 日志内容
        /// </summary>
        public object Content { get; set; }
    }
}
