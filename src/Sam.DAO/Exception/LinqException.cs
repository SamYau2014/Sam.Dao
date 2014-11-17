using System;
using System.Runtime.Serialization;

namespace Sam.DAO.DAOException
{
    public class LinqException : DaoException
    {
        public LinqException() { }
        public LinqException(string errorMessage) : base(errorMessage) { }
        public LinqException(string msgFormat, params object[] os) : base(String.Format(msgFormat, os)) { }
        protected LinqException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public LinqException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
