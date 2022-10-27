using System;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when a collection accessor pattern as been defined multiple times.
    /// </summary>
    public class AccessorDefinitionDuplicatePatternException : Exception
    {
        /// <summary>
        /// The pattern that was duplicated
        /// </summary>
        public readonly string Pattern;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessorDefinitionDuplicatePatternException"/> class.
        /// </summary>
        /// <param name="pattern">Pattern that has been duplicated.</param>
        public AccessorDefinitionDuplicatePatternException(string pattern) : base($"Pattern '{pattern}' has been defined multiple times")
        {
            Pattern = pattern;
        }
    }
}
