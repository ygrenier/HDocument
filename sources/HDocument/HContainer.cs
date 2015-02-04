using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc
{
    /// <summary>
    /// Base of HNode container
    /// </summary>
    /// <remarks>
    /// Like XDocument, HContainer manage a content wich can be string or a node.
    /// </remarks>
    public abstract class HContainer : HNode
    {
        /// <summary>
        /// Content
        /// </summary>
        internal object content;

        internal HContainer()
        {
        }

        internal HContainer(HContainer other)
        {
            if (other.content is string)
            {
                this.content = other.content;
            }
            else
            {
                HNode n = (HNode)other.content;
                if (n != null)
                {
                    do
                    {
                        n = n.nextNode;
                        AppendNode(n.CloneNode());
                    } while (n != other.content);
                }
            }
        }

        #region Content managemement
        
        /// <summary>
        /// Adding a content to the container.
        /// </summary>
        /// <param name="content">Content to add</param>
        public void Add(object content)
        {
            if (content == null) return;
            Insert(null, content);
        }

        /// <summary>
        /// Adding a list of content
        /// </summary>
        /// <param name="content">List of content</param>
        public void Add(params object[] content)
        {
            Add((object)content);
        }

        /// <summary>
        /// Insert a content before the <param name="insert" /> node.
        /// </summary>
        internal void Insert(HNode insert, object content)
        {
            if (content == null) return;
            HNode n = content as HNode;
            if (n != null)
            {
                InsertNode(insert, n);
                return;
            }
            string s = content as string;
            if (s != null)
            {
                InsertString(insert, s);
                return;
            }
            HAttribute a = content as HAttribute;
            if (a != null)
            {
                AddAttribute(a);
                return;
            }
            IEnumerable e = content as IEnumerable;
            if (e != null)
            {
                foreach (object obj in e)
                    Insert(insert, obj);
                return;
            }
            InsertString(insert, content.ToString());
        }

        /// <summary>
        /// Add node at the end of the list.
        /// </summary>
        void AppendNode(HNode n)
        {
            InsertNode(null, n);
        }

        /// <summary>
        /// Internal insertion of string.
        /// </summary>
        void InsertString(HNode before, String s)
        {
            // Valid the string
            ValidateString(s);
            // If no content the we affect the texte
            if (content == null)
            {
                content = s;
            }
            else if (s.Length > 0)
            {
                if (before == null)
                {
                    // If the content is a string, we concat them
                    if (content is string)
                    {
                        content = (string)content + s;
                    }
                    else
                    {
                        // If the content is an HText node the we adding the string to the node
                        HText tn = content as HText;
                        if (tn != null && !(tn is HCData))
                        {
                            tn.value += s;
                        }
                        else
                        {
                            AppendNode(new HText(s));
                        }
                    }
                }
                else
                {
                    InsertNode(before, new HText(s));
                }
            }
        }

        /// <summary>
        /// Insert <paramref name="node"/> before the <paramref name="before"/> node.
        /// </summary>
        /// <remarks>
        /// If previous is null then insert the node at the beginning of the list.
        /// If previous is current content, then the node is added at the end.
        /// </remarks>
        void InsertNode(HNode before, HNode node)
        {
            // Validate the node
            ValidateNode(node, this);
            // If the node have a parent we clone it
            if (node.Parent != null)
            {
                node = node.CloneNode();
            }
            else
            {
                // If node is the parent of this container, then we clone it
                HNode p = this;
                while (p.parent != null) p = p.Parent;
                if (node == p) node = node.CloneNode();
            }
            // Set the parent
            node.parent = this;
            // Content empty ?
            if (content == null)
            {
                node.nextNode = node;
                content = node;
            }
            else if (before == null)
            {
                ConvertContentTextToNode();
                // Add at the end of the list
                node.nextNode = ((HNode)content).nextNode;
                ((HNode)content).nextNode = node;
                content = node;
            }
            else
            {
                // Search the 'previous' before node
                var previousBefore = before;
                while (previousBefore.nextNode != before)
                    previousBefore = previousBefore.nextNode;
                // Insert the node to the list
                node.nextNode = before;
                previousBefore.nextNode = node;
            }
        }

        /// <summary>
        /// Internal adding an attribute.
        /// </summary>
        internal virtual void AddAttribute(HAttribute attribute)
        {
        }

        /// <summary>
        /// Internal remove a node.
        /// </summary>
        internal void RemoveNode(HNode node)
        {
            // If node is alone, reset the list
            if (node.nextNode == node)
            {
                this.content = null;
            }
            else
            {
                // Find previous node
                var prev = node.nextNode;
                while (prev.nextNode != node) prev = prev.nextNode;
                // Clean the list
                prev.nextNode = node.nextNode;
                // If the node is the last, then prev become the last
                if (this.content == node)
                    this.content = prev;
            }
            // Detach node
            node.parent = null;
            node.nextNode = null;
        }

        /// <summary>
        /// Remove all nodes
        /// </summary>
        public void RemoveNodes()
        {
            if (this.content is HNode)
            {
                HNode node = (HNode)this.content;
                while (node != null)
                {
                    node.parent = null;
                    var n = node.nextNode;
                    node.nextNode = null;
                    node = n;
                }
            }
            this.content = null;
        }

        /// <summary>
        /// If the current content is a string, convert it to a node.
        /// </summary>
        void ConvertContentTextToNode()
        {
            String s = content as String;
            if (s != null && s.Length > 0)
            {
                HText n = new HText(s);
                n.parent = this;
                n.nextNode = n;
                content = n;
            }
        }

        /// <remarks>
        /// Validate insertion of the given node. previous is the node after which insertion
        /// will occur. previous == null means at beginning, previous == this means at end.
        /// </remarks>
        internal virtual void ValidateNode(HNode node, HNode previous)
        {
        }

        internal virtual void ValidateString(string s)
        {
        }

        /// <summary>
        /// Enumerate all child elements
        /// </summary>
        protected IEnumerable<HElement> GetElements(String name)
        {
            HNode n = content as HNode;
            if (n != null)
            {
                do
                {
                    n = n.nextNode;
                    HElement e = n as HElement;
                    if (e != null && (name == null || String.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase))) 
                        yield return e;
                } while (n.parent == this && n != content);
            }
        }

        /// <summary>
        /// Returns the elements contained in this container.
        /// </summary>
        public IEnumerable<HElement> Elements()
        {
            return GetElements(null);
        }

        /// <summary>
        /// Returns the elements contained in this container, with a name filter
        /// </summary>
        /// <param name="name">Name to filter</param>
        /// <returns>An enumerable containing the attributes matching the name.</returns>
        public IEnumerable<HElement> Elements(String name)
        {
            return name != null ? GetElements(name) : Enumerable.Empty<HElement>();
        }

        /// <summary>
        /// Returns the nodes contained in this element.
        /// </summary>
        public IEnumerable<HNode> Nodes()
        {
            HNode n = LastNode;
            if (n != null)
            {
                do
                {
                    n = n.nextNode;
                    yield return n;
                } while (n.parent == this && n != content);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the first child node of this node.
        /// </summary>
        public HNode FirstNode
        {
            get
            {
                HNode last = LastNode;
                return last != null ? last.nextNode : null;
            }
        }

        /// <summary>
        /// Get the last child node of this node.
        /// </summary>
        public HNode LastNode
        {
            get
            {
                // If the content is null or a node we found our last node
                if (content == null) return null;
                HNode node = content as HNode;
                if (node != null) return node;
                // If the content is a string we convert it to HText
                string str = content as string;
                if (str != null)
                {
                    if (str.Length == 0) return null;
                    HText txt = new HText(str);
                    txt.parent = this;
                    txt.nextNode = txt;
                    content = txt;
                }
                return (HNode)content;
            }
        }

        /// <summary>
        /// Indicates if contains one or more nodes
        /// </summary>
        public bool HasNodes { get { return content != null; } }

        /// <summary>
        /// Indicates if contains one or more elements
        /// </summary>
        public bool HasElements { get { return GetElements(null).Any(); } }

        #endregion

    }
}
