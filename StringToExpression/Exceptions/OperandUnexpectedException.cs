using StringToExpression.Util;
using System;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when an operand was found but not expected
    /// </summary>
    public class OperandUnexpectedException : Exception
    {
        /// <summary>
        /// StringSegment that contains the operator
        /// </summary>
        public readonly StringSegment OperatorStringSegment;

        /// <summary>
        /// StringSegment where the unexpected operators were found
        /// </summary>
        public readonly StringSegment UnexpectedOperandStringSegment;

     

        public OperandUnexpectedException(StringSegment operatorStringSegment, StringSegment unexpectedOperandStringSegment) 
            : base($"Unexpected operands '{unexpectedOperandStringSegment.Value}' found while processing '{operatorStringSegment.Value}'. Perhaps an operand is missing")
        {
            OperatorStringSegment = operatorStringSegment;
            UnexpectedOperandStringSegment = unexpectedOperandStringSegment;
        }
    }
}
