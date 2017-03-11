using StringParser.Exceptions;
using StringParser.TokenDefinitions;
using StringParser.Util;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace StringParser.Tokenizer
{
    public class Tokenizer
    {
        /// <summary>
        /// Configuration of the tokens
        /// </summary>
        public readonly IReadOnlyList<GrammerDefinition> TokenDefinitions;

        /// <summary>
        /// Regex to identify tokens
        /// </summary>
        protected readonly Regex TokenRegex;

        public Tokenizer(params GrammerDefinition[] tokenDefintions)
        {
            TokenDefinitions = tokenDefintions.ToList().AsReadOnly();
            var pattern = string.Join("|", TokenDefinitions.Select(x => $"(?<{x.Name}>{x.Regex})"));
            this.TokenRegex = new Regex(pattern);
        }

        /// <summary>
        /// Convert text into a stream of tokens
        /// </summary>
        /// <param name="text">text to tokenize</param>
        /// <returns>stream of tokens</returns>
        public IEnumerable<Token> Tokenize(string text)
        {
            var matches = TokenRegex.Matches(text).OfType<Match>();

            var expectedIndex = 0;
            foreach (var match in matches)
            {
                if (match.Index > expectedIndex)
                    throw new TokenUnexpectedException(new StringSegment(text, expectedIndex, match.Index - expectedIndex));
                expectedIndex = match.Index + match.Length;

                var matchedTokenDefinition = TokenDefinitions.FirstOrDefault(x => match.Groups[x.Name].Success);
                if (matchedTokenDefinition.Ignore)
                    continue;

                yield return new Token(
                    definition: matchedTokenDefinition,
                    value: match.Value,
                    sourceMap: new StringSegment(text, match.Index, match.Length));
            };

        }
    }
}
