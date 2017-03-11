using System;
using System.Collections.Generic;

namespace StringParser.Util
{
    /// <summary>
    /// Represent a part of a string
    /// </summary>
    public class StringSegment
    {
        /// <summary>
        /// Source string
        /// </summary>
        public readonly String SourceString;

        /// <summary>
        /// Start of the range
        /// </summary>
        public readonly int Start;

        /// <summary>
        /// End of the range
        /// </summary>
        public readonly int End;

        /// <summary>
        /// The length of the range
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// The string value of this segment
        /// </summary>
        public string Value => SourceString.Substring(Start, Length);

        public StringSegment(string sourceString, int start, int length)
        {
            if (sourceString == null)
                throw new ArgumentNullException(nameof(sourceString));

            this.SourceString = sourceString;
            this.Start = start;
            this.Length = length;
            this.End = start + length;
        }


        /// <summary>
        /// Determines if segment is to the right of this segment
        /// </summary>
        /// <param name="segment">segment to check</param>
        /// <returns></returns>
        public bool IsRightOf(StringSegment segment)
        {
            if (segment == null)
                throw new ArgumentNullException(nameof(segment));
            if (this.SourceString != segment.SourceString)
                throw new ArgumentException($"{nameof(segment)} must have the same source string", nameof(segment));
            return End >= segment.Start;
        }

        /// <summary>
        /// Determines if index is to the right of this segment
        /// </summary>
        /// <param name="index">index to check</param>
        /// <returns></returns>
        public bool IsRightOf(int index)
        {
            return End >= index;
        }

        /// <summary>
        /// Determines if segment is to the left of this segment
        /// </summary>
        /// <param name="segment">segment to check</param>
        /// <returns></returns>
        public bool IsLeftOf(StringSegment segment)
        {
            if (segment == null)
                throw new ArgumentNullException(nameof(segment));
            if (this.SourceString != segment.SourceString)
                throw new ArgumentException($"{nameof(segment)} must have the same source string", nameof(segment));
            return Start <= segment.End;
        }

        /// <summary>
        /// Determines if index is to the left of this segment
        /// </summary>
        /// <param name="index">index to check</param>
        /// <returns></returns>
        public bool IsLeftOf(int index)
        {
            return Start <= index;
        }


        /// <summary>
        /// Create a segment that encompasses the all the given segments
        /// </summary>
        /// <param name="segments">segments to encompass</param>
        /// <returns></returns>
        public static StringSegment Encompass(IEnumerable<StringSegment> segments)
        {
           
            var enumerator = segments.GetEnumerator();
            if (!enumerator.MoveNext())
                throw new ArgumentException($"{nameof(segments)} must have at least one item", nameof(segments));

            string sourceString = enumerator.Current.SourceString;
            var maxStart = enumerator.Current.Start;
            var maxEnd = enumerator.Current.End;
            while(enumerator.MoveNext())
            {
                if(enumerator.Current.SourceString != sourceString)
                    throw new ArgumentException($"{nameof(segments)} must all have the same source string", nameof(segments));
                maxStart = Math.Max(enumerator.Current.Start, maxStart);
                maxEnd = Math.Max(enumerator.Current.End, maxEnd);
            }

            return new StringSegment(sourceString, maxStart, maxEnd - maxStart);
        }

        
        public override string ToString()
        {
            return Value;
        }
    }
}
