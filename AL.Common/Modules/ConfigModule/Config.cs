#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：配置处理的默认实现
*
*　　	创建人： 阿凌
*       创建人Email：513845034@qq.com
*       
*****************************************************************************/

#endregion
using System;
using System.IO;
using System.Xml.Serialization;
using AL.Common.Modules.LogModule;
using AL.Common.Models;

namespace AL.Common.Modules.ConfigModule
{

    /// <summary>
    /// 默认配置处理
    /// </summary>
    public class Config : IConfig
    {
        private static readonly string _defaultPath;

        static Config()
        {
            _defaultPath = AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// 设置字典配置信息
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <param name="key"></param>
        /// <param name="dirConfig"></param>
        /// <returns></returns>
        public Result SetConfig<TConfig>(string key, TConfig dirConfig) where TConfig : class, new()
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key", "配置键值不能为空！");

            if (dirConfig == null)
                throw new ArgumentNullException("dirConfig", "配置信息不能为空！");

            string path = string.Concat(_defaultPath, "\\", "Config");

            Result result;
            FileStream fs = null;
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                fs = new FileStream(string.Concat(path, "\\", key, ".config"), FileMode.Create, FileAccess.Write);

                var type = typeof(TConfig);

                XmlSerializer xmlSer = new XmlSerializer(type);
                xmlSer.Serialize(fs, dirConfig);

                result = new Result();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("错误描述：{0}    详情：{1}", ex.Message, ex.StackTrace));
                throw ex;
            }
            finally
            {
                if (fs != null)
                    fs.Dispose();
            }
            return result;
        }


        /// <summary>
        ///   获取字典配置
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public TConfig GetConfig<TConfig>(string key) where TConfig : class, new()
        {

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key", "配置键值不能为空！");

            string path = string.Concat(_defaultPath, "\\", "Config");

            TConfig t = default(TConfig);
            FileStream fs = null;
            try
            {
                string fileFullName = string.Concat(path, "\\", key, ".config");

                if (!File.Exists(fileFullName))
                    return t;

                fs = new FileStream(fileFullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                var type = typeof(TConfig);

                XmlSerializer xmlSer = new XmlSerializer(type);
                t = (TConfig)xmlSer.Deserialize(fs);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("错误描述：{0}    详情：{1}", ex.Message, ex.StackTrace));
                throw ex;
            }
            finally
            {
                if (fs != null)
                    fs.Dispose();
            }

            return t;
        }

        /// <summary>
        ///  移除配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Result RemoveConfig(string key)
        {
            string path = string.Concat(_defaultPath, "\\", "Config");
            string fileName = string.Concat(path, "\\", key, ".config");

            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("错误描述：{0}    详情：{1}", ex.Message, ex.StackTrace));
                throw ex;

            }
            return new Result( "移除字典配置时出错", ResultTypes.InnerError);
        }


    }
}
