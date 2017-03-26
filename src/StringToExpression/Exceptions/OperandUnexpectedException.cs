using StringToExpression.Util;
using System;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when an operand was found but not expected
    /// </summary>
    public class OperandUnexpectedException : ParseException
    {
        /// <summary>
        /// StringSegment that contains the operator
        /// </summary>
        public readonly StringSegment OperatorStringSegment;

        /// <summary>
        /// StringSegment where the unexpected operators were found
        /// </summary>
        public readonly StringSegment UnexpectedOperandStringSegment;


        public OperandUnexpectedException(StringSegment unexpectedOperandStringSegment)
           : base(unexpectedOperandStringSegment, $"Unexpected operands '{unexpectedOperandStringSegment.Value}' found. Perhaps an operator is missing")
        {
            UnexpectedOperandStringSegment = unexpectedOperandStringSegment;
            OperatorStringSegment = null;
        }

        public OperandUnexpectedException(StringSegment operatorStringSegment, StringSegment unexpectedOperandStringSegment) 
            : base(unexpectedOperandStringSegment, $"Unexpected operands '{unexpectedOperandStringSegment.Value}' found while processing '{operatorStringSegment.Value}'. Perhaps an operator is missing")
        {
            OperatorStringSegment = operatorStringSegment;
            UnexpectedOperandStringSegment = unexpectedOperandStringSegment;
        }
    }
}
