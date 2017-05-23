using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AL.Common.Helper;
using AL.Common.Models;
using AL.Common.Extention;
using System.Collections.Concurrent;
using AL.Common.Helper.Http;
using AL.Common.Modules.LogModule;

namespace AL.DbContext.Api
{
    /// <summary>
    /// Api 连接器
    /// </summary>
    public class ApiContextBase
    {
        #region 属性
        /// <summary>
        /// 服务地址
        /// </summary>
        private static ConcurrentDictionary<string, string> ConnectionMultiplexer { get; set; }
        /// <summary>
        /// 线程锁
        /// </summary>
        private static readonly object Locker = new object();
        /// <summary>
        /// 访问地址名称
        /// </summary>
        private string ConnectionName { get; set; }
        /// <summary>
        /// 日志记录提供者
        /// </summary>
        private Action<string, string, string> LoggerProvider { get; set; }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connStringName">配置文件连接字符串名称</param>
        public ApiContextBase(string connStringName) : this(connStringName, null)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connStringName">App.Config配置名称</param>
        /// <param name="logger">日志记录函数</param>
        public ApiContextBase(string connStringName, Action<string, string, string> logger)
        {
            if (string.IsNullOrWhiteSpace(connStringName))
                throw new Exception("请设置配置文件连接字符串名称");

            if (ConnectionMultiplexer == null)
                ConnectionMultiplexer = new ConcurrentDictionary<string, string>();

            ConnectionName = connStringName;
            lock (Locker)
            {
                if (!ConnectionMultiplexer.ContainsKey(connStringName))
                {
                    string _url = ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
                    ConnectionMultiplexer[ConnectionName] = _url;
                }
            }
            if (logger != null)
                LoggerProvider = logger;

        }

        #region Post请求
        /// <summary>
        /// Post请求需授权数据
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <typeparam name="TParameter">测试类型</typeparam>
        /// <param name="Method">请求方法</param>
        /// <param name="Data">请求数据</param>
        /// <param name="Token">授权令牌</param>
        /// <returns></returns>
        public virtual TResult Post<TResult, TParameter>(string Method, TParameter Data, string Token) where TResult : Result, new() where TParameter : class, new()
        {
            return Post<TResult, TParameter>(Method + "?token=" + Token, Data);
        }
        /// <summary>
        /// Post请求
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <typeparam name="TParameter">测试类型</typeparam>
        /// <param name="Method">请求方法</param>
        /// <param name="Data">请求数据</param>
        /// <returns></returns>
        public virtual TResult Post<TResult, TParameter>(string Method, TParameter Data) where TResult : Result, new() where TParameter : class, new()
        {
            string param = "";
            if (Data != null)
            {
                param = Data.ToJsonString();
            }
            return Post<TResult>(Method, param, ContentType.json);
        }

        /// <summary>
        /// Post请求数据 
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="Method">请求方法</param>
        /// <param name="Data">请求数据</param>
        /// <param name="Token">授权令牌</param>
        /// <returns></returns>
        public virtual TResult Post<TResult>(string Method, string Data, string Token) where TResult : Result, new()
        {
            return Post<TResult>(Method + "?token=" + Token, Data);
        }

        /// <summary>
        /// Post请求数据 
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="Method">请求方法</param>
        /// <param name="Data">请求数据</param>
        /// <param name="Type">请求数据类型</param>
        /// <returns></returns>
        public virtual TResult Post<TResult>(string Method, string Data, ContentType Type = ContentType.x_www_form_urlencoded) where TResult : Result, new()
        {
            string url = string.Empty;
            if (Method.Contains("http://") || Method.Contains("https://"))
                url = Method;
            else
                url = GetUrl(Method);
            try
            {
                HttpRequest request = new HttpRequest(url);
                request.contentType = Type;
                string Message = request.PostRequest(Data);
                if (LoggerProvider != null)
                    LoggerProvider("Post->" + Method, Data.ToJsonString(), Message);
                if (string.IsNullOrEmpty(Message))
                    return GetErrorResult<TResult>("请求处理失败");
                return Message.ConvertToResult<TResult>();
            }
            catch (HttpRequestException ex)
            {
                LogHelper.Error(url + Data, ex.Message, ex);
                return GetErrorResult<TResult>("请求处理失败");
            }
            catch (Exception ex)
            {
                LogHelper.Error(url + Data, ex.Message, ex);
                return GetErrorResult<TResult>("系统繁忙");
            }
        }

        /// <summary>
        /// Post请求数据 
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="Method">请求方法</param>
        /// <param name="Data">请求数据</param>
        /// <returns></returns>
        public virtual async Task<TResult> PostAsync<TResult>(string Method, string Data) where TResult : Result, new()
        {
            string url = string.Empty;
            if (Method.Contains("http://") || Method.Contains("https://"))
                url = Method;
            else
                url = GetUrl(Method);
            try
            {
                HttpRequest request = new HttpRequest(url);
                string Message = await request.PostRequestAsync(Data);
                if (LoggerProvider != null)
                    LoggerProvider("Post->" + Method, Data.ToJsonString(), Message);
                if (string.IsNullOrEmpty(Message))
                    return GetErrorResult<TResult>("请求处理失败");
                return Message.ConvertToResult<TResult>();
            }
            catch (HttpRequestException ex)
            {
                LogHelper.Error(url + Data, ex.Message, ex);
                return GetErrorResult<TResult>("请求处理失败");
            }
            catch (Exception ex)
            {
                LogHelper.Error(url+Data, ex.Message, ex);
                return GetErrorResult<TResult>("系统繁忙");
            }
        }
        #endregion

        #region Get请求
        /// <summary>
        /// Get请求需授权数据
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <typeparam name="TParameter">测试类型</typeparam>
        /// <param name="Method">请求方法</param>
        /// <param name="Data">请求数据</param>
        /// <param name="Token">授权令牌</param>
        /// <returns></returns>
        public virtual TResult Get<TResult, TParameter>(string Method, TParameter Data, string Token) where TResult : Result, new() where TParameter : class, new()
        {
            return Get<TResult, TParameter>(Method + "?token=" + Token, Data);
        }
        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <typeparam name="TParameter">测试类型</typeparam>
        /// <param name="Method">请求方法</param>
        /// <param name="Data">请求数据</param>
        /// <returns></returns>
        public virtual TResult Get<TResult, TParameter>(string Method, TParameter Data) where TResult : Result, new() where TParameter : class, new()
        {
            string param = "";
            if (Data != null)
            {
                Dictionary<string, string> list = Data.ObjectConvertDictionary<TParameter>();
                param = UtilsHelper.StringStructure(list);
            }
            return Get<TResult>(Method, param);
        }
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="Method">请求方法</param>
        /// <param name="Data">请求数据</param>
        /// <param name="Token">授权令牌</param>
        /// <returns></returns>
        public virtual TResult Get<TResult>(string Method, string Data, string Token) where TResult : Result, new()
        {
            return Get<TResult>(Method + "?token=" + Token, Data);
        }
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="Method">请求方法</param>
        /// <param name="Data">请求数据</param>
        /// <returns></returns>
        public virtual TResult Get<TResult>(string Method, string Data) where TResult : Result, new()
        {
            string url = GetUrl(Method);
            try
            {
                HttpRequest request = new HttpRequest(url);
                string Message = request.GetRequest(Data);
                if (LoggerProvider != null)
                    LoggerProvider("GET->" + Method, Data, Message);
                if (string.IsNullOrEmpty(Message))
                    return GetErrorResult<TResult>("请求处理失败");
                return Message.ConvertToResult<TResult>();
            }
            catch (HttpRequestException ex)
            {
                LogHelper.Error(url+Data, ex.Message, ex);
                return GetErrorResult<TResult>("请求处理失败");
            }
            catch (Exception ex)
            {

                LogHelper.Error(url + Data, ex.Message, ex);
                return GetErrorResult<TResult>("系统繁忙");
            }
        }

        #endregion

        #region 工具类
        /// <summary>
        /// 获取请求地址
        /// </summary>
        /// <returns></returns>
        private string GetUrl(string Method)
        {
            string url;
            if (Method.Contains("http://") || Method.Contains("https://"))
                url = Method;
            else
                url = ServiceUrl + Method;
            return url;
        }
        /// <summary>
        /// 返回错误对象
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        private TResult GetErrorResult<TResult>(string message) where TResult : Result, new()
        {
            TResult res = new TResult();
            res.Message = message;
            res.Status = (int)ResultTypes.InnerError;
            res.Success = false;
            return res;
        }
        /// <summary>
        /// 获取链接地址
        /// </summary>
        private string ServiceUrl
        {
            get
            {
                if (ConnectionMultiplexer.ContainsKey(ConnectionName))
                    return ConnectionMultiplexer[ConnectionName];
                else return "";
            }
        }
        #endregion

    }
}
