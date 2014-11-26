using Sam.DAO.ExFunc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Sam.DAO.Attribute;
using Sam.DAO.Tool;
using Sam.DAO.DAOException;


namespace Sam.DAO.Entity
{
    /* 实体基类
     * 由于实体中的属性要与数据库中的字段一一对应，所以基类及子类中不能再有除数据库字段外的
     * 其他属性，相关功能应使用字段或者方法替代
     */
    public abstract class BaseEntity : IEntity //: ContextBoundObject
    {
        private  PropertyInfo[] _primaryKey;                      //主键集合
        private bool _isSetRelationFlag;                             //是否设置了关联属性
     
        public bool IsSetRelation()
        {
            return _isSetRelationFlag;
        }

        public void SetRelationFlag(bool isSetRelationFlag)
        {
            _isSetRelationFlag = isSetRelationFlag;
        }

        /// <summary>
        /// 判断是否主键
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public bool IsPrimaryKey(PropertyInfo property)
        {
            if (_primaryKey == null)
                _primaryKey = this.GetPrimaryKey();
            return _primaryKey.Contains(property);
        }

        /// <summary>
        /// 判断是否自增
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public bool IsAutoIncrement(PropertyInfo property)
        {
            object[] attributes = property.GetCustomAttributes(typeof (PropertyAttribute), false);
            if (attributes.Length == 0)
                return false;
            PropertyAttribute propertyAttribute = (PropertyAttribute) attributes[0];
            return propertyAttribute.AutoIncrement;
        }

        public bool IsSequences(PropertyInfo property)
        {
            object[] attributes = property.GetCustomAttributes(typeof(PropertyAttribute), false);
            if (attributes.Length == 0)
                return false;
            PropertyAttribute propertyAttribute = (PropertyAttribute)attributes[0];
            return !string.IsNullOrEmpty(propertyAttribute.Sequences);
        }

        public IDictionary<PropertyInfo, RelationAttribute> GetRelationProperties()
        {
            IDictionary<PropertyInfo, RelationAttribute> dict = new Dictionary<PropertyInfo, RelationAttribute>();
            foreach (PropertyInfo property in this.GetType().GetProperties(false))
            {
                object[] attributes = property.GetCustomAttributes(typeof(RelationAttribute), false);
                if (attributes.Length > 0)
                {
                    RelationAttribute ra = attributes[0] as RelationAttribute;
                    if (ra.Key != "" && ra.RelationKey != "")
                        dict.Add(property, ra);
                    else
                        throw new DaoException(property.Name + ":未正确设置关联属性");
                }
            }
            return dict;
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <returns></returns>
        public string GetTableName()
        {
            TableAttribute tableAttribute = (TableAttribute)this.GetType().GetCustomAttributes(typeof(TableAttribute), false)[0];
            if (tableAttribute != null)
                return tableAttribute.Name;
            return this.GetType().Name;
        }

        /// <summary>
        /// 获取列名
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string GetColumnName(string propertyName)
        {
            PropertyInfo property = this.GetType().GetProperty(propertyName);
            if (property == null)
                return propertyName;
            object[] attributes = property.GetCustomAttributes(typeof(PropertyAttribute), false);
            if (attributes.Length == 0)
                return propertyName;
            PropertyAttribute propertyAttribute = (PropertyAttribute)attributes[0];
            return propertyAttribute == null ? propertyName : propertyAttribute.Name;
        }

        public string GetSequences(PropertyInfo property)
        {
            object[] attributes = property.GetCustomAttributes(typeof(PropertyAttribute), false);
            if (attributes.Length == 0)
                return string.Empty;
            PropertyAttribute propertyAttribute = (PropertyAttribute)attributes[0];
            return propertyAttribute.Sequences;
        }

        /// <summary>
        /// 获取主键属性
        /// </summary>
        /// <returns></returns>
        public PropertyInfo[] GetPrimaryKey()
        {
            if (_primaryKey != null)
                return _primaryKey;
            ArrayList list = new ArrayList();
            foreach (PropertyInfo property in this.GetType().GetProperties(false))
            {
                object[] attributes = property.GetCustomAttributes(typeof(PropertyAttribute), false);

                if (attributes.Length == 0)
                    continue;
                PropertyAttribute propertyAttribute = (PropertyAttribute)attributes[0];
                if (propertyAttribute.IsPrimaryKey)
                {
                    list.Add(property);
                }
            }
            return (PropertyInfo[])list.ToArray(typeof(PropertyInfo));
        }

        public object GetValueByColumnName(string columnName)
        {
            return (from property in this.GetType().GetProperties() where GetColumnName(property.Name) == columnName select property.GetValue(this, null)).FirstOrDefault();
        }

        /// <summary>
        /// 把datarow转换成实体
        /// </summary>
        /// <param name="dr"></param>
        public void FromDataRow(DataRow dr)
        {
            PropertyInfo[] properties = this.GetType().GetProperties(false);

            foreach (PropertyInfo property in properties)
            {
                String columnname = GetColumnName(property.Name);
                DataColumn col = dr.Table.Columns[columnname];
                if (col != null)
                    property.SetValue(this, GetValue(property.PropertyType, dr[col]), null);
            }
        }

        public void FromDataReader(IDataReader reader)
        {
            PropertyInfo[] properties = this.GetType().GetProperties(false);

            foreach (PropertyInfo property in properties)
            {
                String columnname = GetColumnName(property.Name);
                if(reader[columnname]!=null&&reader[columnname]!=DBNull.Value)
                {
                    property.SetValue(this, GetValue(property.PropertyType, reader[columnname]), null);
                }
              
            }
        }

        /// <summary>
        /// 把从数据库中得到的数据转换成实体属性相应的类型
        /// </summary>
        /// <param name="propertyType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object GetValue(Type propertyType, object value)
        {
            Type netType = TypeAdapter.GetNetType(propertyType);
            if (value.ToString() == string.Empty)
                return null;
            return Convert.ChangeType(value, netType);

        }
    }
}

