using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StringParser.Parser;
using StringParser.Tokenizer;
using System.Collections;
using StringParser.Util;
using StringParser.Exceptions;

namespace StringParser.TokenDefinitions
{
    public class UnaryOperatorDefinition : OperatorDefinition
    {
        public UnaryOperatorDefinition(string name,
           string regex,
           int operatorPrecedence,
           RelativePosition operandPosition,
           Func<Expression, Expression> expressionBuilder)
           : base(
            name: name, 
            regex: regex, 
            orderOfPrecedence: operatorPrecedence,
            paramaterPositions: new[] { operandPosition },
            expressionBuilder: param => expressionBuilder(param[0]))
        {
        }
       
    }
}
