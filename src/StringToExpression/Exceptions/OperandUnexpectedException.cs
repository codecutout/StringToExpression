using StringToExpression.Util;
using System;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when an operand was found but not expected.
    /// </summary>
    public class OperandUnexpectedException : ParseException
    {
        /// <summary>
        /// String segment that contains the operator.
        /// </summary>
        public readonly StringSegment OperatorStringSegment;

        /// <summary>
        /// String segment where the unexpected operators were found.
        /// </summary>
        public readonly StringSegment UnexpectedOperandStringSegment;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperandUnexpectedException"/> class.
        /// </summary>
        /// <param name="unexpectedOperandStringSegment">The location that caused the exception.</param>
        public OperandUnexpectedException(StringSegment unexpectedOperandStringSegment)
           : base(unexpectedOperandStringSegment, $"Unexpected operands '{unexpectedOperandStringSegment.Value}' found. Perhaps an operator is missing")
        {
            UnexpectedOperandStringSegment = unexpectedOperandStringSegment;
            OperatorStringSegment = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperandUnexpectedException"/> class.
        /// </summary>
        /// <param name="operatorStringSegment">The operator that was executing when the opreand was encountered.</param>
        /// <param name="unexpectedOperandStringSegment">The location of the operand that was unexpected.</param>
        public OperandUnexpectedException(StringSegment operatorStringSegment, StringSegment unexpectedOperandStringSegment) 
            : base(unexpectedOperandStringSegment, $"Unexpected operands '{unexpectedOperandStringSegment.Value}' found while processing '{operatorStringSegment.Value}'. Perhaps an operator is missing")
        {
            OperatorStringSegment = operatorStringSegment;
            UnexpectedOperandStringSegment = unexpectedOperandStringSegment;
        }
    }
}
