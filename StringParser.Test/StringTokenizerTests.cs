using StringParser.Exceptions;
using StringParser.TokenDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace StringParser.Test
{
    public class StringTokenizerTests
    {
        [Fact]
        public void Should_tokenize_with_definition_and_value()
        {
            var language = new Language(
                new TokenDefinition() { Name = "PLUS", Regex = @"\+" },
                new TokenDefinition() { Name = "SUBTRACT", Regex = @"\-" },
                new TokenDefinition() { Name = "NUMBER", Regex = @"\d*\.?\d+?" },
                new TokenDefinition() { Name = "WHITESPACE", Regex = @"\s+", Skip = true }
                );

            var tokens = language.Tokenize("1 + 2 + 3 - 5").ToList();
            var tokenNames = tokens.Select(x => $"{x.Definition.Name} {x.Value}");

            Assert.Equal(new[] {
                "NUMBER 1",
                "PLUS +",
                "NUMBER 2",
                "PLUS +",
                "NUMBER 3",
                "SUBTRACT -",
                "NUMBER 5",
            },tokenNames);
        }

        [Fact]
        public void When_unknown_token_should_throw()
        {
            var language = new Language(
                new TokenDefinition() { Name = "PLUS", Regex = @"\+" },
                new TokenDefinition() { Name = "SUBTRACT", Regex = @"\-" },
                new TokenDefinition() { Name = "NUMBER", Regex = @"\d*\.?\d+?" },
                new TokenDefinition() { Name = "WHITESPACE", Regex = @"\s+", Skip = true }
                );
            var exception = Assert.Throws<InvalidTokenException>(() => language.Tokenize("1 + 2 * 3 - 5").ToList());
            Assert.Equal("*", exception.Token);
            Assert.Equal(6, exception.Index);
        }

        [Fact]
        public void When_skip_should_not_return_token()
        {
            var language = new Language(
                new TokenDefinition() { Name = "A", Regex = @"A" },
                new TokenDefinition() { Name = "B", Regex = @"B", Skip = true }

                );

            var tokens = language.Tokenize("AABBAA").ToList();
            Assert.Equal(new[] {
                "A","A","A","A"
            }, tokens.Select(x => $"{x.Value}"));
        }

        [Fact]
        public void When_regex_complicated_should_function_correctly()
        {
            var language = new Language(
                new TokenDefinition() { Name = "LITERAL", Regex = @"\'([a-zA-Z0-9]+)\'" },
                new TokenDefinition() { Name = "AND", Regex = @"[Aa][Nn][Dd]" },
                new TokenDefinition() { Name = "EQ", Regex = @"[Ee][Qq]" },
                new TokenDefinition() { Name = "NUMBER", Regex = @"\d*\.?\d+?" },
                new TokenDefinition() { Name = "WHITESPACE", Regex = @"\s+", Skip = true }
                );

            var tokens = language.Tokenize("1 EQ '1' and 2 eq '2' ").ToList();
            var tokenNames = tokens.Select(x => $"{x.Definition.Name} {x.Value}");


            Assert.Equal(new[] {
                "NUMBER 1",
                "EQ EQ",
                "LITERAL '1'",
                "AND and",
                "NUMBER 2",
                "EQ eq",
                "LITERAL '2'",
            }, tokenNames);
        }
    }
}
