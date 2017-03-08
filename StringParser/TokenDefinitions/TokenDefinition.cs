using StringParser.Exceptions;
using StringParser.Tokenizer;
using System.Text.RegularExpressions;

namespace StringParser.TokenDefinitions
{
    public class TokenDefinition
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

        public TokenDefinition(string name, string regex, bool ignore = false)
        {
            if (!NameValidation.IsMatch(name))
                throw new InvalidTokenDefinitionNameException(name);

            this.Name = name;
            this.Regex = regex;
            this.Ignore = ignore;
        }
    }
}
