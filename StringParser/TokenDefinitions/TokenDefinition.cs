using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringParser.TokenDefinitions
{
    public class TokenDefinition
    {
        public string Name { get; set; }

        public string Regex { get; set; }

        public bool Skip { get; set; }
    }
}
