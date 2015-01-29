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
        /// Insert the content after a node
        /// </summary>
        /// <param name="parent">Parent</param>
        /// <param name="node">Node</param>
        /// <param name="content">Content to insert.</param>
        public static void InsertAfter(HContainer parent, HNode node, params object[] content)
        {
            if (content == null || content.Length == 0) return;
            HNode anchor = node;
            foreach (var cnt in content)
                InsertContentAfter(parent, ref anchor, cnt);
        }

        static void InsertContentAfter(HContainer parent, ref HNode node, object content)
        {

        }
    }

}
