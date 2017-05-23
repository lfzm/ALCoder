#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：配置处理实现接口
*
*　　	创建人： 阿凌
*       创建人Email：513845034@qq.com
*       
*****************************************************************************/

#endregion
using AL.Common.Models;

namespace AL.Common.Modules.ConfigModule
{
    /// <summary>
    /// 字典配置接口
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// 添加字典配置
        /// </summary>
        /// <param name="key">配置关键字</param>
        /// <param name="dirConfig">配置具体信息</param>
        /// <typeparam name="TConfig">配置信息类型</typeparam>
        /// <returns></returns>
        Result SetConfig<TConfig>(string key, TConfig dirConfig) where TConfig : class ,new();

        /// <summary>
        /// 添加字典配置
        /// </summary>
        /// <param name="key">配置关键字</param>
        /// <typeparam name="TConfig">配置信息类型</typeparam>
        /// <returns></returns>
        TConfig GetConfig<TConfig>(string key) where TConfig : class ,new();

        /// <summary>
        /// 移除配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Result RemoveConfig(string key);

    }
}