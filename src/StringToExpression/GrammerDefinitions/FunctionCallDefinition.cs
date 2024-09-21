using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StringToExpression.Parser;
using StringToExpression.Tokenizer;
using StringToExpression.Exceptions;
using StringToExpression.Util;

namespace StringToExpression.GrammerDefinitions
{
    /// <summary>
    /// Represents a the grammer for a function call.
    /// </summary>
    /// <seealso cref="StringToExpression.GrammerDefinitions.BracketOpenDefinition" />
    public class FunctionCallDefinition : BracketOpenDefinition
    {

        public class Overload
        {
            /// <summary>
            /// Argument types that the function accepts.
            /// </summary>
            public readonly IReadOnlyList<Type> ArgumentTypes;

            /// <summary>
            /// A function given the arguments, outputs a new operand.
            /// </summary>
            public readonly Func<Expression[], Expression> ExpressionBuilder;

            public Overload(
                IEnumerable<Type> argumentTypes,
                Func<Expression[], Expression> expressionBuilder)
            {
                this.ArgumentTypes = argumentTypes?.ToList();
                this.ExpressionBuilder = expressionBuilder;
            }
        }


        /// <summary>
        /// Function overlaods
        /// </summary>
        public readonly IReadOnlyList<Overload> Overloads;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionCallDefinition"/> class.
        /// </summary>
        /// <param name="name">The name of the definition.</param>
        /// <param name="regex">The regex to match tokens.</param>
        /// <param name="argumentTypes">The argument types that the function accepts.</param>
        /// <param name="expressionBuilder">The function given the single operand expressions, outputs a new operand.</param>
        public FunctionCallDefinition(
            string name,
            string regex,
            IEnumerable<Type> argumentTypes,
            Func<Expression[], Expression> expressionBuilder)
            : this(name, regex, new[] { new Overload(argumentTypes, expressionBuilder) })
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionCallDefinition"/> class.
        /// </summary>
        /// <param name="name">The name of the definition.</param>
        /// <param name="regex">The regex to match tokens.</param>
        /// <param name="overloads">list of overloads avilable for function</param>
        public FunctionCallDefinition(
            string name,
            string regex,
            IEnumerable<Overload> overloads)
            : base(name, regex)
        {
            var overloadList = overloads?.ToList();
            if (overloadList.Count == 0)
            {
                throw new ArgumentException("Must specify at least one overlaod", nameof(overloads));
            }
            this.Overloads = overloadList;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionCallDefinition"/> class.
        /// </summary>
        /// <param name="name">The name of the definition.</param>
        /// <param name="regex">The regex to match tokens.</param>
        /// <param name="expressionBuilder">The function given the single operand expressions, outputs a new operand.</param>
        public FunctionCallDefinition(string name, string regex,Func<Expression[], Expression> expressionBuilder)
           : this(name, regex, null, expressionBuilder)
        {
        }


        public Overload MatchOverload(Operator bracketOpen, Stack<Operand> bracketOperands, Operator bracketClose, out IEnumerable<Expression> typedArguments)
        {
            var functionSourceMap = StringSegment.Encompass(
                bracketOpen.SourceMap,
                StringSegment.Encompass(bracketOperands.Select(x => x.SourceMap)),
                bracketClose.SourceMap);

            var possibleOverloads = Overloads
               .Where(x => x.ArgumentTypes == null || x.ArgumentTypes.Count == bracketOperands.Count)
               .OrderBy(x=>x.ArgumentTypes == null);

            // No viable overloads, user has probably inputted wrong number of arguments
            if (!possibleOverloads.Any())
            {
                throw new FunctionArgumentCountException(
                    functionSourceMap, 
                    Overloads.First().ArgumentTypes.Count, 
                    bracketOperands.Count);
            }

            foreach(var possibleOverload in possibleOverloads)
            {
                //null argument types is treated as a I can accept anything
                if (possibleOverload.ArgumentTypes == null)
                {
                    typedArguments = bracketOperands.Select(x => x.Expression);
                    return possibleOverload;
                }

                var argumentMatch = bracketOperands.Zip(possibleOverload.ArgumentTypes, (o, t) => {
                    var canConvert = ExpressionConversions.TryConvert(o.Expression, t, out var result);
                    return new { CanConvert = canConvert, Operand = o, ArgumentType = t, ConvertedOperand = result };
                });


                if (argumentMatch.All(x => x.CanConvert))
                {
                    typedArguments = argumentMatch.Select(x => x.ConvertedOperand);
                    return possibleOverload;
                }

                // If we have only a single possible overlaod but we arguement types dont align
                // we will throw an error as though they had the wrong types
                if (possibleOverloads.Count() == 1)
                {
                    var badConvert = argumentMatch.First(x => !x.CanConvert);
                    throw new FunctionArgumentTypeException(badConvert.Operand.SourceMap, badConvert.ArgumentType, badConvert.Operand.Expression.Type);
                }
            }

            //We had multiple overloads, but none of them matched
            throw new FunctionOverlaodNotFoundException(StringSegment.Encompass(bracketOperands.Select(x => x.SourceMap)));


        }

        /// <summary>
        /// Applies the bracket operands. Executes the expressionBuilder with all the operands in the brackets.
        /// </summary>
        /// <param name="bracketOpen">The operator that opened the bracket.</param>
        /// <param name="bracketOperands">The list of operands within the brackets.</param>
        /// <param name="bracketClose">The operator that closed the bracket.</param>
        /// <param name="state">The current parse state.</param>
        /// <exception cref="FunctionArgumentCountException">When the number of opreands does not match the number of arguments</exception>
        /// <exception cref="FunctionArgumentTypeException">When argument Type does not match the type of the expression</exception>
        /// <exception cref="OperationInvalidException">When an error occured while executing the expressionBuilder</exception>
        public override void ApplyBracketOperands(Operator bracketOpen, Stack<Operand> bracketOperands, Operator bracketClose, ParseState state)
        {
            var functionSourceMap = StringSegment.Encompass(
                bracketOpen.SourceMap, 
                StringSegment.Encompass(bracketOperands.Select(x => x.SourceMap)), 
                bracketClose.SourceMap);

            var overload = MatchOverload(bracketOpen, bracketOperands, bracketClose, out var functionArguments);

           
            var functionArgumentsArray = functionArguments.ToArray();
            Expression output;
            try
            {
                output = overload.ExpressionBuilder(functionArgumentsArray);
            }
            catch(Exception ex)
            {
                throw new OperationInvalidException(functionSourceMap, ex);
            }
            if(output != null)
                state.Operands.Push(new Operand(output, functionSourceMap));
        }
    }
}
