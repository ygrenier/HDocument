using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc
{
    /// <summary>
    /// Html comment.
    /// </summary>
    public class HComment : HNode
    {
        internal string value;

        /// <summary>
        /// Create a new comment node.
        /// </summary>
        public HComment(String value)
        {
            if (value == null) throw new ArgumentNullException("value");
            this.value = value;
        }

        /// <summary>
        /// Create a new comment from another.
        /// </summary>
        public HComment(HComment other)
        {
            if (other == null) throw new ArgumentNullException("other");
            this.value = other.value;
        }

        internal override HNode CloneNode()
        {
            return new HComment(this);
        }

        /// <summary>
        /// Get or set the content of this comment.
        /// </summary>
        public String Value
        {
            get { return this.value; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                this.value = value;
            }
        }
    }

}
