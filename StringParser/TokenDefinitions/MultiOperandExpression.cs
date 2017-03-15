using StringParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StringParser.TokenDefinitions
{
    /// <summary>
    /// A custom expression that holds a list of operands. Used primary to pass arguments to functions
    /// </summary>
    public class MultiOperandExpression : Expression
    {
        public IReadOnlyList<Operand> Operands;

        public MultiOperandExpression(IEnumerable<Operand> operands)
        {
            Operands = operands.ToList().AsReadOnly();
        }
    }
}
