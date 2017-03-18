using StringToExpression.Util;
using System;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when an unknown token is found
    /// </summary>
    public class TokenUnexpectedException : Exception
    {
        /// <summary>
        /// Segment where the token was found
        /// </summary>
        public readonly StringSegment UnexpectedTokenStringSegment;

        public TokenUnexpectedException(StringSegment unexpectedTokenStringSegment) 
            : base($"Unexpected token '{unexpectedTokenStringSegment.Value}' found")
        {
            UnexpectedTokenStringSegment = unexpectedTokenStringSegment;
        }
    }
}
