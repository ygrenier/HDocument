using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc.Parser
{

    /// <summary>
    /// Parsing position
    /// </summary>
    public struct ParsePosition : IEquatable<ParsePosition>
    {
        /// <summary>
        /// No position
        /// </summary>
        public static readonly ParsePosition None = new ParsePosition() {
            Position = -1,
            Line = -1,
            Column = -1
        };

        /// <summary>
        /// New position
        /// </summary>
        public ParsePosition(int pos, int line, int column)
            : this()
        {
            this.Position = pos;
            this.Line = line;
            this.Column = column;
        }

        /// <summary>
        /// Move the position only
        /// </summary>
        public ParsePosition AddPosition(int delta)
        {
            return new ParsePosition(Position + delta, Line, Column);
        }

        /// <summary>
        /// Move to the next line
        /// </summary>
        /// <param name="incPos">Indicate if we increment the position</param>
        public ParsePosition NextLine(bool incPos = true)
        {
            return new ParsePosition(
                Position + (incPos ? 1 : 0),
                Line + 1,
                0
                );
        }

        /// <summary>
        /// Equality
        /// </summary>
        public bool Equals(ParsePosition other)
        {
            return this.Position == other.Position && this.Line == other.Line && this.Column == other.Column;
        }

        /// <summary>
        /// Equality
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is ParsePosition)
                return Equals((ParsePosition)obj);
            return base.Equals(obj);
        }

        /// <summary>
        /// Hash code
        /// </summary>
        public override int GetHashCode()
        {
            return this.Position ^ this.Line ^ this.Column;
        }

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} (L:{1} / C:{2})", Position, Line, Column);
        }

        /// <summary>
        /// Increment
        /// </summary>
        public static ParsePosition operator ++(ParsePosition p)
        {
            return new ParsePosition(p.Position + 1, p.Line, p.Column + 1);
        }

        /// <summary>
        /// Equality
        /// </summary>
        public static bool operator ==(ParsePosition p1, ParsePosition p2)
        {
            return p1.Equals(p2);
        }

        /// <summary>
        /// Non Equality
        /// </summary>
        public static bool operator !=(ParsePosition p1, ParsePosition p2)
        {
            return !p1.Equals(p2);
        }

        /// <summary>
        /// Stream position
        /// </summary>
        public int Position { get; private set; }
        /// <summary>
        /// Line position
        /// </summary>
        public int Line { get; private set; }
        /// <summary>
        /// Column position
        /// </summary>
        public int Column { get; private set; }

    }

}
