using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringParser.Exceptions
{
    public class InvalidTokenException : Exception
    {
        public readonly int Index;
        public readonly string Token;

        public InvalidTokenException(int index, string token) : base($"Invalid token '{token}' found at index {index}")
        {
            Index = index;
            Token = token;
        }
    }
}
