#region Copyright (C) 2017 AL系列开源项目

/***************************************************************************
*　　	文件功能描述：通用结果枚举
*
*　　	创建人： 阿凌
*       创建人Email：513845034@qq.com
*       
*****************************************************************************/

#endregion

using AL.Common.Extention;

namespace AL.Common.Models
{
    /// <summary>
    ///   结果类型
    /// </summary>
    public enum ResultTypes
    {
        /// <summary>
        /// 成功
        /// </summary>
        [ALDescript("成功")]
        Success = 200,
        /// <summary>
        ///  处理失败
        /// </summary>
        [ALDescript("处理失败")]
        ProcessingFailed = 202,
        /// <summary>
        /// 参数错误
        /// </summary>
        [ALDescript("参数错误")]
        ParaError = 301,
        /// <summary>
        /// 条件不满足
        /// </summary>
        [ALDescript("条件不满足")]
        ParaNotMeet = 310,
        /// <summary>
        /// 添加失败
        /// </summary>
        [ALDescript("添加失败")]
        AddFail = 320,
        /// <summary>
        /// 更新失败
        /// </summary>
        UpdateFail = 330,
        /// <summary>
        /// 当前用户未验证身份信息
        /// </summary>
        UnAuthorize = 401,
        /// <summary>
        /// 对象不存在
        /// </summary>
        ObjectNull = 404,
        /// <summary>
        /// 对象已存在
        /// </summary>
        ObjectExsit = 410,
        /// <summary>
        /// 对象状态不正常
        /// </summary>
        ObjectStateError = 420,
        /// <summary>
        /// 禁止访问
        /// </summary>
        NoRight = 430,
        /// <summary>
        /// 内部错误（服务器错误）
        /// </summary>
        InnerError = 500

    }
}
