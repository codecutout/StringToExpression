using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StringToExpression.Parser;
using StringToExpression.Tokenizer;
using System.Collections;
using StringToExpression.Util;
using StringToExpression.Exceptions;

namespace StringToExpression.TokenDefinitions
{
    public class BinaryOperatorDefinition : OperatorDefinition
    {
        private static RelativePosition[] LeftRight = new[] { RelativePosition.Left, RelativePosition.Right };

        public BinaryOperatorDefinition(string name,
           string regex,
           int orderOfPrecedence,
           Func<Expression, Expression, Expression> expressionBuilder)
           : base(
            name: name,
            regex: regex,
            orderOfPrecedence: orderOfPrecedence,
            paramaterPositions: LeftRight,
            expressionBuilder: param =>
            {
                var left = param[0];
                var right = param[1];
                ExpressionConversions.TryImplicitlyConvert(ref left, ref right);
                return expressionBuilder(left, right);
            })
        {
        }
    }
       
}
