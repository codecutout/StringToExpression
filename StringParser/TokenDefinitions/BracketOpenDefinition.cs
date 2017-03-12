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
    }
}
