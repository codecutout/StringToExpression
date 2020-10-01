using StringToExpression.Exceptions;
using StringToExpression.LanguageDefinitions;
using StringToExpression.Test.Fixtures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace StringToExpression.Test.Languages.ODataFilter
{
    public class ODataEnumerationTests
    {
        public enum Numbers { One = 1, Two = 2, Three = 3, Five = 5 }

        public class EnumHolder
        {
            public Numbers Number { get; set; }

            public Numbers? NullableNumber { get; set; }

            public string NumberString { get; set; }

            public bool Order { get; set; } = true;
        }


        [Theory]
        [InlineData("Number eq 2", new[] { Numbers.Two })]
        [InlineData("Number ne 2", new[] { Numbers.One, Numbers.Three, Numbers.Five })]
        [InlineData("Number ne 4", new[] { Numbers.One, Numbers.Two, Numbers.Three, Numbers.Five })]
        [InlineData("Number eq 'Two'", new[] { Numbers.Two })]
        [InlineData("Number eq 'two'", new[] { Numbers.Two })]
        [InlineData("Number eq 'tWo'", new[] { Numbers.Two })]
        [InlineData("Number eq toupper('five')", new[] { Numbers.Five })]
        [InlineData("Number eq NullableNumber", new[] { Numbers.One, Numbers.Two, Numbers.Three, Numbers.Five })]
        [InlineData("Number ne NullableNumber", new Numbers[0])]

        [InlineData("2 eq Number", new[] { Numbers.Two })]
        [InlineData("2 ne Number", new[] { Numbers.One, Numbers.Three, Numbers.Five })]
        [InlineData("4 ne Number", new[] { Numbers.One, Numbers.Two, Numbers.Three, Numbers.Five })]
        [InlineData("'Two' eq Number", new[] { Numbers.Two })]
        [InlineData("'two' eq Number", new[] { Numbers.Two })]
        [InlineData("'tWo' eq Number", new[] { Numbers.Two })]
        [InlineData("toupper('five') eq Number", new[] { Numbers.Five })]
        [InlineData("NullableNumber eq Number", new[] { Numbers.One, Numbers.Two, Numbers.Three, Numbers.Five })]
        [InlineData("NullableNumber ne Number", new Numbers[0])]
        [InlineData("order eq false", new[] { Numbers.Five })]
        public void When_filtering_enumeration_should_parse(string query, Numbers[] expectedNumbers)
        {
            var data = new[]{
                new EnumHolder() { Number = Numbers.One, NullableNumber = Numbers.One },
                new EnumHolder() { Number = Numbers.Two, NullableNumber = Numbers.Two },
                new EnumHolder() { Number = Numbers.Three, NullableNumber = Numbers.Three },
                new EnumHolder() { Number = Numbers.Five, NullableNumber = Numbers.Five, Order = false },
            }.AsQueryable();

            var filter = new ODataFilterLanguage().Parse<EnumHolder>(query);
            var filtered = data.Where(filter).ToList();

            var distinct = filtered.Select(x => x.Number).Distinct().ToArray();
            Assert.Equal(expectedNumbers, distinct);
        }

        [Theory]
        [InlineData("NullableNumber eq 2", new[] { Numbers.Two, })]
        [InlineData("NullableNumber ne 2", new[] { Numbers.One, Numbers.Three, Numbers.Five, (Numbers)0 })]
        [InlineData("NullableNumber ne 4", new[] { Numbers.One, Numbers.Two, Numbers.Three, Numbers.Five, (Numbers)0 })]
        [InlineData("NullableNumber eq null", new[] { (Numbers)0 })]
        [InlineData("NullableNumber ne null", new[] { Numbers.One, Numbers.Two, Numbers.Three, Numbers.Five })]
        [InlineData("NullableNumber eq 'Two'", new[] { Numbers.Two, })]
        [InlineData("NullableNumber ne 'tWo'", new[] { Numbers.One, Numbers.Three, Numbers.Five, (Numbers)0 })]

        [InlineData("2 eq NullableNumber", new[] { Numbers.Two, })]
        [InlineData("2 ne NullableNumber", new[] { Numbers.One, Numbers.Three, Numbers.Five, (Numbers)0 })]
        [InlineData("4 ne NullableNumber", new[] { Numbers.One, Numbers.Two, Numbers.Three, Numbers.Five, (Numbers)0 })]
        [InlineData("null eq NullableNumber", new[] { (Numbers)0 })]
        [InlineData("null ne NullableNumber", new[] { Numbers.One, Numbers.Two, Numbers.Three, Numbers.Five })]
        [InlineData("'Two' eq NullableNumber", new[] { Numbers.Two, })]
        [InlineData("'tWo' ne NullableNumber", new[] { Numbers.One, Numbers.Three, Numbers.Five, (Numbers)0 })]

        public void When_filtering_nullable_enumeration_should_parse(string query, Numbers[] expectedNumbers)
        {

            var data = new[]{
                new EnumHolder() { NullableNumber = Numbers.One },
                new EnumHolder() { NullableNumber = Numbers.Two },
                new EnumHolder() { NullableNumber = Numbers.Three },
                new EnumHolder() { NullableNumber = Numbers.Five },
                new EnumHolder() { NullableNumber = null },
            }.AsQueryable();

            var filter = new ODataFilterLanguage().Parse<EnumHolder>(query);
            var filtered = data.Where(filter).ToList();

            //we will treat 0 as null (limit put on us by attributes)
            var expectedNumbersWithNull = expectedNumbers.Select(x => x == (Numbers)0 ? null : (Numbers?)x);

            var distinct = filtered.Select(x => x.NullableNumber).Distinct().ToArray();
            Assert.Equal(expectedNumbersWithNull, distinct);
        }

        [Theory]
        [InlineData("Number eq 'Four'")] //not a valid enum value
        [InlineData("Number eq 2.8")] //double cant be a valid enum
        [InlineData("Number eq null")] //number is not nullable cant be null
        [InlineData("Number eq NumberString")] //we do not support non-constant strings
        public void When_filtering_bad_enum_const_should_error_on_parse(string query)
        {
            var ex = Assert.Throws<OperationInvalidException>(() => new ODataFilterLanguage().Parse<EnumHolder>(query));
        }

    }
}