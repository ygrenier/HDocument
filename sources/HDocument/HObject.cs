using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc
{

    /// <summary>
    /// Base of Html Object
    /// </summary>
    public abstract class HObject
    {

        #region Fields

        /// <summary>
        /// Parent
        /// </summary>
        internal HContainer parent = null;

        #endregion

        #region Methods

        /// <summary>
        /// Enumerate all parents
        /// </summary>
        protected IEnumerable<HElement> GetParents(String name)
        {
            HContainer p = parent;
            while (p != null)
            {
                HElement e = p as HElement;
                if (e != null && (name == null || String.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase)))
                    yield return e;
                p = p.parent;
            }
        }

        /// <summary>
        /// Returns the parent elements
        /// </summary>
        public IEnumerable<HElement> Parents()
        {
            return GetParents(null);
        }

        /// <summary>
        /// Returns the parent element, with a name filter
        /// </summary>
        public IEnumerable<HElement> Parents(String name)
        {
            return GetParents(name);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Document parent
        /// </summary>
        public HDocument Document
        {
            get
            {
                HObject o = this;
                while (o.parent != null)
                    o = o.parent;
                return o as HDocument;
            }
        }

        /// <summary>
        /// Element parent
        /// </summary>
        public HElement Parent
        {
            get { return parent as HElement; }
        }

        #endregion

    }

}
