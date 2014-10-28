
namespace Sam.DAO.Tool
{
    public static class DbParameterProviderFactory
    {
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
