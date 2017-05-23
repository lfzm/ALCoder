#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：返回结果对象处理服务
*
*　　	创建人： 阿凌
*       创建人Email：513845034@qq.com
*       
*****************************************************************************/

#endregion

using AL.Common.Extention;
using System;

namespace AL.Common.Models
{
    /// <summary>
    ///  返回结果对象处理服务
    /// </summary>
    public static class ResultExtention
    {
        /// <summary>
        /// 将结果实体转换成其他结果实体
        /// </summary>
        /// <typeparam name="TResult">输出对象</typeparam>
        /// <typeparam name="TPara"></typeparam>
        /// <returns>输出对象</returns>
        public static Result<TResult> ConvertToResult<TPara, TResult>(this Result<TPara> source,
            Func<TPara, TResult> func = null)
        {
            Result<TResult> ot = new Result<TResult>();
            ot.Status = source.Status;
            ot.Message = source.Message;
            ot.Success = source.Success;
            if (func != null && source.Data != null)
            {
                ot.Data = func(source.Data);
            }
            return ot;
        }

        /// <summary>
        ///   将结果实体转换成其他结果实体   --转化结果是通过 泛型 定义的Result实体
        ///   仅转化 Ret和 Message 的值  
        /// </summary>
        /// <typeparam name="TResult">输出对象</typeparam>
        /// <returns>输出对象</returns>
        public static Result<TResult> ConvertToResultOnly<TResult>(this Result source)
        {
            Result<TResult> ot = new Result<TResult>();
            ot.Status = source.Status;
            ot.Message = source.Message;
            ot.Success = source.Success;
            return ot;
        }

        /// <summary>
        ///  将结果实体转换成其他结果实体   --转化结果是通过 继承 定义的Result实体  
        ///    仅转化 Ret和 Message 的值
        /// </summary>
        /// <typeparam name="TResult">输出对象</typeparam>
        /// <returns>输出对象</returns>
        public static TResult ConvertToResult<TResult>(this Result source)
            where TResult : Result, new()
        {
            TResult ot = new TResult();
            ot.Status = source.Status;
            ot.Message = source.Message;
            ot.Success = source.Success;
            return ot;
        }

        /// <summary>
        /// Json字符串转 Result对象
        /// </summary>
        /// <typeparam name="TResult">输出对象</typeparam>
        /// <param name="source">Json字符串</param>
        /// <returns>输出对象</returns>
        public static TResult ConvertToResult<TResult>(this string source) where TResult : Result
        {
            return source.ToFromJson<TResult>();
        }
        /// <summary>
        /// 结果对象转换为String
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToJsonString(this Result source)
        {
           return JsonExtention.ToJsonIgnoreNullString( source);
        }
     
    }
}
