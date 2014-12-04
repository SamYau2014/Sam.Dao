#pragma warning disable 1591
using System;
using System.Data.Common;

namespace Sam.DAO.Tool
{
    public class DbParameterProvider:IDbParameterProvider
    {
        public void SetParameter(string paraName, object value, Type valueType,ref DbParameter para)
        {
            para.ParameterName = paraName;
            para.Value = value??DBNull.Value;
            para.DbType = TypeAdapter.GetDbType(TypeAdapter.GetNetType(valueType));
        }
    }
}
#pragma warning restore 1591