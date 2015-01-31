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
        /// <summary>
        /// Create a new HTML document
        /// </summary>
        public HDocument()
        {
        }

        /// <summary>
        /// Create a new HTML document with a content
        /// </summary>
        public HDocument(params object[] content):this()
        {
            Add((object)content);
        }

        /// <summary>
        /// Create a new HTML document from another.
        /// </summary>
        public HDocument(HDocument other)
            : base(other)
        {
        }

        /// <summary>
        /// Clone the document.
        /// </summary>
        internal override HNode CloneNode()
        {
            return new HDocument(this);
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

        /// <summary>
        /// Gets the root element of the XML Tree for this document.
        /// </summary>
        public HElement Root { get { return  FindFirstNode<HElement>(); } }

    }

}
