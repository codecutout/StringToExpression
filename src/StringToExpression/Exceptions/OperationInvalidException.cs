using StringToExpression.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception thrown when there is a generic issue processing the Expressions. Usually caused by grammer definition configurations.
    /// </summary>
    public class OperationInvalidException : ParseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationInvalidException"/> class.
        /// </summary>
        /// <param name="errorSegment">The location that caused the exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public OperationInvalidException(StringSegment errorSegment, Exception innerException)
            : base(errorSegment, $"Unable to perform operation '{errorSegment}'", innerException)
        {

        }
    }
}
