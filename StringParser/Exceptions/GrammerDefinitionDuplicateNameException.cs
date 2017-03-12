using System;

namespace StringParser.Exceptions
{
    /// <summary>
    /// Exception when a grammer definition is configured with an invalid name
    /// </summary>
    public class GrammerDefinitionDuplicateNameException : Exception
    {
        /// <summary>
        /// The name that was duplicated
        /// </summary>
        public readonly string GrammerDefinitionName;

        public GrammerDefinitionDuplicateNameException(string grammerDefinitionName) : base($"Grammer definition name '{grammerDefinitionName}' has been defined multiple times")
        {
            GrammerDefinitionName = grammerDefinitionName;
        }
    }
}
