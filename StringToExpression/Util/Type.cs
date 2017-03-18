using System;
using System.Linq.Expressions;
using System.Reflection;

namespace StringToExpression.Util
{
    public static class Type<T>
    {
        /// <summary>
        /// Find a MemberInfo based on a linq expression
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="memberSelector">expression selecting a member</param>
        /// <returns></returns>
        public static MemberInfo Member<TOut>(Expression<Func<T, TOut>> memberSelector)
        {
            if (memberSelector == null)
                throw new ArgumentNullException(nameof(memberSelector));

            var exp = memberSelector.Body;

            //loop through to get rid of converts
            while (exp is UnaryExpression)
                exp = ((UnaryExpression)exp).Operand;

            var memberExpression = exp as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException($"{nameof(memberSelector)} is a not a valid member");
            return memberExpression.Member;
        }

        /// <summary>
        /// Finds a MethodInfo based on a linq exprssion
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="methodSelector">expression selecting a method</param>
        /// <returns></returns>
        public static MethodInfo Method<TOut>(Expression<Func<T, TOut>> methodSelector)
        {
            if (methodSelector == null)
                throw new ArgumentNullException(nameof(methodSelector));

            var exp = methodSelector.Body;

            //loop through to get rid of converts
            while (exp is UnaryExpression)
                exp = ((UnaryExpression)exp).Operand;

            var methodExpression = exp as MethodCallExpression;
            if (methodExpression == null)
                throw new ArgumentException($"{nameof(methodSelector)} is a not a valid method");
            return methodExpression.Method;
        }
    }
}
