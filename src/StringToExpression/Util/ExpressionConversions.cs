using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StringToExpression.Util
{
    /// <summary>
    /// Provides utilities for converting Expressions.
    /// </summary>
    public static class ExpressionConversions
    {
        private static Dictionary<Type, Type[]> ImplicitConverstions = new Dictionary<Type, Type[]>
        {
            { typeof(sbyte),   new[] { typeof(sbyte), typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) }},
            { typeof(byte),    new[] { typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) }},
            { typeof(short),   new[] { typeof(short),typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) }},
            { typeof(ushort),  new[] { typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) }},
            { typeof(int),     new[] { typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) }},
            { typeof(uint),    new[] { typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) }},
            { typeof(long),    new[] { typeof(long), typeof(float), typeof(double), typeof(decimal) }},
            { typeof(char),    new[] { typeof(char), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) }},
            { typeof(float),   new[] { typeof(float), typeof(double), /*not safe, but convineant -->*/ typeof(decimal) }},
            { typeof(ulong),   new[] { typeof(ulong), typeof(float), typeof(double), typeof(decimal) }},
            { typeof(double),   new[] { typeof(double), /*not safe, but convineant -->*/ typeof(decimal) }},
            { typeof(decimal),   new[] { typeof(decimal) }},
        };

        /// <summary>
        /// Tries a type that both passed types can safetly convert to.
        /// </summary>
        /// <param name="type1"></param>
        /// <param name="type2"></param>
        /// <param name="commonType">Type that both passed types can convert to.</param>
        /// <returns></returns>
        private static bool TryGetCommonType(Type type1, Type type2, out Type commonType)
        {
            commonType = null;
            Type[] possibleType1;
            if (!ImplicitConverstions.TryGetValue(type1, out possibleType1))
                return false;
            Type[] possibleType2;
            if (!ImplicitConverstions.TryGetValue(type2, out possibleType2))
                return false;

            commonType = possibleType1.Intersect(possibleType2).FirstOrDefault();
            if (commonType == null)
                return false;

            return true;
        }

        /// <summary>
        /// Attempt to convert the expression into a boolean.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static bool TryBoolean(ref Expression exp)
        {
            if (exp.Type == typeof(bool))
                return true;

            var left = exp;
            var right = (Expression)Expression.Constant(true);
            if (!ExpressionConversions.TryImplicitlyConvert(ref left, ref right))
                return false;
            exp = Expression.Equal(left, right);
            return true;
        }

        /// <summary>
        /// Attempts to perform the implicit conversion so both expressions are the same type.
        /// </summary>
        /// <param name="exp1">first expression.</param>
        /// <param name="exp2">second expression.</param>
        /// <returns>
        ///     <c>true</c> if a common type exists; otherwise, <c>false</c>.
        /// </returns>
        public static bool TryImplicitlyConvert(ref Expression exp1, ref Expression exp2)
        {
            var type1 = exp1.Type;
            var type2 = exp2.Type;

            //same type, nothing to do here
            if (type1 == type2)
                return true;

            var nullableType1 = Nullable.GetUnderlyingType(exp1.Type);
            var nullableType2 = Nullable.GetUnderlyingType(exp2.Type);
            var nullable = nullableType1 != null || nullableType2 != null;

            type1 = nullableType1 ?? type1;
            type2 = nullableType2 ?? type2;

            Type commonType;
            if (nullable && type1 == type2)
            {
                //if the underlying type is the same, the common type is
                //just the nullable version
                commonType = typeof(Nullable<>).MakeGenericType(type1);
            }
            else if (IsNullConstant(exp1))
            {
                // strings are already nullables
                if (type2 == typeof(string))
                {
                    commonType = type2;
                }
                else
                {
                    //one of our expressions is null, so convert the other side to a nullable
                    commonType = typeof(Nullable<>).MakeGenericType(type2);
                }
            }
            else if (IsNullConstant(exp2))
            {
                // strings are already nullables
                if (type1 == typeof(string))
                {
                    commonType = type1;
                }
                else
                {
                    //the other side of the expression is null so convert the first side to a nullable
                    commonType = typeof(Nullable<>).MakeGenericType(type1);
                }
                
            }
            else if (TryGetCommonType(type1, type2, out commonType))
            {
                //we have a common type, if we had a nullable type to begin
                //with convert our common type to a nullable as well
                if (nullable)
                    commonType = typeof(Nullable<>).MakeGenericType(commonType);
            }
            else
            {
                return false;
            }

                      
            if (exp1.Type != commonType)
                exp1 = Expression.Convert(exp1, commonType);
            if (exp2.Type != commonType)
                exp2 = Expression.Convert(exp2, commonType);
            return true;

        }

        /// <summary>
        /// Converts an expression to the given type only if it is not that type already.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="type"></param>
        /// <returns>Expression of the given type</returns>
        public static Expression Convert(Expression exp, Type type)
        {
            return exp.Type == type
                ? exp
                : Expression.Convert(exp, type);
        }

        /// <summary>
        /// Determines if expression is a null constant.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns>
        ///   <c>true</c> if the expression is a null constant; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullConstant(Expression exp)
        {
            var constantExpression = exp as ConstantExpression;
            if (constantExpression == null)
                return false;
            return constantExpression.Value == null;
        }

      
    }
}
