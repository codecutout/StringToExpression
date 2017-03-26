using StringToExpression.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// A base class for all managed exceptions while parsing a string, can be used a catch all within a try/catch.
    /// </summary>
    public abstract class ParseException : Exception
    {
        /// <summary>
        /// The location that caused the exception.
        /// </summary>
        public readonly StringSegment ErrorSegment;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        /// <param name="errorSegment">The location that caused the exception.</param>
        /// <param name="message">A message describing the exception.</param>
        public ParseException(StringSegment errorSegment, string message)
            :base(message)
        {
            ErrorSegment = errorSegment;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        /// <param name="errorSegment">The location that caused the exception.</param>
        /// <param name="message">A message describing the exception.</param>
        /// <param name="innerException">The exception that caused this exception.</param>
        public ParseException(StringSegment errorSegment, string message, Exception innerException)
            :base(message, innerException)
        {
            ErrorSegment = errorSegment;
        }
    }
}
