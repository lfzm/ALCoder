#region Copyright (C) 2017 AL系列开源项目
/*       
　　	文件功能描述：log4net 日志记录组件

　　	创建人：阿凌
        创建人Email：513845034@qq.com

        使用描述：
        * Application_Start事件中读取配置文件 
        * //注意：指明绝对路径 
        * //log4net.config为自定义的配置文件 
        * log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(Server.MapPath("~/Practice/log4net.config"))); 
*/
#endregion

using AL.Common.Extention;
//using log4net;
using System;
using System.Diagnostics;

namespace AL.Common.Helper
{
    /// <summary>
    /// log4net 日志记录
    /// </summary>
    public static class Log4Helper
    {
        //public static void Error(object message)
        //{
        //    LogManager.GetLogger(Log4Helper.GetCurrentMethodFullName()).Error(message.ToStr());
        //}
        //public static void Error(object message, Exception ex)
        //{
        //    LogManager.GetLogger(Log4Helper.GetCurrentMethodFullName()).Error(message.ToStr(), ex);
        //}
        //public static void Info(object message)
        //{
        //    LogManager.GetLogger(Log4Helper.GetCurrentMethodFullName()).Info(message.ToStr());
        //}

        //public static void Info(object message, Exception ex)
        //{
        //    LogManager.GetLogger(Log4Helper.GetCurrentMethodFullName()).Info(message.ToStr(), ex);
        //}

        //public static void Debug(object message)
        //{
        //    LogManager.GetLogger(Log4Helper.GetCurrentMethodFullName()).Debug(message.ToStr());
        //}

        //public static void Debug(object message, Exception ex)
        //{
        //    LogManager.GetLogger(Log4Helper.GetCurrentMethodFullName()).Debug(message.ToStr(), ex);
        //}


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

        private static string ToStr(this object message)
        {
            try
            {
                if (!(message is string))
                    message = message.ToJsonFormatString();
                return message.ToString();
            }
            catch (Exception ex)
            {
                return message.ToString();
            }
        }
    }

}
