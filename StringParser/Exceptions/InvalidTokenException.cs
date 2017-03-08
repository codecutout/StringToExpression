using StringParser.Util;
using System;

namespace StringParser.Exceptions
{
    /// <summary>
    /// Exception when an unknown token is found
    /// </summary>
    public class InvalidTokenException : Exception
    {
        /// <summary>
        /// The string that was unrecognized
        /// </summary>
        public readonly string Token;

        /// <summary>
        /// Index within the source string where the token was found
        /// </summary>
        public readonly Span SourceMap;

        public InvalidTokenException(string token, Span sourceMap) : base($"Invalid token '{token}' found at index {sourceMap.Start}")
        {
            Token = token;
            SourceMap = sourceMap;
        }
    }
}
