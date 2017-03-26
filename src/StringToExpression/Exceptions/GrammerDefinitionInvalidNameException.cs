using System;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when a grammer definition is configured with an invalid name.
    /// </summary>
    public class GrammerDefinitionInvalidNameException : Exception
    {
        /// <summary>
        /// The name that was invalid.
        /// </summary>
        public readonly string GrammerDefinitionName;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrammerDefinitionInvalidNameException"/> class.
        /// </summary>
        /// <param name="grammerDefinitionName">Name of the grammer definition that was invalid.</param>
        public GrammerDefinitionInvalidNameException(string grammerDefinitionName) : base($"Invalid grammer definition name '{grammerDefinitionName}' name may only contain [a-zA-Z0-9_]")
        {
            GrammerDefinitionName = grammerDefinitionName;
        }
    }
}
