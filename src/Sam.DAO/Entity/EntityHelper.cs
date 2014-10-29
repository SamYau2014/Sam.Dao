using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Sam.DAO.Attribute;
using Sam.DAO.Builder.Data;
using Sam.DAO.ExFunc;
using Sam.DAO.Linq;
using Sam.DAO.Tool;
using Sam.DAO.InnerException;

namespace Sam.DAO.Entity
{
    public class EntityHelper
    {
        private const string _selectFormat = "select * from {0} {1}";
        private const string _insertFormat = "insert into {0}({1}) values({2})";
        private const string _updateFormat = "update {0} set {1} {2}";
        private const string _deleteFormat = "delete from {0} {1}";

        private readonly Dictionary<string, BaseEntity> _foreignEntityPool = new Dictionary<string, BaseEntity>();


        /// <summary>
        /// 数据库访问对象
        /// </summary>
        //  private readonly DbHelper _dbHelper;
        private readonly DB _dbHelper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper"></param>
        public EntityHelper(DB dbHelper)
        {
            _dbHelper = dbHelper;
        }

        /// <summary>
        /// 把datatable转换为对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        private IEnumerable<T> FromDataTable<T>(DataTable dt) where T : IEntity, new()
        {
            if (dt == null)
                return null;
            var list = new List<T>();

            foreach (DataRow dr in dt.Rows)
            {
                T entity = new T();
                entity.FromDataRow(dr);

                //BindRalationProperty(entity, null);
                list.Add(entity);
            }
            return list;
        }

        //给关联属性赋值
        private void BindRalationProperty(IEntity entity, BaseEntity foreignEntity)  
        {
            //if (entity.IsSetRelation())
            //    return;
        
            var relationProperties = entity.GetRelationProperties();
            if (relationProperties == null || relationProperties.Count == 0)
            {
                return;
            }
            if (relationProperties.Count > 1)
                throw new DaoException("目前不支持多个外键查询");
            Type relationEntityType;

            if (foreignEntity.GetType().IsGenericType)
            {
                relationEntityType = foreignEntity.GetType().GetGenericArguments()[0];
            }
            else
            {
                relationEntityType = foreignEntity.GetType();
            }

            if (!relationEntityType.IsSubclassOf(typeof(BaseEntity)))
                throw new Exception("关联属性中的实体类必须派生自BaseEntity");

            //关联属性中的实体实例
            // var relationObj = Activator.CreateInstance(relationEntityType) as BaseEntity;
            //主表名
            //  string tableName = entity.GetTableName();
            //关联表名
            string relationTableName = foreignEntity.GetTableName();

            string sql = string.Empty, relationSql = string.Empty, keyName = relationTableName+".";
            foreach (KeyValuePair<PropertyInfo, RelationAttribute> kvp in relationProperties)
            {
                //关联属性
                var property = kvp.Key;
                //关联属性的特性
                var relationAttribute = kvp.Value;
                //关联属性中的实体类型
          
               

                //相关联的属性的值
                var propertyValue = entity.GetValueByColumnName(relationAttribute.Key);

            
                if (propertyValue == null)
                    continue;

                string clauseValue;

                if (propertyValue is string)
                    clauseValue = string.Format("'{0}'", propertyValue.ToString().Replace("'", "''"));
                else
                    clauseValue = propertyValue.ToString();

                keyName += "." + clauseValue;

                if (relationSql == string.Empty)
                    relationSql = string.Format(" {0}={1} ", relationAttribute.RelationKey, clauseValue);
                else
                    relationSql += " and" + string.Format(" {0}={1} ", relationAttribute.RelationKey, clauseValue);
              //  string sql = string.Format(_selectFormat, relationTableName, relationSql);
              
            }

            //判断是否已经查询过该实体
            if (_foreignEntityPool.ContainsKey(keyName))
            {
                foreignEntity = _foreignEntityPool[keyName];
                return;
            }

            DataTable dt = _dbHelper.ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreignEntity.FromDataRow(dt.Rows[0]);
                _foreignEntityPool[keyName] = foreignEntity;
                #region old code
                /*
                    if (property.PropertyType.IsGenericType)
                    {
                        var dType = typeof(List<>);
                        var listType = dType.MakeGenericType(relationEntityType);
                        var list = property.GetValue(entity, null);
                        if (list == null)
                        {
                            list = Activator.CreateInstance(listType);
                        }
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (parentData != null&&parentData.GetType() == relationEntityType)
                            {
                                relationObj = parentData as BaseEntity;
                            }
                            else
                            {
                                relationObj = Activator.CreateInstance(relationEntityType) as BaseEntity;
                                relationObj.FromDataRow(dr);
                            }
                            MethodInfo listContainMethod = listType.GetMethod("Contains");
                            var flag = listContainMethod.Invoke(list, new object[] { relationObj });
                            if (flag == null || (bool)flag == false)
                            {
                                MethodInfo listAddMethod = listType.GetMethod("Add");
                                listAddMethod.Invoke(list, new object[] { relationObj });
                            }
                            entity.SetRelationFlag(true);
                            if (!relationObj.IsSetRelation())
                                BindRalationProperty(relationObj,entity);
                        }
                        property.SetValue(entity, list, null);
                    }
                    else
                    {
                        if (parentData != null)
                            relationObj = parentData as BaseEntity;
                        else
                        {
                            relationObj = Activator.CreateInstance(relationEntityType) as BaseEntity;
                            relationObj.FromDataRow(dt.Rows[0]);
                        }
                        property.SetValue(entity, relationObj, null);
                        entity.SetRelationFlag(true);
                        if (!relationObj.IsSetRelation())
                            BindRalationProperty(relationObj,entity);
                    }*/
                #endregion
            }
         //   return;
        }

        //创建主键sql
        private string CreatePrimaryKeySql<T>(T entity) where T : BaseEntity
        {
            PropertyInfo[] pis = entity.GetPrimaryKey();
            if (pis == null || pis.Length == 0)
            {
                throw new MissingPrimaryKeyException();
            }
            string primaryKeySql = string.Empty;
            foreach (PropertyInfo property in pis)
            {
                string columnName = entity.GetColumnName(property.Name);
                if (primaryKeySql == string.Empty)
                {
                    primaryKeySql = columnName + "=" + GetDbValue(property.GetValue(entity, null), property.PropertyType);
                }
                else
                {
                    primaryKeySql += " and " + columnName + "=" + GetDbValue(property.GetValue(entity, null), property.PropertyType);
                }
            }
            return primaryKeySql;
        }

        private string GetDbValue(object value, Type propertyType)
        {
            Type type = TypeAdapter.GetNetType(propertyType);
            if (TypeAdapter.IsNetNumberType(type))
            {
                return value.ToString();
            }
            else if (TypeAdapter.IsNetDateType(type))
            {
               // return string.Format(DbConfig.PreParameterChar, value);
                return string.Format(_dbHelper.GetDbConfig.PreParameterChar, value);
            }
            else
            {
                return string.Format("'{0}'", value);
            }
        }

        public DataTable GetTables()
        {
            return _dbHelper.GetTables();
        }

        public IEnumerable<ColumnInfo> GetTableSchema(string tableName)
        {
            DataTable dt = _dbHelper.GetTableSchema(tableName);
            if (dt != null)
            {
                return FromDataTable<ColumnInfo>(dt);
            }
            else
                return null;
        }

        #region 查询
        /// <summary>
        /// 根据主键返回一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public T Find<T>(params object[] keyValues) where T : BaseEntity, new()
        {
            T entity = new T();
            PropertyInfo[] primaryKeys = entity.GetPrimaryKey();
            for (int i = 0; i < primaryKeys.Length; i++)
            {
                primaryKeys[i].SetValue(entity, keyValues[i], null);
            }
            string whereSql = " where " + CreatePrimaryKeySql(entity);
            string sql = string.Format(_selectFormat, entity.GetTableName(), whereSql);
            DataTable dt = _dbHelper.ExecuteDataTable(sql);
            return dt==null?null:FromDataTable<T>(dt).ToArray()[0];
        }

        public int Count<T>(Expression<Func<T, bool>> func) where T : BaseEntity, new()
        {
            string sql = "select count(1) from {0} {1}";
            T entity = new T();
            SqlInfo sqlinfo = new SqlInfo();
            string whereSql = string.Empty;
            IList<KeyValue> keyvalues = new List<KeyValue>();
            if (func != null)
            {
                whereSql = ExpressionParser<T>.Parse(func).ToSql(ref keyvalues, _dbHelper.GetDbConfig);
                if (whereSql != string.Empty)
                    whereSql = " where " + whereSql.Replace("Linq", _dbHelper.GetDbConfig.PreParameterChar + "Linq");
            }
            sqlinfo.Sql = string.Format(sql, entity.GetTableName(), whereSql);
            foreach (var kv in keyvalues)
            {
              //  KeyValue kv = param.GetKeyValue();
                DbParameter parameter = _dbHelper.CreateParameter();
                DbParameterProviderFactory.CreateParameterProvider(_dbHelper.GetDbConfig.DbType).SetParameter(_dbHelper.GetDbConfig.PreParameterChar + kv.LinqKeyName, kv.Value, kv.ValueType, ref parameter);
                sqlinfo.Parameters.Add(parameter);
            }
            return Convert.ToInt32(_dbHelper.ExecuteScalar(sqlinfo));
        }

        public IEnumerable<CombinationEntity<T1,T2>> Select<T1, T2>(Expression<Func<T1, bool>> func = null)
            where T1 : BaseEntity, new()
            where T2 : BaseEntity, new()
        {
            var t1Array = Select(func,null);
            if (t1Array == null)
                return null;
            var enumerable = t1Array as T1[] ?? t1Array.ToArray();
            CombinationEntity<T1,T2>[] comEntity = new CombinationEntity<T1,T2>[enumerable.Length];
            for (int i = 0; i < comEntity.Length; i++)
            {
                T2 t2 = new T2();
                BindRalationProperty(enumerable[i], t2);
                comEntity[i] = new CombinationEntity<T1, T2>(enumerable[i], t2);
            }
            _foreignEntityPool.Clear();
            return comEntity;
        }

        public IEnumerable<T> Select<T>(string sql,params DbParameter[] parameters) where T : BaseEntity, new()
        {
            var sqlInfo = new SqlInfo {Sql = sql};
            if (parameters != null && parameters.Length > 0)
                sqlInfo.Parameters = parameters.ToList();
            var dt = _dbHelper.ExecuteDataTable(sqlInfo);
            return FromDataTable<T>(dt);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public IEnumerable<T> Select<T>(Expression<Func<T, bool>> func, string[] properties) where T : BaseEntity, new()
        {
            T entity = new T();
        
            SqlInfo sqlinfo = new SqlInfo();
            string whereSql = string.Empty;
            IList<KeyValue> keyvalues = new List<KeyValue>();
            if (func != null)
            {
                whereSql = ExpressionParser<T>.Parse(func).ToSql(ref keyvalues, _dbHelper.GetDbConfig);
                if (whereSql != string.Empty)
                    whereSql = " where " + whereSql.Replace("Linq", _dbHelper.GetDbConfig.PreParameterChar + "Linq");
                sqlinfo.Sql = string.Format(_selectFormat, entity.GetTableName(), whereSql);
            }
            else
            {
                sqlinfo.Sql = CreatePageSql(entity, 1, 500, whereSql, string.Empty);
            }
            if (properties != null)
            {
                string columnNames = entity.GetType().GetProperties().
                    Where(property => properties.Contains(property.Name)).
                    Aggregate(string.Empty, (current, property) => current + ("," + entity.GetColumnName(property.Name)));

                columnNames = columnNames.Substring(1);
                int pos = sqlinfo.Sql.IndexOf('*');
                sqlinfo.Sql = sqlinfo.Sql.Substring(0, pos) + columnNames + sqlinfo.Sql.Substring(pos + 1);
            }

            foreach (var kv in keyvalues)
            {
                DbParameter parameter = _dbHelper.CreateParameter();
                DbParameterProviderFactory.CreateParameterProvider(_dbHelper.GetDbConfig.DbType).SetParameter(_dbHelper.GetDbConfig.PreParameterChar + kv.LinqKeyName, kv.Value, kv.ValueType, ref parameter);
                sqlinfo.Parameters.Add(parameter);
            }

            DataTable dt;
            if (sqlinfo.Parameters.Count == 0)
                dt = _dbHelper.ExecuteDataTable(sqlinfo.Sql);
            else
            {
                dt = _dbHelper.ExecuteDataTable(sqlinfo);
            }
            return FromDataTable<T>(dt);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">当前页数，从1开始算</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="func">查询条件</param>
        /// <param name="orderFunc">排序条件</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        /*
        public IEnumerable<T> Select<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> func, Expression<Func<T, object>> orderFunc, out int recordCount, bool isAsc = true) where T : BaseEntity, new()
        {
            recordCount = Count<T>(func);
            if (recordCount > 0)
            {
                T entity = new T();
                SqlInfo sqlinfo = new SqlInfo();
                //   IList<KeyValueClause> keyValueClauses = new List<KeyValueClause>();
                IList<KeyValue> keyvalues = new List<KeyValue>();

                string whereSql = string.Empty;
                if (func != null)
                {
                    whereSql = ExpressionParser<T>.Parse(func).ToSql(ref keyvalues, _dbHelper.GetDbConfig);
                    if (whereSql != string.Empty)
                        whereSql = " where " + whereSql.Replace("Linq", _dbHelper.GetDbConfig.PreParameterChar + "Linq");
                }
                string orderSql = orderFunc == null ? string.Empty : entity.OrderBy<T>(orderFunc, isAsc);
                sqlinfo.Sql = CreatePageSql(entity, pageIndex, pageSize, whereSql, orderSql);
                foreach (var kv in keyvalues)
                {
                    DbParameter parameter = _dbHelper.CreateParameter();
                    DbParameterProviderFactory.CreateParameterProvider(_dbHelper.GetDbConfig.DbType).SetParameter(
                        _dbHelper.GetDbConfig.PreParameterChar + kv.LinqKeyName, kv.Value, kv.ValueType, ref parameter);
                    sqlinfo.Parameters.Add(parameter);
                }
                DataTable dt;
                if (sqlinfo.Parameters.Count == 0)
                    dt = _dbHelper.ExecuteDataTable(sqlinfo.Sql);
                else
                    dt = _dbHelper.ExecuteDataTable(sqlinfo);
                return FromDataTable<T>(dt);
            }
            return null;
        }
        */
        public IEnumerable<T> Select<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> func, out int recordCount,bool hasCount, params OrderFunction<T>[] orderFuncs) where T : BaseEntity, new()
        {
            recordCount = 0;
            if (hasCount)
                recordCount = Count<T>(func);
            if ((hasCount&&recordCount > 0)||!hasCount)
            {
                T entity = new T();
                SqlInfo sqlinfo = new SqlInfo();
                //   IList<KeyValueClause> keyValueClauses = new List<KeyValueClause>();
                IList<KeyValue> keyvalues = new List<KeyValue>();

                string whereSql = string.Empty;
                if (func != null)
                {
                    whereSql = ExpressionParser<T>.Parse(func).ToSql(ref keyvalues, _dbHelper.GetDbConfig);
                    if (whereSql != string.Empty)
                        whereSql = " where " + whereSql.Replace("Linq", _dbHelper.GetDbConfig.PreParameterChar + "Linq");
                }
               // string orderSql = orderFunc == null ? string.Empty : entity.OrderBy<T>(orderFunc, isAsc);
                string orderSql = "";
                if (orderFuncs != null)
                {
                    foreach (var orderfunc in orderFuncs)
                    {
                        orderSql += "," + entity.OrderBy<T>(orderfunc.func, orderfunc.isAsc);
                    }

                    orderSql = orderSql.Substring(1,orderSql.Length-1);
                }
                sqlinfo.Sql = CreatePageSql(entity, pageIndex, pageSize, whereSql, orderSql);
                foreach (var kv in keyvalues)
                {
                  //  KeyValue kv = param.GetKeyValue();
                    DbParameter parameter = _dbHelper.CreateParameter();
                    DbParameterProviderFactory.CreateParameterProvider(_dbHelper.GetDbConfig.DbType).SetParameter(_dbHelper.GetDbConfig.PreParameterChar + kv.LinqKeyName, kv.Value, kv.ValueType, ref parameter);
                    sqlinfo.Parameters.Add(parameter);
                }
                DataTable dt;
                if (sqlinfo.Parameters.Count == 0)
                    dt = _dbHelper.ExecuteDataTable(sqlinfo.Sql);
                else
                    dt = _dbHelper.ExecuteDataTable(sqlinfo);
                return FromDataTable<T>(dt);
            }
            return null;
        }


        /// <summary>
        /// 创建分页sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="whereSql"></param>
        /// <param name="orderSql"></param>
        /// <returns></returns>
        private string CreatePageSql<T>(T entity, int pageIndex, int pageSize, string whereSql, string orderSql) where T : BaseEntity
        {
            //若排序条件为空，则第一个主键升序排序，若无主键则获取第一个属性升序排序
            if (orderSql == string.Empty)
            {
                PropertyInfo[] primaryKeys = entity.GetPrimaryKey();
                if (primaryKeys != null && primaryKeys.Length > 0)
                {
                    orderSql = entity.GetColumnName(primaryKeys[0].Name) + " asc ";
                }
                else
                {
                    orderSql = entity.GetColumnName(entity.GetType().GetProperties(false)[0].Name) + " asc ";
                }
            }

            int startIndex = (pageIndex - 1) * pageSize;
            int endIndex = startIndex + pageSize;

            switch (_dbHelper.GetDbConfig.DbType)
            {
                case DataBaseType.mySql:
                    return string.Format(_dbHelper.GetDbConfig.PageSql, entity.GetTableName(), startIndex, pageSize, whereSql, orderSql);
                case DataBaseType.sqlServer:
                case DataBaseType.Oracle:
                    return string.Format(_dbHelper.GetDbConfig.PageSql, entity.GetTableName(), startIndex, endIndex, whereSql, orderSql);
                default:
                    return string.Empty;
            }
        }
        #endregion

        #region 插入
        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <returns>受影响行数</returns>
        public int Insert<T>(T entity) where T:BaseEntity
        {
          return  _dbHelper.ExecuteNonQuery(CreateInsertSqlInfo(entity));
        }

        /// <summary>
        /// 添加，适用于有自增字段的表，成功后返回主键值(oracle目前不支持)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Insert2<T>(T entity) where T : BaseEntity
        {
            object obj = _dbHelper.ExecuteScalar(CreateInsertSqlInfo(entity));
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 事务批量添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        public bool Insert<T>(T[] entities) where T : BaseEntity
        {
            SqlInfo[] sqlInfos = new SqlInfo[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                sqlInfos[i] = CreateInsertSqlInfo(entities[i]);
            }
           return _dbHelper.ExecuteTransaction(sqlInfos);
        }

        private SqlInfo CreateInsertSqlInfo<T>(T entity) where T : BaseEntity
        {
            string fieldSql = string.Empty;
            string valueParameterSql = string.Empty;
            SqlInfo sqlinfo = new SqlInfo();
            foreach (PropertyInfo property in entity.GetType().GetProperties(false))
            {
                // string columnName;
                if (entity.IsSequences(property))
                {
                    string columnName = entity.GetColumnName(property.Name);
                    if (fieldSql != "")
                    {
                        fieldSql += "," + columnName;
                        valueParameterSql += "," + entity.GetSequences(property) + ".NEXTVAL";
                    }
                    else
                    {
                        fieldSql = columnName;
                        valueParameterSql = entity.GetSequences(property) + ".NEXTVAL";
                    }
                }
                else
                {
                    if (entity.IsAutoIncrement(property))
                        continue;
                    string columnName = entity.GetColumnName(property.Name);
                    string parameterName = _dbHelper.GetDbConfig.PreParameterChar + columnName;
                    object parameterValue = property.GetValue(entity, null);
                    if (parameterValue != null)
                    {
                        if (fieldSql != "")
                        {
                            fieldSql += "," + columnName;
                            valueParameterSql += "," + parameterName;
                        }
                        else
                        {
                            fieldSql = columnName;
                            valueParameterSql = parameterName;
                        }
                        DbParameter parameter = _dbHelper.CreateParameter();
                        DbParameterProviderFactory.CreateParameterProvider(_dbHelper.GetDbConfig.DbType).SetParameter(
                            parameterName, parameterValue, property.PropertyType, ref parameter);
                        sqlinfo.Parameters.Add(parameter);
                    }
                }
            }

            sqlinfo.Sql = string.Format(_insertFormat, entity.GetTableName(), fieldSql, valueParameterSql);
            //如果是mysql或者sqlserver，插入成功后返回主键值，oracle没有自增字段，其他数据未测试
            if (_dbHelper.GetDbConfig.DbType == DataBaseType.mySql || _dbHelper.GetDbConfig.DbType == DataBaseType.sqlServer)
                sqlinfo.Sql = sqlinfo.Sql + ";select @@Identity;";
            return sqlinfo;
        }
        #endregion

        #region 修改
        /// <summary>
        /// 根据主键修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public int Update<T>(T entity) where T : BaseEntity
        {
            PropertyInfo[] pis = entity.GetType().GetProperties(false);
            int len = pis.Length;
            string[] properties = new string[len];
            for (int i = 0; i < len; i++)
            {
                properties[i] = pis[i].Name;
            }
           return Update(entity, properties);
        }

        /// <summary>
        /// 根据主键修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="properties">需要更新的属性</param>
        public int Update<T>(T entity, string[] properties) where T : BaseEntity
        {
           return Update(entity, null, properties);
        }

        /// <summary>
        /// 根据条件修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="func">条件</param>
        /// <param name="properties">需要更新的属性</param>
        public int Update<T>(T entity, Expression<Func<T, bool>> func,string[] properties) where T : BaseEntity
        {
            if (properties == null || properties.Length == 0)
                throw new ArgumentNullException("需要更新的属性集合不能为空");
           return _dbHelper.ExecuteNonQuery(CreateUpdateSqlInfo(entity, func,properties));
        }

        private SqlInfo CreateUpdateSqlInfo<T>(T entity, Expression<Func<T, bool>> func, string[] properties) where T : BaseEntity
        {
            string setSql = string.Empty;
            string whereSql = string.Empty;
            SqlInfo sqlInfo = new SqlInfo();
            if (func == null)
            {
                whereSql = CreatePrimaryKeySql(entity);
                if (whereSql != "")
                    whereSql = " where " + whereSql;
            }
            else
            {
                //   IList<KeyValueClause> keyValueClauses = new List<KeyValueClause>();
                IList<KeyValue> keyvalues = new List<KeyValue>();

                whereSql = ExpressionParser<T>.Parse(func).ToSql(ref keyvalues, _dbHelper.GetDbConfig);
                //whereSql = entity.Where(func);
                if (whereSql != "")
                    whereSql = " where " + whereSql.Replace("Linq", _dbHelper.GetDbConfig.PreParameterChar + "Linq");
                foreach (var kv in keyvalues)
                {
                   // KeyValue kv = param.GetKeyValue();
                    DbParameter parameter = _dbHelper.CreateParameter();
                    DbParameterProviderFactory.CreateParameterProvider(_dbHelper.GetDbConfig.DbType).SetParameter(_dbHelper.GetDbConfig.PreParameterChar + kv.LinqKeyName, kv.Value, kv.ValueType, ref parameter);
                    sqlInfo.Parameters.Add(parameter);
                }
            }
           
            foreach (PropertyInfo property in entity.GetType().GetProperties(false))
            {
                if (entity.IsPrimaryKey(property))
                    continue;
                if (!properties.Contains(property.Name))
                    continue;

                string columnName = entity.GetColumnName(property.Name);
                string parameterName = _dbHelper.GetDbConfig.PreParameterChar + columnName;
                object parameterValue = property.GetValue(entity, null);
                if (setSql == string.Empty)
                {
                    setSql = columnName + "=" + parameterName;
                }
                else
                {
                    setSql += "," + columnName + "=" + parameterName;
                }
                DbParameter parameter = _dbHelper.CreateParameter();
                DbParameterProviderFactory.CreateParameterProvider(_dbHelper.GetDbConfig.DbType).SetParameter(parameterName, parameterValue, property.PropertyType, ref parameter);
                sqlInfo.Parameters.Add(parameter);
            }
            sqlInfo.Sql = string.Format(_updateFormat, entity.GetTableName(), setSql, whereSql);
            return sqlInfo;
        }
        #endregion

        #region 删除
        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public int Delete<T>(T entity) where T : BaseEntity
        {
           return _dbHelper.ExecuteNonQuery(CreateDeleteSqlInfo(entity, null));
        }

        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        public int Delete<T>(Expression<Func<T, bool>> func) where T : BaseEntity, new()
        {
           return _dbHelper.ExecuteNonQuery(CreateDeleteSqlInfo(new T(), func));
        }

        private SqlInfo CreateDeleteSqlInfo<T>(T entity, Expression<Func<T, bool>> func) where T:BaseEntity
        {
            string whereSql = string.Empty;
            SqlInfo sqlInfo = new SqlInfo();
            if (func == null)
            {
                whereSql = CreatePrimaryKeySql(entity);
                if (whereSql != "")
                    whereSql = " where " + whereSql;
            }
            else
            {

                IList<KeyValue> keyvalues = new List<KeyValue>();
                // whereSql = entity.Where(func);
                whereSql = ExpressionParser<T>.Parse(func).ToSql(ref keyvalues, _dbHelper.GetDbConfig);
                if (whereSql != "")
                    whereSql = " where " + whereSql.Replace("Linq", _dbHelper.GetDbConfig.PreParameterChar + "Linq");
                foreach (var kv in keyvalues)
                {
                  //  KeyValue kv = param.GetKeyValue();
                    DbParameter parameter = _dbHelper.CreateParameter();
                    DbParameterProviderFactory.CreateParameterProvider(_dbHelper.GetDbConfig.DbType).SetParameter(_dbHelper.GetDbConfig.PreParameterChar + kv.LinqKeyName, kv.Value, kv.ValueType, ref parameter);
                    sqlInfo.Parameters.Add(parameter);
                }
            }
            sqlInfo.Sql = string.Format(_deleteFormat, entity.GetTableName(), whereSql);
            return sqlInfo;
        }
        #endregion
    }
}
