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
    public class BracketOpenDefinition : GrammerDefinition
    {
        public BracketOpenDefinition(string name, string regex)
            : base(name, regex)
        {

        }

        public override void Apply(Token token, ParseState state)
        {
            state.Operators.Push(new Operator(this, token.SourceMap, () =>
            {
                //if we ever executed this its because the correspdoning close bracket didnt
                //pop it from the operators
                throw new BracketUnmatchedException(token.SourceMap);
            }));
        }

        public virtual void ApplyBracketOperands(Operator bracketOpen, Stack<Operand> bracketOperands, Operator bracketClose, ParseState state)
        {
            if (bracketOperands.Count == 0) {
                var insideBrackets = StringSegment.Between(bracketOpen.SourceMap, bracketClose.SourceMap);
                throw new OperandExpectedException(insideBrackets);
            }
            else if (bracketOperands.Count > 1)
            {
                var operandSpan = StringSegment.Encompass(bracketOperands.Skip(1).Select(x=>x.SourceMap));
                throw new OperandUnexpectedException(operandSpan);
            }


            var bracketOperand = bracketOperands.Pop();
            var sourceMap = StringSegment.Encompass(new[] { bracketOpen.SourceMap, bracketOperand.SourceMap, bracketClose.SourceMap });

            state.Operands.Push(new Operand(bracketOperand.Expression, sourceMap));

        }
    }
}
