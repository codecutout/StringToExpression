using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StringParser.Util
{
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
            { typeof(float),   new[] { typeof(float), typeof(double), /*not safe, but convineant*/ typeof(decimal) }},
            { typeof(ulong),   new[] { typeof(ulong), typeof(float), typeof(double), typeof(decimal) }},
            { typeof(double),   new[] { typeof(double), /*not safe, but convineant*/ typeof(decimal) }},
            { typeof(decimal),   new[] { typeof(decimal) }},
        };

        /// <summary>
        /// Attempts to perform the implicit conversion so both expressions are the same type
        /// </summary>
        /// <param name="exp1">first expression</param>
        /// <param name="exp2">second expression</param>
        /// <returns></returns>
        public static bool TryImplicitlyConvert(ref Expression exp1, ref Expression exp2)
        {
            //same type, nothing to do here
            if (exp1.Type == exp2.Type)
                return true;

            Type[] possibleType1;
            if (!ImplicitConverstions.TryGetValue(exp1.Type, out possibleType1))
                return false;
            Type[] possibleType2;
            if (!ImplicitConverstions.TryGetValue(exp2.Type, out possibleType2))
                return false;

            var typeToCastTo = possibleType1.Intersect(possibleType2).FirstOrDefault();
            if (typeToCastTo == null)
                return false;
           

            if (exp1.Type != typeToCastTo)
                exp1 = Expression.Convert(exp1, typeToCastTo);
            if (exp2.Type != typeToCastTo)
                exp2 = Expression.Convert(exp2, typeToCastTo);
            return true;

        }
    }
}
