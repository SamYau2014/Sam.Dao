#pragma warning disable 1591
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
            obj.FromDataRow(row);
            return obj;
        }

        public static IEnumerable<T> DataTableToList<T>(DataTable dt) where T : Entity.BaseEntity
        {
            return from DataRow row in dt.Rows select DataRowToObject<T>(row);
        }
    }
}
#pragma warning restore 1591