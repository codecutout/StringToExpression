using StringToExpression.LanguageDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StringToExpression.Test.Languages.Arithmetic
{
    public class ArithmeticLangaugeTests
    {
        [Theory]
        [InlineData("1 + 1", 2)]
        [InlineData("2 + 2 * 5", 12)]
        [InlineData("(2 + 2) * 5", 20)]
        [InlineData("4 - 2 * 5", -6)]
        [InlineData("(4 - 2) * 5", 10)]
        [InlineData("2.5 * 4", 10)]
        [InlineData("2.5 * 3", 7.5)]
        [InlineData("9 / 10", 0.9)]
        [InlineData("10 / 9", 1.111)]
        [InlineData("10 / 5 * 2", 4)]
        [InlineData("10 % 3", 1)]
        [InlineData("Pi", 3.14159)]
        [InlineData("sin(Pi)", 0)]
        [InlineData("Sin(Pi * 1 / 2)", 1)]
        [InlineData("SIN(Pi * 1 / 4)", 0.707)]
        [InlineData("cos(Pi)", -1)]
        [InlineData("Cos(Pi * 1 / 2)", 0)]
        [InlineData("COS(Pi * 1 / 4)", 0.707)]
        [InlineData("tan(Pi)", 0)]
        [InlineData("sqrt(12 * 3)", 6)]
        [InlineData("SQRT(sqrt(81))", 3)]
        [InlineData("Pow(12, 2)", 144)]
        public void When_no_parameters_should_evaluate(string math, decimal result)
        {
            var language = new ArithmeticLanguage();
            var function = language.Parse(math).Compile();

            Assert.Equal(result, function(), 3);

        }
    }
}
