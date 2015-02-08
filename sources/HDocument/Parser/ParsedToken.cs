using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc.Parser
{

    /// <summary>
    /// Parsed token
    /// </summary>
    public abstract class ParsedToken
    {
        /// <summary>
        /// Type of the token
        /// </summary>
        public abstract ParsedTokenType TokenType { get; }
    }

}
