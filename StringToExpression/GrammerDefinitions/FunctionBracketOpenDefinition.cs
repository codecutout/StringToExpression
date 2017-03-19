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
    public class FunctionBracketOpenDefinition : BracketOpenDefinition
    {
        public FunctionBracketOpenDefinition(string name, string regex)
            : base(name, regex)
        {

        }

        public override void ApplyBracketOperands(Stack<Operand> bracketOperands, ParseState state)
        {
            var bracketSpan = StringSegment.Encompass(bracketOperands.Select(x => x.SourceMap));
            var multiOperandExpression = new MultiOperandExpression(bracketOperands);
      
            state.Operands.Push(new Operand(multiOperandExpression, bracketSpan));

        }
    }
}
