using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StringToExpression.Parser
{
    public class Accessor
    {
        public readonly ParameterExpression Expression;
        public readonly string Pattern;

        public Accessor(ParameterExpression expression, string pattern = null)
        {
            Expression = expression;
            Pattern = pattern;
        }

        public static implicit operator Accessor(ParameterExpression expression)
        {
            return new Accessor(expression);
        }

        public static implicit operator ParameterExpression(Accessor accessor)
        {
            return accessor.Expression;
        }
    }
}
