using HDoc.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HDoc
{

    /// <summary>
    /// Serializer for HTML
    /// </summary>
    public class HSerializer
    {

        #region Helpers

        /// <summary>
        /// List of void elements
        /// </summary>
        protected static String[] VoidElements = new String[] { 
            "area", "base", "br", "col", "embed", "hr", "img", "input", 
            "keygen", "link", "meta", "param", "source", "track", "wbr" 
        };

        /// <summary>
        /// List of raw text elements
        /// </summary>
        protected static String[] RawTextElements = new String[]{
            "script", "style"
        };

        /// <summary>
        /// List of escapable raw text elements
        /// </summary>
        protected static String[] EscapableRawTextElements = new String[]{
            "textarea", "title"
        };

        /// <summary>
        /// Check if a tag is a void element
        /// </summary>
        protected virtual bool IsVoidElement(string tag)
        {
            return VoidElements.Any(ve => String.Equals(ve, tag, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Check if a tag is a raw text element
        /// </summary>
        protected virtual bool IsRawElement(String tag)
        {
            return RawTextElements.Any(ve => String.Equals(ve, tag, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Check if a tag is an escapable raw text element
        /// </summary>
        protected virtual bool IsEscapableRawElement(String tag)
        {
            return EscapableRawTextElements.Any(ve => String.Equals(ve, tag, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Serialise an HTML document
        /// </summary>
        public void Serialize(HDocument html, TextWriter writer)
        {
            if (html == null) throw new ArgumentNullException("html");
            if (writer == null) throw new ArgumentNullException("writer");
            SerializeNode(html, writer);
        }

        /// <summary>
        /// Direct serialization of a HTML document to string
        /// </summary>
        public String Serialize(HDocument html)
        {
            if (html == null) throw new ArgumentNullException("html");
            return SerializeNode(html);
        }

        /// <summary>
        /// Serialize a node
        /// </summary>
        protected virtual void SerializeNode(HNode node, TextWriter writer)
        {
            if (node is HDocument)
                SerializeDocument((HDocument)node, writer);
            else if (node is HDocumentType)
                SerializeDocumentType((HDocumentType)node, writer);
            else if (node is HXmlDeclaration)
                SerializeXmlDeclaration((HXmlDeclaration)node, writer);
            else if (node is HComment)
                SerializeComment((HComment)node, writer);
            else if (node is HCData)
                SerializeCData((HCData)node, writer);
            else if (node is HText)
                SerializeText((HText)node, writer);
            else if (node is HElement)
                SerializeElement((HElement)node, writer);
        }

        /// <summary>
        /// Serialize a node
        /// </summary>
        public String SerializeNode(HNode node)
        {
            if (node == null) throw new ArgumentNullException("node");
            StringBuilder html = new StringBuilder();
            using (var writer = new StringWriter(html))
                SerializeNode(node, writer);
            return html.ToString();
        }

        /// <summary>
        /// Internal document serialization
        /// </summary>
        protected virtual void SerializeDocument
            (HDocument html, TextWriter writer)
        {
            SerializeContainer(html, writer);
        }

        /// <summary>
        /// Serialize a container
        /// </summary>
        protected virtual void SerializeContainer(HContainer container, TextWriter writer)
        {
            // Loop on all nodes
            foreach (var node in container.Nodes())
                SerializeNode(node, writer);
        }

        /// <summary>
        /// Serialize a comment
        /// </summary>
        protected virtual void SerializeComment(HComment comment, TextWriter writer)
        {
            writer.Write("<!-- ");
            writer.Write(HEntity.HtmlEncode(comment.Value));
            writer.Write(" -->");
        }

        /// <summary>
        /// Serialize a cdata
        /// </summary>
        protected virtual void SerializeCData(HCData cdata, TextWriter writer)
        {
            writer.Write("<![CDATA[");
            writer.Write(cdata.Value);
            writer.Write("]]>");
        }

        /// <summary>
        /// Serialize a text
        /// </summary>
        protected virtual void SerializeText(HText text, TextWriter writer)
        {
            writer.Write(HEntity.HtmlEncode(text.Value));
        }

        /// <summary>
        /// Serialize an element
        /// </summary>
        protected virtual void SerializeElement(HElement element, TextWriter writer)
        {
            writer.Write("<{0}", element.Name);
            foreach (var attr in element.Attributes())
                SerializeAttribute(attr.Name, attr.Value, writer);
            if (element.HasNodes || !IsVoidElement(element.Name))
            {
                writer.Write(">");
                if (IsRawElement(element.Name))
                {
                    SerializeRawText(element.Nodes(), writer);
                }
                else if (IsEscapableRawElement(element.Name))
                {
                    SerializeEscapableRawText(element.Nodes(), writer);
                }
                else
                {
                    SerializeContainer(element, writer);
                }
                writer.Write("</{0}>", element.Name);
            }
            else
            {
                writer.Write(" />");
            }
        }

        /// <summary>
        /// Serialize a raw text content
        /// </summary>
        protected virtual void SerializeRawText(IEnumerable<HNode> content, TextWriter writer)
        {
            foreach (var node in content.OfType<HText>())
                writer.Write(node.Value);
        }

        /// <summary>
        /// Serialize an escapable text content
        /// </summary>
        protected virtual void SerializeEscapableRawText(IEnumerable<HNode> content, TextWriter writer)
        {
            foreach (var node in content.OfType<HText>())
                writer.Write(HEntity.HtmlEncode(node.Value));
        }

        /// <summary>
        /// Serialize a xml declaration
        /// </summary>
        protected virtual void SerializeXmlDeclaration(HXmlDeclaration xmldecl, TextWriter writer)
        {
            writer.Write("<?xml");
            if (xmldecl.Version != null)
            {
                SerializeAttribute("version", xmldecl.Version, writer);
            }
            if (xmldecl.Encoding != null)
            {
                SerializeAttribute("encoding", xmldecl.Encoding, writer);
            }
            if (xmldecl.Standalone != null)
            {
                SerializeAttribute("standalone", xmldecl.Standalone, writer);
            }
            writer.Write(" ?>");
        }

        /// <summary>
        /// Serialize a document type
        /// </summary>
        protected virtual void SerializeDocumentType(HDocumentType doctype, TextWriter writer)
        {
            writer.Write("<!DOCTYPE");
            if (doctype.RootElement != null)
            {
                writer.Write(" ");
                writer.Write(doctype.RootElement);
            }
            if (doctype.KindDoctype != null || doctype.FPI != null || doctype.Uri != null)
            {
                writer.Write(" ");
                writer.Write(doctype.KindDoctype ?? "PUBLIC");
            }
            if (doctype.FPI != null)
            {
                writer.Write(" \"{0}\"", doctype.FPI);
            }
            if (doctype.Uri != null)
            {
                writer.Write(" \"{0}\"", doctype.Uri);
            }
            writer.Write(">");
        }

        /// <summary>
        /// Serialize an attribute
        /// </summary>
        protected virtual void SerializeAttribute(String name, String value, TextWriter writer)
        {
            if (!String.IsNullOrWhiteSpace(name))
            {
                writer.Write(" {0}=\"{1}\"", name.Trim(), HEntity.HtmlEncode(value));
            }
        }

        #endregion

        #region Deserialization

        void ProcessError(Exception error, Func<Exception, bool> errorHandler)
        {
            if (errorHandler != null)
                if (errorHandler(error))
                    return;
            throw error;
        }

        void Protect(Func<Exception, bool> errorHandler, Action action)
        {
            if (action == null) return;
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (errorHandler != null)
                    if (errorHandler(ex))
                        return;
                throw;
            }
        }

        T Protect<T>(Func<Exception, bool> errorHandler, Func<T> action)
        {
            if (action == null) return default(T);
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                if (errorHandler != null)
                    if (errorHandler(ex))
                        return default(T);
                throw;
            }
        }

        void ProtectAddOnPeek(Stack<HElement> stack, object content, Func<Exception, bool> errorHandler)
        {
            Protect(errorHandler, () => stack.Peek().Add(content));
        }

        /// <summary>
        /// Parse with error intercepts
        /// </summary>
        ParsedToken ParseNext(HParser parser, Func<Exception, bool> errorHandler)
        {
            ParsedToken result = null;
            do
            {
                try
                {
                    result = parser.Parse();
                    if (result == null) break;
                }
                catch (Exception ex)
                {
                    if (errorHandler != null)
                        if (errorHandler(ex))
                            continue;
                    throw;
                }
            } while (result == null);
            return result;
        }

        /// <summary>
        /// Parse a content text with error intercepts
        /// </summary>
        ParsedText ParseContentTextNext(String tagEnd, HParser parser, Func<Exception, bool> errorHandler)
        {
            ParsedText result = null;
            do
            {
                try
                {
                    result = parser.ParseContentText(tagEnd);
                    if (result == null) break;
                }
                catch (Exception ex)
                {
                    if (errorHandler != null)
                        if (errorHandler(ex))
                            continue;
                    throw;
                }
            } while (result == null);
            return result;
        }

        /// <summary>
        /// Deserialize a HTML document
        /// </summary>
        public HDocument DeserializeDocument(TextReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            // Create result
            HDocument result = new HDocument();
            result.Encoding = null;
            // Load nodes in the document
            result.Add(Deserialize(reader, err => {
                if (err is ParseError)
                {
                    result.AddParseError((ParseError)err);
                    return true;
                }
                return false;
            }));
            // Check encoding
            if (result.Encoding == null)
            {
                if (reader is StreamReader)
                    result.Encoding = ((StreamReader)reader).CurrentEncoding;
                else
                    result.Encoding = Encoding.UTF8;
            }
            return result;
        }

        /// <summary>
        /// Deserialize as a list of nodes
        /// </summary>
        public IEnumerable<HNode> Deserialize(TextReader reader, Func<Exception, bool> errorHandler = null)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            // Create the parser
            var parser = new HParser(reader);
            var token = ParseNext(parser, errorHandler);
            Stack<HElement> opened = new Stack<HElement>();
            HXmlDeclaration currentXDecl = null;
            String tag;
            bool inTag = false;
            HNode tokenToReturns = null;
            while (token != null)
            {
                switch (token.TokenType)
                {
                    case ParsedTokenType.Text:
                        var htxt = new HText(HEntity.HtmlDecode(((ParsedText)token).Text));
                        if (opened.Count > 0)
                            ProtectAddOnPeek(opened, htxt, errorHandler);
                        else
                            tokenToReturns = htxt;
                        break;
                    case ParsedTokenType.CData:
                        var hcd = new HCData(((ParsedCData)token).Text);
                        if (opened.Count > 0)
                            ProtectAddOnPeek(opened, hcd, errorHandler);
                        else
                            tokenToReturns = hcd;
                        break;
                    case ParsedTokenType.Comment:
                        var hcom = new HComment(HEntity.HtmlDecode(((ParsedComment)token).Text));
                        if (opened.Count > 0)
                            ProtectAddOnPeek(opened, hcom, errorHandler);
                        else
                            tokenToReturns = hcom;
                        break;
                    case ParsedTokenType.OpenTag:
                        opened.Push(new HElement(((ParsedTag)token).TagName));
                        inTag = true;
                        break;
                    case ParsedTokenType.AutoClosedTag:
                        System.Diagnostics.Debug.Assert(opened.Count > 0, "Opened tags are empty when receiving AutoClosedTag.");
                        System.Diagnostics.Debug.Assert(opened.Peek().Name == ((ParsedTag)token).TagName, "AutoClosedTag and opened element are not same tag name.");
                        var actag = opened.Pop();
                        inTag = false;
                        if (opened.Count > 0)
                            ProtectAddOnPeek(opened, actag, errorHandler);
                        else
                            tokenToReturns = actag;
                        break;
                    case ParsedTokenType.CloseTag:
                        System.Diagnostics.Debug.Assert(opened.Count > 0, "Opened tags are empty when receiving CloseTag.");
                        System.Diagnostics.Debug.Assert(opened.Peek().Name == ((ParsedTag)token).TagName, "CloseTag and opened element are not same tag name.");
                        // Tag with text content
                        inTag = false;
                        String tagName = opened.Peek().Name;
                        if (IsRawElement(tagName) || IsEscapableRawElement(tagName))
                        {
                            token = ParseContentTextNext(tagName, parser, errorHandler);
                            continue;
                        }
                        break;
                    case ParsedTokenType.EndTag:
                        tag = ((ParsedTag)token).TagName;
                        HElement helm;
                        // Close all elements that not matching the tag
                        while (opened.Count > 0 && !String.Equals(opened.Peek().Name, tag, StringComparison.OrdinalIgnoreCase))
                        {
                            helm = opened.Pop();
                            if (opened.Count > 0)
                                ProtectAddOnPeek(opened, helm, errorHandler);
                            else
                                yield return helm;
                        }
                        // If we are opened tag, then close it because we find our element
                        if (opened.Count > 0)
                        {
                            helm = opened.Pop();
                            if (opened.Count > 0)
                                ProtectAddOnPeek(opened, helm, errorHandler);
                            else
                                yield return helm;
                        }
                        break;
                    case ParsedTokenType.OpenProcessInstruction:
                        if (currentXDecl != null)
                        {
                            while ((token = ParseNext(parser, errorHandler)) != null && token.TokenType != ParsedTokenType.CloseProcessInstruction)
                                ;
                            ProcessError(new ParseError("XML declaration already opened."), errorHandler);
                        }
                        tag = ((ParsedTag)token).TagName;
                        if (!String.Equals("xml", tag, StringComparison.OrdinalIgnoreCase))
                        {
                            while ((token = ParseNext(parser, errorHandler)) != null && token.TokenType != ParsedTokenType.CloseProcessInstruction)
                                ;
                            ProcessError(new ParseError(String.Format("Unexpected '{0}' process instruction.", tag)), errorHandler);
                        }
                        currentXDecl = new HXmlDeclaration(null, null, null);
                        inTag = true;
                        break;
                    case ParsedTokenType.CloseProcessInstruction:
                        if (currentXDecl == null)
                        {
                            ProcessError(new ParseError("No XML declaration opened."), errorHandler);
                        }
                        else
                        {
                            if (opened.Count > 0)
                                ProtectAddOnPeek(opened, currentXDecl, errorHandler);
                            else
                                tokenToReturns = currentXDecl;
                        }
                        inTag = false;
                        currentXDecl = null;
                        break;
                    case ParsedTokenType.Doctype:
                        var vs = ((ParsedDoctype)token).Values ?? new String[0];
                        var hdt = new HDocumentType(
                            vs.Length > 0 ? vs[0] : null,
                            vs.Length > 1 ? vs[1] : null,
                            vs.Length > 2 ? vs[2] : null,
                            vs.Length > 3 ? vs[3] : null
                            );
                        if (opened.Count > 0)
                            ProtectAddOnPeek(opened, hdt, errorHandler);
                        else
                            tokenToReturns = hdt;
                        break;
                    case ParsedTokenType.Attribute:
                        var attr = (ParsedAttribute)token;
                        // Xml declaration ?
                        if (currentXDecl != null)
                        {
                            if (String.Equals("version", attr.Name, StringComparison.OrdinalIgnoreCase))
                                currentXDecl.Version = attr.Value;
                            else if (String.Equals("encoding", attr.Name, StringComparison.OrdinalIgnoreCase))
                                currentXDecl.Encoding = attr.Value;
                            else if (String.Equals("standalone", attr.Name, StringComparison.OrdinalIgnoreCase))
                                currentXDecl.Standalone = attr.Value;
                            else
                                ProcessError(new ParseError(String.Format("Invalid XML declaration attribute : ''", attr.Name)), errorHandler);
                        }
                        System.Diagnostics.Debug.Assert(opened.Count > 0, "No element opened for the attribtue.");
                        ProtectAddOnPeek(opened, new HAttribute(attr.Name, attr.Value), errorHandler);
                        break;
                    //default:
                    //    break;
                }
                // Returns a token if we have one
                if (tokenToReturns != null)
                {
                    yield return tokenToReturns;
                    tokenToReturns = null;
                }
                // Next token
                token = ParseNext(parser, errorHandler);
            }
            // Close all opened elements
            while (opened.Count > 0)
                yield return opened.Pop();
        }

        #endregion

        /// <summary>
        /// Get the default serializer
        /// </summary>
        public static HSerializer DefaultSerializer
        {
            get { return _DefaultSerializer ?? (_DefaultSerializer = new HSerializer()); }
        }
        static HSerializer _DefaultSerializer;

    }

}
