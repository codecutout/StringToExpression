using StringToExpression.Util;
using System;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when a function does not have the correct number of operands.
    /// </summary>
    public class FunctionArgumentCountException : ParseException
    {
        /// <summary>
        /// String segment that contains the bracket that contains the incorrect number of operands.
        /// </summary>
        public readonly StringSegment FunctionStringSegment;

        /// <summary>
        /// Expected number of operands.
        /// </summary>
        public readonly int ExpectedOperandCount;

        /// <summary>
        /// Actual number of operands.
        /// </summary>
        public readonly int ActualOperandCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionArgumentCountException"/> class.
        /// </summary>
        /// <param name="functionStringSegment">The location of the function arguments.</param>
        /// <param name="expectedOperandCount">The Expected number of operands.</param>
        /// <param name="actualOperandCount">The actual number of operands.</param>
        public FunctionArgumentCountException(StringSegment functionStringSegment, int expectedOperandCount, int actualOperandCount) 
            : base(functionStringSegment, $"Function '{functionStringSegment.Value}' contains {actualOperandCount} operand{(actualOperandCount > 1 ? "s" : "")} but was expecting {expectedOperandCount} operand{(expectedOperandCount > 1 ? "s" : "")}")
        {
            FunctionStringSegment = functionStringSegment;
            ExpectedOperandCount = expectedOperandCount;
            ActualOperandCount = actualOperandCount;
        }
    }
}
