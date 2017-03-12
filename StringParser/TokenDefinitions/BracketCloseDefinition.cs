using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StringParser.Parser;
using StringParser.Tokenizer;
using StringParser.Exceptions;

namespace StringParser.TokenDefinitions
{
    public class BracketCloseDefinition : GrammerDefinition
    {
        public readonly GrammerDefinition BracketOpenDefinition;

        public BracketCloseDefinition(string name, string regex, GrammerDefinition bracketOpenDefinition)
            : base(name, regex)
        {
            if (bracketOpenDefinition == null)
                throw new ArgumentNullException(nameof(bracketOpenDefinition));
            this.BracketOpenDefinition = bracketOpenDefinition;
        }

        public override void Apply(Token token, ParseState state)
        {
            while(state.Operators.Count > 0)
            {
                var op = state.Operators.Pop();
                if (op.Definition == BracketOpenDefinition)
                    return;
                op.Execute();
            }

            //We have pop'd through all the operators and not found an open bracket
            throw new BracketUnmatchedException(token.SourceMap);
        }
    }
}
