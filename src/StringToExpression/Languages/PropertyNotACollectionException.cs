using StringToExpression.Languages.StringToExpression.GrammerDefinitions;
using StringToExpression.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception thrown when a collection method is attempted to be called on a property that is not a collection
    /// </summary>
    public class PropertyNotACollectionException : ParseException
    {
        public PropertyNotACollectionException(StringSegment errorSegment, StringSegment property, string collectionMethodName)
            : base(errorSegment, $"{property} is not a collection and can not use the '{collectionMethodName}' method")
        {

        }
    }
}
