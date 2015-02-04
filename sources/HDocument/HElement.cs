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

        /// <summary>
        /// Create an new element with a content
        /// </summary>
        public HElement(String name, object content)
            : this(name)
        {
            Add(content);
        }

        /// <summary>
        /// Create an new element with a multiple content
        /// </summary>
        public HElement(String name, params object[] content)
            : this(name, (object)content)
        {
        }

        /// <summary>
        /// Create a new element from an another.
        /// </summary>
        /// <param name="other">
        /// Another element that will be copied to this element.
        /// </param>
        public HElement(HElement other) : base(other) {
            this.Name = other.Name;
            HAttribute a = other.lastAttribute;
            if (a != null) {
                do {
                    a = a.nextAttribute;
                    AddAttribute(new HAttribute(a));
                } while (a != other.lastAttribute);
            }
        }

        #endregion

        internal override HNode CloneNode()
        {
            return new HElement(this);
        }

        #region Attributes management

        /// <summary>
        /// Returns the <see cref="HAttribute"/> associated with a name.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="HAttribute"/> to get.
        /// </param>
        /// <returns>
        /// The <see cref="HAttribute"/> with the name passed in.  If there is no attribute
        /// with this name then null is returned.
        /// </returns>
        public HAttribute Attribute(String name)
        {
            HAttribute a = lastAttribute;
            if (name != null && a != null)
            {
                do
                {
                    a = a.nextAttribute;
                    if (String.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase)) return a;
                } while (a != lastAttribute);
            }
            return null;
        }

        /// <summary>
        /// Returns the attributes associated to this element.
        /// </summary>
        /// <returns>An enumerable containing all attributes.</returns>
        public IEnumerable<HAttribute> Attributes()
        {
            return GetAttributes(null);
        }

        /// <summary>
        /// Returns the attributes associated to this element, with a name filter.
        /// </summary>
        /// <param name="name">Name to filter</param>
        /// <returns>An enumerable containing the attributes matching the name.</returns>
        public IEnumerable<HAttribute> Attributes(String name)
        {
            return name != null ? GetAttributes(name) : Enumerable.Empty<HAttribute>();
        }

        /// <summary>
        /// Adding an attribute
        /// </summary>
        internal override void AddAttribute(HAttribute attribute)
        {
            if (Attribute(attribute.Name) != null)
                throw new InvalidOperationException(String.Format("The attribute '{0}' already defined", attribute.Name));
            // Attribute already affected in a parent
            if (attribute.parent != null) attribute = new HAttribute(attribute);
            // Insert attribute
            attribute.parent = this;
            if (lastAttribute == null)
            {
                attribute.nextAttribute = attribute;
            }
            else
            {
                attribute.nextAttribute = lastAttribute.nextAttribute;
                lastAttribute.nextAttribute = attribute;
            }
            lastAttribute = attribute;
        }

        /// <summary>
        /// Removing an attribute
        /// </summary>
        internal void RemoveAttribute(HAttribute attribute)
        {
            // If attribute is alone, reset the list
            if (attribute.nextAttribute == attribute)
            {
                this.lastAttribute = null;
            }
            else
            {
                // Find previous attribute
                var prev = attribute.nextAttribute;
                while (prev.nextAttribute != attribute) prev = prev.nextAttribute;
                // Clean the list
                prev.nextAttribute = attribute.nextAttribute;
                // If attribute is the last, then prev become the last
                if (this.lastAttribute == attribute)
                    this.lastAttribute = prev;
            }
            // Detach attribute
            attribute.parent = null;
            attribute.nextAttribute = null;
        }

        /// <summary>
        /// Enumerate attributes with an optional name filter.
        /// </summary>
        /// <param name="name">Name filtered</param>
        /// <returns>Enumerate the attributes. If <paramref name="name"/> is null, all attributes are returned.</returns>
        IEnumerable<HAttribute> GetAttributes(String name)
        {
            HAttribute a = lastAttribute;
            if (a != null)
            {
                do
                {
                    a = a.nextAttribute;
                    if (name == null || String.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase))
                        yield return a;
                } while (a.parent == this && a != lastAttribute);
            }
        }

        /// <summary>
        /// Remove all attributes
        /// </summary>
        public void RemoveAttributes()
        {
            var attr = this.lastAttribute;
            while (attr != null)
            {
                attr.parent = null;
                var n = attr.nextAttribute;
                attr.nextAttribute = null;
                attr = n;
            }
            this.lastAttribute = null;
        }

        #endregion

        #region Content management

        /// <summary>
        /// Can't accept a document or document type nodes
        /// </summary>
        internal override void ValidateNode(HNode node, HNode previous)
        {
            base.ValidateNode(node, previous);
            if (node is HDocument)
                throw new ArgumentException("Can't add a document in a element.");
            if (node is HDocumentType)
                throw new ArgumentException("Can't add a document type in a element.");
        }

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
