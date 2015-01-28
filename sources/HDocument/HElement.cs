using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc
{

    /// <summary>
    /// Html element
    /// </summary>
    /// <remarks>
    /// An element has a tagname, some attributes, and content.
    /// </remarks>
    public class HElement : HContainer
    {
        #region Fields

        /// <summary>
        /// Reference the last attribute of the list
        /// </summary>
        internal HAttribute lastAttribute = null;

        #endregion

        #region Ctors & Dests

        /// <summary>
        /// Create a new element
        /// </summary>
        public HElement(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            this.Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Tag name
        /// </summary>
        public String Name { get; private set; }

        #endregion

    }

}
