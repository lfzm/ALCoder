using Newtonsoft.Json;

namespace Zzll.Net.Framework
{
    /// <summary>
    /// 响应对象
    /// </summary>
    public class Response
    {
        public Response(Request req)
        {
            this.req = req;
            this.id = req.id;
            this.code = req.code;
        }
        /// <summary>
        /// 请求编号
        /// </summary>
        [JsonIgnore]
        public long id { get; set; }
        /// <summary>
        /// 请求对象
        /// </summary>
        [JsonIgnore]
        public Request req { get; set; }
        /// <summary>
        /// 操作代码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 响应功能报文
        /// </summary>
        public string para { get; set; }
    }
}
