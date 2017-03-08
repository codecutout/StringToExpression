using System;

namespace StringParser.Exceptions
{
    /// <summary>
    /// Exception when a token definition is configured with an invalid name
    /// </summary>
    public class InvalidTokenDefinitionNameException : Exception
    {
        /// <summary>
        /// The name that was invalid
        /// </summary>
        public readonly string TokenDefinitionName;

        public InvalidTokenDefinitionNameException(string tokenDefinitionName) : base($"Invalid token definition name '{tokenDefinitionName}' name may only contain [a-zA-Z0-9_]")
        {
            TokenDefinitionName = tokenDefinitionName;
        }
    }
}
