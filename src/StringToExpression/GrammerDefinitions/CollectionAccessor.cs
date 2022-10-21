using StringToExpression.Exceptions;
using StringToExpression.Parser;
using StringToExpression.Tokenizer;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace StringToExpression.GrammerDefinitions
{
    public class CollectionAccessor : GrammerDefinition
    {
        public readonly Func<string, Operand[], ParameterExpression[], Expression> ExpressionBuilder;


        public CollectionAccessor(string name, string regex)
            : base(name, regex)
        {

        }

        public override void Apply(Token token, ParseState state)
        {
            var tokenValue = token.Value.Replace(":", "");
            var genType = state.Operands.ToArray().FirstOrDefault().Expression.Type.GenericTypeArguments.FirstOrDefault();

            var paramExpression = Expression.Parameter(genType, tokenValue);

            if (!state.Parameters.Select(p => p.Pattern).Contains(tokenValue))
            {
                state.Parameters.Add(new Accessor(paramExpression, tokenValue));
            }
            else
            {
                throw new AccessorDefinitionDuplicatePatternException(tokenValue);
            }
        }
    }
}
