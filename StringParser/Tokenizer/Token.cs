using StringParser.TokenDefinitions;
using StringParser.Util;
using System;

namespace StringParser.Tokenizer
{
    /// <summary>
    /// An indivdual piece of the complete input
    /// </summary>
    public class Token
    {
        /// <summary>
        /// The Type of token and how it is defined
        /// </summary>
        public readonly TokenDefinition Definition;

        /// <summary>
        /// The value of the token
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// Where this token is relative to other tokens from the same input
        /// </summary>
        public readonly int TokenIndex;

        /// <summary>
        /// Where from the original input was this token sourced
        /// </summary>
        public readonly Span SourceMap;

        public Token(TokenDefinition definition, string value, int tokenIndex, Span sourceMap)
        {
            this.Definition = definition;
            this.Value = value;
            this.TokenIndex = tokenIndex;
            this.SourceMap = sourceMap;
        }
    }
}
