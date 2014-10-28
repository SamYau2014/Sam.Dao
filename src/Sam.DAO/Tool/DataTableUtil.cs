using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Sam.DAO.Tool
{
    public class DataTableUtil
    {
        public static T DataRowToObject<T>(DataRow row) where T:Entity.BaseEntity
        {
            T obj = Activator.CreateInstance<T>();
            if (obj != null)
            {
                obj.GetType().GetProperties().ToList().ForEach(m =>
                {
                    string columnName = obj.GetColumnName(m.Name);
                    if (row.Table.Columns.Contains(columnName))
                    {
                        var columnValue = row[columnName];
                        if (m.PropertyType != typeof(DateTime) || !string.IsNullOrEmpty(columnValue.ToString()))
                            m.SetValue(obj, Convert.ChangeType(row[columnName], m.PropertyType), null);
                    }
                });
            }
            return obj;
        }

        public static IEnumerable<T> DataTableToList<T>(DataTable dt) where T : Entity.BaseEntity
        {
            foreach (DataRow row in dt.Rows)
            {
                yield return DataRowToObject<T>(row);
            }
        }
    }
}
