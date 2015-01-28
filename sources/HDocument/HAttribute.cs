using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc
{

    /// <summary>
    /// Html Attribute
    /// </summary>
    public class HAttribute : HObject
    {
        private String _Value;
        internal HAttribute nextAttribute;

        /// <summary>
        /// Create a new attribute
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        public HAttribute(String name, String value = "")
        {
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("name");
            if (value == null) throw new ArgumentNullException("value");
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Name
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// Value
        /// </summary>
        public String Value
        {
            get { return _Value; }
            set {
                if (value == null) throw new ArgumentNullException("value");
                _Value = value; 
            }
        }

    }

}
