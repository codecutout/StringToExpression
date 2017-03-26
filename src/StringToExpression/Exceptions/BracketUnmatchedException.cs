using StringToExpression.Util;
using System;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when a bracket does not have a match.
    /// </summary>
    public class BracketUnmatchedException : ParseException
    {
        /// <summary>
        /// String segment that contains the bracket that is unmatched.
        /// </summary>
        public readonly StringSegment BracketStringSegment;

        /// <summary>
        /// Initializes a new instance of the <see cref="BracketUnmatchedException"/> class.
        /// </summary>
        /// <param name="bracketStringSegment">The string segment that contains the bracket that is unmatched.</param>
        public BracketUnmatchedException(StringSegment bracketStringSegment) 
            : base(bracketStringSegment, $"Bracket '{bracketStringSegment.Value}' is unmatched")
        {
            BracketStringSegment = bracketStringSegment;
        }
    }
}
