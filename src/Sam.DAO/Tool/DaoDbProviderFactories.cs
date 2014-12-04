using System.Data;
using System.Data.Common;

namespace Sam.DAO.Tool
{
    /// <summary>
    /// 
    /// </summary>
    public static class DaoDbProviderFactories 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerRow"></param>
        /// <returns></returns>
        public static DbProviderFactory GetFactory(DataRow providerRow)
        {
            switch (providerRow["InvariantName"].ToString())
            {
               // case "MySql.Data.MySqlClient":
              //      return new MySql.Data.MySqlClient.MySqlClientFactory();
                case "System.Data.OracleClient":
                    return new Oracle.DataAccess.Client.OracleClientFactory();
               // case "System.Data.SQLite":
               //     return new System.Data.SQLite.SQLiteFactory();
                default:
                    return DbProviderFactories.GetFactory(providerRow);
            }     
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerInvariantName"></param>
        /// <returns></returns>
        public static DbProviderFactory GetFactory(string providerInvariantName)
        {

            switch (providerInvariantName)
            {
               // case "MySql.Data.MySqlClient":
               //     return new MySql.Data.MySqlClient.MySqlClientFactory();
                case "System.Data.OracleClient":
                case "Oracle.DataAccess.Client":
                    return new Oracle.DataAccess.Client.OracleClientFactory();
              //  case "System.Data.SQLite":
              //      return new System.Data.SQLite.SQLiteFactory();
                default:
                    return DbProviderFactories.GetFactory(providerInvariantName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DataTable GetFactoryClasses()
        {
            DataTable table = DbProviderFactories.GetFactoryClasses();
            table.Rows.Add("MySqlClient Data Provider", ".Net Framework Data Provider For MySql", "MySql.Data.MySqlClient", "MySql.Data.MySqlClient.MySqlClientFactory");
            table.Rows.Add("Oracle Data Provider", ".Net Framework Data Provider For Oracle", "Oracle.DataAccess.Client", "Oracle.DataAccess.Client.OracleClientFactory,Oracle.DataAccess");
            return table;
        }

    }
}
