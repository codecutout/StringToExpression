using StringParser.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StringParser.Parser
{
    public class Operand
    {
        public readonly Expression Expression;

        public readonly StringSegment SourceMap;

        public Operand(Expression expression, StringSegment sourceMap)
        {
            this.Expression = expression;
            this.SourceMap = sourceMap;
        }
    }
}
