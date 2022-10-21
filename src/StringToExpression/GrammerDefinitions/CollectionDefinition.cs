using StringToExpression.Exceptions;
using StringToExpression.Parser;
using StringToExpression.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace StringToExpression.GrammerDefinitions
{
    public class CollectionDefinition : BracketOpenDefinition
    {
        public readonly Func<string, string, ParameterExpression[], Expression> ExpressionBuilder;

        public CollectionDefinition(string name, string regex, Func<string, string, Expression> expressionBuilder)
            : this(name, regex, (v, sm, a) => expressionBuilder(v, sm))
        {

        }

        public CollectionDefinition(string name, string regex, Func<string, string, ParameterExpression[], Expression> expressionBuilder)
            : base(name, regex)
        {
            ExpressionBuilder = expressionBuilder ?? throw new ArgumentNullException(nameof(expressionBuilder));
        }


        /// <summary>
        /// Applies the bracket operands. Executes the expressionBuilder with all the operands in the brackets.
        /// </summary>
        /// <param name="bracketOpen">The operator that opened the bracket.</param>
        /// <param name="bracketOperands">The list of operands within the brackets.</param>
        /// <param name="bracketClose">The operator that closed the bracket.</param>
        /// <param name="state">The current parse state.</param>
        /// <exception cref="FunctionArgumentCountException">When the number of opreands does not match the number of arguments</exception>
        /// <exception cref="FunctionArgumentTypeException">When argument Type does not match the type of the expression</exception>
        /// <exception cref="OperationInvalidException">When an error occured while executing the expressionBuilder</exception>
        public override void ApplyBracketOperands(Operator bracketOpen, Stack<Operand> bracketOperands, Operator bracketClose, ParseState state)
        {
            string methodName = "";
            var operandSource = StringSegment.Encompass(bracketOperands.Select(x => x.SourceMap));

            switch (bracketOpen.SourceMap.Value)
            {
                case "all(":
                    methodName = nameof(Enumerable.All);
                    break;
                case "any(":
                    methodName = nameof(Enumerable.Any);
                    break;                
                default:
                    throw new NotImplementedException($"{bracketOpen.SourceMap.Value} is an unknown collection definition");
            }

            var operandType = state.Operands.First().Expression.Type.GetGenericArguments()[0];

            var methodCall = Expression.Call(
                typeof(Enumerable), methodName, new[] { operandType },
                state.Operands.Pop().Expression, Expression.Lambda(bracketOperands.Pop().Expression, state.Parameters.LastOrDefault().Expression));

            var functionSourceMap = StringSegment.Encompass(bracketOpen.SourceMap, operandSource);

            state.Operands.Push(new Operand(methodCall, functionSourceMap));
        }
    }
}
