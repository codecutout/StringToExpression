using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StringToExpression.Parser;
using StringToExpression.Tokenizer;
using StringToExpression.Exceptions;

namespace StringToExpression.GrammerDefinitions
{
    /// <summary>
    /// Represents the grammer that seperates items in a list.
    /// </summary>
    /// <seealso cref="StringToExpression.GrammerDefinitions.GrammerDefinition" />
    public class ListDelimiterDefinition : GrammerDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListDelimiterDefinition"/> class.
        /// </summary>
        /// <param name="name">The name of the definition.</param>
        /// <param name="regex">The regex to match tokens.</param>
        public ListDelimiterDefinition(string name, string regex)
            : base(name, regex)
        {

        }

        /// <summary>
        /// Applies the token to the parsing state. Adds an error operator, it is expected that a close bracket will consume the
        /// error operator before it gets executed.
        /// </summary>
        /// <param name="token">The token to apply.</param>
        /// <param name="state">The state to apply the token to.</param>
        public override void Apply(Token token, ParseState state)
        {
            state.Operators.Push(new Operator(this, token.SourceMap, () =>
            {
                //if we ever executed this its because the correspdoning close bracket didnt
                //pop it from the operators
                throw new ListDelimeterNotWithinBrackets(token.SourceMap);
            }));
        }
    }
}
