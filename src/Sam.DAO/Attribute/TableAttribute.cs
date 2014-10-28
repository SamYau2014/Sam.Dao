using System;

namespace Sam.DAO.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute:System.Attribute
    {
        public string Name { get; set; }
    }
}
