using HDoc.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HDoc
{

    /// <summary>
    /// HTML Parser
    /// </summary>
    public class HParser
    {
        #region Inner types

        /// <summary>
        /// Parse states
        /// </summary>
        enum ParseState
        {
            /// <summary>
            /// Out of tag
            /// </summary>
            Content,
            /// <summary>
            /// In comment
            /// </summary>
            Comment,
            /// <summary>
            /// In a process instruction
            /// </summary>
            ProcessInstruction,
            /// <summary>
            /// In a tag
            /// </summary>
            Tag,
            /// <summary>
            /// In a end tag
            /// </summary>
            EndTag,
        }

        #endregion

        ParseState _State;
        ParsedTag _CurrentTag;
        ParsedAttribute _CurrentAttr;
        StringBuilder _CurrentRead;
        ParsePosition _CurrentPosition, _StartTagPosition;
        Stack<Char> _Buffer;
        ParsePosition _NextCharPosition, _CharPosition;
        bool _LastWasCR;

        /// <summary>
        /// Create a new parser
        /// </summary>
        public HParser(TextReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            this.Reader = reader;
            this.EOF = false;
            this._Buffer = new Stack<char>();
            this._CurrentRead = null;
            this._State = ParseState.Content;
            this._CurrentPosition = new ParsePosition();
            this._NextCharPosition = new ParsePosition();
            this._LastWasCR = false;
        }

        /// <summary>
        /// Save a char in the buffer
        /// </summary>
        protected void SaveChar(Char c)
        {
            _Buffer.Push(c);
        }

        /// <summary>
        /// Add a char in the current read buffer
        /// </summary>
        protected void AddToCurrentRead(Char c)
        {
            if (_CurrentRead == null)
            {
                _CurrentRead = new StringBuilder(10);
                _CurrentPosition = _NextCharPosition;
            }
            _CurrentRead.Append(c);
        }

        /// <summary>
        /// Get the current read content
        /// </summary>
        /// <param name="clean">If true reset the current read buffer.</param>
        /// <returns>String of the current read</returns>
        protected String GetCurrentRead(bool clean = false)
        {
            String res = _CurrentRead != null ? _CurrentRead.ToString() : String.Empty;
            if (clean)
                _CurrentRead = null;
            return res;
        }

        /// <summary>
        /// Read the current char and move the pointer
        /// </summary>
        protected int ReadChar(bool saveToCurrent = true)
        {
            int res = -1;
            bool movePosition = true;
            if (_Buffer.Count > 0)
            {
                res = _Buffer.Pop();
                movePosition = false;
            }
            else
            {
                res = Reader.Read();
            }
            // Save the char to current read ?
            if (res >= 0 && saveToCurrent)
            {
                AddToCurrentRead((Char)res);
            }
            // Move the position ?
            if (res >= 0 && movePosition)
            {
                _CharPosition = _NextCharPosition;
                if (res == '\n')
                {
                    if (!_LastWasCR)
                        _NextCharPosition = _NextCharPosition.NextLine(true);
                    else
                        _NextCharPosition = _NextCharPosition.AddPosition(1);
                }
                else if (res == '\r')
                {
                    _NextCharPosition = _NextCharPosition.NextLine(true);
                }
                else
                {
                    _NextCharPosition++;
                }
            }
            _LastWasCR = res == '\r';

            return res;
        }

        /// <summary>
        /// Parse a text content
        /// </summary>
        protected ParsedContent ParseText()
        {
            // Loop read
            int c;
            while ((c = ReadChar(false)) >= 0 && c != '<')
                AddToCurrentRead((Char)c);
            if (c >= 0)
                SaveChar((Char)c);
            // Returns parse result
            return new ParsedText() {
                Position = _CurrentPosition,
                Text = GetCurrentRead(true)
            };
        }

        /// <summary>
        /// Parse a comment
        /// </summary>
        protected ParsedToken ParseComment()
        {
            // We are in comment
            _State = ParseState.Comment;
            // Read loop
            int c, s = 0;
            // s : state
            // 0 : in comment
            // 1 : first end tag '-'
            // 2 : second end tag '-'
            while ((c = ReadChar()) >= 0)
            {
                if (c == '-')
                {
                    if (s == 0) s = 1;
                    else s = 2;
                }
                else if (c == '>' && s == 2)
                    break;  // End tag stop the loop
                else
                    s = 0;  // Reset state
            }
            if (c < 0)
                throw new ParseError("End of file unexpected, comment not closed.", _NextCharPosition);
            // Back to content state
            _State = ParseState.Content;
            // Returns comment
            String comment = GetCurrentRead(true);
            return new ParsedComment() {
                Position = _CurrentPosition,
                Text = comment.Substring(4, comment.Length - 7).Trim()
            };
        }

        /// <summary>
        /// Parse a start tag
        /// </summary>
        protected ParsedToken ParseStartTag()
        {
            _StartTagPosition = _CurrentPosition;
            int c = ReadChar();
            // Comments ?
            if (c == '!')
            {
                // Expect '--'
                if (ReadChar() != '-' || ReadChar() != '-')
                    throw new ParseError("Comments need to start with '<!--'.", _NextCharPosition);
                return ParseComment();
            }
            // Process instruction ?
            if (c == '?')
            {
                _State = ParseState.ProcessInstruction;
                c = ReadChar(false);
            }
            else if (c == '/')
            {
                _State = ParseState.EndTag;
                c = ReadChar(false);
            }
            else
            {
                _State = ParseState.Tag;
            }
            // Pass whitespace
            while (c >= 0 && Char.IsWhiteSpace((Char)c)) c = ReadChar();
            // Tagname
            if (c < 0 || !Char.IsLetterOrDigit((Char)c))
                throw new ParseError("Invalid tag name. Need to start with an alphanumeric", _NextCharPosition);
            // Loop tag name
            _CurrentRead = null;
            AddToCurrentRead((Char)c);
            while ((c = ReadChar(false)) >= 0 && (Char.IsLetterOrDigit((Char)c) || c == '.' || c == ':' || c == '-'))
                AddToCurrentRead((Char)c);
            // If EndTag
            if (_State == ParseState.EndTag)
            {
                _CurrentTag = ParsedTag.EndTag(GetCurrentRead(true));
                _CurrentTag.Position = _StartTagPosition;

                // Pass whitespace
                while (c >= 0 && Char.IsWhiteSpace((Char)c)) c = ReadChar(false);
                try
                {
                    if (c < 0) throw new ParseError("Unexpected end of stream.", _NextCharPosition);
                    if (IsAttributeNameChar((Char)c)) throw new ParseError("End tag can't contains attribute.", _NextCharPosition);
                    if (c != '>') throw new ParseError("Unexpected char. End tag not closed.", _NextCharPosition);
                }
                catch
                {
                    // Reset steam
                    while (c >= 0 && c != '<' && c != '>') c = ReadChar(false);
                    if (c == '<') SaveChar((Char)c);
                    throw;
                }
                _State = ParseState.Content;
                var result = _CurrentTag;
                _CurrentTag = null;
                return result;
            }
            // Create the tag
            if (c >= 0) SaveChar((Char)c);
            _CurrentTag = _State == ParseState.Tag ? ParsedTag.OpenTag(GetCurrentRead(true)) : ParsedTag.OpenProcessInstruction(GetCurrentRead(true));
            _CurrentTag.Position = _StartTagPosition;
            return _CurrentTag;
        }

        static bool IsAttributeNameChar(Char c)
        {
            return !Char.IsWhiteSpace(c) && c != '\0' && c != '"' && c != '\'' && c != '<' && c != '>' && c != '/' && c != '=' && c != '&';
        }

        /// <summary>
        /// Parse in tag
        /// </summary>
        protected ParsedToken ParseInTag()
        {
            _CurrentRead = null;
            // Whitespaces
            int c;
            while ((c = ReadChar(false)) >= 0 && Char.IsWhiteSpace((Char)c)) ;
            var cpos = _CharPosition;
            // EOF ?
            if (c < 0)
            {
                _CurrentTag = null;
                _State = ParseState.Content;
                throw new ParseError("Unexpected end of file. Tag not closed.", _NextCharPosition);
            }
            // End of auto closed tag ?
            else if (c == '/' && _State == ParseState.Tag)
            {
                bool spaces = false;
                while ((c = ReadChar(false)) >= 0 && Char.IsWhiteSpace((Char)c)) spaces = true;
                if (c != '>')
                {
                    // Prepare a correct next tag
                    SaveChar('>');
                    SaveChar('/');
                    throw new ParseError("Invalid char after '/'. End of auto closed tag expected.", cpos);
                }
                if (spaces)
                {
                    // Prepare a correct next tag
                    SaveChar('>');
                    SaveChar('/');
                    // Raise the error
                    throw new ParseError("Invalid auto closed tag, '/' need to be follow by '>'.", cpos);
                }
                // Returns autoclosed
                var result = ParsedTag.AutoClosedTag(_CurrentTag.TagName);
                result.Position = cpos;
                _CurrentTag = null;
                _CurrentRead = null;
                _State = ParseState.Content;
                return result;
            }
            // End of process instruction
            else if (c == '?' && _State == ParseState.ProcessInstruction)
            {
                c = ReadChar(false);
                if (c != '>') throw new ParseError("Invalid char after '?'. End of process instruction expected.", cpos);
                // Returns processinstruction
                var result = ParsedTag.CloseProcessInstruction(_CurrentTag.TagName);
                result.Position = cpos;
                _CurrentTag = null;
                _CurrentRead = null;
                _State = ParseState.Content;
                return result;
            }
            else if (c == '>')
            {
                if (_State == ParseState.ProcessInstruction)
                    throw new ParseError("A process instruction need to be closed with '?>'.", cpos);
                // Returns close
                var result = ParsedTag.CloseTag(_CurrentTag.TagName);
                result.Position = cpos;
                _CurrentTag = null;
                _CurrentRead = null;
                _State = ParseState.Content;
                return result;
            }
            // Get the attribute name
            if (!IsAttributeNameChar((Char)c))
                throw new ParseError("Unexpected character.", cpos);
            AddToCurrentRead((Char)c);
            while ((c = ReadChar(false)) >= 0 && IsAttributeNameChar((Char)c))
                AddToCurrentRead((Char)c);
            if (c >= 0) SaveChar((Char)c);
            String attrName = GetCurrentRead(true);
            ParsePosition attrPos = cpos;
            // Whitespaces
            while ((c = ReadChar(false)) >= 0 && Char.IsWhiteSpace((Char)c)) ;
            // Attribute whithout value
            if (c != '=')
            {
                SaveChar((Char)c);
                // Attribute whithout content
                return new ParsedAttribute() {
                    Position = attrPos,
                    Name = attrName,
                    Value = null,
                    Quote = '\0'
                };
            }
            // Whitespaces
            while ((c = ReadChar(false)) >= 0 && Char.IsWhiteSpace((Char)c)) ;
            // Search the value
            if (c == 0 || c == '/' || c == '?' || c == '>')
            {
                _CurrentAttr = new ParsedAttribute() {
                    Position = attrPos,
                    Name = attrName,
                    Value = null,
                    Quote = '\0'
                };
                if (c > 0) SaveChar((Char)c);
                throw new ParseError("Attribute value expected.", _NextCharPosition);
            }
            // Quoted value ?
            _CurrentRead = null;
            char quote = '\0';
            if (c == '"' || c == '\'')
            {
                quote = (char)c;
                while ((c = ReadChar(false)) >= 0 && c != quote)
                    AddToCurrentRead((Char)c);
                _CurrentAttr = new ParsedAttribute() {
                    Position = attrPos,
                    Name = attrName,
                    Value = HEntity.HtmlDecode(GetCurrentRead(true)),
                    Quote = quote
                };
                if (c < 0)
                    throw new ParseError("Unexpected end of file. Attribute is not closed.", _NextCharPosition);
                var result = _CurrentAttr;
                _CurrentAttr = null;
                return result;
            }
            // Unquoted value
            AddToCurrentRead((Char)c);
            while ((c = ReadChar(false)) >= 0 && !Char.IsWhiteSpace((Char)c) && c != '"' && c != '\'' && c != '=' && c != '<' && c != '>' && c != '`')
                AddToCurrentRead((Char)c);
            SaveChar((Char)c);
            return new ParsedAttribute() {
                Position = attrPos,
                Name = attrName,
                Value = HEntity.HtmlDecode(GetCurrentRead(true)),
                Quote = quote
            };
        }

        /// <summary>
        /// Parse the next element
        /// </summary>
        public ParsedToken Parse()
        {
            int c;
            // If end of stream when stop here
            if (EOF) return null;
            // Current tag defined
            if (_CurrentTag != null)
            {
                // End tag : error while end tag parsing, reset parser
                if (_CurrentTag.TokenType == ParsedTokenType.EndTag)
                {
                    _CurrentRead = null;
                    _State = ParseState.Content;
                    LastParsed = _CurrentTag;
                    _CurrentTag = null;
                    return LastParsed;
                }
            }
            // Current Attribute defined
            if (_CurrentAttr != null)
            {
                LastParsed = _CurrentAttr;
                _CurrentAttr = null;
                return LastParsed;
            }
            // Current Read not empty ?
            if (_CurrentRead != null)
            {
                bool returnLast = true;
                switch (_State)
                {
                    // Returns a non closed comment
                    case ParseState.Comment:
                        String comment = GetCurrentRead(true);
                        LastParsed = new ParsedComment() {
                            Position = _CurrentPosition,
                            Text = comment.Substring(4).TrimStart()
                        };
                        break;
                    // Returns a text
                    case ParseState.Content:
                        LastParsed = new ParsedText() {
                            Position = _CurrentPosition,
                            Text = GetCurrentRead(true)
                        };
                        break;
                    // Returns a text
                    case ParseState.Tag:
                        LastParsed = new ParsedText() {
                            Position = _CurrentPosition,
                            Text = GetCurrentRead(true)
                        };
                        break;
                    // We forget the result
                    //default:
                    //    returnLast = false;
                    //    break;
                }
                _State = ParseState.Content;
                if (returnLast)
                    return LastParsed;
            }
            // Read loop
            while (true)
            {
                // Read next char
                c = ReadChar();
                // EOF ?
                if (c < 0)
                {
                    // Check unexpected EOF
                    if (_State != ParseState.Content)
                    {
                        _State = ParseState.Content;
                        throw new ParseError("End of file unexpected.", _NextCharPosition);
                    }
                    // Stop the parsing
                    LastParsed = null;
                    EOF = true;
                    return null;
                }
                // Other case
                switch (_State)
                {
                    // In text
                    case ParseState.Content:
                        if (c == '<')
                        {
                            LastParsed = ParseStartTag();
                        }
                        else
                        {
                            LastParsed = ParseText();
                        }
                        return LastParsed;
                    // In tag or process instruction
                    case ParseState.Tag:
                    case ParseState.ProcessInstruction:
                        SaveChar((Char)c);
                        LastParsed = ParseInTag();
                        return LastParsed;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Current reader
        /// </summary>
        public TextReader Reader { get; private set; }

        /// <summary>
        /// Position reading
        /// </summary>
        public ParsePosition ReadPosition { get { return _NextCharPosition; } }

        /// <summary>
        /// The last parsed result
        /// </summary>
        /// <remarks>
        /// Null while the parsing is not started, and when then parsing is finished. 
        /// See <see cref="EOF"/> for the end of stream is reached.
        /// </remarks>
        public ParsedToken LastParsed { get; private set; }

        /// <summary>
        /// End of the stream
        /// </summary>
        public bool EOF { get; private set; }

    }

}
