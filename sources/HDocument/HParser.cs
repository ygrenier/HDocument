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
            // In doctype
            Doctype,
            /// <summary>
            /// In a tag
            /// </summary>
            Tag,
            /// <summary>
            /// In a end tag
            /// </summary>
            EndTag,
        }

        /// <summary>
        /// Read char information
        /// </summary>
        protected struct CharInfo : IEquatable<CharInfo>
        {
            /// <summary>
            /// EOF
            /// </summary>
            public static readonly CharInfo EOF = new CharInfo(-1, ParsePosition.None);

            /// <summary>
            /// New CharInfo
            /// </summary>
            public CharInfo(int c, ParsePosition pos)
                : this()
            {
                CharValue = c;
                Position = pos;
            }

            /// <summary>
            /// Equality with another char
            /// </summary>
            public bool Equals(CharInfo c)
            {
                return CharValue.Equals(c.CharValue);
            }

            /// <summary>
            /// Equals
            /// </summary>
            public override bool Equals(object obj)
            {
                if (obj is CharInfo)
                    return Equals((CharInfo)obj);
                return base.Equals(obj);
            }

            /// <summary>
            /// Hash code
            /// </summary>
            public override int GetHashCode()
            {
                return CharValue.GetHashCode();
            }

            /// <summary>
            /// To string
            /// </summary>
            public override string ToString()
            {
                return AsChar.ToString();
            }

            /// <summary>
            /// Compare with an another CharInfo
            /// </summary>
            public static bool operator ==(CharInfo a, CharInfo b) { return a.CharValue == b.CharValue; }

            /// <summary>
            /// Compare with an another CharInfo
            /// </summary>
            public static bool operator !=(CharInfo a, CharInfo b) { return a.CharValue != b.CharValue; }

            /// <summary>
            /// Compare with an int value
            /// </summary>
            public static bool operator ==(CharInfo ci, int value) { return ci.CharValue == value; }

            /// <summary>
            /// Compare with an int value
            /// </summary>
            public static bool operator !=(CharInfo ci, int value) { return ci.CharValue != value; }

            /// <summary>
            /// Compare with a char value
            /// </summary>
            public static bool operator ==(CharInfo ci, Char value) { return ci.AsChar == value; }

            /// <summary>
            /// Compare with a char value
            /// </summary>
            public static bool operator !=(CharInfo ci, Char value) { return ci.AsChar != value; }

            /// <summary>
            /// Char int value
            /// </summary>
            public int CharValue { get; private set; }

            /// <summary>
            /// Position
            /// </summary>
            public ParsePosition Position { get; private set; }

            /// <summary>
            /// Char
            /// </summary>
            public Char AsChar { get { return (Char)CharValue; } }
        }

        /// <summary>
        /// Parser buffer
        /// </summary>
        protected sealed class ParseBuffer : IDisposable
        {
            ParserSourceReader _Reader;
            Queue<Char> _Buffer;

            /// <summary>
            /// New buffer
            /// </summary>
            internal ParseBuffer(ParserSourceReader reader)
            {
                _Reader = reader;
                _Buffer = new Queue<char>();
            }

            /// <summary>
            /// Close the buffer
            /// </summary>
            public void Dispose()
            {
                _Reader.UnregisterBuffer(this);
            }

            /// <summary>
            /// Add a char in the buffer
            /// </summary>
            internal void Push(CharInfo c)
            {
                if (Count == 0)
                    Position = c.Position;
                _Buffer.Enqueue(c.AsChar);
            }

            /// <summary>
            /// Get the buffer as string
            /// </summary>
            public override string ToString()
            {
                return new String(_Buffer.ToArray());
            }

            /// <summary>
            /// Count of chars
            /// </summary>
            public int Count { get { return _Buffer.Count; } }

            /// <summary>
            /// Position of the start of the buffer
            /// </summary>
            public ParsePosition Position { get; private set; }

        }

        /// <summary>
        /// Parser source reader
        /// </summary>
        protected class ParserSourceReader
        {
            bool _LastWasCR;
            IList<ParseBuffer> _RegisteredBuffers;
            Stack<CharInfo> _UnreadBuffer;
            ParsePosition _Position;

            /// <summary>
            /// New reader
            /// </summary>
            public ParserSourceReader(TextReader source)
            {
                this.Reader = source;
                _UnreadBuffer = new Stack<CharInfo>();
            }

            /// <summary>
            /// Open a new buffer
            /// </summary>
            public ParseBuffer OpenBuffer()
            {
                ParseBuffer result = new ParseBuffer(this);
                if (_RegisteredBuffers == null)
                    _RegisteredBuffers = new List<ParseBuffer>();
                _RegisteredBuffers.Add(result);
                return result;
            }

            /// <summary>
            /// Remove a buffer
            /// </summary>
            internal void UnregisterBuffer(ParseBuffer parseBuffer)
            {
                if (_RegisteredBuffers != null)
                {
                    _RegisteredBuffers.Remove(parseBuffer);
                    if (_RegisteredBuffers.Count == 0)
                        _RegisteredBuffers = null;
                }
            }

            /// <summary>
            /// Peek the current char
            /// </summary>
            public CharInfo Peek()
            {
                // If unread char returns the peek
                if (_UnreadBuffer.Count > 0)
                    return _UnreadBuffer.Peek();
                // Read
                int c = Reader.Peek();
                if (c < 0) return CharInfo.EOF;
                return new CharInfo(c, Position);
            }

            /// <summary>
            /// Read the next char
            /// </summary>
            public CharInfo Read()
            {
                CharInfo res = CharInfo.EOF;
                bool moveNextPosition = true;
                // If unread char get the first
                if (_UnreadBuffer.Count > 0)
                {
                    res = _UnreadBuffer.Pop();
                    // We calculate the next position if the buffer is empty
                    moveNextPosition = _UnreadBuffer.Count == 0;
                }
                else
                {
                    res = new CharInfo(Reader.Read(), Position);
                    // Add in buffer only if new char
                    if (_RegisteredBuffers != null)
                    {
                        foreach (var buffer in _RegisteredBuffers)
                        {
                            buffer.Push(res);
                        }
                    }
                }
                // Move the position ?
                if (res != CharInfo.EOF && moveNextPosition)
                {
                    _Position = res.Position;
                    if (res == '\n')
                    {
                        if (!_LastWasCR)
                            _Position = _Position.NextLine(true);
                        else
                            _Position = _Position.AddPosition(1);
                    }
                    else if (res == '\r')
                    {
                        _Position = _Position.NextLine(true);
                    }
                    else
                    {
                        _Position++;
                    }
                }
                _LastWasCR = res == '\r';
                // Returns result
                return res;
            }

            /// <summary>
            /// Save a char as unread
            /// </summary>
            /// <param name="c"></param>
            public void Unread(CharInfo c)
            {
                _UnreadBuffer.Push(c);
            }

            /// <summary>
            /// Reader source
            /// </summary>
            public TextReader Reader { get; private set; }

            /// <summary>
            /// Current position
            /// </summary>
            public ParsePosition Position
            {
                get
                {
                    // If unread buffer, returns the position in the buffer
                    if (_UnreadBuffer.Count > 0)
                        return _UnreadBuffer.Peek().Position;
                    // Returns the current position
                    return _Position;
                }
            }

        }

        #endregion

        ParseState _State;
        ParsedToken _CurrentToken;
        ParsedAttribute _CurrentAttr;
        StringBuilder _CurrentRead;
        ParsePosition _CurrentPosition, _StartTagPosition;
        Stack<Char> _Buffer;
        ParseBuffer _TagBuffer;

        /// <summary>
        /// Create a new parser
        /// </summary>
        public HParser(TextReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            this.SourceReader = new ParserSourceReader(reader);
            this.EOF = false;
            this._Buffer = new Stack<char>();
            this._CurrentRead = null;
            this._State = ParseState.Content;
            this._CurrentPosition = new ParsePosition();
        }

        /// <summary>
        /// Save a char in the buffer
        /// </summary>
        protected void SaveChar(CharInfo c)
        {
            SourceReader.Unread(c);
        }

        /// <summary>
        /// Add a char in the current read buffer
        /// </summary>
        protected void AddToCurrentRead(CharInfo c)
        {
            if (_CurrentRead == null)
            {
                _CurrentRead = new StringBuilder(10);
                _CurrentPosition = c.Position;
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
        protected CharInfo ReadChar(bool saveToCurrent = true)
        {
            var ci = SourceReader.Read();
            if (ci != CharInfo.EOF)
            {
                if (saveToCurrent)
                {
                    AddToCurrentRead(ci);
                }
            }
            return ci;
        }

        void ResetTagBuffer()
        {
            if (_TagBuffer != null)
                _TagBuffer.Dispose();
            _TagBuffer = null;
        }

        /// <summary>
        /// Parse a text content
        /// </summary>
        protected ParsedContent ParseText()
        {
            // Loop read
            CharInfo c;
            while ((c = ReadChar(false)) != CharInfo.EOF && c != '<')
                AddToCurrentRead(c);
            if (c != CharInfo.EOF)
                SaveChar(c);
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
            CharInfo c;
            int s = 0;
            // s : state
            // 0 : in comment
            // 1 : first end tag '-'
            // 2 : second end tag '-'
            while ((c = ReadChar()) != CharInfo.EOF)
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
            if (c == CharInfo.EOF)
                throw new ParseError("End of file unexpected, comment not closed.", ReadPosition);
            // Back to content state
            _State = ParseState.Content;
            ResetTagBuffer();
            // Returns comment
            String comment = GetCurrentRead(true);
            return new ParsedComment() {
                Position = _CurrentPosition,
                Text = comment.Substring(4, comment.Length - 7).Trim()
            };
        }

        /// <summary>
        /// Parse a doctype
        /// </summary>
        /// <returns></returns>
        protected ParsedToken ParseDoctype()
        {
            // We are in doctype
            _State = ParseState.Doctype;
            var stag = _CurrentPosition;
            // read DOCTYPE
            StringBuilder dc = new StringBuilder();
            dc.Append(ReadChar().AsChar);
            CharInfo c;
            while ((c = ReadChar()) != CharInfo.EOF && Char.IsLetter(c.AsChar))
                dc.Append(c.AsChar);
            if (dc.ToString().ToLower() != "doctype")
                throw new ParseError("DOCTYPE expected.", _CurrentPosition);
            // Read loop
            int s = 0; char quote = '\0';
            // s : state
            // 0 : in tag
            // 1 : in value
            List<String> values = new List<string>();
            while ((c = ReadChar()) != CharInfo.EOF)
            {
                if (s == 0)
                {
                    // Start a value ?
                    if (c == '"' || c == '\'')
                    {
                        quote = c.AsChar;
                        _CurrentRead = null;
                        s = 1;
                    }
                    else if (Char.IsLetterOrDigit(c.AsChar))
                    {
                        quote = '\0';
                        _CurrentRead = null;
                        AddToCurrentRead(c);
                        s = 1;
                    }
                    else if (c == '>')
                    {
                        break;
                    }
                }
                else if (s == 1)
                {
                    // End of the value ?
                    if (quote == c.AsChar)
                    {
                        var v = GetCurrentRead(true);
                        values.Add(v.Substring(0, v.Length - 1));
                        s = 0;
                    }
                    else if (c == '>' || (quote == '\0' && !Char.IsLetterOrDigit(c.AsChar)))
                    {
                        var v = GetCurrentRead(true);
                        values.Add(v.Substring(0, v.Length - 1));
                        SaveChar(c);
                        s = 0;
                    }
                }
            }
            if (s == 1 && _CurrentRead != null)
            {
                values.Add(GetCurrentRead(true));
            }
            _CurrentRead = null;
            _CurrentToken = new ParsedDoctype() {
                Position = stag,
                Values = values.ToArray()
            };
            if (c == CharInfo.EOF)
                throw new ParseError("End of file unexpected, doctype not closed.", ReadPosition);
            // Back to content state
            _State = ParseState.Content;
            ResetTagBuffer();
            // Returns doctype
            var result = _CurrentToken;
            _CurrentToken = null;
            return result;
        }

        /// <summary>
        /// Parse a start tag
        /// </summary>
        protected ParsedToken ParseStartTag()
        {
            _StartTagPosition = _CurrentPosition;
            _TagBuffer = SourceReader.OpenBuffer();
            CharInfo c = ReadChar();
            // Comments ?
            if (c == '!')
            {
                // Expect '--' or 'DOCTYPE'
                c = ReadChar();
                if (Char.IsLetter(c.AsChar))
                {
                    SaveChar(c);
                    return ParseDoctype();
                }
                else if (c == '-')
                {
                    if (ReadChar() != '-')
                        throw new ParseError("Comments need to start with '<!--'.", ReadPosition);
                    return ParseComment();
                }
                throw new ParseError("Comment or DOCTYPE expected.", ReadPosition);
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
            while (c != CharInfo.EOF && Char.IsWhiteSpace(c.AsChar)) c = ReadChar();
            // Tagname
            if (c == CharInfo.EOF || !Char.IsLetterOrDigit(c.AsChar))
                throw new ParseError("Invalid tag name. Need to start with an alphanumeric", ReadPosition);
            // Loop tag name
            _CurrentRead = null;
            AddToCurrentRead(c);
            while ((c = ReadChar(false)) != CharInfo.EOF && (Char.IsLetterOrDigit(c.AsChar) || c == '.' || c == ':' || c == '-'))
                AddToCurrentRead(c);
            // If EndTag
            if (_State == ParseState.EndTag)
            {
                _CurrentToken = ParsedTag.EndTag(GetCurrentRead(true));
                _CurrentToken.Position = _StartTagPosition;

                // Pass whitespace
                while (c != CharInfo.EOF && Char.IsWhiteSpace(c.AsChar)) c = ReadChar(false);
                try
                {
                    if (c == CharInfo.EOF) throw new ParseError("Unexpected end of stream.", ReadPosition);
                    if (IsAttributeNameChar(c.AsChar)) throw new ParseError("End tag can't contains attribute.", ReadPosition);
                    if (c != '>') throw new ParseError("Unexpected char. End tag not closed.", ReadPosition);
                }
                catch
                {
                    // Reset steam
                    while (c != CharInfo.EOF && c != '<' && c != '>') c = ReadChar(false);
                    if (c == '<') SaveChar(c);
                    throw;
                }
                _State = ParseState.Content;
                ResetTagBuffer();
                var result = _CurrentToken;
                _CurrentToken = null;
                return result;
            }
            // Create the tag
            if (c != CharInfo.EOF) SaveChar(c);
            _CurrentToken = _State == ParseState.Tag ? ParsedTag.OpenTag(GetCurrentRead(true)) : ParsedTag.OpenProcessInstruction(GetCurrentRead(true));
            _CurrentToken.Position = _StartTagPosition;
            return _CurrentToken;
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
            CharInfo c;
            while ((c = ReadChar(false)) != CharInfo.EOF && Char.IsWhiteSpace(c.AsChar)) ;
            var cpos = c.Position;
            // EOF ?
            if (c == CharInfo.EOF)
            {
                _CurrentToken = null;
                _State = ParseState.Content;
                ResetTagBuffer();
                throw new ParseError("Unexpected end of file. Tag not closed.", ReadPosition);
            }
            // End of auto closed tag ?
            else if (c == '/' && _State == ParseState.Tag)
            {
                CharInfo saveSlash = c;
                bool spaces = false;
                while ((c = ReadChar(false)) != CharInfo.EOF && Char.IsWhiteSpace(c.AsChar)) spaces = true;
                if (c != '>')
                {
                    // Prepare a correct next tag
                    SaveChar(new CharInfo('>', (c == CharInfo.EOF) ? saveSlash.Position : c.Position));
                    SaveChar(saveSlash);
                    throw new ParseError("Invalid char after '/'. End of auto closed tag expected.", cpos);
                }
                if (spaces)
                {
                    // Prepare a correct next tag
                    SaveChar(c);
                    SaveChar(saveSlash);
                    // Raise the error
                    throw new ParseError("Invalid auto closed tag, '/' need to be follow by '>'.", cpos);
                }
                // Returns autoclosed
                var result = ParsedTag.AutoClosedTag(((ParsedTag)_CurrentToken).TagName);
                result.Position = cpos;
                _CurrentToken = null;
                _CurrentRead = null;
                _State = ParseState.Content;
                ResetTagBuffer();
                return result;
            }
            // End of process instruction
            else if (c == '?' && _State == ParseState.ProcessInstruction)
            {
                c = ReadChar(false);
                if (c != '>') throw new ParseError("Invalid char after '?'. End of process instruction expected.", cpos);
                // Returns processinstruction
                var result = ParsedTag.CloseProcessInstruction(((ParsedTag)_CurrentToken).TagName);
                result.Position = cpos;
                _CurrentToken = null;
                _CurrentRead = null;
                _State = ParseState.Content;
                ResetTagBuffer();
                return result;
            }
            else if (c == '>')
            {
                // Check tag
                if (_State == ParseState.ProcessInstruction)
                    throw new ParseError("A process instruction need to be closed with '?>'.", cpos);
                // Returns close
                var result = ParsedTag.CloseTag(((ParsedTag)_CurrentToken).TagName);
                result.Position = cpos;
                _CurrentToken = null;
                _CurrentRead = null;
                _State = ParseState.Content;
                ResetTagBuffer();
                return result;
            }
            // Get the attribute name
            if (!IsAttributeNameChar(c.AsChar))
                throw new ParseError("Unexpected character.", cpos);
            AddToCurrentRead(c);
            while ((c = ReadChar(false)) != CharInfo.EOF && IsAttributeNameChar(c.AsChar))
                AddToCurrentRead(c);
            if (c != CharInfo.EOF) SaveChar(c);
            String attrName = GetCurrentRead(true);
            ParsePosition attrPos = cpos;
            // Whitespaces
            while ((c = ReadChar(false)) != CharInfo.EOF && Char.IsWhiteSpace(c.AsChar)) ;
            // Attribute whithout value
            if (c != '=')
            {
                SaveChar(c);
                // Attribute whithout content
                return new ParsedAttribute() {
                    Position = attrPos,
                    Name = attrName,
                    Value = null,
                    Quote = '\0'
                };
            }
            // Whitespaces
            while ((c = ReadChar(false)) != CharInfo.EOF && Char.IsWhiteSpace(c.AsChar)) ;
            // Search the value
            if (c == 0 || c == '/' || c == '?' || c == '>')
            {
                _CurrentAttr = new ParsedAttribute() {
                    Position = attrPos,
                    Name = attrName,
                    Value = null,
                    Quote = '\0'
                };
                if (c != CharInfo.EOF) SaveChar(c);
                throw new ParseError("Attribute value expected.", ReadPosition);
            }
            // Quoted value ?
            _CurrentRead = null;
            char quote = '\0';
            if (c == '"' || c == '\'')
            {
                quote = c.AsChar;
                while ((c = ReadChar(false)) != CharInfo.EOF && c != quote)
                    AddToCurrentRead(c);
                _CurrentAttr = new ParsedAttribute() {
                    Position = attrPos,
                    Name = attrName,
                    Value = HEntity.HtmlDecode(GetCurrentRead(true)),
                    Quote = quote
                };
                if (c == CharInfo.EOF)
                    throw new ParseError("Unexpected end of file. Attribute is not closed.", ReadPosition);
                var result = _CurrentAttr;
                _CurrentAttr = null;
                return result;
            }
            // Unquoted value
            AddToCurrentRead(c);
            while ((c = ReadChar(false)) != CharInfo.EOF && !Char.IsWhiteSpace(c.AsChar) && c != '"' && c != '\'' && c != '=' && c != '<' && c != '>' && c != '`')
                AddToCurrentRead(c);
            SaveChar(c);
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
            CharInfo c;
            // If end of stream when stop here
            if (EOF) return null;
            // Current token defined
            if (_CurrentToken != null)
            {
                // End tag out Doctype : error while end tag or doctype parsing, reset parser
                if (_CurrentToken.TokenType == ParsedTokenType.EndTag || _CurrentToken.TokenType == ParsedTokenType.Doctype)
                {
                    _CurrentRead = null;
                    _State = ParseState.Content;
                    ResetTagBuffer();
                    LastParsed = _CurrentToken;
                    _CurrentToken = null;
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
                    case ParseState.Doctype:
                    case ParseState.ProcessInstruction:
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
                ResetTagBuffer();
                if (returnLast)
                    return LastParsed;
            }
            // Read loop
            while (true)
            {
                // Read next char
                c = ReadChar();
                // EOF ?
                if (c == CharInfo.EOF)
                {
                    // Check unexpected EOF
                    if (_State != ParseState.Content)
                    {
                        _State = ParseState.Content;
                        ResetTagBuffer();
                        throw new ParseError("End of file unexpected.", ReadPosition);
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
                    case ParseState.Doctype:
                    case ParseState.ProcessInstruction:
                        SaveChar(c);
                        LastParsed = ParseInTag();
                        return LastParsed;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Parse a content as raw text.
        /// </summary>
        /// <remarks>
        /// This method is used for parsing the content of the script, style tag content.
        /// The parsing is continue until matching the end of the <paramref name="tag"/>.
        /// If <paramref name="tag"/> is null or empty then we accept all endtag.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Raised when the parser is not in a normal state : all tag need to be closed.
        /// </exception>
        /// <param name="tag">Tag name for the end tag expected.</param>
        /// <returns>Content</returns>
        public ParsedText ParseContentText(String tag)
        {
            // Verify
            if (this._State != ParseState.Content)
                throw new InvalidOperationException("Can't read a content in a opened tag.");
            // Read loop
            var start = SourceReader.Position;
            CharInfo c;
            while ((c = ReadChar(false)) != CharInfo.EOF)
            {
                // End detected ?
                if (c == '<')
                {
                    var endTagPos = c.Position;
                    StringBuilder saveTag = new StringBuilder(15);
                    saveTag.Append(c);
                    while ((c = ReadChar(false)) != CharInfo.EOF && Char.IsWhiteSpace(c.AsChar)) saveTag.Append(c.AsChar);
                    if (c == '/')
                    {
                        // Pass '/'
                        saveTag.Append(c);
                        while ((c = ReadChar(false)) != CharInfo.EOF && Char.IsWhiteSpace(c.AsChar)) saveTag.Append(c.AsChar);
                        if (c != CharInfo.EOF)
                        {
                            // Pass tag name
                            StringBuilder tagName = new StringBuilder(10);
                            saveTag.Append(c.AsChar);
                            tagName.Append(c.AsChar);
                            while ((c = ReadChar(false)) != CharInfo.EOF && IsAttributeNameChar(c.AsChar))
                            {
                                saveTag.Append(c.AsChar);
                                tagName.Append(c.AsChar);
                            }
                            // We find the good end tag ?
                            if (c != CharInfo.EOF)
                            {
                                if (String.IsNullOrEmpty(tag) || String.Equals(tagName.ToString(), tag, StringComparison.OrdinalIgnoreCase))
                                {
                                    SaveChar(c);
                                    // Search the good end
                                    while ((c = ReadChar(false)) != CharInfo.EOF && Char.IsWhiteSpace(c.AsChar)) saveTag.Append(c.AsChar);
                                    if (c == '>')
                                    {
                                        // Save the end tag for the next parse
                                        _CurrentToken = ParsedTag.EndTag(tagName.ToString());
                                        _CurrentToken.Position = endTagPos;
                                        // Exit the loop
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    // If here then we don't find a good end tag we convert to 'text'
                    var etp = endTagPos;
                    foreach (var st in saveTag.ToString())
                    {
                        AddToCurrentRead(new CharInfo(st, etp++));
                    }
                }
                // 
                AddToCurrentRead(c);
            }
            if (c != CharInfo.EOF)
                SaveChar(c);
            // Returns parse result
            LastParsed = new ParsedText() {
                Position = start,
                Text = GetCurrentRead(true)
            };
            return (ParsedText)LastParsed;
        }

        /// <summary>
        /// Current reader
        /// </summary>
        public TextReader Reader { get { return SourceReader.Reader; } }

        /// <summary>
        /// Current source reader
        /// </summary>
        protected ParserSourceReader SourceReader { get; private set; }

        /// <summary>
        /// Position reading
        /// </summary>
        public ParsePosition ReadPosition { get { return SourceReader.Position; } }

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
