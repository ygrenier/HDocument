using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc.Parser
{
    /// <summary>
    /// Parsed doctype
    /// </summary>
    public class ParsedDoctype : ParsedToken
    {
        /// <summary>
        /// Token type
        /// </summary>
        public override ParsedTokenType TokenType { get { return ParsedTokenType.Doctype; } }

        /// <summary>
        /// Values
        /// </summary>
        public String[] Values { get; internal set; }
    }
}
