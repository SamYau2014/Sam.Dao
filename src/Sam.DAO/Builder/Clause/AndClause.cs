﻿using Sam.DAO.Builder.Data;

namespace Sam.DAO.Builder.Clause
{
    internal class AndClause : ConditionClause
    {
        public AndClause(params Condition[] conditions):base("and",conditions)
        {

        }
    }
}
