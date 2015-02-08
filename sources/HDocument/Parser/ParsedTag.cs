using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc.Parser
{
    /// <summary>
    /// A parsed tag information
    /// </summary>
    public class ParsedTag : ParsedToken
    {
        ParsedTokenType _TokenType;

        internal static ParsedTag OpenTag(String tag)
        {
            return new ParsedTag() { TagName = tag, _TokenType = ParsedTokenType.OpenTag };
        }
        internal static ParsedTag AutoClosedTag(String tag)
        {
            return new ParsedTag() { TagName = tag, _TokenType = ParsedTokenType.AutoClosedTag };
        }
        internal static ParsedTag CloseTag(String tag)
        {
            return new ParsedTag() { TagName = tag, _TokenType = ParsedTokenType.CloseTag };
        }
        internal static ParsedTag EndTag(String tag)
        {
            return new ParsedTag() { TagName = tag, _TokenType = ParsedTokenType.EndTag };
        }
        internal static ParsedTag OpenProcessInstruction(String tag)
        {
            return new ParsedTag() { TagName = tag, _TokenType = ParsedTokenType.OpenProcessInstruction };
        }
        internal static ParsedTag CloseProcessInstruction(String tag)
        {
            return new ParsedTag() { TagName = tag, _TokenType = ParsedTokenType.CloseProcessInstruction };
        }

        /// <summary>
        /// Tag name
        /// </summary>
        public String TagName { get; set; }

        /// <summary>
        /// Token type
        /// </summary>
        public override ParsedTokenType TokenType { get { return _TokenType; } }

    }
}
