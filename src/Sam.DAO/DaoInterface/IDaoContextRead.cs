#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using Sam.DAO.Entity;
using Sam.DAO.Tool;

namespace Sam.DAO.DaoInterface
{
    /// <summary>
    /// 读操作
    /// </summary>
    public interface IDaoContextRead
    {
        /// <summary>
        /// 统计符合条件的个数，条件为null时统计所有
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">查询条件</param>
        /// <returns></returns>
        int Count<T>(Expression<Func<T, bool>> func=null)where T : BaseEntity, new();

        /// <summary>
        /// 根据主键查找实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValues">主键值，主键为组合主键时按顺序值集合</param>
        /// <returns></returns>
        T Find<T>(params object[] keyValues) where T : BaseEntity, new();

        /// <summary>
        /// 联表查询
        /// </summary>
        /// <typeparam name="T1">主表内容</typeparam>
        /// <typeparam name="T2">外键表内容</typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        IEnumerable<CombinationEntity<T1, T2>> Select<T1, T2>(Expression<Func<T1, bool>> func = null)
            where T1 : BaseEntity, new()
            where T2 : BaseEntity, new();

        /// <summary>
        /// 查询实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">sql语句参数</param>
        /// <returns>实体集合</returns>
        IEnumerable<T> Select<T>(string sql, params DbParameter[] parameters) where T : BaseEntity, new();

        /// <summary>
        /// 查询实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">lambda表达式的条件</param>
        /// <returns></returns>
        IEnumerable<T> Select<T>(Expression<Func<T, bool>> func=null) where T : BaseEntity, new();

        /// <summary>
        /// 查询实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">lambda表达式的条件</param>
        /// <param name="properties">需要显示的字段</param>
        /// <returns></returns>
        IEnumerable<T> Select<T>(Expression<Func<T, bool>> func, string[] properties) where T : BaseEntity, new();

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">当前页数，从1开始算</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="func">查询条件</param>
        /// <param name="orderFunc">排序条件,仅支持单字段排序</param>
        /// <param name="recordCount">总共记录 </param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        IEnumerable<T> Select<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> func, Expression<Func<T, object>> orderFunc, out int recordCount, bool isAsc=true) where T : BaseEntity, new();

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">当前页数，从1开始算</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="func">查询条件</param>
        /// <param name="orderFunc"> </param>
        /// <param name="isAsc">是否顺序（true为顺序，false为倒序） </param>
        /// <returns></returns>
        IEnumerable<T> Select<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> func,
                                 Expression<Func<T, object>> orderFunc, bool isAsc=true) where T : BaseEntity, new();

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">当前页数，从1开始算</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="func">查询条件</param>
        /// <param name="orderFuncs"> 排序集合</param>
        /// <returns></returns>
        IEnumerable<T> Select<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> func,
                                 params OrderFunction<T>[] orderFuncs) where T : BaseEntity, new();
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">当前页数，从1开始算</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="func">查询条件</param>
        /// <param name="recordCount">返回总数</param>
        /// <param name="orderFuncs">排序条件组合，仅支持数值类型</param>
        /// <returns></returns>
        IEnumerable<T> Select<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> func, out int recordCount, params OrderFunction<T>[] orderFuncs) where T : BaseEntity, new();
    }
}
#pragma warning restore 1591