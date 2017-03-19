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
        public readonly StringSegment ExceptionLocation;

        public ParseException(StringSegment exceptionLocation, string message)
        {
            ExceptionLocation = exceptionLocation;
        }
    }
}
