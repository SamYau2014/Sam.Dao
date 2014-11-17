using System;
using System.Runtime.Serialization;

namespace Sam.DAO.DAOException
{
    public class DaoException : System.Exception
    {
        public DaoException() { }
        public DaoException(string errorMessage) : base(errorMessage) { }
        public DaoException(string msgFormat, params object[] os) : base(String.Format(msgFormat, os)) { }
        protected DaoException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public DaoException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
