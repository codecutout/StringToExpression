using StringParser.TokenDefinitions;
using System.Collections.Generic;
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
        public IReadOnlyList<TokenDefinition> TokenDefinitions => Tokenizer.TokenDefinitions;

        /// <summary>
        /// Tokenizer to generate tokens
        /// </summary>
        public readonly Tokenizer.Tokenizer Tokenizer;

        /// <summary>
        /// Parser to convert tokens into an expression
        /// </summary>
        //public readonly Parser.Parser Parser;

        public Language(params TokenDefinition[] tokenDefintions)
        {
            Tokenizer = new Tokenizer.Tokenizer(tokenDefintions);
            //Parser = new Parser.Parser();
        }

        /// <summary>
        /// Converts a string into an expression
        /// </summary>
        /// <param name="text">input string</param>
        /// <returns>expression that represents the string</returns>
        public Expression Parse(string text)
        {
            var tokenStream = this.Tokenizer.Tokenize(text);
            //TODO: var expression = Parser.Parse(tokenStream);
            return null;
        }
    }
}
