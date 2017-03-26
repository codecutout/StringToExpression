using StringToExpression.Util;
using System;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when an unknown grammer is encountered.
    /// </summary>
    public class GrammerUnknownException : ParseException
    {
        /// <summary>
        /// string segment where the token was found.
        /// </summary>
        public readonly StringSegment UnexpectedGrammerStringSegment;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrammerUnknownException"/> class.
        /// </summary>
        /// <param name="unexpectedGrammerStringSegment">The location of the unknown grammer.</param>
        public GrammerUnknownException(StringSegment unexpectedGrammerStringSegment) 
            : base(unexpectedGrammerStringSegment, $"Unexpected token '{unexpectedGrammerStringSegment.Value}' found")
        {
            UnexpectedGrammerStringSegment = unexpectedGrammerStringSegment;
        }
    }
}
