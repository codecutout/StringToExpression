using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StringParser.Parser;
using StringParser.Tokenizer;

namespace StringParser.TokenDefinitions
{
    public class OperandDefinition : GrammerDefinition
    {
        public readonly Func<string, Expression> ExpressionBuilder;

        public OperandDefinition(string name, string regex, Func<string, Expression> expressionBuilder)
            : base(name, regex)
        {
            if (expressionBuilder == null)
                throw new ArgumentNullException(nameof(expressionBuilder));
            ExpressionBuilder = expressionBuilder;
        }

        public override void Apply(Token token, ParseState state)
        {
            var expression = ExpressionBuilder(token.Value);
            state.Operands.Push(new Operand(expression, token.SourceMap));
        }
    }
}
