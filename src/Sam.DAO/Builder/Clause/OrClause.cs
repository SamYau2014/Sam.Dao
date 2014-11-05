using Sam.DAO.Builder.Data;

namespace Sam.DAO.Builder.Clause
{
    internal class OrClause : ConditionClause
    {
        public OrClause(params Condition[] conditions) : base("or", conditions)
        {

        }
    }
}
