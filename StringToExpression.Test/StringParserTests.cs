using StringToExpression.Exceptions;
using StringToExpression.GrammerDefinitions;
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
    public class StringParserTests
    {
        [Fact]
        public void Should_parse_basic_expression()
        {
            var language = new Language(
                new OperatorDefinition(
                    name:"PLUS", 
                    regex: @"\+", 
                    paramaterPositions: new[] { RelativePosition.Left, RelativePosition.Right },
                    expressionBuilder: args => Expression.Add(args[0], args[1])),
                new OperandDefinition(
                    name:"NUMBER", 
                    regex: @"\d*\.?\d+?",
                    expressionBuilder: x => Expression.Constant(decimal.Parse(x))),
                new GrammerDefinition(name: "WHITESPACE", regex: @"\s+", ignore: true)
                );
           
            var expression = language.Parse<decimal>("1 + 2 + 3 + 5");
            Assert.Equal(11, expression.Compile()());
        }

        [Fact]
        public void When_too_many_operaters_should_throw()
        {
            var language = new Language(
                new OperatorDefinition(
                    name: "PLUS",
                    regex: @"\+",
                    paramaterPositions: new[] { RelativePosition.Left, RelativePosition.Right },
                    expressionBuilder: args => Expression.Add(args[0], args[1])),
                new OperandDefinition(
                    name: "NUMBER",
                    regex: @"\d*\.?\d+?",
                    expressionBuilder: x => Expression.Constant(decimal.Parse(x))),
                new GrammerDefinition(name: "WHITESPACE", regex: @"\s+", ignore: true)
                );

            var exception = Assert.Throws<OperandExpectedException>(() => language.Parse<decimal>("1 + + 5"));
            Assert.Equal("1 + [+] 5", exception.OperatorStringSegment.Highlight());
            Assert.Equal("1 + []+ 5", exception.ExpectedOperandStringSegment.Highlight());
        }

        [Fact]
        public void When_too_many_operand_should_throw()
        {
            var language = new Language(
                new OperatorDefinition(
                    name: "PLUS",
                    regex: @"\+",
                    paramaterPositions: new[] { RelativePosition.Left, RelativePosition.Right },
                    expressionBuilder: args => Expression.Add(args[0], args[1])),
                new OperandDefinition(
                    name: "NUMBER",
                    regex: @"\d*\.?\d+?",
                    expressionBuilder: x => Expression.Constant(decimal.Parse(x))),
                new GrammerDefinition(name: "WHITESPACE", regex: @"\s+", ignore: true)
                );

            var exception = Assert.Throws<OperandUnexpectedException>(() => language.Parse<decimal>("1 + 5 5"));
            Assert.Equal("1 [+] 5 5", exception.OperatorStringSegment.Highlight());
            Assert.Equal("1 + 5 [5]", exception.UnexpectedOperandStringSegment.Highlight());
        }


        [Fact]
        public void Should_obey_operator_precedence()
        {
            var language = new Language(
                new OperatorDefinition(
                    name: "PLUS",
                    regex: @"\+",
                    orderOfPrecedence: 2,
                    paramaterPositions: new[] { RelativePosition.Left, RelativePosition.Right },
                    expressionBuilder: args => Expression.Add(args[0], args[1])),
                  new OperatorDefinition(
                    name: "MULTIPLY",
                    regex: @"\*",
                    orderOfPrecedence: 1,
                    paramaterPositions: new[] { RelativePosition.Left, RelativePosition.Right },
                    expressionBuilder: args => Expression.Multiply(args[0], args[1])),
                new OperandDefinition(
                    name: "NUMBER",
                    regex: @"\d*\.?\d+?",
                    expressionBuilder: x => Expression.Constant(decimal.Parse(x))),
                new GrammerDefinition(name: "WHITESPACE", regex: @"\s+", ignore: true)
                );

            var expression = language.Parse<decimal>("1 + 2 * 3 + 5");
            Assert.Equal(12, expression.Compile()());
        }

        [Fact]
        public void Should_apply_brackets()
        {
            BracketOpenDefinition openBracket;
            var language = new Language(
                new OperatorDefinition(
                    name: "PLUS",
                    regex: @"\+",
                    orderOfPrecedence: 1,
                    paramaterPositions: new[] { RelativePosition.Left, RelativePosition.Right },
                    expressionBuilder: args => Expression.Add(args[0], args[1])),
                  new OperatorDefinition(
                    name: "MULTIPLY",
                    regex: @"\*",
                    orderOfPrecedence: 2,
                    paramaterPositions: new[] { RelativePosition.Left, RelativePosition.Right },
                    expressionBuilder: args => Expression.Multiply(args[0], args[1])),
                openBracket = new BracketOpenDefinition(
                    name: "OPENBRACKET",
                    regex: @"\("),
                new BracketCloseDefinition(
                    name: "CLOSEBRACKET",
                    regex: @"\)",
                    bracketOpenDefinition: openBracket),
                new OperandDefinition(
                    name: "NUMBER",
                    regex: @"\d*\.?\d+?",
                    expressionBuilder: x => Expression.Constant(decimal.Parse(x))),
                new GrammerDefinition(name: "WHITESPACE", regex: @"\s+", ignore: true)
                );

            var expression = language.Parse<decimal>("(1 + 2) * (3 + 5)");
            Assert.Equal(24, expression.Compile()());
        }

        [Fact]
        public void Should_run_single_param_functions()
        {
            BracketOpenDefinition openBracket;
            var language = new Language(
                new OperatorDefinition(
                    name: "PLUS",
                    regex: @"\+",
                    orderOfPrecedence: 10,
                    paramaterPositions: new[] { RelativePosition.Left, RelativePosition.Right },
                    expressionBuilder: args => Expression.Add(args[0], args[1])),
                  new OperatorDefinition(
                    name: "SIN",
                    regex: @"sin",
                    orderOfPrecedence: 1,
                    paramaterPositions: new[] { RelativePosition.Right },
                    expressionBuilder: args => Expression.Call(typeof(Math).GetMethod("Sin"), args[0])),
                openBracket = new BracketOpenDefinition(
                    name: "OPENBRACKET",
                    regex: @"\("),
                new BracketCloseDefinition(
                    name: "CLOSEBRACKET",
                    regex: @"\)",
                    bracketOpenDefinition: openBracket),
                new OperandDefinition(
                    name: "NUMBER",
                    regex: @"\d*\.?\d+?",
                    expressionBuilder: x => Expression.Constant(double.Parse(x))),
                new GrammerDefinition(name: "WHITESPACE", regex: @"\s+", ignore: true)
                );

            var expression = language.Parse<double>("sin(1+2)+3");
            Assert.Equal(3.14, expression.Compile()(), 2);
        }

        //[Fact]
        public void Should_run_two_param_functions()
        {
            BracketOpenDefinition openBracket;
            var language = new Language(
                new OperatorDefinition(
                    name: "PLUS",
                    regex: @"\+",
                    orderOfPrecedence: 1,
                    paramaterPositions: new[] { RelativePosition.Left, RelativePosition.Right },
                    expressionBuilder: args => Expression.Add(args[0], args[1])),
                  new OperatorDefinition(
                    name: "LOG",
                    regex: @"[Ll]og",
                    orderOfPrecedence: 10,
                    paramaterPositions: new[] { RelativePosition.Right, RelativePosition.Right },
                    expressionBuilder: args => Expression.Call(typeof(Math).GetMethod("Log"), args)),
                openBracket = new BracketOpenDefinition(
                    name: "OPENBRACKET",
                    regex: @"\("),
                new BracketCloseDefinition(
                    name: "CLOSEBRACKET",
                    regex: @"\)",
                    bracketOpenDefinition: openBracket),
                new OperandDefinition(
                    name: "NUMBER",
                    regex: @"\d*\.?\d+?",
                    expressionBuilder: x => Expression.Constant(double.Parse(x))),
                new GrammerDefinition(name: "WHITESPACE", regex: @"\s+", ignore: true)
                );

            var expression = language.Parse<double>("Log(1024,2) + 5");
            Assert.Equal(15, expression.Compile()());
        }


    }
}
