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
        /// <summary>
        /// List of void elements
        /// </summary>
        protected static String[] VoidElements = new String[] { 
            "area", "base", "br", "col", "embed", "hr", "img", "input", 
            "keygen", "link", "meta", "param", "source", "track", "wbr" 
        };

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
            else if (node is HContainer)
                SerializeContainer((HContainer)node, writer);
            else
                // Default use ToString()
                writer.Write(node.ToString());
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
            writer.Write("<!--");
            writer.Write(HEntity.HtmlEncode(comment.Value));
            writer.Write("-->");
        }

        /// <summary>
        /// Serialize a cdata
        /// </summary>
        protected virtual void SerializeCData(HCData cdata, TextWriter writer)
        {
            writer.Write("<![CDATA[");
            writer.Write(HEntity.HtmlEncode(cdata.Value));
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
        /// Check if a tag is a void element
        /// </summary>
        protected virtual bool IsVoidElement(string p)
        {
            return VoidElements.Any(ve => String.Equals(ve, p, StringComparison.OrdinalIgnoreCase));
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
                SerializeContainer(element, writer);
                writer.Write("</{0}>", element.Name);
            }
            else
            {
                writer.Write(" />");
            }
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
            writer.Write(" ?>\r\n");
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
            writer.Write(">\r\n");
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

    }

}
