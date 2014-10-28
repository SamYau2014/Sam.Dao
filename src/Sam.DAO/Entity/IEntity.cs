using System.Collections.Generic;
using System.Reflection;
using Sam.DAO.Attribute;

namespace Sam.DAO.Entity
{
    public interface IEntity
    {
        string GetColumnName(string propertyName);

        void FromDataRow(System.Data.DataRow dr);

        string GetTableName();

        IDictionary<PropertyInfo, RelationAttribute> GetRelationProperties();

        object GetValueByColumnName(string columnName);

        bool IsSetRelation();

        void SetRelationFlag(bool isSetRelationFlag);
    }
}
