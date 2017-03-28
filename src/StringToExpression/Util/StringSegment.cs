using System;
using System.Collections.Generic;

namespace StringToExpression.Util
{
    /// <summary>
    /// Represent a part of a string.
    /// </summary>
    public class StringSegment
    {
        /// <summary>
        /// The source string.
        /// </summary>
        public readonly String SourceString;

        /// <summary>
        /// The start position of this segment within the source string.
        /// </summary>
        public readonly int Start;

        /// <summary>
        /// The end position of this segment within the source string.
        /// </summary>
        public readonly int End;

        /// <summary>
        /// The length of this segment.
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// The string value contained within this segment.
        /// </summary>
        public string Value => SourceString.Substring(Start, Length);

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSegment"/> class.
        /// </summary>
        /// <param name="sourceString">The source string.</param>
        /// <param name="start">The start position of this segment within the source string.</param>
        /// <param name="length">The length of this segment.</param>
        /// <exception cref="System.ArgumentNullException">sourceString</exception>
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
        /// Determines if given segment is to the right of this segment.
        /// </summary>
        /// <param name="segment">segment to check.</param>
        /// <returns>
        ///   <c>true</c> if passed segment is to the right of this segment; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">segment</exception>
        /// <exception cref="System.ArgumentException">segment - when this segment and passed segment have different source strings</exception>
        public bool IsRightOf(StringSegment segment)
        {
            if (segment == null)
                throw new ArgumentNullException(nameof(segment));
            if (this.SourceString != segment.SourceString)
                throw new ArgumentException($"{nameof(segment)} must have the same source string", nameof(segment));
            return segment.End <= Start;
        }

        /// <summary>
        /// Determines if index is to the right of this segment.
        /// </summary>
        /// <param name="index">index to check.</param>
        /// <returns>
        ///   <c>true</c> if index is to the right of this segment; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRightOf(int index)
        {
            return Start >= index;
        }

        /// <summary>
        /// Determines if segment is to the left of this segment.
        /// </summary>
        /// <param name="segment">segment to check</param>
        /// <returns>
        ///   <c>true</c> if passed segment is to the left of this segment; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">segment</exception>
        /// <exception cref="System.ArgumentException">segment -  when this segment and passed segment have different source strings</exception>
        public bool IsLeftOf(StringSegment segment)
        {
            if (segment == null)
                throw new ArgumentNullException(nameof(segment));
            if (this.SourceString != segment.SourceString)
                throw new ArgumentException($"{nameof(segment)} must have the same source string", nameof(segment));
            return segment.Start >= End;
        }

        /// <summary>
        /// Determines if index is to the left of this segment.
        /// </summary>
        /// <param name="index">index to check.</param>
        /// <returns>
        ///   <c>true</c> if index is to the left of this segment; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLeftOf(int index)
        {
            return End <= index;
        }


        /// <summary>
        /// Create a segment that encompasses all the passed segments
        /// </summary>
        /// <param name="segments">segments to encompass</param>
        /// <returns>segment that enompasses all the passed segments</returns>
        /// <exception cref="System.ArgumentException">
        /// segments - when does not contain at least one item
        /// or
        /// segments - when all segments do not have the same source strings
        /// </exception>
        public static StringSegment Encompass(IEnumerable<StringSegment> segments)
        {
           
            var enumerator = segments.GetEnumerator();
            if (!enumerator.MoveNext())
                throw new ArgumentException($"{nameof(segments)} must have at least one item", nameof(segments));

            string sourceString = enumerator.Current.SourceString;
            var minStart = enumerator.Current.Start;
            var maxEnd = enumerator.Current.End;
            while(enumerator.MoveNext())
            {
                if(enumerator.Current.SourceString != sourceString)
                    throw new ArgumentException($"{nameof(segments)} must all have the same source string", nameof(segments));
                minStart = Math.Min(enumerator.Current.Start, minStart);
                maxEnd = Math.Max(enumerator.Current.End, maxEnd);
            }

            return new StringSegment(sourceString, minStart, maxEnd - minStart);
        }

        /// <summary>
        /// Create a segment that encompasses all the passed segments
        /// </summary>
        /// <param name="segments">segments to encompass</param>
        /// <returns>segment that enompasses all the passed segments</returns>
        /// <exception cref="System.ArgumentException">
        /// segments - when does not contain at least one item
        /// or
        /// segments - when all segments do not have the same source strings
        /// </exception>
        public static StringSegment Encompass(params StringSegment[] segments)
        {
            return Encompass((IEnumerable<StringSegment>)segments);
        }

        /// <summary>
        /// Determines if this segment is between (and not within) the two passed segments.
        /// </summary>
        /// <param name="segment1">The first segment.</param>
        /// <param name="segment2">The second segment.</param>
        /// <returns>
        ///   <c>true</c> if this segment is between the two passed segments; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// segment1 - when segment does not have the same source string
        /// or
        /// segment2 - when segment does not have the same source string
        /// </exception>
        public bool IsBetween(StringSegment segment1, StringSegment segment2)
        {
            if (this.SourceString != segment1.SourceString)
                throw new ArgumentException($"{nameof(segment1)} must have the same source string", nameof(segment1));
            if (this.SourceString != segment2.SourceString)
                throw new ArgumentException($"{nameof(segment2)} must have the same source string", nameof(segment2));
            return segment1.End <= this.Start && segment2.Start >= this.End;
        }

        /// <summary>
        /// Creates a segment which fits between (and not within) the two passed segments.
        /// </summary>
        /// <param name="segment1">The first segment.</param>
        /// <param name="segment2">The second segment.</param>
        /// <returns>A StringSegment which is between the two passed segments</returns>
        /// <exception cref="System.ArgumentException">when the two segments do not have the same soruce string</exception>
        public static StringSegment Between(StringSegment segment1, StringSegment segment2)
        {
            if(segment1.SourceString != segment2.SourceString)
                throw new ArgumentException($"{nameof(segment1)} and {nameof(segment2)} must the same source string");
            return new StringSegment(
                        segment1.SourceString,
                        segment1.End,
                        segment2.Start - segment1.End);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Value;
        }
    }
}
