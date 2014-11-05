using System;
using System.Linq.Expressions;
using Sam.DAO.Entity;

namespace Sam.DAO.DaoInterface
{
   public interface IDaoContextWrite
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        int Insert<T>(T entity) where T : BaseEntity;

        /// <summary>
        /// 事务添加多个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        bool Insert<T>(T[] entities) where T : BaseEntity;

        /// <summary>
        /// 添加，适用于有自增字段的表，成功后返回主键值（暂时不支持oracle）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>自增值</returns>
        int Insert2<T>(T entity) where T : BaseEntity;

        /// <summary>
        /// 根据主键修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        int Update<T>(T entity) where T : BaseEntity;

        /// <summary>
        /// 根据主键修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="properties">要修改的属性集合</param>
        int Update<T>(T entity, string[] properties) where T : BaseEntity;

        /// <summary>
        /// 根据条件修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="whereFunc">修改条件，若条件为null，则修改整个表的数据（请谨慎使用）</param>
        /// <param name="properties">要修改的属性集合</param>
        int Update<T>(T entity, Expression<Func<T, bool>> whereFunc, string[] properties) where T : BaseEntity;

        /// <summary>
        /// 按主键删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        int Delete<T>(T entity) where T : BaseEntity;

        /// <summary>
        /// 按条件删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereFunc">删除条件，若条件为null，则删除整个表的数据（请谨慎使用）</param>
        int Delete<T>(Expression<Func<T, bool>> whereFunc) where T : BaseEntity, new();
    }
}
