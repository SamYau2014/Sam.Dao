#pragma warning disable 1591
using System;
using System.Data.Common;

namespace Sam.DAO.Tool
{
    public interface IDbParameterProvider
    {
        void SetParameter(string paraName, object value, Type valueType, ref DbParameter para);
    }
}
#pragma warning restore 1591