using StringToExpression.Util;
using System;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when an operand was expected but not found.
    /// </summary>
    public class OperandExpectedException : ParseException
    {
        /// <summary>
        /// String segment that contains the operator.
        /// </summary>
        public readonly StringSegment OperatorStringSegment;

        /// <summary>
        /// String segment where the operators were expected.
        /// </summary>
        public readonly StringSegment ExpectedOperandStringSegment;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperandExpectedException"/> class.
        /// </summary>
        /// <param name="expectedOperandStringSegment">The location where the operand was expected to be.</param>
        public OperandExpectedException(StringSegment expectedOperandStringSegment)
            : base(expectedOperandStringSegment, $"Expected operands to be found")
        {
            OperatorStringSegment = null;
            ExpectedOperandStringSegment = expectedOperandStringSegment;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperandExpectedException"/> class.
        /// </summary>
        /// <param name="operatorStringSegment">The operator that was expecting the operand.</param>
        /// <param name="expectedOperandStringSegment">The location where the operand was expected to be.</param>
        public OperandExpectedException(StringSegment operatorStringSegment, StringSegment expectedOperandStringSegment) 
            : base(expectedOperandStringSegment, $"Expected operands to be found for '{operatorStringSegment.Value}'")
        {
            OperatorStringSegment = operatorStringSegment;
            ExpectedOperandStringSegment = expectedOperandStringSegment;
        }
    }
}
