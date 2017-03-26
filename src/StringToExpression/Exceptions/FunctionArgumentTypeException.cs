using StringToExpression.Util;
using System;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when a function argument is not the expected type
    /// </summary>
    public class FunctionArgumentTypeException : ParseException
    {
        /// <summary>
        /// StringSegment that contains the argument of incorrect type
        /// </summary>
        public readonly StringSegment ArgumentStringSegment;

        /// <summary>
        /// Argument type expected
        /// </summary>
        public readonly Type ExpectedType;

        /// <summary>
        /// Argument type
        /// </summary>
        public readonly Type ActualType;


        public FunctionArgumentTypeException(StringSegment argumentStringSegment, Type expectedType, Type actualType) 
            : base(argumentStringSegment, $"Argument '{argumentStringSegment.Value}' type expected {expectedType} but was {actualType}")
        {
            ArgumentStringSegment = argumentStringSegment;
            ExpectedType = expectedType;
            ActualType = actualType;
        }
    }
}
