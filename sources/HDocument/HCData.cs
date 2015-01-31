using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc
{

    /// <summary>
    /// CData node
    /// </summary>
    public class HCData : HText
    {

        /// <summary>
        /// Create a new CData node
        /// </summary>
        public HCData(string value) : base(value) { }

        /// <summary>
        /// Create a new CData from another XCData object.
        /// </summary>
        public HCData(HCData other) : base(other) { }

        /// <summary>
        /// Clone the node
        /// </summary>
        internal override HNode CloneNode() {
            return new HCData(this);
        }

    }

}
