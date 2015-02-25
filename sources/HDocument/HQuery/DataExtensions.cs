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
        /// Retrieve a data value from an element
        /// </summary>
        public static String Data(this HElement element, String key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieve a data value from the first element of a set
        /// </summary>
        public static String Data(this IEnumerable<HElement> elements, String key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set a data value to an element
        /// </summary>
        public static HElement Data(this HElement element, String key, String value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set a data value to a set of elements
        /// </summary>
        public static IEnumerable<HElement> Data(this IEnumerable<HElement> elements, String key, String value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set an object as key-value pairs of data to an element
        /// </summary>
        public static HElement Data(this HElement element, object values)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set an object as key-value pairs of data to a set of elements
        /// </summary>
        public static IEnumerable<HElement> Data(this IEnumerable<HElement> elements, object values)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove a list of data values to an element
        /// </summary>
        public static HElement RemoveData(this HElement element, params String[] names)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove a list of data values to a set of elements
        /// </summary>
        public static IEnumerable<HElement> RemoveData(this  IEnumerable<HElement> elements, params String[] names)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates if an element contains any data value
        /// </summary>
        public static bool HasData(this HElement element)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates if the first element of a set contains any data value
        /// </summary>
        public static bool HasData(this IEnumerable<HElement> elements)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates if an element contains a data value
        /// </summary>
        public static bool HasData(this HElement element, String key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates if the first element of a set contains a data value
        /// </summary>
        public static bool HasData(this IEnumerable<HElement> elements, String key)
        {
            throw new NotImplementedException();
        }

    }

}
