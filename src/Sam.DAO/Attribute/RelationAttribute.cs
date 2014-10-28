using System;

namespace Sam.DAO.Attribute
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class RelationAttribute:System.Attribute
    {
        /// <summary>
        /// 主表字段
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 关联表字段
        /// </summary>
        public string RelationKey { get; set; }

    }
}
