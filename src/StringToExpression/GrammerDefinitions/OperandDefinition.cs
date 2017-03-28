using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StringToExpression.Parser;
using StringToExpression.Tokenizer;
using StringToExpression.Exceptions;

namespace StringToExpression.GrammerDefinitions
{
    /// <summary>
    /// Represents a piece of grammer that defines an operand.
    /// </summary>
    /// <seealso cref="StringToExpression.GrammerDefinitions.GrammerDefinition" />
    public class OperandDefinition : GrammerDefinition
    {
        /// <summary>
        /// A function to generate the operator's Expression.
        /// </summary>
        public readonly Func<string, ParameterExpression[], Expression> ExpressionBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperandDefinition"/> class.
        /// </summary>
        /// <param name="name">The name of the definition.</param>
        /// <param name="regex">The regex to match tokens.</param>
        /// <param name="expressionBuilder">The function to generate the operator's Expression given the token's value</param>
        /// <exception cref="System.ArgumentNullException">expressionBuilder</exception>
        public OperandDefinition(string name, string regex, Func<string, Expression> expressionBuilder)
            : this(name, regex, (v,a)=>expressionBuilder(v))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperandDefinition"/> class.
        /// </summary>
        /// <param name="name">The name of the definition.</param>
        /// <param name="regex">The regex to match tokens.</param>
        /// <param name="expressionBuilder">The function to generate the operator's Expression given the token's value and parsing state parameters.</param>
        /// <exception cref="System.ArgumentNullException">expressionBuilder</exception>
        public OperandDefinition(string name, string regex, Func<string, ParameterExpression[], Expression> expressionBuilder)
           : base(name, regex)
        {
            if (expressionBuilder == null)
                throw new ArgumentNullException(nameof(expressionBuilder));
            ExpressionBuilder = expressionBuilder;
        }

        /// <summary>
        /// Applies the token to the parsing state. Adds the result of the expression builder to the state.
        /// </summary>
        /// <param name="token">The token to apply.</param>
        /// <param name="state">The state to apply the token to.</param>
        /// <exception cref="StringToExpression.Exceptions.OperationInvalidException">When an error is encounted while running the expressionBuilder</exception>
        public override void Apply(Token token, ParseState state)
        {
            Expression expression;
            try
            {
                expression = ExpressionBuilder(token.Value, state.Parameters.ToArray());
            }
            catch (Exception ex)
            {
                throw new OperationInvalidException(token.SourceMap, ex);
            }
            
            state.Operands.Push(new Operand(expression, token.SourceMap));
        }
    }
}
