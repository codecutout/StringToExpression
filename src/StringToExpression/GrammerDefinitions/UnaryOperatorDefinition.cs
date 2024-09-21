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
    /// Represents an operator that takes a single operand.
    /// </summary>
    /// <seealso cref="StringToExpression.GrammerDefinitions.OperatorDefinition" />
    public class UnaryOperatorDefinition : OperatorDefinition
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UnaryOperatorDefinition"/> class.
        /// </summary>
        /// <param name="name">The name of the definition.</param>
        /// <param name="regex">The regex to match tokens.</param>
        /// <param name="orderOfPrecedence">The relative order this operator should be applied. Lower orders are applied first.</param>
        /// <param name="operandPosition">The relative positions where the single operand can be found.</param>
        /// <param name="expressionBuilder">The function given the single operand expressions, outputs a new operand.</param>
        public UnaryOperatorDefinition(string name,
           string regex,
           int orderOfPrecedence,
           RelativePosition operandPosition,
           Func<Expression, Expression> expressionBuilder)
           : this(
                name, 
                regex, 
                orderOfPrecedence,
                operandPosition,
                (_, param) => expressionBuilder(param))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnaryOperatorDefinition"/> class.
        /// </summary>
        /// <param name="name">The name of the definition.</param>
        /// <param name="regex">The regex to match tokens.</param>
        /// <param name="orderOfPrecedence">The relative order this operator should be applied. Lower orders are applied first.</param>
        /// <param name="operandPosition">The relative positions where the single operand can be found.</param>
        /// <param name="expressionBuilder">The function given the single operand expressions, outputs a new operand.</param>
        public UnaryOperatorDefinition(string name,
           string regex,
           int orderOfPrecedence,
           RelativePosition operandPosition,
           Func<string, Expression, Expression> expressionBuilder)
           : base(
            name: name,
            regex: regex,
            orderOfPrecedence: orderOfPrecedence,
            paramaterPositions: new[] { operandPosition },
            expressionBuilder: (token, param) => expressionBuilder(token, param[0]))
        {
        }

    }
}
