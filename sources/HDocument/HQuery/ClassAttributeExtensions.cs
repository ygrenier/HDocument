using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc
{

    /// <summary>
    /// Class Attribute manipulation methods
    /// </summary>
    public static class ClassAttributeExtensions
    {

        /// <summary>
        /// Extract classes from an element
        /// </summary>
        public static String[] GetClasses(this HElement element)
        {
            HAttribute clsAttr = element != null ? element.Attribute("class") : null;
            if (clsAttr == null|| clsAttr.Value==null) return new String[0];
            return clsAttr.Value.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Determine if the element is assigned the given class.
        /// </summary>
        /// <param name="element">Source element.</param>
        /// <param name="className">The class name to search for.</param>
        /// <returns>Returns true if the class is assigned.</returns>
        public static bool HasClass(this HElement element, String className)
        {
            // If invalid argument then not found
            if (element == null || string.IsNullOrWhiteSpace(className))
                return false;
            var cls = element.GetClasses();
            return cls.Any(c => String.Equals(className, c, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determine wether any of the matched elements are assigned the given class.
        /// </summary>
        /// <param name="elements">Source elements.</param>
        /// <param name="className">The class name to search for.</param>
        /// <returns>Returns true if the class is assigned to an element.</returns>
        public static bool HasClass(this IEnumerable<HElement> elements, String className)
        {
            // If invalid argument then not found
            if (elements == null || string.IsNullOrWhiteSpace(className))
                return false;
            // Browse elements
            foreach (var element in elements)
            {
                if (element == null) continue;
                if (element.HasClass(className))
                    return true;
            }
            return false;
        }

        // TODO AddClass
        // TODO RemoveClass
        // TODO ToggleClass
    }

}
