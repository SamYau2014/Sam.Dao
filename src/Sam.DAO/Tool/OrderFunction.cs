using System;
using System.Linq.Expressions;
using Sam.DAO.DAOException;

namespace Sam.DAO.Tool
{
   /// <summary>
   /// 
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public class OrderFunction<T> where T:Entity.BaseEntity
    {
       private Expression<Func<T, object>> _func;
       private bool _isAsc = true;
       /// <summary>
       /// 
       /// </summary>
       /// <exception cref="LinqException"></exception>
       public Expression<Func<T, object>> Func { get{return _func;}
           set {
               if (!IsMemberExpression(value.Body))
                   throw new LinqException("排序函数只能使用属性成员");
               _func = value;
           }
       }

       /// <summary>
       /// 是否升序
       /// </summary>
       public bool IsAsc { get{return _isAsc;} set{_isAsc=value;} }

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
