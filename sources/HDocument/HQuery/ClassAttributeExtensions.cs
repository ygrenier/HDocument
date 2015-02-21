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
        /// Helper to extract class names from string
        /// </summary>
        internal static String[] ExtractClassNames(String className)
        {
            return className != null ? className.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries) : new String[0];
        }

        /// <summary>
        /// Extract classes from an element
        /// </summary>
        public static String[] GetClasses(this HElement element)
        {
            HAttribute clsAttr = element != null ? element.Attribute("class") : null;
            if (clsAttr == null|| clsAttr.Value==null) return new String[0];
            return ExtractClassNames(clsAttr.Value);
        }

        #region HasClass

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

        #endregion

        #region AddClass

        /// <summary>
        /// Adds the specified class(es) to the element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <param name="classNames">One or more classes to be added to the class attribute of the element.</param>
        /// <returns>Element</returns>
        public static HElement AddClass(this HElement element, IEnumerable<String> classNames)
        {
            if (element != null && classNames != null)
            {
                element.Attribute("class", String.Join(" ", element.GetClasses().Concat(classNames).Distinct(StringComparer.OrdinalIgnoreCase)));
            }
            return element;
        }

        /// <summary>
        /// Adds the specified classes to each of the set of matched elements.
        /// </summary>
        /// <param name="elements>Source elements.</param>
        /// <param name="className">One or more classes to be added to the class attribute of each matched element.</param>
        /// <returns>Source elements updated</returns>
        public static IEnumerable<HElement> AddClass(this IEnumerable<HElement> elements, IEnumerable<String> classNames)
        {
            if (elements != null && classNames != null)
            {
                foreach (var element in elements)
                {
                    element.AddClass(classNames);
                }
            }
            return elements;
        }

        /// <summary>
        /// Adds the specified class(es) to the element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <param name="className">One or more space-separated classes to be added to the class attribute of the element.</param>
        /// <returns>Element</returns>
        public static HElement AddClass(this HElement element, String className)
        {
            return element.AddClass(ExtractClassNames(className));
        }

        /// <summary>
        /// Adds the specified class(es) to each of the set of matched elements.
        /// </summary>
        /// <param name="elements>Source elements.</param>
        /// <param name="className">One or more space-separated classes to be added to the class attribute of each matched element.</param>
        /// <returns>Source elements updated</returns>
        public static IEnumerable<HElement> AddClass(this IEnumerable<HElement> elements, String className)
        {
            return elements.AddClass(ExtractClassNames(className));
        }

        /// <summary>
        /// Add the specified class(es) returns by a callback method to each of the set of matched elements.
        /// </summary>
        /// <param name="elements">Source elements.</param>
        /// <param name="getClassName">Callback method returning the class name to added.</param>
        /// <returns>Source elements updated.</returns>
        public static IEnumerable<HElement> AddClass(this IEnumerable<HElement> elements, Func<HElement, int, String> getClassName)
        {
            if (elements != null && getClassName != null)
            {
                int i = 0;
                foreach (var element in elements)
                {
                    element.AddClass(getClassName(element, i++));
                }
            }
            return elements;
        }

        #endregion

        #region Remove Class

        /// <summary>
        /// Remove all classes to an element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static HElement RemoveClass(this HElement element)
        {
            if (element != null)
            {
                var attr = element.Attribute("class");
                if (attr != null)
                    attr.Remove();
            }
            return element;
        }

        /// <summary>
        /// Remove some class names to an element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static HElement RemoveClass(this HElement element, String[] className)
        {
            if (element != null && className != null && className.Length > 0)
            {
                var classes = element.GetClasses();
                element.Attribute("class", String.Join(" ", classes.Except(className, StringComparer.OrdinalIgnoreCase)));
            }
            return element;
        }

        /// <summary>
        /// Remove all classes to each of elements.
        /// </summary>
        /// <param name="elements">Source elements.</param>
        /// <returns>Source elements updated.</returns>
        public static IEnumerable<HElement> RemoveClass(this IEnumerable<HElement> elements)
        {
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    element.RemoveClass();
                }
            }
            return elements;
        }

        /// <summary>
        /// Remove all class names to each of the elements.
        /// </summary>
        /// <param name="elements">Source elements.</param>
        /// <param name="className">Class names to remove</param>
        /// <returns>Source elements updated.</returns>
        public static IEnumerable<HElement> RemoveClass(this IEnumerable<HElement> elements, String[] className)
        {
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    element.RemoveClass(className);
                }
            }
            return elements;
        }

        /// <summary>
        /// Remove one or more space-separated classes to be removed from the class attribute of the element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static HElement RemoveClass(this HElement element, String className)
        {
            return element.RemoveClass(ExtractClassNames(className));
        }

        /// <summary>
        /// Remove one or more space-separated classes to be removed from the class attribute of each matched element
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static IEnumerable<HElement> RemoveClass(this IEnumerable<HElement> elements, String className)
        {
            return elements.RemoveClass(ExtractClassNames(className));
        }

        /// <summary>
        /// Remove the specified class(es) returns by a callback method to each of the set of matched elements.
        /// </summary>
        /// <param name="elements">Source elements.</param>
        /// <param name="getClassName">Callback method returning the class name to removed.</param>
        /// <returns>Source elements updated.</returns>
        public static IEnumerable<HElement> RemoveClass(this IEnumerable<HElement> elements, Func<HElement, int, String> getClassName)
        {
            if (elements != null && getClassName != null)
            {
                int i = 0;
                foreach (var element in elements)
                {
                    element.RemoveClass(getClassName(element, i++));
                }
            }
            return elements;
        }

        #endregion

        #region ToggleClass

        /// <summary>
        /// Add or remove one or more classes to the element, depending on either the class's presence.
        /// </summary>
        public static HElement ToggleClass(this HElement element, String[] className)
        {
            if (element != null && className != null)
            {
                foreach (var cName in className)
                {
                    if (element.HasClass(cName))
                        element.RemoveClass(cName);
                    else
                        element.AddClass(cName);
                }
            }
            return element;
        }

        /// <summary>
        /// Add or remove one or more classes to the element, depending on either the class's presence or the value of the state argument.
        /// </summary>
        public static HElement ToggleClass(this HElement element, String[] className, bool state)
        {
            if (element != null && className != null)
            {
                if (state)
                    element.AddClass(className);
                else
                    element.RemoveClass(className);
            }
            return element;
        }

        /// <summary>
        /// Add or remove one or more classes to the element, depending on either the class's presence.
        /// </summary>
        public static HElement ToggleClass(this HElement element, String className)
        {
            return element.ToggleClass(ExtractClassNames(className));
        }

        /// <summary>
        /// Add or remove one or more classes to the element, depending on either the class's presence or the value of the state argument.
        /// </summary>
        public static HElement ToggleClass(this HElement element, String className, bool state)
        {
            return element.ToggleClass(ExtractClassNames(className), state);
        }

        /// <summary>
        /// Add or remove one or more classes from each element in the set of matched elements, depending on either the class's presence.
        /// </summary>
        public static IEnumerable<HElement> ToggleClass(this IEnumerable<HElement> elements, String[] className)
        {
            if (elements != null && className != null)
            {
                foreach (var element in elements)
                {
                    element.ToggleClass(className);
                }
            }
            return elements;
        }

        /// <summary>
        /// Add or remove one or more classes from each element in the set of matched elements, depending on either the class's presence or the value of the state argument.
        /// </summary>
        public static IEnumerable<HElement> ToggleClass(this IEnumerable<HElement> elements, String[] className, bool state)
        {
            if (elements != null && className != null)
            {
                foreach (var element in elements)
                {
                    element.ToggleClass(className, state);
                }
            }
            return elements;
        }

        /// <summary>
        /// Add or remove one or more classes from each element in the set of matched elements, depending on either the class's presence.
        /// </summary>
        public static IEnumerable<HElement> ToggleClass(this IEnumerable<HElement> elements, String className)
        {
            return elements.ToggleClass(ExtractClassNames(className));
        }

        /// <summary>
        /// Add or remove one or more classes from each element in the set of matched elements, depending on either the class's presence or the value of the state argument.
        /// </summary>
        public static IEnumerable<HElement> ToggleClass(this IEnumerable<HElement> elements, String className, bool state)
        {
            return elements.ToggleClass(ExtractClassNames(className), state);
        }

        /// <summary>
        /// Add or remove the specified class(es) returns by a callback method from each element in the set of matched elements, depending on either the class's presence.
        /// </summary>
        public static IEnumerable<HElement> ToggleClass(this IEnumerable<HElement> elements, Func<HElement, int, String> getClassName)
        {
            if (elements != null && getClassName != null)
            {
                int i = 0;
                foreach (var element in elements)
                {
                    element.ToggleClass(getClassName(element, i++));
                }
            }
            return elements;
        }

        /// <summary>
        /// Add or remove the specified class(es) returns by a callback method from each element in the set of matched elements, depending on either the class's presence or the value of the state argument.
        /// </summary>
        public static IEnumerable<HElement> ToggleClass(this IEnumerable<HElement> elements, Func<HElement, int, String> getClassName, bool state)
        {
            if (elements != null && getClassName != null)
            {
                int i = 0;
                foreach (var element in elements)
                {
                    element.ToggleClass(getClassName(element, i++), state);
                }
            }
            return elements;
        }

        #endregion

    }

}
