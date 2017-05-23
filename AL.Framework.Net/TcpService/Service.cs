using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zzll.Net.Framework.Helper;
using Newtonsoft.Json;

namespace Zzll.Net.Framework.TcpService
{
    /// <summary>
    /// Tcp服务端处理器
    /// </summary>
    public class Service : BaseHandle<Config>
    {
        #region ==========属性========== 
        /// <summary>
        /// TCP服务
        /// </summary>
        protected TcpService svr { get; set; }
        /// <summary>
        /// 当前在线用户量
        /// </summary>
        public int ClientOnlineCount
        {
            get
            {
                return svr.SessionTable.Count;
            }
        }

        #endregion

        /// <summary>
        /// 启动Tcp服务端
        /// </summary>
        public override void Start(Func<Request, Response> func)
        {
            if (base.config == null)
                throw new Exception("未设置Tcp服务端服务配置");
            //设置回调函数
            base.Handle_Event = func;
            if (config.Port <= 0)
                return;
            svr = new TcpService(config.Port, config.Encoding);
            svr.Suffix = config.Suffix;
            svr.DefaultMaxClient = config.DefaultMaxClient;
            svr.ServerStart += new TcpService.Dlgestr(ServerStart);
            svr.ServerError += new TcpService.Dlgestr(ServerError);
            svr.ServerStop += new TcpService.Dlgestr(ServerStop);
            svr.ClientRecvData += new TcpService.NetEvent(ClientRecvData);
            svr.ClientClose += new TcpService.NetClientEvent(ClientClose);
            svr.ClientConn += new TcpService.NetClientEvent(ClientConn);
            svr.IsHeartbeat = config.IsHeartbeat;
            svr.Start();
            Console.WriteLine(DateTime.Now.ToString("MM-dd HH:mm:ss:fff") + " 服务器启动，开放端口：" + config.Port);
            LogHelper.Add("启动TCP通讯服务->StartService", config.Port, LogType.TcpService);
        }

        #region ========通讯服务======== 
        /// <summary>
        /// 消息发送
        /// </summary>
        /// <param name="resp">响应对象</param>
        public void Send(Response resp)
        {
            if (resp == null)//响应为空
                return;
            Session client = ((TcpServiceRequest)resp.req).client;
            if (client == null)
                return;
            LogHelper.Add(client.Id + "->" + resp.id + "->Send", resp.para, LogType.TcpService);
            //获取IP白名单加密方式
            ClientWhite white = config.ClientWhiteList.Find(f => f.ClientIp.Split(',').Contains(client.ConnIp));
            if (white == null)
            {
                LogHelper.Add("【ERROR】" + client.ConnIp, "未在白名单找到报文", LogType.Error);
                return;
            }
            if (white.encryptType != EncryptType.Normal)
                LogHelper.Add(client.Id + "->" + resp.id + "->加密获取密文", resp.para, LogType.TcpService);
            resp.para = Encryption(resp.para, white);
            //返回响应
            svr.Send(client, resp.para);
        }
        /// <summary>
        /// 数据接收事件
        /// </summary>
        /// <param name="msg">请求报文</param>
        /// <param name="client">会话上下文</param>
        void ClientRecvData(string msg, Session client)
        {
            try
            {
                //生成请求唯一标示
                long id = Utils.GetReqId();

                #region 获取请求对象
                LogHelper.Add(client.Id + "->" + id + "->ClientRecvData", msg, LogType.TcpService);

                //获取IP白名单
                ClientWhite white = config.ClientWhiteList.Find(f => f.ClientIp.Split(',').Contains(client.ConnIp));
                if (white == null)
                {
                    LogHelper.Add("【ERROR】" + id, "未在白名单找到报文", LogType.Error);
                    return;
                }
                try
                {
                    msg = Decryption(msg, white); //报文解密
                }
                catch (Exception ex)
                {
                    LogHelper.Add("【ERROR】" + id + "->报文解密失败", ex.Message, LogType.Error);
                    return;
                }

                if (white.encryptType != EncryptType.Normal)
                    LogHelper.Add(client.Id + "->" + id + "->解密获取明文", msg, LogType.TcpService);

                TcpServiceRequest req = null;
                try
                {
                    req = JsonConvert.DeserializeObject<TcpServiceRequest>(msg);
                    req.id = id;
                    req.channel = RequestChannel.TcpService;
                    req.client = client;
                }
                catch (Exception ex)
                {
                    LogHelper.Add("【ERROR】" + id + "->启动处理,报文解析错误", ex.Message, LogType.Error);
                    return;
                }
                if (req == null)
                {
                    LogHelper.Add("【ERROR】" + id + "->启动处理,报文解析错误", msg, LogType.Error);
                    return;
                }
                //判断该客户端是否允许访问该功能
                if (!white.FuncList.Split(',').Contains("ALL") && !white.FuncList.Split(',').Contains(req.code))
                {
                    LogHelper.Add(id.ToString(), "该服务器不支持此功能", LogType.Error);
                    return;
                }
                #endregion

                //发送给处理区
                if (base.Handle_Event == null)
                    return;
                Response resp = base.Handle_Event(req);
                if (resp == null)
                {
                    LogHelper.Add("【ERROR】" + id, "响应为空", LogType.Error);
                    return;
                }
                //发送响应报文00
                Send(resp);
            }
            catch (Exception ex)
            {
                svr.Send(client, "0");
                LogHelper.Add("【ERROR】" + client.Id + "->请求,内部错误", "msg:" + msg + "；" + ex.Message, LogType.Error);
            }
        }
        /// <summary>
        /// 客户端断开事件
        /// </summary>
        /// <param name="e"></param>
        void ClientClose(Session e)
        {
            LogHelper.Add(e.Id + "->ClientClose", "IP:" + e.ConnIp + ",Port:" + e.Port, LogType.TcpService);
        }
        /// <summary>
        /// 客户端连接事件
        /// </summary>
        /// <param name="e"></param>
        void ClientConn(Session e)
        {
            LogHelper.Add(e.Id + "->ClientConn", "IP:" + e.ConnIp + ",Port:" + e.Port, LogType.TcpService);
            //判断IP是否在白名单中
            if (config.ClientWhiteList.Where(f => f.ClientIp.Split(',').Contains(e.ConnIp)).Count() == 0)
            {
                LogHelper.Add(e.Id.ToString(), "客户端不在白名单", LogType.Error);
                svr.Close(e.Client);
                return;
            }
        }
        /// <summary>
        /// 服务启动事件
        /// </summary>
        /// <param name="msg"></param>
        void ServerStart(string msg)
        {
            LogHelper.Add("->ServerStart", msg, LogType.TcpService);
        }
        /// <summary>
        /// 服务处理失败事件
        /// </summary>
        /// <param name="msg"></param>
        void ServerError(string msg)
        {
            LogHelper.Add("->ServerError", msg, LogType.TcpService);

        }
        /// <summary>
        /// 服务停止事件
        /// </summary>
        /// <param name="msg"></param>
        void ServerStop(string msg)
        {
            LogHelper.Add("->ServerStop", msg, LogType.TcpService);
        }
        #endregion

        #region ========加密机制========
        /// <summary>
        /// 加密报文
        /// </summary>
        /// <param name="packets">明文</param>
        /// <param name="white">白名单设置</param>
        /// <returns></returns>
        public string Encryption(string packets, ClientWhite white)
        {
            if (white.encryptType == EncryptType.DES3)
            {
                #region DES3层加密
                string secretKey = Helper.Encryption.getKey(white.Secretkey);//获取秘钥
                                                                             //生成签名
                string sign = Helper.Encryption.EncryptMD5(packets, new EncodHelper(config.Encoding).ToEncoding());
                //获取报文密文
                string ciphertext = Helper.Encryption.Encrypt3ECB(packets, secretKey, new EncodHelper(config.Encoding).ToEncoding());
                string length = ciphertext.Length.ToString();
                for (int i = length.Length; i < 6; i++)
                {
                    length = "0" + length;
                }
                packets = length + ciphertext + sign;
                return packets;
                #endregion
            }
            else if (white.encryptType == EncryptType.MD5)
            {
                #region MD5签名验证
                string secretKey = Helper.Encryption.getKey(white.Secretkey);//获取秘钥
                string sign = Helper.Encryption.EncryptMD5(packets + secretKey, new EncodHelper(config.Encoding).ToEncoding());//获取报文签名
                return packets + sign;
                #endregion
            }
            else
                return packets;
        }

        /// <summary>
        /// 解密报文
        /// </summary>
        /// <param name="packets">密文</param>
        /// <param name="white">白名单设置</param>
        /// <returns></returns>
        public string Decryption(string packets, ClientWhite white)
        {
            if (white.encryptType == EncryptType.DES3)
            {
                #region DES3层加密
                string secretKey = Helper.Encryption.getKey(white.Secretkey);
                Encoding e = new EncodHelper(config.Encoding).ToEncoding();
                //获取签名长度
                int length = Convert.ToInt32(packets.Substring(0, 6));
                //获取签名
                string sgin = packets.Substring(length + 6, 32);
                //获取加密的报文
                string ciphertext = packets.Substring(6, length);
                //进行解密报文
                string data = Helper.Encryption.Decrypt3ECB(ciphertext, secretKey, e);
                //验证报文是否正确
                ciphertext = Helper.Encryption.EncryptMD5(data, e);
                if (!ciphertext.Equals(sgin))
                    throw new Exception("报文签名无效");
                return data;
                #endregion
            }
            else if (white.encryptType == EncryptType.MD5)
            {
                #region MD5签名验证
                string secretKey = Helper.Encryption.getKey(white.Secretkey);
                //截取报文的Sign
                string reqSign = packets.Substring(packets.Length - 32, 32);
                //获取报文信息
                string packet = packets.Substring(0, packets.Length - 32);
                //生成报文签名
                string sign = Helper.Encryption.EncryptMD5(packets + secretKey, new EncodHelper(config.Encoding).ToEncoding());//获取报文签名
                if (sign == reqSign)
                    throw new Exception("报文签名无效");
                return packet;
                #endregion
            }
            else
                return packets;

        }
        #endregion
    }
}
