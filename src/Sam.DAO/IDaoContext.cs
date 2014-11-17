using Sam.DAO.DaoInterface;

namespace Sam.DAO
{
    public interface IDaoContext:IDaoContextRead,IDaoContextWrite,ISqlOperation
    {
        DB GetDbHelper();
    }
}
