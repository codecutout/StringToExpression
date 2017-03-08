namespace StringParser.Util
{
    /// <summary>
    /// Represent a range of integers
    /// </summary>
    public struct Span
    {
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

        public Span(int start, int end)
        {
            if (start < end)
            {
                this.Start = start;
                this.End = end;
            }
            else
            {
                this.Start = end;
                this.End = start;
            }
            this.Length = this.End - this.Start;
        }

    }
}
