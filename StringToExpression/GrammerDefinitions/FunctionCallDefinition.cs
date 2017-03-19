using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StringToExpression.Parser;
using StringToExpression.Tokenizer;
using StringToExpression.Exceptions;
using StringToExpression.Util;

namespace StringToExpression.GrammerDefinitions
{
    public class FunctionCallDefinition : BracketOpenDefinition
    {
        public readonly IReadOnlyList<Type> ArgumentTypes;

        public readonly Func<Expression[], Expression> ExpressionBuilder;

        public FunctionCallDefinition(
            string name,
            string regex,
            IEnumerable<Type> argumentTypes,
            Func<Expression[], Expression> expressionBuilder)
            : base(name, regex)
        {
            this.ArgumentTypes = argumentTypes?.ToList()?.AsReadOnly();
            this.ExpressionBuilder = expressionBuilder;
        }

        public FunctionCallDefinition(string name, string regex,Func<Expression[], Expression> expressionBuilder)
           : this(name, regex, null, expressionBuilder)
        {
        }

        public override void ApplyBracketOperands(Operator bracketOpen, Stack<Operand> bracketOperands, ParseState state)
        {
            var operandSource = StringSegment.Encompass(bracketOperands.Select(x => x.SourceMap));
            //if we have been given specific argument types validate them
            if (ArgumentTypes != null)
            {
                var expectedArgumentCount = ArgumentTypes.Count;
                if (expectedArgumentCount != bracketOperands.Count)
                    throw new FunctionArgumentCountException(
                        operandSource,
                        expectedArgumentCount,
                        bracketOperands.Count);

                var invalidType = bracketOperands
                    .Zip(ArgumentTypes, (o, t) => new { Operand = o, Type = t })
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

            var output = ExpressionBuilder(bracketOperands.Select(x => x.Expression).ToArray());
            if(output != null)
                state.Operands.Push(new Operand(output, StringSegment.Encompass(new[] { bracketOpen.SourceMap, operandSource })));
        }
    }
}
