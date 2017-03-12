using StringParser.TokenDefinitions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StringParser.LanguageDefinitions
{
    public class ODataFilterLanguage : Language
    {
        public ODataFilterLanguage()
            : base(BuildGrammer()){
        }

        public static GrammerDefinition[] BuildGrammer()
        {
            BracketOpenDefinition openBracket;
            return new[]
            {
                //Types
                 new OperandDefinition(
                    name:"GUID",
                    regex: @"guid'[0-9A-Fa-f]{8}\-[0-9A-Fa-f]{4}\-[0-9A-Fa-f]{4}\-[0-9A-Fa-f]{4}\-[0-9A-Fa-f]{12}'",
                    expressionBuilder: x => Expression.Constant(Guid.Parse(x.Substring("guid".Length).Trim('\'')))),

                new OperandDefinition(
                    name:"STRING",
                    regex: @"'(?:\\.|[^'])*'",
                    expressionBuilder: x => Expression.Constant(x.Trim('\''))),
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
                    regex: @"\-?\d+?\.\d+?f",
                    expressionBuilder: x => Expression.Constant(float.Parse(x.TrimEnd('f')))),
                new OperandDefinition(
                    name:"DOUBLE",
                    regex: @"\-?\d+\.?\d+?d",
                    expressionBuilder: x => Expression.Constant(double.Parse(x.TrimEnd('d')))),
                new OperandDefinition(
                    name:"DECIMAL_EXPLICIT",
                    regex: @"\-?\d+\.?\d+?[m|M]",
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
              
                //Logical Operators
                new BinaryOperatorDefinition(
                    name:"EQ",
                    regex: @"eq",
                    orderOfPrecedence:11,
                    expressionBuilder: (left,right) => Expression.Equal(left, right)),
                new BinaryOperatorDefinition(
                    name:"NE",
                    regex: @"ne",
                    orderOfPrecedence:12,
                    expressionBuilder: (left,right) => Expression.NotEqual(left, right)),

                new BinaryOperatorDefinition(
                    name:"GT",
                    regex: @"gt",
                    orderOfPrecedence:13,
                    expressionBuilder: (left,right) => Expression.GreaterThan(left, right)),
                new BinaryOperatorDefinition(
                    name:"GE",
                    regex: @"ge",
                    orderOfPrecedence:14,
                    expressionBuilder: (left,right) => Expression.GreaterThanOrEqual(left, right)),

                new BinaryOperatorDefinition(
                    name:"LT",
                    regex: @"lt",
                    orderOfPrecedence:15,
                    expressionBuilder: (left,right) => Expression.LessThan(left, right)),
                new BinaryOperatorDefinition(
                    name:"LE",
                    regex: @"le",
                    orderOfPrecedence:16,
                    expressionBuilder: (left,right) => Expression.LessThanOrEqual(left, right)),

                new BinaryOperatorDefinition(
                    name:"AND",
                    regex: @"and",
                    orderOfPrecedence:17,
                    expressionBuilder: (left,right) => Expression.And(left, right)),
                new BinaryOperatorDefinition(
                    name:"OR",
                    regex: @"or",
                    orderOfPrecedence:18,
                    expressionBuilder: (left,right) => Expression.Or(left, right)),

                new UnaryOperatorDefinition(
                    name:"NOT",
                    regex: @"not",
                    operatorPrecedence:19,
                    operandPosition: RelativePosition.Right,
                    expressionBuilder: (arg) => Expression.Not(arg)),
            
            
                //Arithmatic Operators
                 new BinaryOperatorDefinition(
                    name:"ADD",
                    regex: @"add",
                    orderOfPrecedence: 1,
                    expressionBuilder: (left,right) => Expression.Add(left, right)),
                new BinaryOperatorDefinition(
                    name:"SUB",
                    regex: @"sub",
                    orderOfPrecedence: 2,
                    expressionBuilder: (left,right) => Expression.Add(left, right)),
                new BinaryOperatorDefinition(
                    name:"MUL",
                    regex: @"mul",
                    orderOfPrecedence: 3,
                    expressionBuilder: (left,right) => Expression.Multiply(left, right)),
                new BinaryOperatorDefinition(
                    name:"DIV",
                    regex: @"div",
                    orderOfPrecedence: 4,
                    expressionBuilder: (left,right) => Expression.Divide(left, right)),
                new BinaryOperatorDefinition(
                    name:"MOD",
                    regex: @"mod",
                    orderOfPrecedence: 5,
                    expressionBuilder: (left,right) => Expression.Modulo(left, right)),

                //Brackets
                openBracket = new BracketOpenDefinition(
                    name: "OPENBRACKET",
                    regex: @"\("),
                new BracketCloseDefinition(
                    name: "CLOSEBRACKET",
                    regex: @"\)",
                    bracketOpenDefinition: openBracket),

                 //Properties
                 new OperandDefinition(
                    name:"PROPERTY",
                    regex: @"(?<![0-9])[A-Za-z_][A-Za-z0-9_]*",
                    expressionBuilder: (value, parameters) => {
                    return Expression.MakeMemberAccess(parameters[0], parameters[0].Type.GetProperty(value));
                    }),


                //Whitespace
                new GrammerDefinition(name: "WHITESPACE", regex: @"\s+", ignore: true)
            };
        }
    }
}
