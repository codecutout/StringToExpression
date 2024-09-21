using System;
using System.Collections.Generic;
using System.Text;

namespace StringToExpression.Languages
{
    using global::StringToExpression.GrammerDefinitions;
    using global::StringToExpression.Parser;
    using global::StringToExpression.Util;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection.Emit;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using global::StringToExpression.Exceptions;

    namespace StringToExpression.GrammerDefinitions
    {
        public class CollectionMethodDefinition : BracketOpenDefinition
        {
            public readonly Regex UnderlayingRegex; 
            public readonly string CollectionMethodName; 
            public readonly Func<string, string, ParameterExpression[], Expression> ExpressionBuilder;

            public CollectionMethodDefinition(
                string name, 
                string collectionMethodName
            ): base(name, $@"/{collectionMethodName}\(\s*([A-Za-z_][A-Za-z0-9_]*)\s*:")
            {
                UnderlayingRegex = new Regex(Regex);
                CollectionMethodName = collectionMethodName;
            }


            public override void Apply(Tokenizer.Token token, ParseState state)
            {
                var variableName = UnderlayingRegex.Match(token.Value).Groups[1].Value;

                var iEnumerableType = state.Operands.Peek().Expression.Type.GetInterfaces()
                    .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));

                if (iEnumerableType == null)
                {
                    throw new PropertyNotACollectionException(token.SourceMap, state.Operands.Peek().SourceMap, CollectionMethodName);
                }

                // Add the variable as a parameter so it can be used by other functions
                var variableType = iEnumerableType.GetGenericArguments()[0];
                state.Parameters.Add(ParameterExpression.Parameter(variableType, variableName));

                base.Apply(token, state);
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
            /// <exception cref="PropertyNotACollectionException">When an error occured while executing the expressionBuilder</exception>
            public override void ApplyBracketOperands(Operator bracketOpen, Stack<Operand> bracketOperands, Operator bracketClose, ParseState state)
            {
                var sourceMap = StringSegment.Encompass(
                    bracketOpen.SourceMap,
                    StringSegment.Encompass(bracketOperands.Select(x => x.SourceMap)),
                    bracketClose.SourceMap);

                if(bracketOperands.Count != 1)
                {
                    throw new FunctionArgumentCountException(sourceMap, 1, bracketOperands.Count);
                }
                
                //Our parameter is out of scope now so remove it from the list
                var parameter = state.Parameters.Last();
                state.Parameters.Remove(parameter);

                var instance = state.Operands.Pop().Expression;
                var lambbda = Expression.Lambda(
                    bracketOperands.Pop().Expression,
                    parameter);

                try
                {
                    var methodCall = Expression.Call(
                        typeof(Enumerable),
                        CollectionMethodName,
                        new[] { instance.Type.GetGenericArguments()[0] },
                        new[] { instance, lambbda });
                    state.Operands.Push(new Operand(methodCall, sourceMap));
                }
                catch (Exception ex)
                {
                    throw new OperationInvalidException(sourceMap, ex);
                }
            }
        }
    }
}
