
namespace Sam.DAO.Tool
{
    /// <summary>
    /// DbParameterProviderFactory
    /// </summary>
    public static class DbParameterProviderFactory
    {
        /// <summary>
        /// Create a IDbParameterProvider
        /// </summary>
        /// <param name="dbType">DataBaseType</param>
        /// <returns></returns>
        public static IDbParameterProvider CreateParameterProvider(DataBaseType dbType)
        {
            switch (dbType)
            {
                case DataBaseType.Oledb:
                    return new OleDbParameterProvider();
                default:
                    return new DbParameterProvider();
            }
        }
    }
}
