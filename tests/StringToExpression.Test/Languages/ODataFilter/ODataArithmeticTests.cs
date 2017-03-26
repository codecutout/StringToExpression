using StringToExpression.LanguageDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StringToExpression.Test.Languages.ODataFilter
{
    public class ODataArithmeticTests
    {
        [Theory]
        [InlineData("(1 add 1) eq 2")]
        [InlineData("(2 add 2 mul 5) eq 12")]
        [InlineData("((2 add 2) mul 5) eq 20")]
        [InlineData("(4 sub 2 mul 5) eq -6")]
        [InlineData("((4 sub 2) mul 5) eq 10")]
        [InlineData("(2.5 mul 4) eq 10")]
        [InlineData("(2.5 mul 3) eq 7.5")]
        [InlineData("(9m div 10) eq 0.9")]
        [InlineData("(22.5 div 9) eq 2.5")]
        [InlineData("(10 div 5 mul 2) eq 4")]
        public void When_arithmetic_should_evaluate(string query)
        {
            var filter = new ODataFilterLanguage().Parse<object>(query);
            var stringParserCompiled = filter.Compile();
            Assert.True(stringParserCompiled(null));
        }
    }
}
