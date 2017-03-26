using StringToExpression.Exceptions;
using StringToExpression.GrammerDefinitions;
using StringToExpression.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace StringToExpression.Test
{
    public class ParserErrorTests
    {
        public readonly Language Language;

        public ParserErrorTests()
        {
            BracketOpenDefinition openBracket, logFn, errorFn;
            GrammerDefinition listDelimeter;
            Language = new Language(
                new OperatorDefinition(
                    name: "PLUS",
                    regex: @"\+",
                    orderOfPrecedence: 1,
                    paramaterPositions: new[] { RelativePosition.Left, RelativePosition.Right },
                    expressionBuilder: args => Expression.Add(args[0], args[1])),
                new OperatorDefinition(
                    name: "MULTIPLY",
                    regex: @"\*",
                    orderOfPrecedence: 1,
                    paramaterPositions: new[] { RelativePosition.Left, RelativePosition.Right },
                    expressionBuilder: args => Expression.Multiply(args[0], args[1])),
                 logFn = new FunctionCallDefinition(
                    name: "LOG",
                    regex: @"[Ll]og\(",
                    argumentTypes: new[] { typeof(double), typeof(double) },
                    expressionBuilder: args => Expression.Call(Type<int>.Method(x => Math.Log(0, 0)), args)),
                 errorFn = new FunctionCallDefinition(
                    name: "ERROR",
                    regex: @"error\(",
                    argumentTypes: new[] { typeof(double), typeof(double) },
                    expressionBuilder: args => { throw new NotImplementedException("I am a function error"); }),
                openBracket = new BracketOpenDefinition(
                    name: "OPENBRACKET",
                    regex: @"\("),
                listDelimeter = new ListDelimiterDefinition(
                    name: "COMMA",
                    regex: @"\,"),
                new BracketCloseDefinition(
                    name: "CLOSEBRACKET",
                    regex: @"\)",
                    bracketOpenDefinitions: new[] { openBracket, logFn, errorFn },
                    listDelimeterDefinition: listDelimeter),
                new OperandDefinition(
                    name: "NUMBER",
                    regex: @"\d*\.?\d+?",
                    expressionBuilder: x => Expression.Constant(decimal.Parse(x))),
                 new OperandDefinition(
                    name: "POOP",
                    regex: @"💩",
                    expressionBuilder: x => { throw new NotImplementedException("I am an operand error"); }),
                new OperandDefinition(
                    name: "STRING",
                    regex: @"'.*?'",
                    expressionBuilder: x => Expression.Constant(x)),
                new GrammerDefinition(name: "WHITESPACE", regex: @"\s+", ignore: true)
                );
        }


        [Theory]
        [InlineData("2 + xxxx + 3", typeof(GrammerUnknownException), 4,8)]
        [InlineData("2 + + 3", typeof(OperandExpectedException), 3,3)]

        [InlineData("2 + 2 2 + 3", typeof(OperandUnexpectedException), 6,7)]
        [InlineData("2 3", typeof(OperandUnexpectedException), 2,3)]
        [InlineData("2 (3 * 4)", typeof(OperandUnexpectedException), 2, 9)]

        [InlineData("2 + 2, 3 * 3", typeof(ListDelimeterNotWithinBrackets), 5,6)]

        [InlineData("2 + (2*3", typeof(BracketUnmatchedException), 4, 5)]
        [InlineData("2 + 2)*3", typeof(BracketUnmatchedException), 5, 6)]
        [InlineData("2 + (5 + (2 * 3) + 1", typeof(BracketUnmatchedException), 4, 5)]
        [InlineData(")", typeof(BracketUnmatchedException), 0, 1)]
        [InlineData("Log(", typeof(BracketUnmatchedException), 0, 4)]

        [InlineData("2 + (5 + 2,,4) + 1", typeof(OperandExpectedException), 11,11)]
        [InlineData("2 + (5 + 2,  ) + 1", typeof(OperandExpectedException), 11,13)]
        [InlineData("2 + ( , 5 + 2) + 1", typeof(OperandExpectedException), 5, 6)]
        [InlineData("2 + () + 1", typeof(OperandExpectedException), 5, 5)]
        [InlineData("2 + (,) + 1", typeof(OperandExpectedException), 6, 6)]
        [InlineData("", typeof(OperandExpectedException), 0, 0)]
        [InlineData("*", typeof(OperandExpectedException), 1, 1)]

        [InlineData("Log(1024,2,2)", typeof(FunctionArgumentCountException), 4,12)]
        [InlineData("Log(1024)", typeof(FunctionArgumentCountException), 4,8)]
        [InlineData("Log(1024,'2')", typeof(FunctionArgumentTypeException), 9,12)]
        [InlineData("2 + '2'", typeof(OperationInvalidException), 0,7)]
        [InlineData("2 + error(2,3)", typeof(OperationInvalidException), 4,13)]
        [InlineData("2 + 💩 * 3", typeof(OperationInvalidException), 4,6)] //also interesting as double-byte unicode have length 2



        public void When_invalid_should_throw_with_indication_of_error_location(string text, Type exceptionType, int errorStart, int errorEnd)
        {
            var exception = Assert.Throws(exceptionType, () => Language.Parse<decimal>(text));

            Assert.IsAssignableFrom<ParseException>(exception);
            var parseException = (ParseException)exception;
            Assert.Equal(errorStart, parseException.ErrorSegment.Start);
            Assert.Equal(errorEnd, parseException.ErrorSegment.End);
        }
    }
}
