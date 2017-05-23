#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：通用返回结果实体（带ID）
*
*　　	创建人： 阿凌
*       创建人Email：513845034@qq.com
*       
*****************************************************************************/

#endregion
namespace AL.Common.Models
{
    /// <summary>
    /// 结果返回 带ID
    /// </summary>
   public class ResultId:Result
    {
        /// <summary>
        /// 
        /// </summary>
        public ResultId():base()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id"></param>
        public ResultId(long id)
            : base( null, ResultTypes.Success)
        {
            Id = id;
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ret"></param>
        /// <param name="message"></param>
        public ResultId(string message,int ret):base(message, ret)
        {
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ret"></param>
        public ResultId(string message, ResultTypes ret)
            : base(message,ret)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ret"></param>
        /// <param name="id">Id</param>
        public ResultId(string message, ResultTypes ret,long id)
            : base(message, ret)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ret"></param>
        /// <param name="id">Id</param>
        public ResultId(string message, int ret, long id)
            : base(message, ret)
        {
        }

        /// <summary>
        /// 返回的关键值，如返回添加是否成功并返回id
        /// </summary>
        public long Id { get; set; }
    }
}
