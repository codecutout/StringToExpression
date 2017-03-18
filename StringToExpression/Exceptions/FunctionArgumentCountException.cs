using StringToExpression.Util;
using System;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when an bracket does not have the correct number of operands
    /// </summary>
    public class FunctionArgumentCountException : BracketOperandCountException
    {
        public FunctionArgumentCountException(StringSegment bracketStringSegment, int expectedOperandCount, int actualOperandCount) 
            : base(bracketStringSegment, expectedOperandCount, actualOperandCount)
        {

        }
    }
}
