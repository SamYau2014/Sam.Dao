using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Sam.DAO.Attribute;
using Sam.DAO.Tool;

namespace Sam.DAO.Entity
{
    public class ColumnInfo : IEntity
    {
        public string ColumnName { get; set; }

        [Property(Name = "DataType")]
        public Type DataType { get; set; }

        [Property(Name="IsKey")]
        public bool IsPrimaryKey { get; set; }

        [Property(Name = "AllowDBNull")]
        public bool IsNullable { get; set; }

        public bool IsAutoIncrement { get; set; }

        [Property(Name="ColumnSize")]
        public int Length { get; set; }

        [Property(Name = "NumericPrecision")]
        public int Precision { get; set; }

        [Property(Name = "NumericScale")]
        public int Scale { get; set; }

        /// <summary>
        /// 把datarow转换成实体
        /// </summary>
        /// <param name="dr"></param>
        public void FromDataRow(DataRow dr)
        {
            PropertyInfo[] properties = this.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                String columnname = GetColumnName(property.Name);
                DataColumn col = dr.Table.Columns[columnname];
                if (col != null)
                    property.SetValue(this, GetValue(property.PropertyType, dr[col]), null);
            }
        }

        /// <summary>
        /// 把从数据库中得到的数据转换成实体属性相应的类型
        /// </summary>
        /// <param name="propertyType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private object GetValue(Type propertyType, object value)
        {
            Type netType = TypeAdapter.GetNetType(propertyType);
            if (value.ToString() == string.Empty)
            {
                return null;
            }
            else
            {
                if (netType == typeof(bool))
                {
                    if (value.ToString().ToUpper() == "YES" || value.ToString().ToUpper() == "Y")
                        value = 1;
                    if (value.ToString().ToUpper() == "NO" || value.ToString().ToUpper() == "N")
                        value = 0;
                }
                if (netType == typeof(System.Type))
                {
                    return value;
                }
                return Convert.ChangeType(value, netType);
            }

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
            if (attributes == null || attributes.Length == 0)
                return propertyName;
            PropertyAttribute propertyAttribute = (PropertyAttribute)attributes[0];
            return propertyAttribute == null ? propertyName : propertyAttribute.Name;
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <returns></returns>
        public string GetTableName()
        {
            return "ColumnInfo";
        }

        public IDictionary<PropertyInfo, RelationAttribute> GetRelationProperties()
        {
            return null;
        }

        public object GetValueByColumnName(string columnName)
        {
            return null;
        }

        public bool IsSetRelation()
        {
            return false;
        }

        public void SetRelationFlag(bool isSetRelationFlag)
        {
        }
    }
}
