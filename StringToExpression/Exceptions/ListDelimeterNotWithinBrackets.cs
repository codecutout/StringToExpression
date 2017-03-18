using StringToExpression.Util;
using System;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when a list delimeter is not within brackets
    /// </summary>
    public class ListDelimeterNotWithinBrackets : Exception
    {
        /// <summary>
        /// StringSegment that contains the delimeter that is unconstrained
        /// </summary>
        public readonly StringSegment DelimeterStringSegment;

  
        public ListDelimeterNotWithinBrackets(StringSegment delimeterStringSegment) 
            : base($"List delimeter '{delimeterStringSegment.Value}' is not within brackets")
        {
            DelimeterStringSegment = delimeterStringSegment;
        }
    }
}
