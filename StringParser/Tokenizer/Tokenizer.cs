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
        public readonly IReadOnlyList<TokenDefinition> TokenDefinitions;

        public Tokenizer(params TokenDefinition[] tokenDefintions)
        {
            TokenDefinitions = tokenDefintions.ToList().AsReadOnly();
        }

        /// <summary>
        /// Convert text into a stream of tokens
        /// </summary>
        /// <param name="text">text to tokenize</param>
        /// <returns>stream of tokens</returns>
        public IEnumerable<Token> Tokenize(string text)
        {
            var pattern = string.Join("|", TokenDefinitions.Select(x => $"(?<{x.Name}>{x.Regex})"));
            var regex = new Regex(pattern);

            var test = regex.GroupNameFromNumber(1);
            var matches = regex.Matches(text).OfType<Match>();

            var tokenIndex = 0;
            var expectedIndex = 0;
            foreach (var match in matches)
            {
                if (match.Index > expectedIndex)
                    throw new InvalidTokenException(text.Substring(expectedIndex, match.Index - expectedIndex), new Span(expectedIndex, match.Index));
                expectedIndex = match.Index + match.Length;

                var matchedTokenDefinition = TokenDefinitions.FirstOrDefault(x => match.Groups[x.Name].Success);
                if (matchedTokenDefinition.Ignore)
                    continue;

                yield return new Token(
                    definition: matchedTokenDefinition,
                    value: match.Value,
                    tokenIndex: tokenIndex++,
                    sourceMap: new Span(match.Index, match.Index + match.Length));
            };

        }
    }
}
