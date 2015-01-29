﻿using System;
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
