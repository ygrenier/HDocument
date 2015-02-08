using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc.Parser
{

    /// <summary>
    /// Type of token
    /// </summary>
    public enum ParsedTokenType
    {
        /// <summary>
        /// Normal text
        /// </summary>
        Text,
        /// <summary>
        /// CData
        /// </summary>
        CData,
        /// <summary>
        /// Comments
        /// </summary>
        Comment,
        /// <summary>
        /// Open tag : &lt;tag ... 
        /// </summary>
        OpenTag,
        /// <summary>
        /// Auto closed tag : ... /&gt;
        /// </summary>
        AutoClosedTag,
        /// <summary>
        /// Close tag : ... &gt;
        /// </summary>
        CloseTag,
        /// <summary>
        /// Close tag : &lt;/tag&gt;
        /// </summary>
        EndTag,
        /// <summary>
        /// Open Process instruction : &lt;? ...
        /// </summary>
        OpenProcessInstruction,
        /// <summary>
        /// Close Process instruction : ... ?&gt; 
        /// </summary>
        CloseProcessInstruction,
        /// <summary>
        /// Attribute
        /// </summary>
        Attribute
    }

}
