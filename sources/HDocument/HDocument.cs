using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc
{

    /// <summary>
    /// Html Document
    /// </summary>
    public class HDocument : HContainer
    {
        IList<ParseError> _ParseErrors;

        /// <summary>
        /// Create a new HTML document
        /// </summary>
        public HDocument()
        {
            Encoding = Encoding.GetEncoding("iso-8859-1");
        }

        /// <summary>
        /// Create a new HTML document with a content
        /// </summary>
        public HDocument(params object[] content)
            : this()
        {
            Add((object)content);
        }

        /// <summary>
        /// Create a new HTML document from another.
        /// </summary>
        public HDocument(HDocument other)
            : base(other)
        {
            this.Encoding = other.Encoding;
        }

        /// <summary>
        /// Clone the document.
        /// </summary>
        internal override HNode CloneNode()
        {
            return new HDocument(this);
        }

        /// <summary>
        /// Clone the document
        /// </summary>
        public HDocument Clone()
        {
            return (HDocument)CloneNode();
        }

        /// <summary>
        /// Find the node of a type
        /// </summary>
        /// <remarks>
        /// This is just an helper more fast than an GetElements().FirstOrDefault().
        /// </remarks>
        T FindFirstNode<T>() where T : HNode
        {
            HNode n = content as HNode;
            if (n != null)
            {
                do
                {
                    n = n.nextNode;
                    T e = n as T;
                    if (e != null) return e;
                } while (n != content);
            }
            return null;
        }

        internal override void ValidateNode(HNode node, HNode previous)
        {
            base.ValidateNode(node, previous);
            // Can't accept CData
            if (node is HCData)
                throw new ArgumentException("Can't add CData in a document");
            if (node is HDocument)
                throw new ArgumentException("Can't add a document in a document");

            // If node is a text, validate the string
            if (node is HText)
                ValidateString(((HText)node).Value);
            else if (node is HXmlDeclaration)
            {
                // If the xml declaration is defined, we can't add an another
                if (XmlDeclaration != null)
                    throw new ArgumentException("Xml declaration is alreay defined.");

                // We can't add a xml declaration after the document type
                if (DocumentType != null)
                    throw new ArgumentException("Can't add a xml declaration after the document type.");

                // We can't add a xml declaration after the root
                if (Root != null)
                    throw new ArgumentException("Can't add a xml declaration after the root node.");
            }
            else if (node is HDocumentType)
            {
                // If the document type is defined, we can't add an another
                if (DocumentType != null)
                    throw new ArgumentException("Document type is alreay defined.");

                // We can't add a document type after the root
                if (Root != null)
                    throw new ArgumentException("Can't add a document type after the root node.");

            }
            else if (node is HElement)
            {
                // If root is defined, then we can't add a new element
                if (Root != null)
                    throw new ArgumentException("Root is already defined.");
            }
        }

        /// <summary>
        /// Can't accept a string non whitespace
        /// </summary>
        internal override void ValidateString(string s)
        {
            if (!String.IsNullOrWhiteSpace(s))
                throw new ArgumentException("Can't add non whitespace text in a document.");
        }

        internal void AddParseError(ParseError error)
        {
            if (_ParseErrors == null)
                _ParseErrors = new List<ParseError>();
            _ParseErrors.Add(error);
        }

        /// <summary>
        /// Encoding of the document
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Get the XML declaration node for this document.
        /// </summary>
        public HXmlDeclaration XmlDeclaration { get { return FindFirstNode<HXmlDeclaration>(); } }

        /// <summary>
        /// Get the document type node for this document.
        /// </summary>
        public HDocumentType DocumentType { get { return FindFirstNode<HDocumentType>(); } }

        /// <summary>
        /// Gets the root element of the XML Tree for this document.
        /// </summary>
        public HElement Root { get { return  FindFirstNode<HElement>(); } }

        /// <summary>
        /// List of errors when parsed
        /// </summary>
        public ParseError[] ParseErrors { get { return _ParseErrors != null ? _ParseErrors.ToArray() : new ParseError[0]; } }
    }

}
