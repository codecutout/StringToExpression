using StringParser.Util;
using System;

namespace StringParser.Exceptions
{
    /// <summary>
    /// Exception when an operand was expected but not found
    /// </summary>
    public class OperandExpectedException : Exception
    {
        /// <summary>
        /// StringSegment that contains the operator
        /// </summary>
        public readonly StringSegment OperatorStringSegment;

        /// <summary>
        /// StringSegment where the operators were expected
        /// </summary>
        public readonly StringSegment ExpectedOperandStringSegment;

     

        public OperandExpectedException(StringSegment operatorStringSegment, StringSegment expectedOperandStringSegment) 
            : base($"Expected operands to be found for '{operatorStringSegment.Value}'")
        {
            OperatorStringSegment = operatorStringSegment;
            ExpectedOperandStringSegment = expectedOperandStringSegment;
        }
    }
}
