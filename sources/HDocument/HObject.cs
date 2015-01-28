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
