using LinqToQuerystring;
using StringToExpression.Exceptions;
using StringToExpression.LanguageDefinitions;
using StringToExpression.Test.Fixtures;
using StringToExpression.GrammerDefinitions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace StringToExpression.Test
{
    public class LinqToQueryStringEquivalenceTests : IClassFixture<LinqToQuerystringTestDataFixture>
    {
        LinqToQuerystringTestDataFixture Data;

        public LinqToQueryStringEquivalenceTests(LinqToQuerystringTestDataFixture data)
        {
            this.Data = data;
        }

        [Theory]
        [InlineData("Complete")]
        [InlineData("not Complete")]
        [InlineData("Name eq 'Apple'")]
        [InlineData("'Apple' eq Name")]
        [InlineData("not Name eq 'Apple'")]
        [InlineData("Name ne 'Apple'")]
        [InlineData("not Name ne 'Apple'")]
        [InlineData(@"not Name ne 'Apple'")]
        [InlineData("Age eq 4")]
        [InlineData("Age gt -4")]
        [InlineData("not Age eq 4")]
        [InlineData("Age ne 4")]
        [InlineData("not Age ne 4")]
        [InlineData("Age gt 3")]
        [InlineData("not Age gt 3")]
        [InlineData("Age ge 3")]
        [InlineData("not Age ge 3")]
        [InlineData("Age lt 3")]
        [InlineData("not Age lt 3")]
        [InlineData("Age le 3")]
        [InlineData("not Age le 3")]
        [InlineData("Population eq 40000000000L")]
        [InlineData("Population gt -40000000000L")]
        [InlineData("not Population eq 40000000000L")]
        [InlineData("Population ne 40000000000L")]
        [InlineData("not Population ne 40000000000L")]
        [InlineData("Population gt 30000000000L")]
        [InlineData("not Population gt 30000000000L")]
        [InlineData("Population ge 30000000000L")]
        [InlineData("not Population ge 30000000000L")]
        [InlineData("Population lt 30000000000L")]
        [InlineData("not Population lt 30000000000L")]
        [InlineData("Population le 30000000000L")]
        [InlineData("not Population le 30000000000L")]
        [InlineData("Code eq 34")]
        [InlineData("Code eq 0x22")]
        [InlineData("not Code eq 0x22")]
        [InlineData("Code ne 0x22")]
        [InlineData("not Code ne 0x22")]
        [InlineData("Code gt 0xCC")]
        [InlineData("not Code gt 0xCC")]
        [InlineData("Code ge 0xCC")]
        [InlineData("not Code ge 0xCC")]
        [InlineData("Code lt 0xCC")]
        [InlineData("not Code lt 0xCC")]
        [InlineData("Code le 0xCC")]
        [InlineData("not Code le 0xCC")]
        [InlineData("Guid eq guid'" + LinqToQuerystringTestDataFixture.guid1 + "'")]
        [InlineData("not Guid eq guid'" + LinqToQuerystringTestDataFixture.guid1 + "'")]
        [InlineData("Guid ne guid'" + LinqToQuerystringTestDataFixture.guid1 + "'")]
        [InlineData("not Guid ne guid'" + LinqToQuerystringTestDataFixture.guid1 + "'")]
        [InlineData("Cost eq 444.444f")]
        [InlineData("Cost gt -444.444f")]
        [InlineData("not Cost eq 444.444f")]
        [InlineData("Cost ne 444.444f")]
        [InlineData("not Cost ne 444.444f")]
        [InlineData("Cost gt 333.333f")]
        [InlineData("not Cost gt 333.333f")]
        [InlineData("Cost ge 333.333f")]
        [InlineData("not Cost ge 333.333f")]
        [InlineData("Cost lt 333.333f")]
        [InlineData("not Cost lt 333.333f")]
        [InlineData("Cost le 333.333f")]
        [InlineData("not Cost le 333.333f")]
        [InlineData("Value eq 444.444")]
        [InlineData("Cost gt -444.444")]
        [InlineData("not Value eq 444.444")]
        [InlineData("Value ne 444.444")]
        [InlineData("not Value ne 444.444")]
        [InlineData("Value gt 333.333")]
        [InlineData("not Value gt 333.333")]
        [InlineData("Value ge 333.333")]
        [InlineData("Value ge 333d")]
        [InlineData("not Value ge 333.333")]
        [InlineData("Value lt 333.333")]
        [InlineData("not Value lt 333.333")]
        [InlineData("Value le 333.333")]
        [InlineData("not Value le 333.333")]
        [InlineData("Score eq 0.4m")]
        [InlineData("Score eq 0.4")]
        [InlineData("Score eq 0.4m and Value eq 444.444")]
        [InlineData("Score gt -0.4m")]
        [InlineData("not Score eq 0.4m")]
        [InlineData("Score ne 0.4m")]
        [InlineData("not Score ne 0.4m")]
        [InlineData("Score gt 0.3m")]
        [InlineData("not Score gt 0.3m")]
        [InlineData("Score ge 0.3m")]
        [InlineData("not Score ge 0.3m")]
        [InlineData("Score lt 0.3m")]
        [InlineData("not Score lt 0.3m")]
        [InlineData("Score le 0.3m")]
        [InlineData("not Score le 0.3m")]
        [InlineData("Date eq datetime'2002-01-01T00:00'")]
        [InlineData("not Date eq datetime'2002-01-01T00:00'")]
        [InlineData("Date ne datetime'2002-01-01T00:00'")]
        [InlineData("not Date ne datetime'2002-01-01T00:00'")]
        [InlineData("Date gt datetime'2003-01-01T00:00'")]
        [InlineData("not Date gt datetime'2003-01-01T00:00'")]
        [InlineData("Date ge datetime'2003-01-01T00:00'")]
        [InlineData("not Date ge datetime'2003-01-01T00:00'")]
        [InlineData("Date lt datetime'2003-01-01T00:00'")]
        [InlineData("not Date lt datetime'2003-01-01T00:00'")]
        [InlineData("Date le datetime'2003-01-01T00:00'")]
        [InlineData("not Date le datetime'2003-01-01T00:00'")]
        [InlineData("Complete eq true")]
        [InlineData("not Complete eq true")]
        [InlineData("Complete ne true")]
        [InlineData("not Complete ne true")]
        [InlineData("Name eq 'Custard' and Age ge 2")]
        [InlineData("Name eq 'Custard' and not Age lt 2")]
        [InlineData("Name eq 'Banana' or Date gt datetime'2003-01-01T00:00'")]
        [InlineData("Name eq 'Banana' or not Date le datetime'2003-01-01T00:00'")]
        [InlineData("Name eq 'Apple' and Complete eq true or Date gt datetime'2003-01-01T00:00'")]
        [InlineData("Name eq 'Apple' and Complete eq true or not Date le datetime'2003-01-01T00:00'")]
        [InlineData("Name eq 'Apple' and (Complete eq true or Date gt datetime'2003-01-01T00:00')")]
        [InlineData("not (Name eq 'Apple' and (Complete eq true or Date gt datetime'2003-01-01T00:00'))")]
        public void When_concrete_data_should_return_same_results_as_linqToQuerystring(string query)
        {
            var linqToQuerystringFiltered = Data.ConcreteCollection.LinqToQuerystring("?$filter=" + query).ToList();

            var filter = new ODataFilterLanguage().Parse<LinqToQuerystringTestDataFixture.ConcreteClass>(query);
            var stringParserFiltered = Data.ConcreteCollection.Where(filter).ToList();

            Assert.Equal(linqToQuerystringFiltered, stringParserFiltered);
        }

        [Theory]
        [InlineData(@"Name eq 'Apple\\Bob'")]
        [InlineData(@"Name eq 'Apple\bBob'")]
        [InlineData(@"Name eq 'Apple\tBob'")]
        [InlineData(@"Name eq 'Apple\nBob'")]
        [InlineData(@"Name eq 'Apple\fBob'")]
        [InlineData(@"Name eq 'Apple\rBob'")]
        [InlineData(@"Name eq 'Apple""Bob'")]
        [InlineData(@"Name eq 'Apple\'Bob'")]
        public void When_edgecase_data_should_return_same_results_as_linqToQuerystring(string query)
        {

            var linqToQuerystringFiltered = Data.EdgeCaseCollection.LinqToQuerystring("?$filter=" + query).ToList();

            var filter = new ODataFilterLanguage().Parse<LinqToQuerystringTestDataFixture.ConcreteClass>(query);
            var stringParserFiltered = Data.EdgeCaseCollection.Where(filter).ToList();

            Assert.Equal(linqToQuerystringFiltered, stringParserFiltered);
        }

        [Theory]
        [InlineData("Age eq 1")]
        [InlineData("1 eq Age")]
        [InlineData("Age ne 1")]
        [InlineData("1 ne Age")]
        [InlineData("Age gt 0")]
        [InlineData("2 gt Age")]
        [InlineData("Age ge 1")]
        [InlineData("1 ge Age")]
        [InlineData("Age lt 2")]
        [InlineData("0 lt Age")]
        [InlineData("Age le 1")]
        [InlineData("1 le Age")]
        [InlineData("Age eq null")]
        [InlineData("null eq Age")]
        [InlineData("Age ne null")]
        [InlineData("null ne Age")]
        [InlineData("Age gt null")]
        [InlineData("null gt Age")]
        [InlineData("Age ge null")]
        [InlineData("null ge Age")]
        [InlineData("Age lt null")]
        [InlineData("null lt Age")]
        [InlineData("Age le null")]
        [InlineData("null le Age")]
        [InlineData("Date eq datetime'2002-01-01T00:00'")]
        [InlineData("Complete eq true")]
        [InlineData("Complete")]
        [InlineData("Complete eq false")]
        [InlineData("not Complete eq true")]
        [InlineData("not Complete")]
        [InlineData("not Complete eq false")]
        [InlineData("Population eq 10000000000L")]
        [InlineData("Value eq 111.111")]
        [InlineData("Cost eq 111.111f")]
        [InlineData("Code eq 0x00")]
        [InlineData("Guid eq guid'" + LinqToQuerystringTestDataFixture.guid0 + "'")]
        [InlineData("Name eq null")]
        [InlineData("null eq Name")]
        [InlineData("Name ne null")]
        [InlineData("null ne Name")]
        public void When_nullable_data_should_return_same_results_as_linqToQuerystring(string query)
        {
            var linqToQuerystringFiltered = Data.NullableCollection.LinqToQuerystring("?$filter=" + query).ToList();

            var filter = new ODataFilterLanguage().Parse<LinqToQuerystringTestDataFixture.NullableClass>(query);
            var stringParserFiltered = Data.NullableCollection.Where(filter).ToList();

            Assert.Equal(linqToQuerystringFiltered, stringParserFiltered);
        }

        [Theory]
        [InlineData("startswith(Name,'Sat')")]
        [InlineData("endswith(Name,'day')")]
        [InlineData("substringof('urn',Name)")]
        [InlineData("(substringof('Mond',Name)) or (substringof('Tues',Name))")]
        [InlineData(@"substringof('sat',tolower(Name))")]
        [InlineData(@"substringof('SAT',toupper(Name))")]
        [InlineData(@"year(Date) eq 2005")]
        [InlineData(@"month(Date) eq 6")]
        [InlineData(@"day(Date) eq 2")]
        [InlineData(@"hour(Date) eq 10")]
        [InlineData(@"minute(Date) eq 20")]
        [InlineData(@"second(Date) eq 50")]
        public void When_functions_return_same_results_as_linqToQuerystring(string query)
        {
            var linqToQuerystringFiltered = Data.FunctionConcreteCollection.LinqToQuerystring("?$filter=" + query).ToList();

            var filter = new ODataFilterLanguage().Parse<LinqToQuerystringTestDataFixture.ConcreteClass>(query);
            var stringParserFiltered = Data.FunctionConcreteCollection.Where(filter).ToList();

            Assert.Equal(linqToQuerystringFiltered, stringParserFiltered);
        }


        [Theory]
        [InlineData("concrete/complete")]
        [InlineData("substringof('a',concrete/name)")]
        public void When_property_paths_results_as_linqToQuerystring(string query)
        {
            var linqToQuerystringFiltered = Data.ComplexCollection.LinqToQuerystring("?$filter=" + query).ToList();

            var filter = new ODataFilterLanguage().Parse<LinqToQuerystringTestDataFixture.ComplexClass>(query);
            var stringParserFiltered = Data.ComplexCollection.Where(filter).ToList();

            Assert.Equal(linqToQuerystringFiltered, stringParserFiltered);
        }



        [Fact(Skip ="Performance sanity check.")]
        public void Should_be_faster_than_linqToQuerystring()
        {
            var baseDatetime = new DateTime(2003, 01, 01);

            var linqToQueryStringStopwatch = new Stopwatch();
            linqToQueryStringStopwatch.Start();
            for(int i = 0; i < 10000; i++)
            {
                var date = baseDatetime.AddDays(i).ToString("s");
                var linqToQuerystringFiltered = Data.ConcreteCollection.LinqToQuerystring($"?$filter=Name eq 'Apple' and (Complete eq true or Date gt datetime'{date}')");
            }
            linqToQueryStringStopwatch.Stop();


           
            var parseStringStopwatch = new Stopwatch();
            parseStringStopwatch.Start();
            var language = new ODataFilterLanguage();
            for (int i = 0; i < 10000; i++)
            {
                var date = baseDatetime.AddDays(i).ToString("s");
                var filter = language.Parse<LinqToQuerystringTestDataFixture.ConcreteClass>($"Name eq 'Apple' and (Complete eq true or Date gt datetime'{date}')");
            }
            parseStringStopwatch.Stop();
            Assert.True(parseStringStopwatch.ElapsedMilliseconds < linqToQueryStringStopwatch.ElapsedMilliseconds);
        }

    }
}
