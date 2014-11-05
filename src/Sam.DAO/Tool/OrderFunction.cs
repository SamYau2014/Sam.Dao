using System;
using System.Linq.Expressions;
using Sam.DAO.DAOException;

namespace Sam.DAO.Tool
{
   public class OrderFunction<T> where T:Entity.BaseEntity
    {
       private Expression<Func<T, object>> _func;
       private bool _isAsc = true;
       public Expression<Func<T, object>> func { get{return _func;}
           set {
               if (!IsMemberExpression(value.Body))
                   throw new LinqException("排序函数只能使用属性成员");
               _func = value;
           }
       }

       public bool isAsc { get{return _isAsc;} set{_isAsc=value;} }

       private static bool IsMemberExpression(Expression value)
       {
           if (value is MemberExpression)
               return true;
           if (value is UnaryExpression)
           {
               var ue = value as UnaryExpression;
               return IsMemberExpression(ue.Operand);
           }
           return false;
       }
    }
}
