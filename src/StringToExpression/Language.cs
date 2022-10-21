using StringToExpression.GrammerDefinitions;
using StringToExpression.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace StringToExpression
{
    /// <summary>
    /// Entry class to converting a string into an expression.
    /// </summary>
    public class Language
    {
        /// <summary>
        /// Configuration used to create the language.
        /// </summary>
        public IReadOnlyList<GrammerDefinition> TokenDefinitions => Tokenizer.GrammerDefinitions;

        /// <summary>
        /// Tokenizer to generate tokens.
        /// </summary>
        public readonly Tokenizer.Tokenizer Tokenizer;

        /// <summary>
        /// Parser to convert tokens into an expression.
        /// </summary>
        public readonly Parser.Parser Parser;

        /// <summary>
        /// Initializes a new instance of the <see cref="Language"/> class.
        /// </summary>
        /// <param name="grammerDefintions">The configuration for this language.</param>
        public Language(params GrammerDefinition[] grammerDefintions)
        {
            Tokenizer = new Tokenizer.Tokenizer(grammerDefintions);
            Parser = new Parser.Parser();
        }

        /// <summary>
        /// Converts a string into an expression.
        /// </summary>
        /// <param name="text">The input string.</param>
        /// <param name="parameters">The parameters that are accessible by the expression.</param>
        /// <returns>
        /// expression that represents the input string.
        /// </returns>
        public Expression Parse(string text, params Accessor[] parameters)
        {
            var tokenStream = this.Tokenizer.Tokenize(text);
            var expression = Parser.Parse(tokenStream, parameters);
            return expression;
        }

        /// <summary>
        /// Converts a string into a function expression.
        /// </summary>
        /// <typeparam name="TOut">The output of the function.</typeparam>
        /// <param name="text">The input string.</param>
        /// <returns>
        /// expression that represents the input string.
        /// </returns>
        public Expression<Func<TOut>> Parse<TOut>(string text)
        {
            var body = this.Parse(text);
            return Expression.Lambda<Func<TOut>>(body);
        }

        /// <summary>
        /// Converts a string into a function expression.
        /// </summary>
        /// <typeparam name="TIn">The input type of the function.</typeparam>
        /// <typeparam name="TOut">The output type of the function.</typeparam>
        /// <param name="text">The input string.</param>
        /// <returns>
        /// expression that represents the input string.
        /// </returns>
        public Expression<Func<TIn, TOut>> Parse<TIn, TOut>(string text)
        {
            var parameters = new[] { (Accessor)Expression.Parameter(typeof(TIn)) };
            var body = this.Parse(text, parameters);

            return Expression.Lambda<Func<TIn, TOut>>(body, parameters.Select(p => p.Expression));
        }
    }
}
