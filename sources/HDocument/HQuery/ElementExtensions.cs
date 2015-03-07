using System;
using System.Collections;
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

        /// <summary>
        /// Add content before the element
        /// </summary>
        public static HElement Before(this HElement element, params HElement[] content)
        {
            if (element != null && content != null)
            {
                element.AddBefore(content.Where(c => c != null).Cast<object>().ToArray());
            }
            return element;
        }

        /// <summary>
        /// Add content before each element of a set of elements
        /// </summary>
        public static IEnumerable<HElement> Before(this IEnumerable<HElement> elements, params HElement[] content)
        {
            if (elements != null && content != null)
            {
                foreach (var element in elements.ToList())
                {
                    element.Before(content);
                }
            }
            return elements;
        }

        /// <summary>
        /// Add content from a callback before each element of a set of elements
        /// </summary>
        public static IEnumerable<HElement> Before(this IEnumerable<HElement> elements, Func<HElement, int, IEnumerable<HElement>> getContent)
        {
            if (elements != null && getContent != null)
            {
                int idx = 0;
                foreach (var element in elements.ToList())
                {
                    element.Before(getContent(element, idx++).ToArray());
                }
            }
            return elements;
        }

        #endregion

        #region InsertAfter()

        /// <summary>
        /// Insert element after the target
        /// </summary>
        public static HElement InsertAfter(this HElement element, HElement target)
        {
            if (element != null && target != null)
            {
                target.After(element);
            }
            return element;
        }

        /// <summary>
        /// Insert the set of elements after the target
        /// </summary>
        public static IEnumerable<HElement> InsertAfter(this IEnumerable<HElement> elements, HElement target)
        {
            if (elements != null && target != null)
            {
                target.After(elements.ToArray());
            }
            return elements;
        }

        #endregion

        #region InsertBefore()

        /// <summary>
        /// Insert element before the target
        /// </summary>
        public static HElement InsertBefore(this HElement element, HElement target)
        {
            if (element != null && target != null)
            {
                target.Before(element);
            }
            return element;
        }

        /// <summary>
        /// Insert the set of elements before the target
        /// </summary>
        public static IEnumerable<HElement> InsertBefore(this IEnumerable<HElement> elements, HElement target)
        {
            if (elements != null && target != null)
            {
                target.Before(elements.ToArray());
            }
            return elements;
        }

        #endregion

        #region Empty()

        /// <summary>
        /// Remove all nodes in the element.
        /// </summary>
        public static HElement Empty(this HElement element)
        {
            if (element != null)
                element.RemoveNodes();
            return element;
        }

        /// <summary>
        /// Remove all nodes in the elements of the sets
        /// </summary>
        public static IEnumerable<HElement> Empty(this IEnumerable<HElement> elements)
        {
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    element.Empty();
                }
            }
            return elements;
        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone the set of elements
        /// </summary>
        public static IEnumerable<HElement> Clone(this IEnumerable<HElement> elements)
        {
            if (elements != null)
                return elements.Select(e => e != null ? e.Clone() : null).ToList();
            return elements;
        }

        #endregion

        #region Append()

        /// <summary>
        /// Append content at the end of the element
        /// </summary>
        public static HElement Append(this HElement element, params object[] content)
        {
            if (element != null)
            {
                element.Add(content);
            }
            return element;
        }

        /// <summary>
        /// Append content at the end of each element of the set.
        /// </summary>
        public static IEnumerable<HElement> Append(this IEnumerable<HElement> elements, params object[] content)
        {
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    element.Append(content);
                }
            }
            return elements;
        }

        /// <summary>
        /// Append content at the end of each element of the set.
        /// </summary>
        public static IEnumerable<HElement> Append(this IEnumerable<HElement> elements, Func<HElement, int, object> getContent)
        {
            if (elements != null && getContent!=null)
            {
                int i = 0;
                foreach (var element in elements)
                {
                    element.Append(getContent(element, i++));
                }
            }
            return elements;
        }

        #endregion

        #region Prepend()

        /// <summary>
        /// Append content at the beginning of the element
        /// </summary>
        public static HElement Prepend(this HElement element, params object[] content)
        {
            if (element != null)
            {
                element.Insert(null, content);
            }
            return element;
        }

        /// <summary>
        /// Append content at the beginning of each element of the set.
        /// </summary>
        public static IEnumerable<HElement> Prepend(this IEnumerable<HElement> elements, params object[] content)
        {
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    element.Prepend(content);
                }
            }
            return elements;
        }

        /// <summary>
        /// Append content at the beginning of each element of the set.
        /// </summary>
        public static IEnumerable<HElement> Prepend(this IEnumerable<HElement> elements, Func<HElement, int, object> getContent)
        {
            if (elements != null && getContent != null)
            {
                int i = 0;
                foreach (var element in elements)
                {
                    element.Prepend(getContent(element, i++));
                }
            }
            return elements;
        }

        #endregion

        #region AppendTo()

        /// <summary>
        /// Append the element to the end of the content of the target
        /// </summary>
        public static T AppendTo<T>(this T element, HElement target)
        {
            if (element != null && target != null)
            {
                target.Append(element);
            }
            return element;
        }

        /// <summary>
        /// Append the set of elements to the end of the content of the target
        /// </summary>
        public static IEnumerable<T> AppendTo<T>(this IEnumerable<T> content, HElement target)
        {
            if (content != null && target != null)
                target.Append(content);
            return content;
        }

        #endregion

        #region PrependTo()

        /// <summary>
        /// Append the element to the beginning of the content of the target
        /// </summary>
        public static T PrependTo<T>(this T element, HElement target)
        {
            if (element != null && target != null)
            {
                target.Prepend(element);
            }
            return element;
        }

        /// <summary>
        /// Append the set of elements to the beginning of the content of the target
        /// </summary>
        public static IEnumerable<T> PrependTo<T>(this IEnumerable<T> content, HElement target)
        {
            if (content != null && target != null)
                target.Prepend(content);
            return content;
        }

        #endregion

        #region Remove()

        /// <summary>
        /// Remove the set of elements
        /// </summary>
        public static IEnumerable<HNode> Remove(this IEnumerable<HNode> nodes)
        {
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    if (node != null)
                        node.Remove();
                }
            }
            return nodes;
        }

        #endregion

        #region ReplaceWith()

        /// <summary>
        /// Replace the element by a new content
        /// </summary>
        public static HElement ReplaceWith(this HElement element, params HElement[] content)
        {
            if (element != null)
            {
                element.Before(content);
                element.Remove();
            }
            return element;
        }

        /// <summary>
        /// Replace each element of the set by a new content
        /// </summary>
        public static IEnumerable<HElement> ReplaceWith(this IEnumerable<HElement> elements, params HElement[] content)
        {
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    element.ReplaceWith(content);
                }
            }
            return elements;
        }

        /// <summary>
        /// Replace each element of the set by a new content returned by a callback
        /// </summary>
        public static IEnumerable<HElement> ReplaceWith(this IEnumerable<HElement> elements, Func<HElement, int, IEnumerable<HElement>> getContent)
        {
            if (elements != null)
            {
                int idx = 0;
                foreach (var element in elements)
                {
                    IEnumerable<HElement> content = getContent != null ? getContent(element, idx++) : null;
                    if (content != null)
                        element.ReplaceWith(content.ToArray());
                    else
                        elements.ReplaceWith();
                }
            }
            return elements;
        }

        #endregion

        #region ReplaceAll()

        /// <summary>
        /// Replace each target element with the set of elements
        /// </summary>
        public static IEnumerable<HElement> ReplaceAll(this IEnumerable<HElement> elements, params HElement[] target)
        {
            if (elements != null)
                target.ReplaceWith(elements.ToArray());
            return elements;
        }

        #endregion

    }
}
