using StringToExpression.Util;
using System;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when an operand was expected but not found
    /// </summary>
    public class OperandExpectedException : ParseException
    {
        /// <summary>
        /// StringSegment that contains the operator
        /// </summary>
        public readonly StringSegment OperatorStringSegment;

        /// <summary>
        /// StringSegment where the operators were expected
        /// </summary>
        public readonly StringSegment ExpectedOperandStringSegment;


        public OperandExpectedException(StringSegment expectedOperandStringSegment)
            : base(expectedOperandStringSegment, $"Expected operands to be found")
        {
            OperatorStringSegment = null;
            ExpectedOperandStringSegment = expectedOperandStringSegment;
        }

        public OperandExpectedException(StringSegment operatorStringSegment, StringSegment expectedOperandStringSegment) 
            : base(expectedOperandStringSegment, $"Expected operands to be found for '{operatorStringSegment.Value}'")
        {
            OperatorStringSegment = operatorStringSegment;
            ExpectedOperandStringSegment = expectedOperandStringSegment;
        }
    }
}
