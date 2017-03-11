using StringParser.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringParser.Parser
{
    public class Operator
    {
        public readonly Action<ParseState> Execute;

        public readonly StringSegment SourceMap;

        public Operator(Action<ParseState> execute, StringSegment sourceMap)
        {
            this.Execute = execute;
            this.SourceMap = sourceMap;
        }
    }
}
