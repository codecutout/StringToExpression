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
                new TokenDefinition(name:"PLUS", regex: @"\+"),
                new TokenDefinition(name:"SUBTRACT", regex: @"\-"),
                new TokenDefinition(name:"NUMBER", regex: @"\d*\.?\d+?"),
                new TokenDefinition(name: "WHITESPACE", regex: @"\s+", ignore: true)
                );

            var tokens = language.Tokenizer.Tokenize("1 + 2 + 3 - 5").ToList();
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
                new TokenDefinition(name:"PLUS", regex: @"\+"),
                new TokenDefinition(name:"SUBTRACT", regex: @"\-"),
                new TokenDefinition(name:"NUMBER", regex: @"\d*\.?\d+?"),
                new TokenDefinition(name: "WHITESPACE", regex: @"\s+", ignore:true)
                );
            var exception = Assert.Throws<InvalidTokenException>(() => language.Tokenizer.Tokenize("1 + 2 * 3 - 5").ToList());
            Assert.Equal("*", exception.Token);
            Assert.Equal(6, exception.SourceMap.Start);
            Assert.Equal(7, exception.SourceMap.End);
        }

        [Fact]
        public void When_skip_should_not_return_token()
        {
            var language = new Language(
                new TokenDefinition(name:"A", regex: @"A"),
                new TokenDefinition(name: "B", regex: @"B", ignore: true)
                );

            var tokens = language.Tokenizer.Tokenize("AABBAA").ToList();
            Assert.Equal(new[] {
                "A","A","A","A"
            }, tokens.Select(x => $"{x.Value}"));
        }

        [Fact]
        public void When_regex_complicated_should_function_correctly()
        {
            var language = new Language(
                new TokenDefinition(name:"LITERAL", regex: @"\'([a-zA-Z0-9]+)\'"),
                new TokenDefinition(name:"AND", regex: @"[Aa][Nn][Dd]"),
                new TokenDefinition(name:"EQ", regex: @"[Ee][Qq]"),
                new TokenDefinition(name:"NUMBER", regex: @"\d*\.?\d+?"),
                new TokenDefinition(name: "WHITESPACE", regex: @"\s+", ignore: true)
                );

            var tokens = language.Tokenizer.Tokenize("1 EQ '1' and 2 eq '2' ").ToList();
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

        [Fact]
        public void When_look_lookahead_should_capture_only_value()
        {
            var language = new Language(
                new TokenDefinition(name:"FUNCTION", regex: @"[a-zA-Z]+(?=\()"),
                new TokenDefinition(name:"WORD", regex: @"[a-zA-Z]+"),
                new TokenDefinition(name:"OPEN_BRACKET", regex: @"\("),
                new TokenDefinition(name:"CLOSER_BRACKET", regex: @"\)"),
                new TokenDefinition(name: "WHITESPACE", regex: @"\s+", ignore: true)
                );

            var tokens = language.Tokenizer.Tokenize("I am some func()").ToList();
            var tokenNames = tokens.Select(x => $"{x.Definition.Name} {x.Value}");

            Assert.Equal(new[] {
                "WORD I",
                "WORD am",
                "WORD some",
                "FUNCTION func",
                "OPEN_BRACKET (",
                "CLOSER_BRACKET )",
            }, tokenNames);
        }

        [Fact]
        public void Should_capture_based_on_order()
        {
            //AB has a higher capture order order than individual A's and B's
            var language1 = new Language(
                new TokenDefinition(name:"AB", regex:"AB"),
                new TokenDefinition(name:"A", regex:"A"),
                new TokenDefinition(name:"B", regex:"B")
                );

            var tokensTypes1 = language1.Tokenizer.Tokenize("AABB")
                .Select(x => $"{x.Definition.Name}")
                .ToList();
            Assert.Equal(new[] {
                "A",
                "AB",
                "B",
            }, tokensTypes1);


            //Individual A's and B's characters have higher capture order than AB
            var language2 = new Language(
               new TokenDefinition(name:"A", regex:"A"),
               new TokenDefinition(name:"B", regex:"B"),
               new TokenDefinition(name:"AB", regex:"AB")
               );

            var tokensTypes2 = language2.Tokenizer.Tokenize("AABB")
                .Select(x => $"{x.Definition.Name}")
                .ToList();
            Assert.Equal(new[] {
                "A",
                "A",
                "B",
                "B",
            }, tokensTypes2);
        }

        [Fact]
        public void When_invalid_definition_name_should_throw()
        {
            var exception = Assert.Throws<InvalidTokenDefinitionNameException>(() => new TokenDefinition(name: "I-am-not a valid name", regex: @"B"));
            Assert.Equal("I-am-not a valid name", exception.TokenDefinitionName);
        }
    }
}
