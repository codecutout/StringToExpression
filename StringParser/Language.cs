using StringParser.Exceptions;
using StringParser.TokenDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StringParser
{
    public class Language
    {
        public class Token
        {
            public TokenDefinition Definition { get; set; }

            public string Value { get; set; }

            public int Index { get; set; }
        }


        public IReadOnlyList<TokenDefinition> TokenDefinitions { get; private set; }

        public Language(params TokenDefinition[] tokenDefintions)
        {
            TokenDefinitions = tokenDefintions.ToList().AsReadOnly();
        }

        internal IEnumerable<Token> Tokenize(string text)
        {
            var pattern = string.Join("|", TokenDefinitions.Select(x => $"(?<{x.Name}>{x.Regex})"));
            var regex = new Regex(pattern);

            var test = regex.GroupNameFromNumber(1);
            var matches = regex.Matches(text).OfType<Match>();

            var expectedIndex = 0;
            foreach (var match in matches)
            {
                if (match.Index > expectedIndex)
                    throw new InvalidTokenException(expectedIndex, text.Substring(expectedIndex, match.Index - expectedIndex));
                expectedIndex = match.Index + match.Length;

                var matchedTokenDefinition = TokenDefinitions.FirstOrDefault(x => match.Groups[x.Name].Success);
                if (matchedTokenDefinition.Skip)
                    continue;

                yield return new Token
                {
                    Definition = matchedTokenDefinition,
                    Value = match.Value,
                    Index = match.Index
                };
                
            }
        }

        public Expression Parse(string text)
        {
            return null;
        }
    }

   
}
