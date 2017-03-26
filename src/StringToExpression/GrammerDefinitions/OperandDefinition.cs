using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StringToExpression.Parser;
using StringToExpression.Tokenizer;
using StringToExpression.Exceptions;

namespace StringToExpression.GrammerDefinitions
{
    public class OperandDefinition : GrammerDefinition
    {
        public readonly Func<string, ParameterExpression[], Expression> ExpressionBuilder;

        public OperandDefinition(string name, string regex, Func<string, Expression> expressionBuilder)
            : this(name, regex, (v,a)=>expressionBuilder(v))
        {

        }

        public OperandDefinition(string name, string regex, Func<string, ParameterExpression[], Expression> expressionBuilder)
           : base(name, regex)
        {
            if (expressionBuilder == null)
                throw new ArgumentNullException(nameof(expressionBuilder));
            ExpressionBuilder = expressionBuilder;
        }

        public override void Apply(Token token, ParseState state)
        {
            Expression expression;
            try
            {
                expression = ExpressionBuilder(token.Value, state.Parameters.ToArray());
            }
            catch (Exception ex)
            {
                throw new OperationInvalidException(token.SourceMap, ex);
            }
            
            state.Operands.Push(new Operand(expression, token.SourceMap));
        }
    }
}
