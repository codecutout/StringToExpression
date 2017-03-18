using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StringToExpression.Parser
{
    public class ParseState
    {
        public List<ParameterExpression> Parameters { get; } = new List<ParameterExpression>();
        public Stack<Operand> Operands { get; } = new Stack<Operand>();
        public Stack<Operator> Operators { get; } = new Stack<Operator>();
       
    }
}
