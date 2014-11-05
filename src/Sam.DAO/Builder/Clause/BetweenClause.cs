using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Sam.DAO.Builder.Data;
using Sam.DAO.Tool;

namespace Sam.DAO.Builder.Clause
{
    internal class BetweenClause : Condition
    {
        private readonly string _column;
        private readonly bool _flag;

        private readonly KeyValue _left, _right;

        public BetweenClause(string column, object left, object right, bool flag)
        {
            _column = column;
            _flag = flag;
            _left = new KeyValue(column + "left", left, 0);
            _right = new KeyValue(column + "right", right, 0);
        }

        public override string ToSql(ref ICollection<DbParameter> parameters, DB dbhelp)
        {
            DbParameter lp = dbhelp.CreateParameter();
            DbParameter rp = dbhelp.CreateParameter();
            DbParameterProviderFactory.CreateParameterProvider(dbhelp.GetDbConfig.DbType).SetParameter(
                dbhelp.GetDbConfig.PreParameterChar + _left.LinqKeyName, _left.NullableValue, _left.ValueType, ref lp);
            DbParameterProviderFactory.CreateParameterProvider(dbhelp.GetDbConfig.DbType).SetParameter(
                dbhelp.GetDbConfig.PreParameterChar + _right.LinqKeyName, _right.NullableValue, _right.ValueType, ref rp);
            parameters.Add(lp);
            parameters.Add(rp);
            StringBuilder sb = new StringBuilder();
            sb.Append(" ");
            sb.Append(_column);
            if (!_flag)
                sb.Append(" Not ");
            sb.Append(" Between ");
            sb.Append(dbhelp.GetDbConfig.PreParameterChar + _left.LinqKeyName);
            sb.Append(" and ");
            sb.Append(dbhelp.GetDbConfig.PreParameterChar + _right.LinqKeyName);
            return sb.ToString();
        }
    }
}
