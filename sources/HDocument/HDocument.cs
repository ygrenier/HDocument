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
    }

}
