using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc
{
    /// <summary>
    /// Data() extensions
    /// </summary>
    public static class DataExtensions
    {
        /// <summary>
        /// Build the data key from a key
        /// </summary>
        internal static String MakeDataKey(String key)
        {
            return String.Concat("data-", key);
        }

        /// <summary>
        /// Retrieve a data value from an element
        /// </summary>
        public static String Data(this HElement element, String key)
        {
            return element.Attr(MakeDataKey(key));
        }

        /// <summary>
        /// Retrieve a data value from the first element of a set
        /// </summary>
        public static String Data(this IEnumerable<HElement> elements, String key)
        {
            return (elements != null ? elements.FirstOrDefault() : null).Data(key);
        }

        /// <summary>
        /// Set a data value to an element
        /// </summary>
        public static HElement Data(this HElement element, String key, String value)
        {
            return element.Data(MakeDataKey(key), value);
        }

        /// <summary>
        /// Set a data value to a set of elements
        /// </summary>
        public static IEnumerable<HElement> Data(this IEnumerable<HElement> elements, String key, String value)
        {
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    element.Data(key, value);
                }
            }
            return elements;
        }

        /// <summary>
        /// Set an object as key-value pairs of data to an element
        /// </summary>
        public static HElement Data(this HElement element, object values)
        {
            if (element != null && values != null)
            {
                foreach (var kv in UtilHelpers.ExtractKeyValues(values))
                {
                    element.Data(MakeDataKey(kv.Key), kv.Value);
                }
            }
            return element;
        }

        /// <summary>
        /// Set an object as key-value pairs of data to a set of elements
        /// </summary>
        public static IEnumerable<HElement> Data(this IEnumerable<HElement> elements, object values)
        {
            if (elements != null && values != null)
            {
                foreach (var element in elements)
                {
                    element.Data(values);
                }
            }
            return elements;
        }

        /// <summary>
        /// Remove a list of data values to an element
        /// </summary>
        public static HElement RemoveData(this HElement element, params String[] keys)
        {
            // Build the key list from the context
            if (keys != null && keys.Length > 0)
            {
                // Convert all keys to data keys
                keys = keys.Select(k => MakeDataKey(k)).ToArray();
            }
            else
            {
                // Extract all data keys
                keys = element.Attributes().Where(attr => attr.Name.StartsWith("data-", StringComparison.OrdinalIgnoreCase)).Select(attr => attr.Name).ToArray();
            }
            foreach (var key in keys)
            {
                element.Attr(key, null);
            }
            return element;
        }

        /// <summary>
        /// Remove a list of data values to a set of elements
        /// </summary>
        public static IEnumerable<HElement> RemoveData(this  IEnumerable<HElement> elements, params String[] keys)
        {
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    element.RemoveData(keys);
                }
            }
            return elements;
        }

        /// <summary>
        /// Indicates if an element contains any data value
        /// </summary>
        public static bool HasData(this HElement element)
        {
            return element.Attributes().Where(attr => attr.Name.StartsWith("data-", StringComparison.OrdinalIgnoreCase)).Any();
        }

        /// <summary>
        /// Indicates if the first element of a set contains any data value
        /// </summary>
        public static bool HasData(this IEnumerable<HElement> elements)
        {
            return (elements != null ? elements.FirstOrDefault() : null).HasData();
        }

        /// <summary>
        /// Indicates if an element contains a data value
        /// </summary>
        public static bool HasData(this HElement element, String key)
        {
            if (String.IsNullOrWhiteSpace(key)) return false;
            return element.Attribute(MakeDataKey(key)) != null;
        }

        /// <summary>
        /// Indicates if the first element of a set contains a data value
        /// </summary>
        public static bool HasData(this IEnumerable<HElement> elements, String key)
        {
            return (elements != null ? elements.FirstOrDefault() : null).HasData(key);
        }

    }

}
