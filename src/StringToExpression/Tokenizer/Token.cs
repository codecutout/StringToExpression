using StringToExpression.GrammerDefinitions;
using StringToExpression.Util;
using System;

namespace StringToExpression.Tokenizer
{
    /// <summary>
    /// An indivdual piece of the complete input.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// The type of token and how it is defined.
        /// </summary>
        public readonly GrammerDefinition Definition;

        /// <summary>
        /// The value stored within the token.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// The original string and position this token was extracted from.
        /// </summary>
        public readonly StringSegment SourceMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="definition">The type of token and how it was defined.</param>
        /// <param name="value">The value stored within the token.</param>
        /// <param name="sourceMap">The original string and position this token was extracted from.</param>
        public Token(GrammerDefinition definition, string value, StringSegment sourceMap)
        {
            this.Definition = definition;
            this.Value = value;
            this.SourceMap = sourceMap;
        }
    }
}
