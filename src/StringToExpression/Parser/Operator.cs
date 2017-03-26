using StringToExpression.GrammerDefinitions;
using StringToExpression.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringToExpression.Parser
{
    /// <summary>
    /// Represents an Operator.
    /// </summary>
    public class Operator
    {
        /// <summary>
        /// Applies the operator, updating the ParseState.
        /// </summary>
        public readonly Action Execute;

        /// <summary>
        /// The original string and position this entire operand is from.
        /// </summary>
        public readonly StringSegment SourceMap;

        /// <summary>
        /// The grammer that defined this Operator.
        /// </summary>
        public readonly GrammerDefinition Definition;

        /// <summary>
        /// Initializes a new instance of the <see cref="Operator"/> class.
        /// </summary>
        /// <param name="definition">The grammer that defined this Operator.</param>
        /// <param name="sourceMap">The original string and position this entire operand is from.</param>
        /// <param name="execute">The action to run when applying this operator.</param>
        public Operator(GrammerDefinition definition, StringSegment sourceMap, Action execute)
        {
            this.Execute = execute;
            this.SourceMap = sourceMap;
            this.Definition = definition;
        }

        public override string ToString()
        {
            return SourceMap.Value;
        }
    }
}
