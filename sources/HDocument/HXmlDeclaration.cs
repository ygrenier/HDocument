using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc
{
    /// <summary>
    /// XML declaration for the XHTML document
    /// </summary>
    public class HXmlDeclaration : HNode
    {

        /// <summary>
        /// Create a new default declaration
        /// </summary>
        public HXmlDeclaration()
            : this("1.0", "utf-8", null)
        {
        }

        /// <summary>
        /// Create an new declaration
        /// </summary>
        public HXmlDeclaration(String version, String encoding, String standalone)
        {
            this.Version = version;
            this.Encoding = encoding;
            this.Standalone = standalone;
        }

        /// <summary>
        /// Create a new declaration from another declaration
        /// </summary>
        /// <param name="other"></param>
        public HXmlDeclaration(HXmlDeclaration other)
        {
            if (other == null) throw new ArgumentNullException("other");
            this.Version = other.Version;
            this.Encoding = other.Encoding;
            this.Standalone = other.Standalone;
        }

        /// <summary>
        /// Clone the declaration
        /// </summary>
        internal override HNode CloneNode()
        {
            return new HXmlDeclaration(this);
        }

        /// <summary>
        /// Indicates if the XHTML is standalone
        /// </summary>
        public String Standalone { get; set; }
        /// <summary>
        /// Indicates version of XML
        /// </summary>
        public String Version { get; set; }
        /// <summary>
        /// Indicates encoding
        /// </summary>
        public String Encoding { get; set; }
    }

}
