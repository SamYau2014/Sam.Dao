﻿#pragma warning disable 1591
using System;
using System.Runtime.Serialization;

namespace Sam.DAO.DAOException
{
    public class DbException : DaoException
    {
        public DbException() { }
        public DbException(string errorMessage) : base(errorMessage) { }
        public DbException(string msgFormat, params object[] os) : base(String.Format(msgFormat, os)) { }
        protected DbException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public DbException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
#pragma warning restore 1591