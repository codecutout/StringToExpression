using System;

namespace StringParser.Exceptions
{
    /// <summary>
    /// Exception when a grammer definition is configured with an invalid name
    /// </summary>
    public class InvalidGrammerNameException : Exception
    {
        /// <summary>
        /// The name that was invalid
        /// </summary>
        public readonly string TokenDefinitionName;

        public InvalidGrammerNameException(string tokenDefinitionName) : base($"Invalid grammer definition name '{tokenDefinitionName}' name may only contain [a-zA-Z0-9_]")
        {
            TokenDefinitionName = tokenDefinitionName;
        }
    }
}
