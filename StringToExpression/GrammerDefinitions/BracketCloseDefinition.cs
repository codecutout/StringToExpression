using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StringToExpression.Parser;
using StringToExpression.Tokenizer;
using StringToExpression.Exceptions;
using StringToExpression.Util;

namespace StringToExpression.GrammerDefinitions
{
    public class BracketCloseDefinition : GrammerDefinition
    {
        public readonly IReadOnlyCollection<BracketOpenDefinition> BracketOpenDefinitions;
        public readonly GrammerDefinition ListDelimeterDefinition;

        public BracketCloseDefinition(string name, string regex,
            IEnumerable<BracketOpenDefinition> bracketOpenDefinitions,
            GrammerDefinition listDelimeterDefinition = null)
            : base(name, regex)
        {
            if (bracketOpenDefinitions == null)
                throw new ArgumentNullException(nameof(bracketOpenDefinitions));
            this.BracketOpenDefinitions = bracketOpenDefinitions.ToList().AsReadOnly();
            this.ListDelimeterDefinition = listDelimeterDefinition;
        }

        public BracketCloseDefinition(string name, string regex,
           BracketOpenDefinition bracketOpenDefinition,
           GrammerDefinition listDelimeterDefinition = null)
            : this(name, regex, new[] { bracketOpenDefinition}, listDelimeterDefinition)
        {

        }

        public override void Apply(Token token, ParseState state)
        {
            Stack<Operand> bracketOperands = new Stack<Operand>();
            while(state.Operators.Count > 0)
            {
                var op = state.Operators.Pop();
                if (BracketOpenDefinitions.Contains(op.Definition))
                {
                    //we have hit the opening bracket we are done
                    bracketOperands.Push(state.Operands.Pop());

                    ((BracketOpenDefinition)op.Definition).ApplyBracketOperands(op, bracketOperands, state);
                    return;
                }
                else if (ListDelimeterDefinition != null && op.Definition == ListDelimeterDefinition)
                {
                    bracketOperands.Push(state.Operands.Pop());
                }
                else
                {
                    //regular operator, execute it
                    op.Execute();
                }
               
            }

            //We have pop'd through all the operators and not found an open bracket
            throw new BracketUnmatchedException(token.SourceMap);
        }
    }
}
