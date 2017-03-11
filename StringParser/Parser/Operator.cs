using StringParser.TokenDefinitions;
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

        public readonly GrammerDefinition Definition;

        public Operator(GrammerDefinition definition, StringSegment sourceMap, Action<ParseState> execute)
        {
            this.Execute = execute;
            this.SourceMap = sourceMap;
            this.Definition = definition;
        }
    }
}
