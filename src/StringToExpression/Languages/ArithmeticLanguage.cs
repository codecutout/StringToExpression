using StringToExpression.GrammerDefinitions;
using StringToExpression.Parser;
using StringToExpression.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StringToExpression.LanguageDefinitions
{
    /// <summary>
    /// Represents a language to that handles basic mathmatics.
    /// </summary>
    public class ArithmeticLanguage
    {
        private readonly Language language;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArithmeticLanguage"/> class.
        /// </summary>
        public ArithmeticLanguage() {
            language = new Language(AllDefinitions().ToArray());
        }

        /// <summary>
        /// Parses the specified text converting it into a expression action.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns></returns>
        public Expression<Func<decimal>> Parse(string text)
        {
            var body = language.Parse(text);
            body = ExpressionConversions.Convert(body, typeof(decimal));
            return Expression.Lambda<Func<decimal>>(body);
        }

        /// <summary>
        /// Parses the specified text converting it into an expression. The expression can take a single parameter
        /// </summary>
        /// <typeparam name="T">the type of the parameter.</typeparam>
        /// <param name="text">The text to parse.</param>
        /// <returns></returns>
        public Expression<Func<T, decimal>> Parse<T>(string text)
        {
            var parameters = new[] { (Accessor)Expression.Parameter(typeof(T)) };
            var body = language.Parse(text, parameters);
            body = ExpressionConversions.Convert(body, typeof(decimal));
            return Expression.Lambda<Func<T, decimal>>(body, parameters.Select(p => p.Expression));
        }

        /// <summary>
        /// Returns all the definitions used by the language.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the definitions for types used within the language.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the definitions for arithmetic operators used within the language.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the definitions for brackets used within the language.
        /// </summary>
        /// <param name="functionCalls">The function calls in the language. (used as opening brackets)</param>
        /// <returns></returns>
        protected virtual IEnumerable<GrammerDefinition> BracketDefinitions(IEnumerable<FunctionCallDefinition> functionCalls)
        {
            BracketOpenDefinition openBracket;
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

        /// <summary>
        /// Returns the definitions for functions used within the language.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the definitions for property names used within the language.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<GrammerDefinition> PropertyDefinitions()
        {
            return new[]
            {
                 new OperandDefinition(
                    name:"PROPERTY_PATH",
                    regex: @"(?<![0-9])([A-Za-z_][A-Za-z0-9_]*\.?)+",
                    expressionBuilder: (value, parameters) => {
                        return value.Split('.').Aggregate((Expression)parameters[0], (exp, prop)=>
                        {
                            return Expression.MakeMemberAccess(exp, TypeShim.GetProperty(exp.Type, prop));
                        });
                    }),
            };
        }

        /// <summary>
        /// Returns the definitions for whitespace used within the language.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<GrammerDefinition> WhitespaceDefinitions()
        {
            return new[]
            {
                new GrammerDefinition(name: "WHITESPACE", regex: @"\s+", ignore: true)
            };
        }


        
    }
}
