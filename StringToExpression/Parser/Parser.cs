using StringToExpression.Tokenizer;
using StringToExpression.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StringToExpression.Parser
{
    public class Parser
    {
        public Expression Parse(IEnumerable<Token> tokens, IEnumerable<ParameterExpression> parameters = null)
        {
            parameters = parameters ?? Enumerable.Empty<ParameterExpression>();

            var compileState = new ParseState();
            compileState.Parameters.AddRange(parameters);
            foreach (var token in tokens)
            {
                token.Definition.Apply(token, compileState);
            }

            var outputExpression = FoldOperators(compileState);
            return outputExpression;
        }

        private Expression FoldOperators(ParseState state)
        {
            while (state.Operators.Count > 0)
            {
                var op = state.Operators.Pop();
                op.Execute();
            }

            if (state.Operands.Count != 1)
                throw new Exception("Operators have not successfully reduced the expression");

            return state.Operands.Peek().Expression;
        }
    }
}
