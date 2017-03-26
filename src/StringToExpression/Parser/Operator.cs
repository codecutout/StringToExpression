using StringToExpression.GrammerDefinitions;
using StringToExpression.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringToExpression.Parser
{
    public class Operator
    {
        public readonly Action Execute;

        public readonly StringSegment SourceMap;

        public readonly GrammerDefinition Definition;

        public Operator(GrammerDefinition definition, StringSegment sourceMap, Action execute)
        {
            this.Execute = execute;
            this.SourceMap = sourceMap;
            this.Definition = definition;
        }

        public override string ToString()
        {
            return SourceMap.Value;
        }
    }
}
