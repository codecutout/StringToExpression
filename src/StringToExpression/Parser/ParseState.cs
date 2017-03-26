using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StringToExpression.Parser
{
    /// <summary>
    /// Represents the current state of the Parsing.
    /// </summary>
    public class ParseState
    {
        /// <summary>
        /// Gets the parameters that can be used during the parsing.
        /// </summary>
        public List<ParameterExpression> Parameters { get; } = new List<ParameterExpression>();

        /// <summary>
        /// Gets the current stack of operands.
        /// </summary>
        public Stack<Operand> Operands { get; } = new Stack<Operand>();

        /// <summary>
        /// Gets  the current stack of operators.
        /// </summary>
        public Stack<Operator> Operators { get; } = new Stack<Operator>();
       
    }
}
