using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc
{

    /// <summary>
    /// Helpers
    /// </summary>
    static class Helpers
    {

        /// <summary>
        /// Insert content after a node
        /// </summary>
        /// <param name="parent">Parent</param>
        /// <param name="node">Node</param>
        /// <param name="content">Content to insert.</param>
        public static void InsertAfter(HContainer parent, HNode node, params object[] content)
        {
            if (parent == null || content == null || content.Length == 0) return;
            HNode anchor = node;
            foreach (var cnt in content)
                anchor = InsertContentAfter(parent, anchor, cnt);
        }

        static HNode InsertContentAfter(HContainer parent, HNode node, object content)
        {
            // If content null stop insert
            if (content == null) return node;

            // Parent is empty ?
            if (parent.content == null)
            {
                // 
            }
            else
            {

            }
            return node;
        }
    }

}
