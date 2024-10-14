using StringToExpression.Exceptions;
using StringToExpression.LanguageDefinitions;
using StringToExpression.Test.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StringToExpression.Test.Languages.ODataFilter
{
    public class ODataTests
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


        [Theory]
        [InlineData("substringof('day', 'Monday') eq true", true)]
        [InlineData("endswith('Monday', 'day') eq true", true)]
        [InlineData("startswith('Monday', 'Mon') eq true", true)]
        [InlineData("length('Monday') eq 6", true)]
        [InlineData("indexof('Monday', 'n') eq 2", true)]
        [InlineData("replace('Monday', 'Mon', 'Satur') eq 'Saturday'", true)]
        [InlineData("substring('Monday', 3) eq 'day'", true)]
        [InlineData("substring('Monday', 3, 2) eq 'da'", true)]
        [InlineData("tolower('Monday') eq 'monday'", true)]
        [InlineData("toupper('Monday') eq 'MONDAY'", true)]
        [InlineData("trim('  Monday  ') eq 'Monday'", true)]
        [InlineData("concat('Mon', 'day') eq 'Monday'", true)]

        [InlineData("day(datetime'2000-01-02T03:04:05') eq 2", true)]
        [InlineData("hour(datetime'2000-01-02T03:04:05') eq 3", true)]
        [InlineData("minute(datetime'2000-01-02T03:04:05') eq 4", true)]
        [InlineData("month(datetime'2000-01-02T03:04:05') eq 1", true)]
        [InlineData("second(datetime'2000-01-02T03:04:05') eq 5", true)]
        [InlineData("year(datetime'2000-01-02T03:04:05') eq 2000", true)]
        
        [InlineData("round(10.4) eq 10", true)]
        [InlineData("round(10.6) eq 11", true)]
        [InlineData("round(10.5) eq 10", true)]
        [InlineData("round(11.5) eq 12", true)]

        [InlineData("floor(10.4) eq 10", true)]
        [InlineData("floor(10.6) eq 10", true)]
        [InlineData("floor(10.5) eq 10", true)]
        [InlineData("floor(11.5) eq 11", true)]

        [InlineData("ceiling(10.4) eq 11", true)]
        [InlineData("ceiling(10.6) eq 11", true)]
        [InlineData("ceiling(10.5) eq 11", true)]
        [InlineData("ceiling(11.5) eq 12", true)]
        public void When_functions_should_evaluate(string query, bool expectedMatch)
        {
            var compiled = new ODataFilterLanguage().Parse<object>(query).Compile();
            Assert.Equal(expectedMatch, compiled(null));

        }

        [Theory]
        [InlineData("(6 and 5) eq 4")]
        [InlineData("(6 or 5) eq 7")]
        [InlineData("'leet' eq 1337")]
        public void When_invalid_operations_should_throw(string query)
        {
            Assert.Throws<OperationInvalidException>(() =>
            {
                new ODataFilterLanguage().Parse<LinqToQuerystringTestDataFixture.ConcreteClass>(query);
            });
        }
    }
}
