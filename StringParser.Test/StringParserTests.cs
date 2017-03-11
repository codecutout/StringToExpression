using StringParser.Exceptions;
using StringParser.TokenDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace StringParser.Test
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
           
            var expression = language.ParseFunc<decimal>("1 + 2 + 3 + 5");
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

            var exception = Assert.Throws<OperandExpectedException>(() => language.ParseFunc<decimal>("1 + + 5"));
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

            var exception = Assert.Throws<OperandUnexpectedException>(() => language.ParseFunc<decimal>("1 + 5 5"));
            Assert.Equal("1 [+] 5 5", exception.OperatorStringSegment.Highlight());
            Assert.Equal("1 + 5 [5]", exception.UnexpectedOperandStringSegment.Highlight());
        }


    }
}
