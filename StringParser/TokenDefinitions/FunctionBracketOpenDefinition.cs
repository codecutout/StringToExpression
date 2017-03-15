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
