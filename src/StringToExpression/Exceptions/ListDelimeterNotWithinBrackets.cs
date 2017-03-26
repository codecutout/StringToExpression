using StringToExpression.Util;
using System;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when a list delimeter is not within brackets.
    /// </summary>
    public class ListDelimeterNotWithinBrackets : ParseException
    {
        /// <summary>
        /// string segment that contains the delimeter that is unconstrained.
        /// </summary>
        public readonly StringSegment DelimeterStringSegment;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListDelimeterNotWithinBrackets"/> class.
        /// </summary>
        /// <param name="delimeterStringSegment">The location where the delimeter was found.</param>
        public ListDelimeterNotWithinBrackets(StringSegment delimeterStringSegment) 
            : base(delimeterStringSegment, $"List delimeter '{delimeterStringSegment.Value}' is not within brackets")
        {
            DelimeterStringSegment = delimeterStringSegment;
        }
    }
}
