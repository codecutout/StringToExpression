using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StringToExpression.Parser;
using StringToExpression.Tokenizer;
using System.Collections;
using StringToExpression.Util;
using StringToExpression.Exceptions;

namespace StringToExpression.GrammerDefinitions
{
    /// <summary>
    /// Defines a position relative to the current element.
    /// </summary>
    public enum RelativePosition
    {
        Left,
        Right
    }

    /// <summary>
    ///  Represents a piece of grammer that defines an operator.
    /// </summary>
    /// <seealso cref="StringToExpression.GrammerDefinitions.GrammerDefinition" />
    public class OperatorDefinition : GrammerDefinition
    {
        /// <summary>
        /// A function given zero or more operands expressions, outputs a new operand.
        /// </summary>
        public readonly Func<Expression[], Expression> ExpressionBuilder;

        /// <summary>
        /// Positions where parameters can be found.
        /// </summary>
        public readonly IReadOnlyList<RelativePosition> ParamaterPositions;

        /// <summary>
        /// Relative order this operator should be applied. Lower orders are applied first.
        /// </summary>
        public readonly int? OrderOfPrecedence;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorDefinition"/> class.
        /// </summary>
        /// <param name="name">The name of the definition.</param>
        /// <param name="regex">The regex to match tokens.</param>
        /// <param name="paramaterPositions">The relative positions where parameters can be found.</param>
        /// <param name="expressionBuilder">The function given zero or more operands expressions, outputs a new operand.</param>
        /// <exception cref="System.ArgumentNullException">
        /// paramaterPositions
        /// or
        /// expressionBuilder
        /// </exception>
        public OperatorDefinition(string name,
           string regex,
           IEnumerable<RelativePosition> paramaterPositions,
           Func<Expression[], Expression> expressionBuilder)
           : this(name, regex, null, paramaterPositions, expressionBuilder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorDefinition"/> class.
        /// </summary>
        /// <param name="name">The name of the definition.</param>
        /// <param name="regex">The regex to match tokens.</param>
        /// <param name="orderOfPrecedence">The telative order this operator should be applied. Lower orders are applied first.</param>
        /// <param name="paramaterPositions">The relative positions where parameters can be found.</param>
        /// <param name="expressionBuilder">The function given zero or more operands expressions, outputs a new operand.</param>
        /// <exception cref="System.ArgumentNullException">
        /// paramaterPositions
        /// or
        /// expressionBuilder
        /// </exception>
        public OperatorDefinition(string name, 
            string regex,
            int? orderOfPrecedence, 
            IEnumerable<RelativePosition> paramaterPositions, 
            Func<Expression[], Expression> expressionBuilder)
            : base(name, regex)
        {
            if (paramaterPositions == null)
                throw new ArgumentNullException(nameof(paramaterPositions));
            if (expressionBuilder == null)
                throw new ArgumentNullException(nameof(expressionBuilder));

            ParamaterPositions = paramaterPositions.ToList().AsReadOnly();
            ExpressionBuilder = expressionBuilder;
            OrderOfPrecedence = orderOfPrecedence;
        }

        /// <summary>
        /// Applies the token to the parsing state
        /// </summary>
        /// <param name="token">The token to apply</param>
        /// <param name="state">The state to apply the token to</param>
        public override void Apply(Token token, ParseState state)
        {
            //Apply previous operators if they have a high precedence and they share an operand
            var anyLeftOperators = this.ParamaterPositions.Any(x => x == RelativePosition.Left);
            while (state.Operators.Count > 0 && this.OrderOfPrecedence != null && anyLeftOperators)
            {
                var prevOperator = state.Operators.Peek().Definition as OperatorDefinition;
                var prevOperatorPrecedence = prevOperator?.OrderOfPrecedence;
                if(prevOperatorPrecedence <= this.OrderOfPrecedence && prevOperator.ParamaterPositions.Any(x=> x == RelativePosition.Right))
                {
                    state.Operators.Pop().Execute();
                }
                else
                {
                    break;
                }
            }
            

            state.Operators.Push(new Operator(this, token.SourceMap, () =>
            {
                //Pop all our right arguments, and check there is the correct number and they are all to the right
                var rightArgs = new Stack<Operand>(state.Operands.PopWhile(x => x.SourceMap.IsRightOf(token.SourceMap)));
                var expectedRightArgs = this.ParamaterPositions.Count(x => x == RelativePosition.Right);
                if (expectedRightArgs > 0 && rightArgs.Count > expectedRightArgs)
                {
                    var spanWhereOperatorExpected = StringSegment.Encompass(rightArgs
                        .Reverse()
                        .Take(rightArgs.Count - expectedRightArgs)
                        .Select(x=>x.SourceMap));
                    throw new OperandUnexpectedException(token.SourceMap, spanWhereOperatorExpected);
                }
                else if(rightArgs.Count < expectedRightArgs)
                {
                    throw new OperandExpectedException(token.SourceMap, new StringSegment(token.SourceMap.SourceString, token.SourceMap.End, 0));
                }


                //Pop all our left arguments, and check they are not to the left of the next operator
                var nextOperatorEndIndex = state.Operators.Count == 0 ? 0 : state.Operators.Peek().SourceMap.End;
                var expectedLeftArgs = this.ParamaterPositions.Count(x => x == RelativePosition.Left);
                var leftArgs = new Stack<Operand>(state.Operands
                    .PopWhile((x,i) => i < expectedLeftArgs && x.SourceMap.IsRightOf(nextOperatorEndIndex)
                ));
                if (leftArgs.Count < expectedLeftArgs)
                {
                    throw new OperandExpectedException(token.SourceMap, new StringSegment(token.SourceMap.SourceString, token.SourceMap.Start, 0));
                }

                //Map the operators into the correct argument positions
                var args = new List<Operand>();
                foreach(var paramPos in this.ParamaterPositions)
                {
                    Operand operand = paramPos == RelativePosition.Right
                        ? rightArgs.Pop()
                        : leftArgs.Pop();
                    args.Add(operand);
                }

              
                //our new source map will encompass this operator and all its operands
                var sourceMapSpan = StringSegment.Encompass(new[] { token.SourceMap }.Concat(args.Select(x => x.SourceMap)));

                Expression expression;
                try
                {
                    expression = ExpressionBuilder(args.Select(x => x.Expression).ToArray());
                }catch(Exception ex)
                {
                    throw new OperationInvalidException(sourceMapSpan, ex);
                }

                state.Operands.Push(new Operand(expression, sourceMapSpan));
            }));
        }
    }
}
