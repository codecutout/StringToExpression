using StringToExpression.Exceptions;
using StringToExpression.Parser;
using StringToExpression.Tokenizer;
using System;
using System.Text.RegularExpressions;

namespace StringToExpression.GrammerDefinitions
{
    /// <summary>
    /// Represents a single piece of grammerand defines how it behaves wihtin the system.
    /// </summary>
    public class GrammerDefinition
    {
        private static Regex NameValidation = new Regex("^[a-zA-Z0-9_]+$");

        /// <summary>
        /// Name of the definition.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Regex to match tokens.
        /// </summary>
        public readonly string Regex;

        /// <summary>
        /// Indicates whether this grammer should be ignored during tokenization.
        /// </summary>
        public readonly bool Ignore;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrammerDefinition" /> class.
        /// </summary>
        /// <param name="name">The name of the definition.</param>
        /// <param name="regex">The regex to match tokens.</param>
        /// <param name="ignore">if set to <c>true</c> will ignore grammer during tokenization.</param>
        /// <exception cref="System.ArgumentNullException">
        /// name
        /// or
        /// regex
        /// </exception>
        /// <exception cref="StringToExpression.Exceptions.GrammerDefinitionInvalidNameException">When the name contains characters other than [a-zA-Z0-9_]</exception>
        public GrammerDefinition(string name, string regex, bool ignore = false)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (regex == null)
                throw new ArgumentNullException(nameof(regex));
            if (!NameValidation.IsMatch(name))
                throw new GrammerDefinitionInvalidNameException(name);

            this.Name = name;
            this.Regex = regex;
            this.Ignore = ignore;
        }

        /// <summary>
        /// Applies the token to the parsing state.
        /// </summary>
        /// <param name="token">The token to apply.</param>
        /// <param name="state">The state to apply the token to.</param>
        public virtual void Apply(Token token, ParseState state)
        {

        }
    }
}
