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

namespace StringToExpression.GrammerDefinitions
{
    /// <summary>
    /// Represents an operator that has two parameters, one to the left and one to the right.
    /// </summary>
    /// <seealso cref="StringToExpression.GrammerDefinitions.OperatorDefinition" />
    public class BinaryOperatorDefinition : OperatorDefinition
    {
        private static RelativePosition[] LeftRight = new[] { RelativePosition.Left, RelativePosition.Right };

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryOperatorDefinition"/> class.
        /// </summary>
        /// <param name="name">The name of the definition.</param>
        /// <param name="regex">The regex to match tokens.</param>
        /// <param name="orderOfPrecedence">The relative order this operator should be applied. Lower orders are applied first.</param>
        /// <param name="expressionBuilder">The function given the single operand expressions, outputs a new operand.</param>
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
