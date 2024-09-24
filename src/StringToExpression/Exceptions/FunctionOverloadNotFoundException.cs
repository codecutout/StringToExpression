using StringToExpression.Util;
using System;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when a cannot find correct overlaod for function
    /// </summary>
    public class FunctionOverlaodNotFoundException : ParseException
    {
        /// <summary>
        /// String segment that contains the bracket that contains the incorrect number of operands.
        /// </summary>
        public readonly StringSegment FunctionStringSegment;

        /// <summary>
        /// Actual type of operands.
        /// </summary>
        public readonly Type[] ActualArgumentTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionOverlaodNotFoundException"/> class.
        /// </summary>
        /// <param name="bracketStringSegment">The location of the function</param>
        public FunctionOverlaodNotFoundException(StringSegment functionStringSegment) 
            : base(functionStringSegment, $"Function '{functionStringSegment.Value}' is not defiend wtih those input types")
        {
            FunctionStringSegment = functionStringSegment;

        }
    }
}
