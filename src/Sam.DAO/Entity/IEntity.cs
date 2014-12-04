#pragma warning disable 1591
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Sam.DAO.Attribute;

namespace Sam.DAO.Entity
{
    public interface IEntity
    {
        string GetColumnName(string propertyName);

        void FromDataRow(System.Data.DataRow dr);

        void FromDataReader(IDataReader reader);

        string GetTableName();

        IDictionary<PropertyInfo, RelationAttribute> GetRelationProperties();

        object GetValueByColumnName(string columnName);

        bool IsSetRelation();

        void SetRelationFlag(bool isSetRelationFlag);
    }
}
#pragma warning restore 1591