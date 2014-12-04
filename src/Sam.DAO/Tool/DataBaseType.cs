#pragma warning disable 1591
namespace Sam.DAO.Tool
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DataBaseType
    {
      unknown=0,  sqlServer=1, mySql=2, SystemOracle=4,Oracle=8, Oledb=16, Odbc=32,SQLite=64
    }
}
#pragma warning restore 1591