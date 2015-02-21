using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc.Parser
{

    /// <summary>
    /// A parsed attribute information
    /// </summary>
    public class ParsedAttribute : ParsedToken
    {
        /// <summary>
        /// Token type
        /// </summary>
        public override ParsedTokenType TokenType { get { return ParsedTokenType.Attribute; } }
        /// <summary>
        /// Name of the attribute
        /// </summary>
        public String Name { get; internal set; }
        /// <summary>
        /// Value od the attribute
        /// </summary>
        /// <remarks>
        /// If null then the attribute is without value
        /// </remarks>
        public String Value { get; internal set; }
        /// <summary>
        /// Char for the value quoting
        /// </summary>
        /// <remarks>
        /// if '\0' then the value is not quoted
        /// </remarks>
        public Char Quote { get; internal set; }
    }

}
