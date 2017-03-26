using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringToExpression.Util
{
    public static class StackExtensions
    {
        public static IEnumerable<T> PopWhile<T>(this Stack<T> stack, Func<T, bool> predicate)
        {
            while (stack.Count > 0 && predicate(stack.Peek()))
                yield return stack.Pop();
        }

        public static IEnumerable<T> PopWhile<T>(this Stack<T> stack, Func<T, int, bool> predicate)
        {
            int count = 0;
            while (stack.Count > 0 && predicate(stack.Peek(), count++))
                yield return stack.Pop();
        }
    }
}
