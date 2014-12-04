using Sam.DAO.DaoInterface;

namespace Sam.DAO
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDaoContext:IDaoContextRead,IDaoContextWrite,ISqlOperation
    {
        /// <summary>
        /// 获取DbHelper
        /// </summary>
        /// <returns>Db</returns>
        DB GetDbHelper();
    }
}
