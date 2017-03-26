using StringToExpression.GrammerDefinitions;
using StringToExpression.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StringToExpression.LanguageDefinitions
{
    public class ArithmeticLanguage
    {
        private readonly Language language;

        public ArithmeticLanguage() {
            language = new Language(AllDefinitions().ToArray());
        }

        public Expression<Func<decimal>> Parse(string text)
        {
            var body = language.Parse(text);
            body = ExpressionConversions.Convert(body, typeof(decimal));
            return Expression.Lambda<Func<decimal>>(body);
        }

        public Expression<Func<T, decimal>> Parse<T>(string text)
        {
            var parameters = new[] { Expression.Parameter(typeof(T)) };
            var body = language.Parse(text, parameters);
            body = ExpressionConversions.Convert(body, typeof(decimal));
            return Expression.Lambda<Func<T, decimal>>(body, parameters);
        }

        protected virtual IEnumerable<GrammerDefinition> AllDefinitions()
        {
            IEnumerable<FunctionCallDefinition> functions;
            var definitions = new List<GrammerDefinition>();
            definitions.AddRange(TypeDefinitions());
            definitions.AddRange(functions = FunctionDefinitions());
            definitions.AddRange(BracketDefinitions(functions));
            definitions.AddRange(ArithmaticOperatorDefinitions());
            definitions.AddRange(PropertyDefinitions());
            definitions.AddRange(WhitespaceDefinitions());
            return definitions;
        }

        protected virtual IEnumerable<GrammerDefinition> TypeDefinitions()
        {
            return new[]
            {
                //Only have decimal to make things easier for casting
                new OperandDefinition(
                    name:"DECIMAL",
                    regex: @"\-?\d+(\.\d+)?",
                    expressionBuilder: x => Expression.Constant(decimal.Parse(x))),
                 new OperandDefinition(
                    name:"PI",
                    regex: @"[Pp][Ii]",
                    expressionBuilder: x => Expression.Constant(Math.PI)),
            };
        }

        

        protected virtual IEnumerable<GrammerDefinition> ArithmaticOperatorDefinitions()
        {
            return new[]
            {
                 new BinaryOperatorDefinition(
                    name:"ADD",
                    regex: @"\+",
                    orderOfPrecedence: 2,
                    expressionBuilder: (left,right) => Expression.Add(left, right)),
                new BinaryOperatorDefinition(
                    name:"SUB",
                    regex: @"\-",
                    orderOfPrecedence: 2,
                    expressionBuilder: (left,right) => Expression.Subtract(left, right)),
                new BinaryOperatorDefinition(
                    name:"MUL",
                    regex: @"\*",
                    orderOfPrecedence: 1,
                    expressionBuilder: (left,right) => Expression.Multiply(left, right)),
                new BinaryOperatorDefinition(
                    name:"DIV",
                    regex: @"\/",
                    orderOfPrecedence: 1,
                    expressionBuilder: (left,right) => Expression.Divide(left, right)),
                new BinaryOperatorDefinition(
                    name:"MOD",
                    regex: @"%",
                    orderOfPrecedence: 1,
                    expressionBuilder: (left,right) => Expression.Modulo(left, right)),
            };
        }

        protected virtual IEnumerable<GrammerDefinition> BracketDefinitions(IEnumerable<FunctionCallDefinition> functionCalls)
        {
            BracketOpenDefinition openBracket;
            BracketOpenDefinition openFunctionBracket;
            ListDelimiterDefinition delimeter;
            return new GrammerDefinition[] {
                openBracket = new BracketOpenDefinition(
                    name: "OPEN_BRACKET",
                    regex: @"\("),
                delimeter = new ListDelimiterDefinition(
                    name: "COMMA",
                    regex: ","),
                new BracketCloseDefinition(
                    name: "CLOSE_BRACKET",
                    regex: @"\)",
                    bracketOpenDefinitions: new[] { openBracket }.Concat(functionCalls),
                    listDelimeterDefinition: delimeter)
            };
        }

        protected virtual IEnumerable<FunctionCallDefinition> FunctionDefinitions()
        {
            return new[]
            {
                 new FunctionCallDefinition(
                    name:"FN_SIN",
                    regex: @"[Ss][Ii][Nn]\(",
                    argumentTypes: new[] {typeof(double) },
                    expressionBuilder: (parameters) => {
                        return Expression.Call(
                            null, 
                            method:Type<object>.Method(x=>Math.Sin(0)),
                            arguments: new [] { parameters[0] });
                    }),
                 new FunctionCallDefinition(
                    name:"FN_COS",
                    regex: @"[Cc][Oo][Ss]\(",
                    argumentTypes: new[] {typeof(double) },
                    expressionBuilder: (parameters) => {
                        return Expression.Call(
                            null,
                            method:Type<object>.Method(x=>Math.Cos(0)),
                            arguments: new [] { parameters[0] });
                    }),
                 new FunctionCallDefinition(
                    name:"FN_TAN",
                    regex: @"[Tt][Aa][Nn]\(",
                    argumentTypes: new[] {typeof(double) },
                    expressionBuilder: (parameters) => {
                        return Expression.Call(
                            null,
                            method:Type<object>.Method(x=>Math.Tan(0)),
                            arguments: new [] { parameters[0] });
                    }),
                new FunctionCallDefinition(
                    name:"FN_SQRT",
                    regex: @"[Ss][Qq][Rr][Tt]\(",
                    argumentTypes: new[] {typeof(double) },
                    expressionBuilder: (parameters) => {
                        return Expression.Call(
                            null,
                            method:Type<object>.Method(x=>Math.Sqrt(0)),
                            arguments: new [] { parameters[0] });
                    }),
                new FunctionCallDefinition(
                    name:"FN_POW",
                    regex: @"[Pp][Oo][Ww]\(",
                    argumentTypes: new[] {typeof(double), typeof(double) },
                    expressionBuilder: (parameters) => {
                        return Expression.Call(
                            null,
                            method:Type<object>.Method(x=>Math.Pow(0,0)),
                            arguments: new [] { parameters[0], parameters[1] });
                    }),

            };
        }

        protected virtual IEnumerable<GrammerDefinition> PropertyDefinitions()
        {
            return new[]
            {
                 //Properties
                 new OperandDefinition(
                    name:"PROPERTY",
                    regex: @"(?<![0-9])[A-Za-z_][A-Za-z0-9_]*",
                    expressionBuilder: (value, parameters) => {
                    return Expression.MakeMemberAccess(parameters[0], parameters[0].Type.GetProperty(value));
                    }),
            };
        }

        protected virtual IEnumerable<GrammerDefinition> WhitespaceDefinitions()
        {
            return new[]
            {
                new GrammerDefinition(name: "WHITESPACE", regex: @"\s+", ignore: true)
            };
        }


        
    }
}
