using StringToExpression.GrammerDefinitions;
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
    /// Provides the base class for parsing OData filter parameters.
    /// </summary>
    public class ODataFilterLanguage
    {
        /// <summary>
        /// Access to common String Members
        /// </summary>
        protected static class StringMembers
        {
            /// <summary>
            /// The MethodInfo for the StartsWith method
            /// </summary>
            public static MethodInfo StartsWith = Type<string>.Method(x => x.StartsWith(default(string)));

            /// <summary>
            /// The MethodInfo for the EndsWith method
            /// </summary>
            public static MethodInfo EndsWith = Type<string>.Method(x => x.EndsWith(default(string)));

            /// <summary>
            /// The MemberInfo for the Length property
            /// </summary>
            public static MemberInfo Length = Type<string>.Member(x => x.Length);

            /// <summary>
            /// The MethodInfo for the Contains method
            /// </summary>
            public static MethodInfo Contains = Type<string>.Method(x => x.Contains(default(string)));

            /// <summary>
            /// The MethodInfo for the ToLower method
            /// </summary>
            public static MethodInfo ToLower = Type<string>.Method(x => x.ToLower());

            /// <summary>
            /// The MethodInfo for the ToUpper method
            /// </summary>
            public static MethodInfo ToUpper = Type<string>.Method(x => x.ToUpper());

            /// <summary>
            /// The MethodInfo for the IndexOf method
            /// </summary>
            public static MethodInfo IndexOf = Type<string>.Method(x => x.IndexOf(default(string)));

            /// <summary>
            /// The MethodInfo for the Replace method
            /// </summary>
            public static MethodInfo Replace = Type<string>.Method(x => x.Replace(default(string), default(string)));

            /// <summary>
            /// The MethodInfo for the Substring method
            /// </summary>
            public static MethodInfo Substring = Type<string>.Method(x => x.Substring(default(int)));

            /// <summary>
            /// The MethodInfo for the Trim method
            /// </summary>
            public static MethodInfo Trim = Type<string>.Method(x => x.Trim());

            /// <summary>
            /// The MethodInfo for the Trim method
            /// </summary>
            public static MethodInfo Concat = Type<string>.Method(x => string.Concat(default(string), default(string)));

            /// <summary>
            /// The MethodInfo for the Substring method that contains a length parameter
            /// </summary>
            public static MethodInfo SubstringWithLength = Type<string>.Method(x => x.Substring(default(int), default(int)));
        }

        /// <summary>
        /// Access to common DateTime Members
        /// </summary>
        protected static class DateTimeMembers
        {
            /// <summary>
            /// The MemberInfo for the Year property
            /// </summary>
            public static MemberInfo Year = Type<DateTime>.Member(x => x.Year);

            /// <summary>
            /// The MemberInfo for the Month property
            /// </summary>
            public static MemberInfo Month = Type<DateTime>.Member(x => x.Month);

            /// <summary>
            /// The MemberInfo for the Day property
            /// </summary>
            public static MemberInfo Day = Type<DateTime>.Member(x => x.Day);

            /// <summary>
            /// The MemberInfo for the Hour property
            /// </summary>
            public static MemberInfo Hour = Type<DateTime>.Member(x => x.Hour);

            /// <summary>
            /// The MemberInfo for the Minute property
            /// </summary>
            public static MemberInfo Minute = Type<DateTime>.Member(x => x.Minute);

            /// <summary>
            /// The MemberInfo for the Second property
            /// </summary>
            public static MemberInfo Second = Type<DateTime>.Member(x => x.Second);
        }

        protected static class MathMembers
        {
            /// <summary>
            /// The MethodInfo for the Round method
            /// </summary>
            public static MethodInfo Round = Type<object>.Method(x => Math.Round(default(double)));

            /// <summary>
            /// The MethodInfo for the Floor method
            /// </summary>
            public static MethodInfo Floor = Type<object>.Method(x => Math.Floor(default(double)));

            /// <summary>
            /// The MethodInfo for the Ceil method
            /// </summary>
            public static MethodInfo Ceiling = Type<object>.Method(x => Math.Ceiling(default(double)));
        }

        private readonly Language language;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataFilterLanguage"/> class.
        /// </summary>
        public ODataFilterLanguage() {
            language = new Language(AllDefinitions().ToArray());
        }

        /// <summary>
        /// Parses the specified text converting it into a predicate expression
        /// </summary>
        /// <typeparam name="T">The input type</typeparam>
        /// <param name="text">The text to parse.</param>
        /// <returns></returns>
        public Expression<Func<T, bool>> Parse<T>(string text)
        {
            var parameters = new[] { Expression.Parameter(typeof(T)) };
            var body = language.Parse(text, parameters);

            ExpressionConversions.TryBoolean(ref body);

            return Expression.Lambda<Func<T, bool>>(body, parameters);
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
            definitions.AddRange(LogicalOperatorDefinitions());
            definitions.AddRange(ArithmeticOperatorDefinitions());
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
                new OperandDefinition(
                    name:"GUID",
                    regex: @"guid'[0-9A-Fa-f]{8}\-[0-9A-Fa-f]{4}\-[0-9A-Fa-f]{4}\-[0-9A-Fa-f]{4}\-[0-9A-Fa-f]{12}'",
                    expressionBuilder: x => Expression.Constant(Guid.Parse(x.Substring("guid".Length).Trim('\'')))),

                new OperandDefinition(
                    name:"STRING",
                    regex: @"'(?:\\.|[^'])*'",
                    expressionBuilder: x => Expression.Constant(x.Trim('\'')
                        .Replace("\\'", "'")
                        .Replace("\\r", "\r")
                        .Replace("\\f", "\f")
                        .Replace("\\n", "\n")
                        .Replace("\\\\", "\\")
                        .Replace("\\b", "\b")
                        .Replace("\\t", "\t"))),
                new OperandDefinition(
                    name:"BYTE",
                    regex: @"0x[0-9A-Fa-f]{1,2}",
                    expressionBuilder: x => Expression.Constant(byte.Parse(x.Substring("0x".Length), NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier))),
                new OperandDefinition(
                    name:"NULL",
                    regex: @"null",
                    expressionBuilder: x => Expression.Constant(null)),
                new OperandDefinition(
                    name:"BOOL",
                    regex: @"true|false",
                    expressionBuilder: x => Expression.Constant(bool.Parse(x))),
                 new OperandDefinition(
                    name:"DATETIME",
                    regex: @"[Dd][Aa][Tt][Ee][Tt][Ii][Mm][Ee]'[^']+'",
                    expressionBuilder: x => Expression.Constant(DateTime.Parse(x.Substring("datetime".Length).Trim('\'')))),
                new OperandDefinition(
                    name:"DATETIMEOFFSET",
                    regex: @"datetimeoffset'[^']+'",
                    expressionBuilder: x => Expression.Constant(DateTimeOffset.Parse(x.Substring("datetimeoffset".Length).Trim('\'')))),

                new OperandDefinition(
                    name:"FLOAT",
                    regex: @"\-?\d+?\.\d*f",
                    expressionBuilder: x => Expression.Constant(float.Parse(x.TrimEnd('f')))),
                new OperandDefinition(
                    name:"DOUBLE",
                    regex: @"\-?\d+\.?\d*d",
                    expressionBuilder: x => Expression.Constant(double.Parse(x.TrimEnd('d')))),
                new OperandDefinition(
                    name:"DECIMAL_EXPLICIT",
                    regex: @"\-?\d+\.?\d*[m|M]",
                    expressionBuilder: x => Expression.Constant(decimal.Parse(x.TrimEnd('m','M')))),
                new OperandDefinition(
                    name:"DECIMAL",
                    regex: @"\-?\d+\.\d+",
                    expressionBuilder: x => Expression.Constant(decimal.Parse(x))),

                new OperandDefinition(
                    name:"LONG",
                    regex: @"\-?\d+L",
                    expressionBuilder: x => Expression.Constant(long.Parse(x.TrimEnd('L')))),
                new OperandDefinition(
                    name:"INTEGER",
                    regex: @"\-?\d+",
                    expressionBuilder: x => Expression.Constant(int.Parse(x))),
            };
        }

        /// <summary>
        /// Returns the definitions for logic operators used within the language.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<GrammerDefinition> LogicalOperatorDefinitions()
        {
            return new GrammerDefinition[]
            {
                new BinaryOperatorDefinition(
                    name:"EQ",
                    regex: @"\b(eq)\b",
                    orderOfPrecedence:11,
                    expressionBuilder: ConvertEnumsIfRequired((left,right) => Expression.Equal(left, right))),
                new BinaryOperatorDefinition(
                    name:"NE",
                    regex: @"\b(ne)\b",
                    orderOfPrecedence:12,
                    expressionBuilder: ConvertEnumsIfRequired((left,right) => Expression.NotEqual(left, right))),

                new BinaryOperatorDefinition(
                    name:"GT",
                    regex: @"\b(gt)\b",
                    orderOfPrecedence:13,
                    expressionBuilder: (left,right) => Expression.GreaterThan(left, right)),
                new BinaryOperatorDefinition(
                    name:"GE",
                    regex: @"\b(ge)\b",
                    orderOfPrecedence:14,
                    expressionBuilder: (left,right) => Expression.GreaterThanOrEqual(left, right)),

                new BinaryOperatorDefinition(
                    name:"LT",
                    regex: @"\b(lt)\b",
                    orderOfPrecedence:15,
                    expressionBuilder: (left,right) => Expression.LessThan(left, right)),
                new BinaryOperatorDefinition(
                    name:"LE",
                    regex: @"\b(le)\b",
                    orderOfPrecedence:16,
                    expressionBuilder: (left,right) => Expression.LessThanOrEqual(left, right)),

                new BinaryOperatorDefinition(
                    name:"AND",
                    regex: @"\b(and)\b",
                    orderOfPrecedence:17,
                    expressionBuilder: (left,right) => Expression.And(left, right)),
                new BinaryOperatorDefinition(
                    name:"OR",
                    regex: @"\b(or)\b",
                    orderOfPrecedence:18,
                    expressionBuilder: (left,right) => Expression.Or(left, right)),

                new UnaryOperatorDefinition(
                    name:"NOT",
                    regex: @"\b(not)\b",
                    orderOfPrecedence:19,
                    operandPosition: RelativePosition.Right,
                    expressionBuilder: (arg) => {
                        ExpressionConversions.TryBoolean(ref arg);
                        return Expression.Not(arg);
                    })
            };
        }

        /// <summary>
        /// Returns the definitions for arithmetic operators used within the language.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<GrammerDefinition> ArithmeticOperatorDefinitions()
        {
            return new[]
            {
                 new BinaryOperatorDefinition(
                    name:"ADD",
                    regex: @"\b(add)\b",
                    orderOfPrecedence: 2,
                    expressionBuilder: (left,right) => Expression.Add(left, right)),
                new BinaryOperatorDefinition(
                    name:"SUB",
                    regex: @"\b(sub)\b",
                    orderOfPrecedence: 2,
                    expressionBuilder: (left,right) => Expression.Subtract(left, right)),
                new BinaryOperatorDefinition(
                    name:"MUL",
                    regex: @"\b(mul)\b",
                    orderOfPrecedence: 1,
                    expressionBuilder: (left,right) => Expression.Multiply(left, right)),
                new BinaryOperatorDefinition(
                    name:"DIV",
                    regex: @"\b(div)\b",
                    orderOfPrecedence: 1,
                    expressionBuilder: (left,right) => Expression.Divide(left, right)),
                new BinaryOperatorDefinition(
                    name:"MOD",
                    regex: @"\b(mod)\b",
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
                    name:"FN_STARTSWITH",
                    regex: @"startswith\(",
                    argumentTypes: new[] {typeof(string), typeof(string) },
                    expressionBuilder: (parameters) => {
                        return Expression.Call(
                            instance:parameters[0], 
                            method:StringMembers.StartsWith,
                            arguments: new [] { parameters[1] });
                    }),
                new FunctionCallDefinition(
                    name:"FN_ENDSWITH",
                    regex: @"endswith\(",
                    argumentTypes: new[] {typeof(string), typeof(string) },
                    expressionBuilder: (parameters) => {
                        return Expression.Call(
                            instance:parameters[0],
                            method:StringMembers.EndsWith,
                            arguments: new [] { parameters[1] });
                    }),
                 new FunctionCallDefinition(
                    name:"FN_SUBSTRINGOF",
                    regex: @"substringof\(",
                    argumentTypes: new[] {typeof(string), typeof(string) },
                    expressionBuilder: (parameters) => {
                        return Expression.Call(
                            instance:parameters[1],
                            method:StringMembers.Contains,
                            arguments: new [] { parameters[0] });
                    }),
                new FunctionCallDefinition(
                    name:"FN_TOLOWER",
                    regex: @"tolower\(",
                    argumentTypes: new[] {typeof(string) },
                    expressionBuilder: (parameters) => {
                        return Expression.Call(
                            instance:parameters[0],
                            method:StringMembers.ToLower);
                    }),
                new FunctionCallDefinition(
                    name:"FN_TOUPPER",
                    regex: @"toupper\(",
                    argumentTypes: new[] {typeof(string) },
                    expressionBuilder: (parameters) => {
                        return Expression.Call(
                            instance:parameters[0],
                            method:StringMembers.ToUpper);
                    }),
                new FunctionCallDefinition(
                    name:"FN_LENGTH",
                    regex: @"length\(",
                    argumentTypes: new[] {typeof(string) },
                    expressionBuilder: (parameters) => {
                        return Expression.MakeMemberAccess(
                            expression:parameters[0],
                            member:StringMembers.Length);
                    }),
                new FunctionCallDefinition(
                    name:"FN_INDEXOF",
                    regex: @"indexof\(",
                    argumentTypes: new[] {typeof(string), typeof(string) },
                    expressionBuilder: (parameters) => {
                        return Expression.Call(
                            instance:parameters[0],
                            method:StringMembers.IndexOf,
                            arguments: new [] { parameters[1] });
                    }),
                new FunctionCallDefinition(
                    name:"FN_REPLACE",
                    regex: @"replace\(",
                    argumentTypes: new[] {typeof(string), typeof(string), typeof(string) },
                    expressionBuilder: (parameters) => {
                        return Expression.Call(
                            instance:parameters[0],
                            method:StringMembers.Replace,
                            arguments: new [] { parameters[1], parameters[2] });
                    }),
                new FunctionCallDefinition(
                    name:"FN_SUBSTRING",
                    regex: @"substring\(",
                    overloads: new[]
                    {
                        new FunctionCallDefinition.Overload(
                            argumentTypes: new[] {typeof(string), typeof(int)},
                            expressionBuilder: (parameters) => {
                                return Expression.Call(
                                    instance:parameters[0],
                                    method:StringMembers.Substring,
                                    arguments: new [] { parameters[1]});
                        }),
                        new FunctionCallDefinition.Overload(
                            argumentTypes: new[] {typeof(string), typeof(int), typeof(int)},
                            expressionBuilder: (parameters) => {
                                return Expression.Call(
                                    instance:parameters[0],
                                    method:StringMembers.SubstringWithLength,
                                    arguments: new [] { parameters[1], parameters[2]});
                        })
                    }),
                new FunctionCallDefinition(
                    name:"FN_TRIM",
                    regex: @"trim\(",
                    argumentTypes: new []{typeof(string)},
                    expressionBuilder: (parameters) => {
                        return Expression.Call(
                            instance:parameters[0],
                            method:StringMembers.Trim);
                    }),
                new FunctionCallDefinition(
                    name:"FN_CONCAT",
                    regex: @"concat\(",
                    argumentTypes: new[] {typeof(string), typeof(string)},
                    expressionBuilder: (parameters) => {
                        return Expression.Call(
                            method:StringMembers.Concat,
                            arguments: new [] { parameters[0], parameters[1]});
                    }),


                 new FunctionCallDefinition(
                    name:"FN_DAY",
                    regex: @"day\(",
                    argumentTypes: new[] {typeof(DateTime) },
                    expressionBuilder: (parameters) => {
                        return Expression.MakeMemberAccess(
                            parameters[0],
                            DateTimeMembers.Day);
                    }),
                 new FunctionCallDefinition(
                    name:"FN_HOUR",
                    regex: @"hour\(",
                    argumentTypes: new[] {typeof(DateTime) },
                    expressionBuilder: (parameters) => {
                        return Expression.MakeMemberAccess(
                            parameters[0],
                            DateTimeMembers.Hour);
                    }),
                new FunctionCallDefinition(
                    name:"FN_MINUTE",
                    regex: @"minute\(",
                    argumentTypes: new[] {typeof(DateTime) },
                    expressionBuilder: (parameters) => {
                        return Expression.MakeMemberAccess(
                            parameters[0],
                            DateTimeMembers.Minute);
                    }),
                new FunctionCallDefinition(
                    name:"FN_MONTH",
                    regex: @"month\(",
                    argumentTypes: new[] {typeof(DateTime) },
                    expressionBuilder: (parameters) => {
                        return Expression.MakeMemberAccess(
                            parameters[0],
                            DateTimeMembers.Month);
                    }),
                new FunctionCallDefinition(
                    name:"FN_YEAR",
                    regex: @"year\(",
                    argumentTypes: new[] {typeof(DateTime) },
                    expressionBuilder: (parameters) => {
                        return Expression.MakeMemberAccess(
                            parameters[0],
                            DateTimeMembers.Year);
                    }),
                new FunctionCallDefinition(
                    name:"FN_SECOND",
                    regex: @"second\(",
                    argumentTypes: new[] {typeof(DateTime) },
                    expressionBuilder: (parameters) => {
                        return Expression.MakeMemberAccess(
                            parameters[0],
                            DateTimeMembers.Second);
                    }),

                new FunctionCallDefinition(
                    name:"FN_ROUND",
                    regex: @"round\(",
                    argumentTypes: new[] {typeof(double) },
                    expressionBuilder: (parameters) => {
                        return Expression.Call(
                            method:MathMembers.Round,
                            arguments: new [] { parameters[0]});
                    }),
                new FunctionCallDefinition(
                    name:"FN_FLOOR",
                    regex: @"floor\(",
                    argumentTypes: new[] {typeof(double) },
                    expressionBuilder: (parameters) => {
                        return Expression.Call(
                            method:MathMembers.Floor,
                            arguments: new [] { parameters[0]});
                    }),
                new FunctionCallDefinition(
                    name:"FN_CEILING",
                    regex: @"ceiling\(",
                    argumentTypes: new[] {typeof(double) },
                    expressionBuilder: (parameters) => {
                        return Expression.Call(
                            method:MathMembers.Ceiling,
                            arguments: new [] { parameters[0]});
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
                 //Properties
                 new OperandDefinition(
                    name:"PROPERTY_PATH",
                    regex: @"(?<![0-9])([A-Za-z_][A-Za-z0-9_]*/?)+",
                    expressionBuilder: (value, parameters) => {
                        return value.Split('/').Aggregate((Expression)parameters[0], (exp, prop)=> Expression.MakeMemberAccess(exp, TypeShim.GetProperty(exp.Type, prop)));
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


        /// <summary>
        /// Wraps the function to convert any constants to enums if required
        /// </summary>
        /// <param name="expFn">Function to wrap</param>
        /// <returns></returns>
        protected Func<Expression, Expression, Expression> ConvertEnumsIfRequired(Func<Expression, Expression, Expression> expFn)
        {
            return (left, right) =>
            {
                var didConvertEnum = ExpressionConversions.TryEnumNumberConvert(ref left, ref right)
                    || ExpressionConversions.TryEnumStringConvert(ref left, ref right, ignoreCase: true);

                return expFn(left, right);
            };
        }
    }
}
