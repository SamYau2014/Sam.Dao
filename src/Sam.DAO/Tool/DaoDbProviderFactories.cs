using System.Data;
using System.Data.Common;

namespace Sam.DAO.Tool
{
    public static class DaoDbProviderFactories 
    {
        public static DbProviderFactory GetFactory(DataRow providerRow)
        {
            switch (providerRow["InvariantName"].ToString())
            {
                case "MySql.Data.MySqlClient":
                    return new MySql.Data.MySqlClient.MySqlClientFactory();
                case "System.Data.OracleClient":
                    return new Oracle.DataAccess.Client.OracleClientFactory();
                case "System.Data.SQLite":
                    return new System.Data.SQLite.SQLiteFactory();
                default:
                    return DbProviderFactories.GetFactory(providerRow);
            }     
        }

        public static DbProviderFactory GetFactory(string providerInvariantName)
        {
            switch (providerInvariantName)
            {
                case "MySql.Data.MySqlClient":
                    return new MySql.Data.MySqlClient.MySqlClientFactory();
                case "System.Data.OracleClient":
                case "Oracle.DataAccess.Client":
                    return new Oracle.DataAccess.Client.OracleClientFactory();
                case "System.Data.SQLite":
                    return new System.Data.SQLite.SQLiteFactory();
                default:
                    return DbProviderFactories.GetFactory(providerInvariantName);
            }
        }

        public static DataTable GetFactoryClasses()
        {
            DataTable table = DbProviderFactories.GetFactoryClasses();
            table.Rows.Add("MySqlClient Data Provider", ".Net Framework Data Provider For MySql", "MySql.Data.MySqlClient", "MySql.Data.MySqlClient.MySqlClientFactory");
            return table;
        }
    }
}
