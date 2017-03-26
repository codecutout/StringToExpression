using StringToExpression.Util;
using System;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when an unknown token is found
    /// </summary>
    public class GrammerUnexpectedException : ParseException
    {
        /// <summary>
        /// Segment where the token was found
        /// </summary>
        public readonly StringSegment UnexpectedGrammerStringSegment;

        public GrammerUnexpectedException(StringSegment unexpectedGrammerStringSegment) 
            : base(unexpectedGrammerStringSegment, $"Unexpected token '{unexpectedGrammerStringSegment.Value}' found")
        {
            UnexpectedGrammerStringSegment = unexpectedGrammerStringSegment;
        }
    }
}
