using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc
{
    /// <summary>
    /// Element extensions
    /// </summary>
    public static class ElementExtensions
    {

        #region After()

        /// <summary>
        /// Add content after the element
        /// </summary>
        public static HElement After(this HElement element, params HElement[] content)
        {
            if (element != null && content != null)
            {
                element.AddAfter(content.Where(c => c != null).Cast<object>().ToArray());
            }
            return element;
        }

        /// <summary>
        /// Add content after each element of a set of elements
        /// </summary>
        public static IEnumerable<HElement> After(this IEnumerable<HElement> elements, params HElement[] content)
        {
            if (elements != null && content != null)
            {
                foreach (var element in elements.ToList())
                {
                    element.After(content);
                }
            }
            return elements;
        }

        /// <summary>
        /// Add content from a callback after each element of a set of elements
        /// </summary>
        public static IEnumerable<HElement> After(this IEnumerable<HElement> elements, Func<HElement, int, IEnumerable<HElement>> getContent)
        {
            if (elements != null && getContent != null)
            {
                int idx = 0;
                foreach (var element in elements.ToList())
                {
                    element.After(getContent(element, idx++).ToArray());
                }
            }
            return elements;
        }

        #endregion

        #region Before()



        #endregion

    }
}
