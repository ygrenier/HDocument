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
            : base(message)
        {
        }

    }
}
