#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：日志类的默认实现
*
*　　	创建人： 阿凌
*       创建人Email：513845034@qq.com
*       
*****************************************************************************/

#endregion
using System;
using System.IO;
using System.Text;
using AL.Common.Extention;

namespace AL.Common.Modules.LogModule
{
    /// <summary>
    /// 系统默认写日志模块
    /// </summary>
    public class LogWriter : ILogWriter
    {
        private readonly string _logBaseDirPath = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public LogWriter()
        {
            _logBaseDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"log");
            if (!Directory.Exists(_logBaseDirPath))
                Directory.CreateDirectory(_logBaseDirPath);
        }

        /// <summary>
        /// 获取日志记录路径
        /// </summary>
        /// <param name="module"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private string getLogFilePath(string module, LogLevelEnum level)
        {
            string dirPath = string.Format(@"{0}\{1}\{2}\", _logBaseDirPath, module, level);
            if (module == ModuleNames.Default)
                dirPath = string.Format(@"{0}\{1}\", _logBaseDirPath, level);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            return string.Concat(dirPath, DateTime.Now.ToString("yyyy-MM-dd"), ".txt");
        }

        private object obj = new object();

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="info"></param>
        public void WriteLog(LogInfo info)
        {
            lock (obj)
            {
                try
                {
                    string filePath = getLogFilePath(info.ModuleName, info.Level);
                    using (StreamWriter sw = new StreamWriter(filePath, true, Encoding.UTF8))
                    {
                        string Message = "";
                        if (info.Message.GetType() == typeof(string))
                            Message = info.Message.ToString();
                        else
                            Message = info.Message.ToJsonString();
                        string text = DateTime.Now.ToString("HH:mm:ss,fff") + "-->";
                        text += (string.IsNullOrEmpty(info.MethodFullName) ? "" : info.MethodFullName + "-->");//日志记录方法地址
                        text += (string.IsNullOrEmpty(info.MessageKey) ? "" : info.MessageKey + "-->");//关键数据
                        text += Message;//消息
                        sw.WriteLine(text);

                    }
                }
                catch (Exception ex)
                {
                    info.ModuleName = "ExError";
                    info.Exception = ex;
                    info.Message = ex.Message;
                    WriteLog(info);
                }
            }
        }

        /// <summary>
        /// 生成错误编号
        /// </summary>
        /// <returns></returns>
        public string GetLogCode(LogInfo info)
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

    }
}
