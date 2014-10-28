using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.DAO.Entity
{
    public class CombinationEntity<T1, T2>
        where T1 : BaseEntity
        where T2 : BaseEntity
    {
        public CombinationEntity(T1 t1, T2 t2)
        {
            MainEntity = t1;
            ForeignEntity = t2;
        }

        public T1 MainEntity { get; private set; }

        public T2 ForeignEntity { get; private set; }
    }
}
