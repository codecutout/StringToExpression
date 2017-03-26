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

        public override void ApplyBracketOperands(Operator bracketOpen, Stack<Operand> bracketOperands, Operator bracketClose, ParseState state)
        {
            var operandSource = StringSegment.Encompass(bracketOperands.Select(x => x.SourceMap));
            var functionArguments = bracketOperands.Select(x => x.Expression);
            //if we have been given specific argument types validate them
            if (ArgumentTypes != null)
            {
                var expectedArgumentCount = ArgumentTypes.Count;
                if (expectedArgumentCount != bracketOperands.Count)
                    throw new FunctionArgumentCountException(
                        operandSource,
                        expectedArgumentCount,
                        bracketOperands.Count);

                functionArguments = bracketOperands.Zip(ArgumentTypes, (o, t) => {
                    try
                    {
                        return ExpressionConversions.Convert(o.Expression, t);
                    }
                    catch(InvalidOperationException)
                    {
                        //if we cant convert to the argument type then something is wrong with the argument
                        //so we will throw it up
                        throw new FunctionArgumentTypeException(o.SourceMap, t, o.Expression.Type);
                    }
                });

            }

            var functionSourceMap = StringSegment.Encompass(bracketOpen.SourceMap, operandSource);
            var functionArgumentsArray = functionArguments.ToArray();
            Expression output;
            try
            {
                output = ExpressionBuilder(functionArgumentsArray);
            }
            catch(Exception ex)
            {
                throw new OperationInvalidException(functionSourceMap, ex);
            }
            if(output != null)
                state.Operands.Push(new Operand(output, functionSourceMap));
        }
    }
}
