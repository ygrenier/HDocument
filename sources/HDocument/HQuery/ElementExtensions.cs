using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        public static HElement After(this HElement element, params HNode[] content)
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
        public static IEnumerable<HElement> After(this IEnumerable<HElement> elements, params HNode[] content)
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
        public static IEnumerable<HElement> After(this IEnumerable<HElement> elements, Func<HElement, int, IEnumerable<HNode>> getContent)
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
        public static HElement Before(this HElement element, params HNode[] content)
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
        public static IEnumerable<HElement> Before(this IEnumerable<HElement> elements, params HNode[] content)
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
        public static IEnumerable<HElement> Before(this IEnumerable<HElement> elements, Func<HElement, int, IEnumerable<HNode>> getContent)
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
        public static HNode InsertAfter(this HNode element, HNode target)
        {
            if (element != null && target != null)
            {
                target.AddAfter(element);
            }
            return element;
        }

        /// <summary>
        /// Insert the set of elements after the target
        /// </summary>
        public static IEnumerable<HNode> InsertAfter(this IEnumerable<HNode> elements, HNode target)
        {
            if (elements != null && target != null)
            {
                target.AddAfter(elements.ToArray());
            }
            return elements;
        }

        #endregion

        #region InsertBefore()

        /// <summary>
        /// Insert element before the target
        /// </summary>
        public static HNode InsertBefore(this HNode element, HElement target)
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
        public static IEnumerable<HNode> InsertBefore(this IEnumerable<HNode> elements, HElement target)
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
        public static HNode AppendTo(this HNode content, HElement target)
        {
            if (content != null && target != null)
            {
                target.Append(content);
            }
            return content;
        }

        /// <summary>
        /// Append the set of elements to the end of the content of the target
        /// </summary>
        public static IEnumerable<HNode> AppendTo(this IEnumerable<HNode> content, HElement target)
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
        public static HElement ReplaceWith(this HElement element, params HNode[] content)
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
        public static IEnumerable<HElement> ReplaceWith(this IEnumerable<HElement> elements, params HNode[] content)
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
        public static IEnumerable<HElement> ReplaceWith(this IEnumerable<HElement> elements, Func<HElement, int, IEnumerable<HNode>> getContent)
        {
            if (elements != null)
            {
                int idx = 0;
                foreach (var element in elements)
                {
                    IEnumerable<HNode> content = getContent != null ? getContent(element, idx++) : null;
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

        #region Wrap()

        /// <summary>
        /// Wrap <paramref name="wrappingElement"/> around the element
        /// </summary>
        public static HElement Wrap(this HElement element, HElement wrappingElement)
        {
            if (element != null && wrappingElement != null)
            {
                element.ReplaceWith(wrappingElement);
                var dp = wrappingElement;
                while (dp.HasElements) dp = dp.Elements().First();
                dp.Append(element);
            }
            return element;
        }

        /// <summary>
        /// Wrap <paramref name="wrappingElement"/> around each element of the set
        /// </summary>
        public static IEnumerable<HElement> Wrap(this IEnumerable<HElement> elements, HElement wrappingElement)
        {
            if (elements != null && wrappingElement != null)
            {
                foreach (var element in elements)
                {
                    element.Wrap(wrappingElement);
                }
            }
            return elements;
        }

        /// <summary>
        /// Wrap a callback result around each element of the set
        /// </summary>
        public static IEnumerable<HElement> Wrap(this IEnumerable<HElement> elements, Func<HElement,int, HElement> getWrappingElement)
        {
            if (elements != null && getWrappingElement != null)
            {
                int idx = 0;
                foreach (var element in elements)
                {
                    element.Wrap(getWrappingElement(element, idx++));
                }
            }
            return elements;
        }

        #endregion

        #region WrapAll()

        /// <summary>
        /// Wrap <paramref name="wrappingElement"/> around all elements of the set.
        /// </summary>
        public static IEnumerable<HElement> WrapAll(this IEnumerable<HElement> elements, HElement wrappingElement)
        {
            if (elements != null)
            {
                var first = elements.FirstOrDefault(e => e != null);
                first.ReplaceWith(wrappingElement);
                var dp = wrappingElement;
                while (dp.HasElements) dp = dp.Elements().First();
                elements.Remove();
                dp.Append(elements);
            }
            return elements;
        }

        #endregion

        #region WrapInner()

        /// <summary>
        /// Wrap <paramref name="wrappingElement"/> around the content of the element
        /// </summary>
        public static HElement WrapInner(this HElement element, HElement wrappingElement)
        {
            if (element != null && wrappingElement != null)
            {
                if (element.HasElements)
                {
                    element.Elements().WrapAll(wrappingElement);
                }
                else
                {
                    element.Append(wrappingElement);
                }
            }
            return element;
        }

        /// <summary>
        /// Wrap <paramref name="wrappingElement"/> around the content of each element of the set
        /// </summary>
        public static IEnumerable<HElement> WrapInner(this IEnumerable<HElement> elements, HElement wrappingElement)
        {
            if (elements != null && wrappingElement != null)
            {
                foreach (var element in elements)
                {
                    element.WrapInner(wrappingElement);
                }
            }
            return elements;
        }

        /// <summary>
        /// Wrap a callback result around the content of each element of the set
        /// </summary>
        public static IEnumerable<HElement> WrapInner(this IEnumerable<HElement> elements, Func<HElement, int, HElement> getWrappingElement)
        {
            if (elements != null && getWrappingElement != null)
            {
                int idx = 0;
                foreach (var element in elements)
                {
                    element.WrapInner(getWrappingElement(element, idx++));
                }
            }
            return elements;
        }

        #endregion

        #region Unwrap()

        /// <summary>
        /// Remove the parent of the element
        /// </summary>
        public static HElement Unwrap(this HElement element)
        {
            if (element != null && element.Parent != null)
            {
                var grandParent = element.Parent.Parent;
                if (grandParent != null)
                {
                    var parentContent = element.Parent.Nodes().ToArray();
                    element.Parent.ReplaceWith(parentContent);
                }
            }
            return element;
        }

        /// <summary>
        /// Remove the parent of the set
        /// </summary>
        public static IEnumerable<HElement> Unwrap(this IEnumerable<HElement> elements)
        {
            if (elements != null)
            {
                var parents = elements.Where(e => e != null && e.Parent != null).Select(e => e.Parent).Distinct();
                foreach (var parent in parents)
                {
                    var grandParent = parent.Parent;
                    if (grandParent != null)
                    {
                        var parentContent = parent.Nodes().ToArray();
                        parent.ReplaceWith(parentContent);
                    }
                }
            }
            return elements;
        }

        #endregion

        #region Parent()

        /// <summary>
        /// Get the parent for each elements of the set
        /// </summary>
        public static IEnumerable<HElement> Parent(this IEnumerable<HNode> elements)
        {
            if (elements != null)
                return elements.Where(e => e != null && e.Parent != null).Select(e => e.Parent).Distinct();
            return Enumerable.Empty<HElement>();
        }

        #endregion

        #region Parents()

        /// <summary>
        /// Get the ancestors of each element in the set
        /// </summary>
        public static IEnumerable<HElement> Parents(this IEnumerable<HNode> elements)
        {
            if (elements != null)
            {
                return elements
                    .Where(e => e != null)
                    .SelectMany(e => e.Parents())
                    .Where(p => p != null)
                    .Distinct();
            }
            return Enumerable.Empty<HElement>();
        }

        #endregion

        #region Slice()

        /// <summary>
        /// Reduce the set of elements
        /// </summary>
        public static IEnumerable<HElement> Slice(this IEnumerable<HElement> elements, int start)
        {
            if (elements != null)
            {
                if (start < 0) start = elements.Count() + start;
                elements.Skip(start);
            }
            return Enumerable.Empty<HElement>();
        }

        /// <summary>
        /// Reduce the set of elements
        /// </summary>
        public static IEnumerable<HElement> Slice(this IEnumerable<HElement> elements, int start, int end)
        {
            if (elements != null)
            {
                if (start < 0) start = elements.Count() + start;
                if (end < 0) end = elements.Count() + end;
                int count = end - start + 1;
                if (count > 1)
                    return elements.Skip(start).Take(count);
            }
            return Enumerable.Empty<HElement>();
        }

        #endregion

        #region Siblings()

        /// <summary>
        /// Get the siblings of the element
        /// </summary>
        public static IEnumerable<HElement> Siblings(this HElement element)
        {
            if (element != null && element.Parent != null)
            {
                return element.Parent.Elements().Where(e => e != null && e != element).Distinct();
            }
            return Enumerable.Empty<HElement>();
        }

        /// <summary>
        /// Get the siblings of each element of the set
        /// </summary>
        public static IEnumerable<HElement> Siblings(this IEnumerable<HElement> elements)
        {
            if (elements != null)
            {
                return elements.SelectMany(e => e.Siblings()).Distinct();
            }
            return Enumerable.Empty<HElement>();
        }

        #endregion

        #region Prev()

        /// <summary>
        /// Get the immediate previous element
        /// </summary>
        public static IEnumerable<HNode> Prev(this IEnumerable<HNode> elements)
        {
            if (elements != null)
            {
                return elements
                    .Where(e => e != null)
                    .Select(e => e.PreviousNode)
                    .Where(p => p != null);
            }
            return Enumerable.Empty<HNode>();
        }

        #endregion

        #region PrevAll()

        /// <summary>
        /// Get all precedings siblings of the element
        /// </summary>
        public static IEnumerable<HNode> PrevAll(this HNode element)
        {
            if (element != null)
            {
                var p = element.PreviousNode;
                while (p != null)
                {
                    yield return p;
                    p = p.PreviousNode;
                }
            }
        }

        /// <summary>
        /// Get all precedings siblings of each element in the set.
        /// </summary>
        public static IEnumerable<HNode> PrevAll(this IEnumerable<HNode> elements)
        {
            if (elements != null)
            {
                return elements
                    .SelectMany(e => e.PrevAll())
                    .Distinct();
            }
            return Enumerable.Empty<HNode>();
        }

        #endregion

        #region Next()

        /// <summary>
        /// Get the immediate next element
        /// </summary>
        public static IEnumerable<HNode> Next(this IEnumerable<HNode> elements)
        {
            if (elements != null)
            {
                return elements
                    .Where(e => e != null)
                    .Select(e => e.NextNode)
                    .Where(p => p != null);
            }
            return Enumerable.Empty<HNode>();
        }

        #endregion

        #region NextAll()

        /// <summary>
        /// Get all next siblings of the element
        /// </summary>
        public static IEnumerable<HNode> NextAll(this HNode element)
        {
            if (element != null)
            {
                var p = element.NextNode;
                while (p != null)
                {
                    yield return p;
                    p = p.NextNode;
                }
            }
        }

        /// <summary>
        /// Get all next siblings of each element in the set.
        /// </summary>
        public static IEnumerable<HNode> NextAll(this IEnumerable<HNode> elements)
        {
            if (elements != null)
            {
                return elements
                    .Where(e => e != null)
                    .SelectMany(e => e.NextAll())
                    .Distinct();
            }
            return Enumerable.Empty<HNode>();
        }

        #endregion

        #region FirstElement()

        /// <summary>
        /// Reduce the set of matched elements to the first one in the set.
        /// </summary>
        public static IEnumerable<HElement> FirstElement(this IEnumerable<HElement> elements)
        {
            if (elements != null)
            {
                var l = elements.FirstOrDefault(e => e != null);
                if (l != null)
                    return new HElement[] { l };
            }
            return Enumerable.Empty<HElement>();
        }

        #endregion

        #region LastElement()

        /// <summary>
        /// Reduce the set of matched elements to the final one in the set.
        /// </summary>
        public static IEnumerable<HElement> LastElement(this IEnumerable<HElement> elements)
        {
            if (elements != null)
            {
                var l = elements.LastOrDefault(e => e != null);
                if (l != null)
                    return new HElement[] { l };
            }
            return Enumerable.Empty<HElement>();
        }

        #endregion

        #region Contents()

        /// <summary>
        /// Get the contents of the element
        /// </summary>
        public static IEnumerable<HNode> Contents(this HElement element)
        {
            if (element != null)
                return element.Nodes();
            return Enumerable.Empty<HNode>();
        }

        /// <summary>
        /// Get the contents of each element of the set
        /// </summary>
        public static IEnumerable<HNode> Contents(this IEnumerable<HElement> elements)
        {
            if (elements != null)
                return elements.Where(e => e != null).SelectMany(e => e.Nodes());
            return Enumerable.Empty<HNode>();
        }

        #endregion

        #region Children()

        /// <summary>
        /// Get te children elements of the elements
        /// </summary>
        public static IEnumerable<HElement> Children(this HElement element)
        {
            if (element != null)
                return element.Elements();
            return Enumerable.Empty<HElement>();
        }

        /// <summary>
        /// Get the children elements of each element of the set
        /// </summary>
        public static IEnumerable<HElement> Children(this IEnumerable<HElement> elements)
        {
            if (elements != null)
                return elements.Where(e => e != null).SelectMany(e => e.Elements());
            return Enumerable.Empty<HElement>();
        }

        #endregion

        #region Text()

        /// <summary>
        /// Get the combined text contents of the element, including his descendants.
        /// </summary>
        public static String Text(this HElement element)
        {
            if (element != null)
            {
                return String.Concat(
                    element
                    .Nodes()
                    .Select(n => (n is HText) ? ((HText)n).Value : (n is HElement) ? ((HElement)n).Text() : String.Empty)
                    );
            }
            return String.Empty;
        }

        /// <summary>
        /// Get the combined text contents of each element in the set, including their descendants.
        /// </summary>
        public static String Text(this IEnumerable<HElement> elements)
        {
            if (elements != null)
            {
                return String.Concat(
                    elements
                    .Where(e => e != null)
                    //.Text() => Optimized by repeat HElement.Text() code.
                    .SelectMany(e => e.Nodes())
                    .Select(n => (n is HText) ? ((HText)n).Value : (n is HElement) ? ((HElement)n).Text() : String.Empty)
                    );
            }
            return String.Empty;
        }

        /// <summary>
        /// Set the content of the element to the specified text.
        /// </summary>
        public static HElement Text(this HElement element, String text)
        {
            if (element != null)
                element.ReplaceWith(new HText(text ?? String.Empty));
            return element;
        }

        /// <summary>
        /// Set the content of each element of the set to the specified text.
        /// </summary>
        public static IEnumerable<HElement> Text(this IEnumerable<HElement> elements, String text)
        {
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    element.Text(text);
                }
            }
            return elements;
        }

        /// <summary>
        /// Set the content of each element of the set to the returned text by the callback.
        /// </summary>
        public static IEnumerable<HElement> Text(this IEnumerable<HElement> elements, Func<HElement, int, String> getText)
        {
            if (elements != null)
            {
                int idx = 0;
                foreach (var element in elements)
                {
                    element.Text(getText != null ? getText(element, idx++) ?? String.Empty : String.Empty);
                }
            }
            return elements;
        }

        #endregion

        #region Html()

        /// <summary>
        /// Get the HTML content of an element
        /// </summary>
        public static String Html(this HElement element)
        {
            if (element != null)
            {
                StringBuilder result = new StringBuilder();
                foreach (var content in element.Nodes())
                {
                    result.Append(HSerializer.DefaultSerializer.SerializeNode(content));
                }
                return result.ToString();
            }
            return String.Empty;
        }

        /// <summary>
        /// Get the html of the first element of the set
        /// </summary>
        public static String Html(this IEnumerable<HElement> elements)
        {
            return elements.FirstElement().FirstOrDefault().Html();
        }

        /// <summary>
        /// Set the HTML content of the element.
        /// </summary>
        public static HElement Html(this HElement element, String html)
        {
            if (element != null)
            {
                element
                    .Empty()
                    .Append(HSerializer.DefaultSerializer.Deserialize(new StringReader(html ?? String.Empty)).ToArray());
            }
            return element;
        }

        /// <summary>
        /// Set the HTML content of each element in the set.
        /// </summary>
        public static IEnumerable<HElement> Html(this IEnumerable<HElement> elements, String html)
        {
            if (elements != null)
            {
                elements
                    .Empty()
                    .Append(HSerializer.DefaultSerializer.Deserialize(new StringReader(html ?? String.Empty)).ToArray());
            }
            return elements;
        }

        /// <summary>
        /// Set the HTML content returned by a callback of each element in the set.
        /// </summary>
        public static IEnumerable<HElement> Html(this IEnumerable<HElement> elements, Func<HElement, int, String> getHtml)
        {
            if (elements != null)
            {
                elements.Empty().Append((e, i) => {
                    String html = getHtml != null ? getHtml(e, i) ?? String.Empty : String.Empty;
                    return HSerializer.DefaultSerializer.Deserialize(new StringReader(html));
                });
            }
            return elements;
        }

        #endregion

    }
}
