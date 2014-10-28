using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace Ldc.DAO.Tool
{
    public static class ExDbProviderFactories
    {
        public static DbProviderFactory GetFactory(System.Data.DataRow providerRow)
        {
            return DbProviderFactories.GetFactory(providerRow);
        }

        public static DbProviderFactory GetFactory(string providerInvariantName)
        {
            DbProviderFactory _factory = null;
            switch (providerInvariantName)
            {
                case "MySql.Data.MySqlClient":
                    _factory = new MySqlClientFactory();
                    break;
                default:
                    _factory = DbProviderFactories.GetFactory(providerInvariantName);
                    break;
            }
            return _factory;
        }

        public static System.Data.DataTable GetFactoryClasses()
        {
            return DbProviderFactories.GetFactoryClasses();
        }
    } 
}
