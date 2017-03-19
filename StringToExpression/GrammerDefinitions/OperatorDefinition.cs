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
    public enum RelativePosition
    {
        Left,
        Right
    }

    public class OperatorDefinition : GrammerDefinition
    {
        public readonly Func<Expression[], Expression> ExpressionBuilder;

        public readonly IReadOnlyList<RelativePosition> ParamaterPositions;

        public readonly int? OrderOfPrecedence;

        public OperatorDefinition(string name,
           string regex,
           IEnumerable<RelativePosition> paramaterPositions,
           Func<Expression[], Expression> expressionBuilder)
           : this(name, regex, null, paramaterPositions, expressionBuilder)
        {
        }

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

                //let the expression do its thing
                var expression = ExpressionBuilder(args.Select(x => x.Expression).ToArray());

                //our new source map will encompass this operator and all its operands
                var sourceMapSpan = StringSegment.Encompass(new[] { token.SourceMap }.Concat(args.Select(x => x.SourceMap)));

                state.Operands.Push(new Operand(expression, sourceMapSpan));
            }));
        }
    }
}
