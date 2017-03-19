using StringToExpression.Exceptions;
using StringToExpression.Parser;
using StringToExpression.Tokenizer;
using System.Text.RegularExpressions;

namespace StringToExpression.GrammerDefinitions
{
    /// <summary>
    /// Defines how a single token is behaves wihtin the system
    /// </summary>
    public class GrammerDefinition
    {
        private static Regex NameValidation = new Regex("^[a-zA-Z0-9_]+$");

        /// <summary>
        /// Name of the definition
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Regex to match tokens
        /// </summary>
        public readonly string Regex;

        /// <summary>
        /// Indicates whether this token should be ignored during tokenization
        /// </summary>
        public readonly bool Ignore;

        public GrammerDefinition(string name, string regex, bool ignore = false)
        {
            if (!NameValidation.IsMatch(name))
                throw new GrammerDefinitionInvalidNameException(name);

            this.Name = name;
            this.Regex = regex;
            this.Ignore = ignore;
        }

        /// <summary>
        /// Applies the token to the state
        /// </summary>
        /// <param name="token">token to apply</param>
        /// <param name="state">state to apply the token to</param>
        public virtual void Apply(Token token, ParseState state)
        {

        }
    }
}
