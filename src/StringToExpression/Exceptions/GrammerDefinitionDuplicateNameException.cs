using System;

namespace StringToExpression.Exceptions
{
    /// <summary>
    /// Exception when a mutlple grammers are configured with same name.
    /// </summary>
    public class GrammerDefinitionDuplicateNameException : Exception
    {
        /// <summary>
        /// The name that was duplicated.
        /// </summary>
        public readonly string GrammerDefinitionName;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrammerDefinitionDuplicateNameException"/> class.
        /// </summary>
        /// <param name="grammerDefinitionName">Name of the duplicated grammer definition.</param>
        public GrammerDefinitionDuplicateNameException(string grammerDefinitionName) : base($"Grammer definition name '{grammerDefinitionName}' has been defined multiple times")
        {
            GrammerDefinitionName = grammerDefinitionName;
        }
    }
}
