using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AL.DbContext.Database
{
    /// <summary>
    /// EF数据访问层基类
    /// </summary>
    public class DbContextBase : System.Data.Entity.DbContext, IRepository
    {
        #region 构造函数
        /// <summary>
        /// 实体创建映射
        /// </summary>
        public Action<DbModelBuilder> ModelCreatingProvider { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="nameOrConnectionString">连接字符串或者配置文件连接字符串名称</param>
        /// <param name="logger">日志记录函数</param>
        public DbContextBase(string nameOrConnectionString, Action<string> logger) : base(nameOrConnectionString)
        {
            try
            {
                if (this.Database.Log == null && logger != null)
                    this.Database.Log = logger;
            }
            catch (Exception)
            {

                throw new Exception("EF底层数据出现问题，请检查...");
            }

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="nameOrConnectionString">连接字符串或者App.Config配置名称</param>
        public DbContextBase(string nameOrConnectionString)
            : this(nameOrConnectionString, null)
        {

        }
        /// <summary>
        /// 创建Model映射
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder != null)
                ModelCreatingProvider(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
        #endregion

        #region 接口实现

        #region 增删改函数
        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="list">实体集合</param>
        public virtual int Inserts<TEntity>(List<TEntity> list) where TEntity : class
        {
            IEnumerable<TEntity> entities = list;

            entities = entities.ToArray();
            string cs = base.Database.Connection.ConnectionString;
            var conn = new SqlConnection(cs);
            conn.Open();

            Type t = typeof(TEntity);

            var bulkCopy = new SqlBulkCopy(conn)
            {
                DestinationTableName = string.Format("[{0}]", t.Name)
            };

            var properties = t.GetProperties().Where(EventTypeFilter).ToArray();
            var table = new DataTable();

            foreach (var property in properties)
            {
                Type propertyType = property.PropertyType;
                if (propertyType.IsGenericType &&
                    propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    propertyType = Nullable.GetUnderlyingType(propertyType);
                }

                table.Columns.Add(new DataColumn(property.Name, propertyType));
            }

            foreach (var entity in entities)
            {
                table.Rows.Add(properties.Select(
                  property => GetPropertyValue(
                  property.GetValue(entity, null))).ToArray());
            }

            bulkCopy.WriteToServer(table);
            conn.Close();
            return table.Rows.Count;
        }
        /// <summary>
        /// 批量添加并提交
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSubmit">是否提交 默认不提交</param>
        public virtual int Insert<TEntity>(TEntity entity, bool isSubmit = false) where TEntity : class
        {
            base.Entry<TEntity>(entity);
            base.Set<TEntity>().Add(entity);
            if (isSubmit)
                return this.SaveChanges();
            else
                return 0;
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="list">更新实体集合</param>
        public virtual int Updates<TEntity>(List<TEntity> list) where TEntity : class
        {
            list.ForEach(entity =>
            {
                this.Update<TEntity>(entity, false);
            });
            return this.SaveChanges();
        }

        private object _updateDuplicate;
        /// <summary>
        /// 开始更新（只更新已经修改的字段）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        public virtual void UpdateBegin<TEntity>(TEntity entity) where TEntity : ICloneable
        {
            _updateDuplicate = entity.Clone();
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSubmit">是否提交 默认不提交</param>
        public virtual int UpdateEnd<TEntity>(TEntity entity, bool isSubmit = false) where TEntity : class
        {
            base.Set<TEntity>().Attach(entity);
            Type type = entity.GetType();

            foreach (var item in type.GetProperties())
            {
                string name = item.Name;
                object oldv = type.GetProperty(name).GetValue(_updateDuplicate);
                object newv = base.Entry(entity).Property(name).CurrentValue;

                if (oldv == null || newv == null)
                {
                    if (oldv == newv)
                        continue;
                    else
                    {
                        base.Entry(entity).Property(name).IsModified = true;
                    }
                }
               else if (!oldv.Equals(newv))
                    base.Entry(entity).Property(name).IsModified = true;
            }

            if (isSubmit)
                return this.SaveChanges();
            else
                return 0;
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSubmit"></param>
        public int Update<TEntity>(TEntity entity, bool isSubmit = false) where TEntity : class
        {
            base.Set<TEntity>().Attach(entity);
            base.Entry(entity).State = EntityState.Modified;
            if (isSubmit)
                return this.SaveChanges();
            else
                return 0;
        }

        /// <summary>
        /// 更新指定字段
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entity">更新实体</param>
        /// <param name="isSubmit">是否提交 默认不提交</param>
        public virtual int Update<TEntity>(Expression<Action<TEntity>> entity, bool isSubmit) where TEntity : class
        {
            TEntity newEntity = typeof(TEntity).GetConstructor(Type.EmptyTypes).Invoke(null) as TEntity;//建立指定类型的实例
            List<string> propertyNameList = new List<string>();
            MemberInitExpression param = entity.Body as MemberInitExpression;
            foreach (var item in param.Bindings)
            {
                string propertyName = item.Member.Name;
                object propertyValue;
                var memberAssignment = item as MemberAssignment;
                if (memberAssignment.Expression.NodeType == ExpressionType.Constant)
                {
                    propertyValue = (memberAssignment.Expression as ConstantExpression).Value;
                }
                else
                {
                    propertyValue = Expression.Lambda(memberAssignment.Expression, null).Compile().DynamicInvoke();
                }
                typeof(TEntity).GetProperty(propertyName).SetValue(newEntity, propertyValue, null);
                propertyNameList.Add(propertyName);
            }
            base.Set<TEntity>().Attach(newEntity);
            base.Configuration.ValidateOnSaveEnabled = false;
            var ObjectStateEntry = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntry(newEntity);
            propertyNameList.ForEach(x => ObjectStateEntry.SetModifiedProperty(x.Trim()));
            if (isSubmit)
                return this.SaveChanges();
            else
                return 0;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entity">删除实体</param>
        /// <param name="isSubmit">是否提交 默认不提交</param>
        public virtual int Delete<TEntity>(TEntity entity, bool isSubmit) where TEntity : class
        {
            base.Set<TEntity>().Attach(entity);
            base.Set<TEntity>().Remove(entity);
            if (isSubmit)
                return this.SaveChanges();
            else
                return 0;
        }

        #endregion

        #region 查询函数
        /// <summary>
        /// 统计数量
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int Count<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return GetEntities(predicate).Count();
        }
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool Exist<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return Count(predicate) > 0;
        }
        /// <summary>
        /// 根据主键取一个
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity GetEntity<TEntity>(params object[] id) where TEntity : class
        {
            return base.Set<TEntity>().Find(id);
        }
        /// <summary>
        /// 根据条件取一个
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual TEntity GetEntity<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return GetEntities(predicate).SingleOrDefault();
        }
        /// <summary>
        /// 实体集对象的可查询结果集
        /// </summary>
        public virtual IQueryable<TEntity> GetEntities<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
        /// <summary>
        /// 返回结果集
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetEntities<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return GetEntities<TEntity>().Where(predicate);
        }
        /// <summary>
        /// 带有排序的结果集
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetEntities<TEntity>(Expression<Func<TEntity, bool>> predicate, Action<Orderable<TEntity>> order) where TEntity : class
        {
            var orderable = new Orderable<TEntity>(GetEntities(predicate).AsQueryable());
            order(orderable);
            return orderable.Queryable;
        }
        /// <summary>
        /// 带分页和排序的结果集
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="order"></param>
        /// <param name="skip"></param>
        /// <param name="count"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetEntities<TEntity>(Expression<Func<TEntity, bool>> predicate, Action<Orderable<TEntity>> order, int skip, int count, out int total) where TEntity : class
        {
            IQueryable<TEntity> source = GetEntities(predicate, order);
            total = ((IEnumerable<TEntity>)source).Count();
            return source.Skip(skip).Take(count);
        }
        #endregion

        #endregion

        #region Methods
        /// <summary>
        /// 提交数据库处理
        /// </summary>
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges(); //在有
            }
            catch (Exception ex)
            {
                string Message = "error:";
                if (ex.InnerException == null)
                    Message += ex.Message + ",";
                else if (ex.InnerException.InnerException == null)
                    Message += ex.InnerException.Message + ",";
                else if (ex.InnerException.InnerException.InnerException == null)
                    Message += ex.InnerException.InnerException.Message + ",";
                throw new Exception(Message);
            }
        }

        /// <summary>
        /// 提交数据库处理
        /// </summary>
        public override Task<int> SaveChangesAsync()
        {
            try
            {
                return base.SaveChangesAsync(); //在有
            }
            catch (Exception ex)
            {
                string Message = "error:";
                if (ex.InnerException == null)
                    Message += ex.Message + ",";
                else if (ex.InnerException.InnerException == null)
                    Message += ex.InnerException.Message + ",";
                else if (ex.InnerException.InnerException.InnerException == null)
                    Message += ex.InnerException.InnerException.Message + ",";
                throw new Exception(Message);
            }
        }
        private bool EventTypeFilter(System.Reflection.PropertyInfo p)
        {
            var attribute = Attribute.GetCustomAttribute(p,
                typeof(AssociationAttribute)) as AssociationAttribute;

            if (attribute == null) return true;
            if (attribute.IsForeignKey == false) return true;
            return false;
        }
        private object GetPropertyValue(object o)
        {
            if (o == null)
                return DBNull.Value;
            return o;
        }
        #endregion
    }

}
