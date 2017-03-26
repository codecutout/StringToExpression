using StringToExpression.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// A base class for all exceptions, can be used a catch all within a try/catch
    /// </summary>
    public abstract class ParseException : Exception
    {
        public readonly StringSegment ErrorSegment;

        public ParseException(StringSegment errorSegment, string message)
            :base(message)
        {
            ErrorSegment = errorSegment;
        }

        public ParseException(StringSegment errorSegment, string message, Exception innerException)
            :base(message, innerException)
        {
            ErrorSegment = errorSegment;
        }
    }
}
