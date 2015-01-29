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

        #region Attributes management



        #endregion

        #region Properties

        /// <summary>
        /// Tag name
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// Gets the first attribute
        /// </summary>
        public HAttribute FirstAttribute
        {
            get { return lastAttribute != null ? lastAttribute.nextAttribute : null; }
        }

        /// <summary>
        /// Gets the last attribute
        /// </summary>
        public HAttribute LastAttribute
        {
            get { return lastAttribute; }
        }

        /// <summary>
        /// Gets a value indicating whether the element as at least one attribute.
        /// </summary>
        public bool HasAttributes
        {
            get { return lastAttribute != null; }
        }

        #endregion

    }

}
