using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc.Parser
{

    /// <summary>
    /// A parsed content
    /// </summary>
    public abstract class ParsedContent : ParsedToken
    {
        /// <summary>
        /// Text content
        /// </summary>
        public String Text { get; internal set; }
    }

}
