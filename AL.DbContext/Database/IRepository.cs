using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AL.DbContext.Database
{
    /// <summary>
    /// 数据存储库接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public  interface IRepository
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entity">添加实体</param>
        /// <param name="isSubmit">是否提交 默认不提交</param>
        int Insert<TEntity>(TEntity entity, bool isSubmit = false) where TEntity : class;
        /// <summary>
        /// 批量添加并提交
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="list"></param>
        int Inserts<TEntity>(List<TEntity> list) where TEntity : class;
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entity">更新实体</param>
        /// <param name="isSubmit">是否提交 默认不提交</param>
        int Update<TEntity>(TEntity entity, bool isSubmit = false) where TEntity : class;
        /// <summary>
        /// 更新列表
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="list"></param>
        int Updates<TEntity>(List<TEntity> list) where TEntity : class;

        /// <summary>
        /// 更新指定字段
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entity">更新实体</param>
        /// <param name="isSubmit">是否提交 默认不提交</param>
        int Update<TEntity>(Expression<Action<TEntity>> entity, bool isSubmit = false) where TEntity : class;
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entity">删除实体</param>
        /// <param name="isSubmit">是否提交 默认不提交</param>
        int Delete<TEntity>(TEntity entity, bool isSubmit = false) where TEntity : class;

        #region 查询
        /// <summary>
        /// 根据主键取一个
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetEntity<TEntity>(params object[] id) where TEntity : class;
        /// <summary>
        /// 根据条件取一个
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TEntity GetEntity<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        /// <summary>
        /// 实体集对象的可查询结果集
        /// </summary>
        IQueryable<TEntity> GetEntities<TEntity>() where TEntity : class;
        /// <summary>
        /// 统计数量
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Count<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        /// <summary>
        /// 返回结果集
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<TEntity> GetEntities<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        /// <summary>
        /// 带有排序的结果集
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        IQueryable<TEntity> GetEntities<TEntity>(Expression<Func<TEntity, bool>> predicate, Action<Orderable<TEntity>> order) where TEntity : class;
        /// <summary>
        /// 带分页和排序的结果集
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="order"></param>
        /// <param name="skip"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IQueryable<TEntity> GetEntities<TEntity>(Expression<Func<TEntity, bool>> predicate, Action<Orderable<TEntity>> order, int skip, int count, out int total) where TEntity : class;
        #endregion


    }
}
