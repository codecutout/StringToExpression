using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StringParser.Parser;
using StringParser.Tokenizer;
using StringParser.Exceptions;
using StringParser.Util;

namespace StringParser.TokenDefinitions
{
    public class FunctionCallDefinition : UnaryOperatorDefinition
    {
        public FunctionCallDefinition(
            string name,
            string regex,
            int orderOfPrecedence,
            IEnumerable<Type> argumentTypes,
            Func<Expression[], Expression> expressionBuilder)
            : base(name, regex, orderOfPrecedence, RelativePosition.Right, (expression) =>
            {
                var argsArray = expression as MultiOperandExpression;
                if (argsArray == null)
                    throw new Exception($"Function call expects single {typeof(MultiOperandExpression).Name} parameter");

                var operands = argsArray.Operands;

                //if we have been given specific argument types validate them
                if (argumentTypes != null)
                {
                    argumentTypes = argumentTypes as Type[] ?? argumentTypes.ToArray();
                    var expectedArgumentCount = argumentTypes.Count();
                    if (expectedArgumentCount != operands.Count)
                        throw new FunctionArgumentCountException(
                            StringSegment.Encompass(operands.Select(x => x.SourceMap)),
                            expectedArgumentCount,
                            operands.Count);

                    var invalidType = operands
                        .Zip(argumentTypes, (o, t) => new { Operand = o, Type = t })
                        .Where(x => !x.Type.IsAssignableFrom(x.Operand.Expression.Type))
                        .FirstOrDefault();

                    if (invalidType != null)
                    {
                        throw new FunctionArgumentTypeException(
                            invalidType.Operand.SourceMap, 
                            invalidType.Operand.Expression.Type, 
                            invalidType.Type);
                    }
                }

                return expressionBuilder(operands.Select(x => x.Expression).ToArray());
            })
        {

        }

        public FunctionCallDefinition(string name, string regex, int orderOfPrecedence, Func<Expression[], Expression> expressionBuilder)
           : this(name, regex, orderOfPrecedence, null, expressionBuilder)
        {
        }
    }
}
