using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc
{

    /// <summary>
    /// Html Node
    /// </summary>
    public abstract class HNode : HObject
    {
        internal HNode nextNode;

        /// <summary>
        /// Clone the node
        /// </summary>
        internal abstract HNode CloneNode();

        #region Node management
        
        /// <summary>
        /// Adds the specified content immediately before this node.
        /// </summary>
        /// <param name="content">
        /// A parameter list of content objects.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the parent is null.
        /// </exception>        
        public void AddBefore(params object[] content)
        {
            if (parent == null) throw new InvalidOperationException("No parent found.");
            parent.Insert(this, content);
        }

        /// <summary>
        /// Adds the specified content immediately after this node.
        /// </summary>
        /// <param name="content">
        /// A parameter list of content objects.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the parent is null.
        /// </exception>
        public void AddAfter(params object[] content)
        {
            if (parent == null) throw new InvalidOperationException("No parent found.");
            parent.Insert(this.NextNode, content);
        }

        /// <summary>
        /// Remove this node from his parent.
        /// </summary>
        public void Remove()
        {
            if (parent == null) throw new InvalidOperationException("No parent found.");
            parent.RemoveNode(this);
        }

        #endregion

        /// <summary>
        /// Returns the HTML of the node
        /// </summary>
        public override string ToString()
        {
            return HSerializer.DefaultSerializer.SerializeNode(this);
        }

        #region Properties

        /// <summary>
        /// Gets the next sibling node of this node.
        /// </summary>
        /// <remarks>
        /// If this property does not have a parent, or if there is no next node,
        /// then this property returns null.
        /// </remarks>
        public HNode NextNode
        {
            get { return parent == null || this == parent.content ? null : nextNode; }
        }

        /// <summary>
        /// Gets the previous sibling node of this node.
        /// </summary>
        /// <remarks>
        /// If this property does not have a parent, or if there is no previous node,
        /// then this property returns null.
        /// </remarks>
        public HNode PreviousNode
        {
            get
            {
                if (parent == null) return null;
                HNode n = ((HNode)parent.content).nextNode;
                HNode p = null;
                while (n != this)
                {
                    p = n;
                    n = n.nextNode;
                }
                return p;
            }
        }

        #endregion

    }

}
