using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringToExpression.Util
{
    /// <summary>
    /// Provides utility functions for Stacks.
    /// </summary>
    public static class StackExtensions
    {
        /// <summary>
        /// Pops the stack while a predicate is met.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stack">The stack.</param>
        /// <param name="predicate">The predicate to determine if we should pop an item.</param>
        /// <returns>Enumerable over the poped items.</returns>
        public static IEnumerable<T> PopWhile<T>(this Stack<T> stack, Func<T, bool> predicate)
        {
            while (stack.Count > 0 && predicate(stack.Peek()))
                yield return stack.Pop();
        }

        /// <summary>
        /// Pops the stack while a predicate is met.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stack">The stack.</param>
        /// <param name="predicate">The predicate to determine if we should pop an item.</param>
        /// <returns>Enumerable over the poped items.</returns>
        public static IEnumerable<T> PopWhile<T>(this Stack<T> stack, Func<T, int, bool> predicate)
        {
            int count = 0;
            while (stack.Count > 0 && predicate(stack.Peek(), count++))
                yield return stack.Pop();
        }
    }
}
