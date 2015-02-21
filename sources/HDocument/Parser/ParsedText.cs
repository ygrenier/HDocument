using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc.Parser
{

    /// <summary>
    /// A parsed text
    /// </summary>
    public class ParsedText : ParsedContent
    {
        /// <summary>
        /// Token type
        /// </summary>
        public override ParsedTokenType TokenType { get { return ParsedTokenType.Text; } }
    }

}
