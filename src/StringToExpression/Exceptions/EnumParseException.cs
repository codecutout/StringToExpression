using System;
using System.Collections.Generic;
using System.Text;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when a string can not be parsed as an enumeration
    /// </summary>
    public class EnumParseException : Exception
    {
        /// <summary>
        /// The string that was attempted to be parsed.
        /// </summary>
        public readonly string StringValue;

        /// <summary>
        /// The enumeration that the string was attempted to be parsed as.
        /// </summary>
        public readonly Type EnumType;

        public EnumParseException(string stringValue, Type enumType, Exception ex)
            : base($"'{stringValue}' is not a valid value for enum type '{enumType}'", ex)
        {
            StringValue = stringValue;
            EnumType = enumType;
        }
    }
}
