using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc
{
    /// <summary>
    /// Text node
    /// </summary>
    public class HText : HNode
    {
        internal string value;

        /// <summary>
        /// Create a new HText class.
        /// </summary>
        /// <param name="text">The string that contains the value of the text node.</param>
        public HText(string text) {
            if (text == null) throw new ArgumentNullException("text");
            value = text;
        }

        /// <summary>
        /// Create a new HText class from another HText object.
        /// </summary>
        /// <param name="other">The text node to copy from.</param>
        public HText(HText other) {
            if (other == null) throw new ArgumentNullException("other");
            value = other.value;
        }

        /// <summary>
        /// Gets or sets the value of this node.
        /// </summary>
        public string Value {
            get {
                return value;
            }
            set {
                if (value == null) throw new ArgumentNullException("value");
                this.value = value;
            }
        }
    }

}
