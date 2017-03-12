using StringParser.TokenDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace StringParser
{
    /// <summary>
    /// Entry class to converting a string into an expression
    /// </summary>
    public class Language
    {
        /// <summary>
        /// Configuration used to create the language
        /// </summary>
        public IReadOnlyList<GrammerDefinition> TokenDefinitions => Tokenizer.TokenDefinitions;

        /// <summary>
        /// Tokenizer to generate tokens
        /// </summary>
        public readonly Tokenizer.Tokenizer Tokenizer;

        /// <summary>
        /// Parser to convert tokens into an expression
        /// </summary>
        public readonly Parser.Parser Parser;

        public Language(params GrammerDefinition[] tokenDefintions)
        {
            Tokenizer = new Tokenizer.Tokenizer(tokenDefintions);
            Parser = new Parser.Parser();
        }

        /// <summary>
        /// Converts a string into an expression
        /// </summary>
        /// <param name="text">input string</param>
        /// <returns>expression that represents the string</returns>
        protected Expression Parse(string text, params ParameterExpression[] parameters)
        {
            var tokenStream = this.Tokenizer.Tokenize(text);
            var expression = Parser.Parse(tokenStream, parameters);
            return expression;
        }

        public Expression<Func<TOut>> Parse<TOut>(string text)
        {
            var body = this.Parse(text);
            return Expression.Lambda<Func<TOut>>(body);
        }

        public Expression<Func<TIn, TOut>> Parse<TIn, TOut>(string text)
        {
            var parameters = new[] { Expression.Parameter(typeof(TIn)) };
            var body = this.Parse(text, parameters);

            return Expression.Lambda<Func<TIn, TOut>>(body, parameters);
        }
    }
}
