using StringToExpression.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StringToExpression.Parser
{
    /// <summary>
    /// Represents an Operand.
    /// </summary>
    public class Operand
    {
        /// <summary>
        /// The Expression that represents this operand.
        /// </summary>
        public readonly Expression Expression;

        /// <summary>
        /// The original string and position this entire operand is from.
        /// </summary>
        public readonly StringSegment SourceMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="Operand"/> class.
        /// </summary>
        /// <param name="expression">The Expression that represents this operand.</param>
        /// <param name="sourceMap">The original string and position this entire operand is from.</param>
        public Operand(Expression expression, StringSegment sourceMap)
        {
            this.Expression = expression;
            this.SourceMap = sourceMap;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return SourceMap.Value;
        }
    }
}
