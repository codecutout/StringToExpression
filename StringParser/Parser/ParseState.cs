using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringParser.Parser
{
    public class ParseState
    {
        public Stack<Operand> Operands { get; } = new Stack<Operand>();
        public Stack<Operator> Operators { get; } = new Stack<Operator>();
       
    }
}
