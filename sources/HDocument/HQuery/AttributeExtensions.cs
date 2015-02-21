using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc
{
    /// <summary>
    /// Attribute extensions
    /// </summary>
    public static class AttributeExtensions
    {
        /// <summary>
        /// Set one attribute for the element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <param name="name">Attribute name.</param>
        /// <param name="value">Attribute value</param>
        /// <returns>The element</returns>
        public static HElement Attribute(this HElement element, String name, String value)
        {
            if (element != null && !String.IsNullOrWhiteSpace(name))
            {
                var attr = element.Attribute(name);
                if (attr == null)
                {
                    attr = new HAttribute(name);
                    element.Add(attr);
                }
                attr.Value = value;
            }
            return element;
        }
    }
}
