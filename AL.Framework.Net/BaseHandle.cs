using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zzll.Net.Framework.Helper;

namespace Zzll.Net.Framework
{
    /// <summary>
    /// 处理基类
    /// </summary>
    public abstract class BaseHandle<T>
    {
        /// <summary>
        /// 配置文件路径
        /// </summary>
        private string _confgPath { get; set; }
        /// <summary>
        /// 处理方法
        /// </summary>
        protected Func<Request, Response> Handle_Event { get; set; }
        /// <summary>
        /// 服务配置
        /// </summary>
        public T config { get; set; }
        /// <summary>
        /// 启动服务方法
        /// </summary>
        public abstract void Start(Func<Request, Response> func);
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="path">配置文件路径</param>
        /// <returns></returns>
        public virtual void GetConfig(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            _confgPath = path;
            config = SerializationHelper.Load<T>(path);
        }
        /// <summary>
        ///保存配置文件
        /// </summary>
        public virtual void SetConfig()
        {
            if (string.IsNullOrEmpty(_confgPath))
                _confgPath = "config.bit";
            SerializationHelper.Save<T>(config, _confgPath);
        }

    }
}
