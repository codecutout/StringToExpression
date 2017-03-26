using StringToExpression.GrammerDefinitions;
using StringToExpression.Util;
using System;

namespace StringToExpression.Tokenizer
{
    /// <summary>
    /// An indivdual piece of the complete input
    /// </summary>
    public class Token
    {
        /// <summary>
        /// The Type of token and how it is defined
        /// </summary>
        public readonly GrammerDefinition Definition;

        /// <summary>
        /// The value of the token
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// Where from the original input was this token sourced
        /// </summary>
        public readonly StringSegment SourceMap;

        public Token(GrammerDefinition definition, string value, StringSegment sourceMap)
        {
            this.Definition = definition;
            this.Value = value;
            this.SourceMap = sourceMap;
        }
    }
}
