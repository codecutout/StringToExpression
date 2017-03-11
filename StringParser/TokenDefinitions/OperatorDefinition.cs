﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StringParser.Parser;
using StringParser.Tokenizer;
using System.Collections;
using StringParser.Util;
using StringParser.Exceptions;

namespace StringParser.TokenDefinitions
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

        public OperatorDefinition(string name, 
            string regex, 
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
        }

        public override void Apply(Token token, ParseState state)
        {
            

            state.Operators.Push(new Operator(this, token.SourceMap, parseState =>
            {
                //Pop all our right arguments, and check there is the correct number and they are all to the right
                var rightArgs = new Stack<Operand>(parseState.Operands.PopWhile(x => x.SourceMap.IsRightOf(token.SourceMap)));
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
                var nextOperatorEndIndex = parseState.Operators.Count == 0 ? 0 : parseState.Operators.Peek().SourceMap.End;
                var expectedLeftArgs = this.ParamaterPositions.Count(x => x == RelativePosition.Left);
                var leftArgs = new Stack<Operand>(parseState.Operands
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

                parseState.Operands.Push(new Operand(expression, sourceMapSpan));
            }));
        }
    }
}