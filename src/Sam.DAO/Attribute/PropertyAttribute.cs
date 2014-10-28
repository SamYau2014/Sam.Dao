using System;

namespace Sam.DAO.Attribute
{
    [AttributeUsage(AttributeTargets.Property, Inherited=false)]
    public class PropertyAttribute:System.Attribute
    {
        public bool IsPrimaryKey { get; set; }
        public string Name { get; set; }
        public bool AutoIncrement { get; set; }
    }
}
