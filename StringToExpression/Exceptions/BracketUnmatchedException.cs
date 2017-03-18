using StringToExpression.Util;
using System;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when an bracket does not have a match
    /// </summary>
    public class BracketUnmatchedException : Exception
    {
        /// <summary>
        /// StringSegment that contains the bracket that is unmatched
        /// </summary>
        public readonly StringSegment BracketStringSegment;

  
        public BracketUnmatchedException(StringSegment bracketStringSegment) 
            : base($"Bracket '{bracketStringSegment.Value}' is unmatched")
        {
            BracketStringSegment = bracketStringSegment;
        }
    }
}
