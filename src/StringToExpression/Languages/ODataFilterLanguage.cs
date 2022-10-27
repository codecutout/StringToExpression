using StringToExpression.GrammerDefinitions;
using StringToExpression.Parser;
using StringToExpression.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

        private readonly Language language;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataFilterLanguage"/> class.
        /// </summary>
        public ODataFilterLanguage()
        {
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
            var parameters = new[] { (Accessor)Expression.Parameter(typeof(T)) };
            var body = language.Parse(text, parameters);

            ExpressionConversions.TryBoolean(ref body);

            return Expression.Lambda<Func<T, bool>>(body, parameters.Select(p => p.Expression));
        }

        /// <summary>
        /// Returns all the definitions used by the language.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<GrammerDefinition> AllDefinitions()
        {
            IEnumerable<FunctionCallDefinition> functions;
            IEnumerable<CollectionDefinition> collections;

            var definitions = new List<GrammerDefinition>();
            definitions.AddRange(TypeDefinitions());
            definitions.AddRange(collections = CollectionDefinitions());
            definitions.AddRange(functions = FunctionDefinitions());
            definitions.AddRange(BracketDefinitions(functions, collections));
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
        protected virtual IEnumerable<GrammerDefinition> BracketDefinitions(IEnumerable<FunctionCallDefinition> functionCalls, IEnumerable<CollectionDefinition> collectionCalls)
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
                    bracketOpenDefinitions: new[] { openBracket }.Concat(functionCalls).Concat(collectionCalls),
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
            };
        }

        /// <summary>
        /// Returns the definitions for property names used within the language.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<GrammerDefinition> PropertyDefinitions()
        {
            return new GrammerDefinition[]
            {
                 //Properties
                 new CollectionAccessor(
                    name: "ACCESSOR", 
                    regex:@"(?<=\()[a-z]+?:"
                    ),
                 new OperandDefinition(
                    name:"PROPERTY_PATH",
                    regex: @"(?<![0-9])([A-Za-z_][A-Za-z0-9_]*\/?(?!any|all))+",
                    expressionBuilder: OperandBuilder
                    ),
            };
        }

        private static Expression OperandBuilder(string value, Accessor[] parameters)
        {
            Expression parameter;
            var values = value.Split('/').ToList();            

            if (parameters.Any(p => !string.IsNullOrWhiteSpace(p.Pattern) && p.Pattern.ToLower() == values[0].ToLower()))
            {
                parameter = parameters.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.Pattern) && p.Pattern.ToLower() == values[0].ToLower());
                values.RemoveAt(0);
            }
            else
            {
                parameter = parameters[0].Expression;
            }

            return values.Aggregate(parameter, CreateMemberAccess);
        }

        private static Expression CreateMemberAccess(Expression exp, string prop)
        {
            var memberAccess = Expression.MakeMemberAccess(exp, TypeShim.GetProperty(exp.Type, prop));
            return memberAccess;
        }

        /// <summary>
        /// Returns the definitions for whitespace used within the language.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<GrammerDefinition> WhitespaceDefinitions()
        {
            return new[]
            {
                new GrammerDefinition(name: "WHITESPACE", regex: @"\s+", ignore: true),
                new GrammerDefinition(name: "SLASH", regex: @"\/(?=any|all)", ignore: true)
            };
        }

        protected virtual IEnumerable<CollectionDefinition> CollectionDefinitions()
        {
            return new[]
            {
                new CollectionDefinition(
                    name: "ANY",
                    regex: @"any\(",
                    expressionBuilder: CreateCollectionAccess
                ),
                new CollectionDefinition(
                    name: "ALL",
                    regex: @"all\(",
                    expressionBuilder: CreateCollectionAccess
                )
            };
        }

        private static Expression CreateCollectionAccess(string param, string source, ParameterExpression[] parameterExpressions)
        {
            //var expParam = Expression.Parameter(parameterExpressions[0], "sc");

            return null;
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
