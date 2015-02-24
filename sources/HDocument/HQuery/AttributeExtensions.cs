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

        #region Attribute/Attr

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

        #endregion

        #region CSS

        /// <summary>
        /// Convert a styleName to style-name.
        /// </summary>
        static String ConvertCamelNameToStyleName(String name)
        {
            StringBuilder result = new StringBuilder();
            foreach (var c in name)
            {
                if (Char.IsUpper(c))
                {
                    if (result.Length > 0) result.Append('-');
                    result.Append(Char.ToLower(c));
                }
                else
                    result.Append(c);
            }
            return result.ToString();
        }

        /// <summary>
        /// Extract all style properties of the element
        /// </summary>
        public static IDictionary<String, String> Css(this HElement element)
        {
            String style = element.Attr("style");
            var result = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
            if (!String.IsNullOrWhiteSpace(style))
            {
                // Split style
                String[] parts = style.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    String name, value;
                    int p = part.IndexOf('=');
                    if (p < 0)
                    {
                        name = part;
                        value = String.Empty;
                    }
                    else
                    {
                        name = part.Substring(0, p);
                        value = part.Substring(p + 1);
                    }
                    result[ConvertCamelNameToStyleName(name)] = value;
                }
            }
            return result;
        }

        /// <summary>
        /// Extract all style properties in the first element in the list
        /// </summary>
        public static IDictionary<String, String> Css(this IEnumerable<HElement> elements)
        {
            return elements.First().Css();
        }

        /// <summary>
        /// Get the value of a style property in the element
        /// </summary>
        public static String Css(this HElement element, String propertyName)
        {
            var styles = element.Css();
            String result;
            if (styles.TryGetValue(ConvertCamelNameToStyleName(propertyName), out result))
                return result;
            return String.Empty;
        }

        /// <summary>
        /// Get the value of a style property in the first element in the list
        /// </summary>
        public static String Css(this IEnumerable<HElement> elements, String propertyName)
        {
            return elements.First().Css(propertyName);
        }

        /// <summary>
        /// Get the values of a list of style properties in the element
        /// </summary>
        public static IEnumerable<String> Css(this HElement element, params String[] propertyNames)
        {
            var styles = element.Css();
            if (propertyNames != null)
            {
                foreach (var propertyName in propertyNames)
                {
                    if (String.IsNullOrWhiteSpace(propertyName)) continue;
                    String result;
                    if (styles.TryGetValue(ConvertCamelNameToStyleName(propertyName), out result))
                        yield return result;
                    else
                        yield return String.Empty;
                }
            }
        }

        /// <summary>
        /// Get the values of a list of style properties in the first element in the list
        /// </summary>
        public static IEnumerable<String> Css(this IEnumerable<HElement> elements, params String[] propertyNames)
        {
            return elements.First().Css(propertyNames);
        }

        // TODO css( String propertyName, String value )
        // TODO css( String propertyName, Func<HElement, int, String> callback )
        // TODO css( Object properties )

        #endregion

    }
}
