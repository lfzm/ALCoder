#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：日志模块的实体定义
*
*　　	创建人： 阿凌
*       创建人Email：513845034@qq.com
*       
*****************************************************************************/

#endregion
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AL.Common.Modules.LogModule
{

    /// <summary>
    /// 日志等级
    /// </summary>
    public enum LogLevelEnum
    {
        /// <summary>
        /// 跟踪查看
        /// </summary>
        Trace,
        /// <summary>
        /// 信息
        /// </summary>
        Info,

        /// <summary>
        /// 错误
        /// </summary>
        Error,

        /// <summary>
        /// 警告
        /// </summary>
        Warning,
    }

    /// <summary>
    /// 日志实体
    /// </summary>
    public sealed class LogInfo
    {
        /// <summary>
        /// 空构造函数
        /// </summary>
        public LogInfo()
        {
        }

        /// <summary>
        /// 日志构造函数
        /// </summary>
        /// <param name="loglevel">日志等级</param>
        /// <param name="methodFullName">日志记录地址</param>
        /// <param name="message">日志</param>
        /// <param name="messageKey">关键信息</param>
        /// <param name="exception">错误信息</param>
        /// <param name="moduleName"></param>
        internal LogInfo(LogLevelEnum loglevel,string methodFullName, object message, string messageKey, Exception exception=null, string moduleName = ModuleNames.Default)
        {
            Level = loglevel;
            ModuleName = moduleName;
            MessageKey = messageKey;
            Message = message;
            Exception = exception;
        }

        /// <summary>
        /// 日志等级
        /// </summary>
        public LogLevelEnum Level { get; set; }
        /// <summary>
        /// 日志类型
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// 日志记录地址
        /// </summary>
        public string MethodFullName { get; set; }
        /// <summary>
        /// 日志信息  可以是复杂类型  如 具体实体类
        /// </summary>
        public object Message { get; set; }
        /// <summary>
        /// 标题信息
        /// </summary>
        public string MessageKey { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// 编号（全局唯一）
        /// </summary>
        public string LogCode { get; set; }
    }

    /// <summary>
    /// 日志写模块
    /// </summary>
    public static class LogHelper
    {

        /// <summary>
        /// 记录日志操作的异步模块
        /// </summary>
        internal static string LogAsynModuleName
        {
            get;
            set;
        }

        private static readonly ConcurrentDictionary<string, ILogWriter> _logDirs =
            new ConcurrentDictionary<string, ILogWriter>();

        /// <summary>
        /// 通过模块名称获取日志模块实例
        /// </summary>
        /// <param name="logModule"></param>
        /// <returns></returns>
        public static ILogWriter GetLogWrite(string logModule)
        {
            if (string.IsNullOrEmpty(logModule))
                logModule = ModuleNames.Default;

            if (_logDirs.ContainsKey(logModule))
                return _logDirs[logModule];

            ILogWriter log;
            if (ModuleConfig.LogWriterProvider != null)
                log = ModuleConfig.LogWriterProvider.Invoke(logModule) ?? new LogWriter();
            else
                log = new LogWriter();
            _logDirs.TryAdd(logModule, log);
            return log;
        }

        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param name="msg">日志信息</param>
        /// <param name="msgKey">关键值</param>
        /// <param name="moduleName">模块名称</param>
        public static string Info(object msg, string msgKey = null, string moduleName = ModuleNames.Default)
        {
            string funllname = GetCurrentMethodFullName();
            return Log(new LogInfo(LogLevelEnum.Info, funllname, msg, msgKey, null, moduleName));
        }

        /// <summary>
        /// 记录警告，用于未处理异常的捕获
        /// </summary>
        /// <param name="msg">日志信息</param>
        /// <param name="msgKey">关键值</param>
        /// <param name="moduleName">模块名称</param>
        public static string Warning(object msg, string msgKey = null, string moduleName = ModuleNames.Default)
        {
            string funllname = GetCurrentMethodFullName();
            return Log(new LogInfo(LogLevelEnum.Warning, funllname, msg, msgKey, null, moduleName));
        }

        /// <summary>
        /// 记录错误，用于捕获到的异常信息记录
        /// </summary>
        /// <param name="msg">日志信息</param>
        /// <param name="msgKey">关键值</param>
        /// <param name="exception">错误信息</param>
        /// <param name="moduleName">模块名称</param>
        public static string Error(object msg, string msgKey = null, Exception exception =null, string moduleName = ModuleNames.Default)
        {
            string funllname = GetCurrentMethodFullName();
            return Log(new LogInfo(LogLevelEnum.Error, funllname, msg,  msgKey, exception, moduleName));
        }

        /// <summary>
        /// 记录错误，用于捕获到的异常信息记录
        /// </summary>
        /// <param name="msg">日志信息</param>
        /// <param name="msgKey">关键值</param>
        /// <param name="moduleName">模块名称</param>
        public static string Trace(object msg, string msgKey = null, string moduleName = ModuleNames.Default)
        {
            string funllname = GetCurrentMethodFullName();
            return Log(new LogInfo(LogLevelEnum.Trace, funllname, msg,msgKey, null, moduleName));
        }

        private static string GetCurrentMethodFullName()
        {
            try
            {
                int num = 2;
                StackTrace stackTrace = new StackTrace();
                int length = stackTrace.GetFrames().Length;
                StackFrame frame;
                string str;
                do
                {
                    try
                    {
                        frame = stackTrace.GetFrame(num++);
                        str = frame.GetMethod().DeclaringType.ToString();
                    }
                    catch (Exception)
                    {
                        frame = null; str = "";
                    }
                }
                while ((!str.EndsWith("Exception") ? 0 : (num < length ? 1 : 0)) != 0);
                string name = "";
                if (frame != null)
                    name = frame.GetMethod().Name;
                return str + "." + name;
            }
            catch (Exception ex)
            {
                return (string)"";
            }
        }
        /// <summary>
        ///   记录日志
        /// </summary>
        /// <param name="info"></param>
        private static string Log(LogInfo info)
        {
            if (string.IsNullOrEmpty(info.ModuleName))
                info.ModuleName = ModuleNames.Default;

            var logWrite = GetLogWrite(info.ModuleName);
            if (logWrite != null)
            {
                info.LogCode = logWrite.GetLogCode(info);
                Task.Run(() => logWrite.WriteLog(info));
            }
            return info.LogCode;
        }


    }
}
