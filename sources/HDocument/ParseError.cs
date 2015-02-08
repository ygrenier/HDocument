using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc
{
    /// <summary>
    /// Parsing error
    /// </summary>
    public class ParseError : Exception
    {

        /// <summary>
        /// New parse error
        /// </summary>
        public ParseError(String message)
            : this(message, Parser.ParsePosition.None)
        {
        }

        /// <summary>
        /// New parse error with position
        /// </summary>
        public ParseError(String message, Parser.ParsePosition pos)
            : base(message)
        {
            this.Position = pos;
        }

        /// <summary>
        /// Position
        /// </summary>
        public Parser.ParsePosition Position { get; private set; }

    }
}
