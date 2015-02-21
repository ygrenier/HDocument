using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc.Parser
{

    /// <summary>
    /// A parsed CData
    /// </summary>
    public class ParsedCData : ParsedContent
    {
        /// <summary>
        /// Token type
        /// </summary>
        public override ParsedTokenType TokenType { get { return ParsedTokenType.CData; } }
    }

}
