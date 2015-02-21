using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                if (String.IsNullOrEmpty(value))
                {
                    if (attr != null)
                        attr.Remove();
                }
                else
                {
                    if (attr == null)
                    {
                        attr = new HAttribute(name);
                        element.Add(attr);
                    }
                    attr.Value = value;
                }
            }
            return element;
        }

        /// <summary>
        /// Get the value of the attribute of the element.
        /// </summary>
        public static String Attr(this HElement element, String name)
        {
            if (element != null && !String.IsNullOrWhiteSpace(name))
            {
                var attr = element.Attribute(name);
                if (attr != null)
                    return attr.Value;
            }
            return null;
        }

        /// <summary>
        /// Get the value of an attribute for the first element in the set of matched elements.
        /// </summary>
        public static String Attr(this IEnumerable<HElement> elements, String name)
        {
            if (elements != null && !String.IsNullOrWhiteSpace(name))
            {
                return elements.FirstOrDefault(e => e != null).Attr(name);
            }
            return null;
        }

        /// <summary>
        /// Set one attribute for the element.
        /// </summary>
        public static HElement Attr(this HElement element, String name, String value)
        {
            return element.Attribute(name, value);
        }

        /// <summary>
        /// Set one attribute for the set of matched elements.
        /// </summary>
        public static IEnumerable<HElement> Attr(this IEnumerable<HElement> elements, String name, String value)
        {
            if (elements != null && !String.IsNullOrWhiteSpace(name))
            {
                foreach (var element in elements)
                {
                    element.Attr(name, value);
                }
            }
            return elements;
        }

        /// <summary>
        /// Set one or more attributes from a POCO objet for the element.
        /// </summary>
        public static HElement Attr(this HElement element, Object attributes)
        {
            if (element != null && attributes != null)
            {
                foreach (var member in attributes.GetType().GetMembers().Where(m => m is PropertyInfo || m is FieldInfo))
                {
                    String mName = member.Name.Replace("_", "-");
                    if (member is PropertyInfo)
                        element.Attr(mName, Convert.ToString(((PropertyInfo)member).GetValue(attributes, null)));
                    else if (member is FieldInfo)
                        element.Attr(mName, Convert.ToString(((FieldInfo)member).GetValue(attributes)));
                }
            }
            return element;
        }

        /// <summary>
        /// Set one or more attributes from a POCO objet for the set of matched elements.
        /// </summary>
        public static IEnumerable<HElement> Attr(this IEnumerable<HElement> elements, Object attributes)
        {
            if (elements != null && attributes != null)
            {
                foreach (var element in elements)
                {
                    element.Attr(attributes);
                }
            }
            return elements;
        }

        /// <summary>
        /// Set one attribute for the set of matched elements with a valeu from a callback method.
        /// </summary>
        public static IEnumerable<HElement> Attr(this IEnumerable<HElement> elements, String name, Func<HElement, int, String> getValue)
        {
            if (elements != null && !String.IsNullOrWhiteSpace(name) && getValue != null)
            {
                int idx = 0;
                foreach (var element in elements)
                {
                    element.Attr(name, getValue(element, idx++));
                }
            }
            return elements;
        }
    }
}
