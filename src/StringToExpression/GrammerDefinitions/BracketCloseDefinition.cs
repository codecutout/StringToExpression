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
    /// <summary>
    /// Represents a closing bracket.
    /// </summary>
    /// <seealso cref="StringToExpression.GrammerDefinitions.GrammerDefinition" />
    public class BracketCloseDefinition : GrammerDefinition
    {
        /// <summary>
        /// The definitions that can be considered as the matching opening bracket.
        /// </summary>
        public readonly IReadOnlyCollection<BracketOpenDefinition> BracketOpenDefinitions;

        /// <summary>
        /// The definition for the delimeter for a list of items.
        /// </summary>
        public readonly GrammerDefinition ListDelimeterDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="BracketCloseDefinition"/> class.
        /// </summary>
        /// <param name="name">The name of the definition.</param>
        /// <param name="regex">The regex to match tokens.</param>
        /// <param name="bracketOpenDefinitions">The definitions that can be considered as the matching opening bracket.</param>
        /// <param name="listDelimeterDefinition">The definition for the delimeter for a list of items.</param>
        /// <exception cref="System.ArgumentNullException">bracketOpenDefinitions</exception>
        public BracketCloseDefinition(string name, string regex,
            IEnumerable<BracketOpenDefinition> bracketOpenDefinitions,
            GrammerDefinition listDelimeterDefinition = null)
            : base(name, regex)
        {
            if (bracketOpenDefinitions == null)
                throw new ArgumentNullException(nameof(bracketOpenDefinitions));
            this.BracketOpenDefinitions = bracketOpenDefinitions.ToList();
            this.ListDelimeterDefinition = listDelimeterDefinition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BracketCloseDefinition"/> class.
        /// </summary>
        /// <param name="name">The name of the definition.</param>
        /// <param name="regex">The regex to match tokens.</param>
        /// <param name="bracketOpenDefinition">The definition that can be considered as the matching opening bracket.</param>
        /// <param name="listDelimeterDefinition">The definition for the delimeter for a list of items.</param>
        public BracketCloseDefinition(string name, string regex,
           BracketOpenDefinition bracketOpenDefinition,
           GrammerDefinition listDelimeterDefinition = null)
            : this(name, regex, new[] { bracketOpenDefinition}, listDelimeterDefinition)
        {

        }

        /// <summary>
        /// Applies the token to the parsing state. Will pop the operator stack executing all the operators storing each of the operands
        /// When we reach an opening bracket it will pass the stored operands to the opening bracket to be processed.
        /// </summary>
        /// <param name="token">The token to apply.</param>
        /// <param name="state">The state to apply the token to.</param>
        /// <exception cref="OperandExpectedException">When there are delimeters but no operands between them.</exception>
        /// <exception cref="BracketUnmatchedException">When there was no matching closing bracket.</exception>
        public override void Apply(Token token, ParseState state)
        {
            Stack<Operand> bracketOperands = new Stack<Operand>();
            StringSegment previousSeperator = token.SourceMap;
            bool hasSeperators = false;

            while(state.Operators.Count > 0)
            {
                var currentOperator = state.Operators.Pop();
                if (BracketOpenDefinitions.Contains(currentOperator.Definition))
                {
                    var operand = state.Operands.Count > 0 ? state.Operands.Peek() : null;
                    var firstSegment = currentOperator.SourceMap;
                    var secondSegment = previousSeperator;
                    if (operand != null && operand.SourceMap.IsBetween(firstSegment, secondSegment))
                    {
                        bracketOperands.Push(state.Operands.Pop());
                    }
                    else if(hasSeperators && (operand == null || !operand.SourceMap.IsBetween(firstSegment, secondSegment)))
                    {
                        //if we have seperators then we should have something between the last seperator and the open bracket.
                        throw new OperandExpectedException(StringSegment.Between(firstSegment, secondSegment));
                    }

                    //pass our all bracket operands to the open bracket method, he will know
                    //what we should do.
                    var closeBracketOperator = new Operator(this, token.SourceMap, () => { });
                    ((BracketOpenDefinition)currentOperator.Definition).ApplyBracketOperands(
                        currentOperator, 
                        bracketOperands, 
                        closeBracketOperator,  
                        state);
                    return;
                }
                else if (ListDelimeterDefinition != null && currentOperator.Definition == ListDelimeterDefinition)
                {
                    hasSeperators = true;
                    var operand = state.Operands.Pop();

                    //if our operator is not between two delimeters, then we are missing an operator
                    var firstSegment = currentOperator.SourceMap;
                    var secondSegment = previousSeperator;
                    if (!operand.SourceMap.IsBetween(firstSegment, secondSegment))
                    {
                        throw new OperandExpectedException(StringSegment.Between(firstSegment, secondSegment));
                    }

                    bracketOperands.Push(operand);
                    previousSeperator = currentOperator.SourceMap;
                }
                else
                {
                    //regular operator, execute it
                    currentOperator.Execute();
                }
               
            }

            //We have pop'd through all the operators and not found an open bracket
            throw new BracketUnmatchedException(token.SourceMap);
        }
    }
}
