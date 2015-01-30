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
            if (other == null) throw new ArgumentNullException("other");
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
            HNode n = content as HNode;
            if (n != null)
            {
                AddNode(n);
                return;
            }
            string s = content as string;
            if (s != null)
            {
                AddString(s);
                return;
            }
            HAttribute a = content as HAttribute;
            if (a != null)
            {
                AddAttribute(a);
                return;
            }
            object[] o = content as object[];
            if (o != null)
            {
                foreach (object obj in o) Add(obj);
                return;
            }
            IEnumerable e = content as IEnumerable;
            if (e != null)
            {
                foreach (object obj in e) Add(obj);
                return;
            }
            AddString(content.ToString());
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
        /// Internal adding a node.
        /// </summary>
        void AddNode(HNode n)
        {
            throw new NotImplementedException();
            //ValidateNode(n, this);
            //if (n.parent != null)
            //{
            //    n = n.CloneNode();
            //}
            //else
            //{
            //    HNode p = this;
            //    while (p.parent != null) p = p.parent;
            //    if (n == p) n = n.CloneNode();
            //}
            //ConvertContentTextToNode();
            //AppendNode(n);
        }

        /// <summary>
        /// Add node in the list.
        /// </summary>
        void AppendNode(HNode n)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Internal adding a string.
        /// </summary>
        void AddString(String s)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Internal adding an attribute.
        /// </summary>
        internal virtual void AddAttribute(HAttribute attribute)
        {
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

        void ValidateNode(HNode node, HContainer parent)
        {
            throw new NotImplementedException();
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

        #endregion

    }
}
