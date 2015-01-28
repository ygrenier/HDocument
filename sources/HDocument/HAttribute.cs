using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc
{

    /// <summary>
    /// Html Attribute
    /// </summary>
    public class HAttribute : HObject
    {
        #region Fields
        private String _Value;
        internal HAttribute nextAttribute;
        #endregion

        #region Const & Dest

        /// <summary>
        /// Create a new attribute
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="value">Value of the attribute</param>
        /// <exception cref="ArgumentNullException">
        /// Throws if the passed name is null or empty, or the value is null.
        /// </exception>
        public HAttribute(String name, String value = "")
        {
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("name");
            if (value == null) throw new ArgumentNullException("value");
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Create a new attribute from an other attribute
        /// </summary>
        /// <param name="other">Attribute to copy from.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws if the passed attribute is null.
        /// </exception>
        public HAttribute(HAttribute other)
        {
            if (other == null) throw new ArgumentNullException("other");
            this.Name = other.Name;
            this.Value = other.Value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes this attribute.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the parent element is null.
        /// </exception>
        public void Remove()
        {
            //if (parent == null) throw new InvalidOperationException(Res.GetString(Res.InvalidOperation_MissingParent));
            //((HElement)parent).RemoveAttribute(this);
            throw new NotImplementedException();
        }

        #endregion

        #region Operators

        /// <summary>
        /// Cast the value of this <see cref="HAttribute"/> to a <see cref="string"/>.
        /// </summary>
        /// <param name="attribute">
        /// The <see cref="HAttribute"/> to cast to <see cref="string"/>.
        /// </param>
        /// <returns>
        /// The content of this <see cref="HAttribute"/> value as a <see cref="string"/>.
        /// </returns>
        public static explicit operator string(HAttribute attribute)
        {
            return (attribute != null) ? attribute._Value : null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Name
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// Value
        /// </summary>
        public String Value
        {
            get { return _Value; }
            set {
                if (value == null) throw new ArgumentNullException("value");
                _Value = value; 
            }
        }

        /// <summary>
        /// Gets the next attribute of the parent element.
        /// </summary>
        /// <remarks>
        /// If this attribute does not have a parent, or if there is no next attribute,
        /// then this property returns null.
        /// </remarks>
        public HAttribute NextAttribute
        {
            get { return parent != null && ((HElement)parent).lastAttribute != this ? nextAttribute : null; }
        }

        /// <summary>
        /// Gets the previous attribute of the parent element.
        /// </summary>
        /// <remarks>
        /// If this attribute does not have a parent, or if there is no previous attribute,
        /// then this property returns null.
        /// </remarks>
        public HAttribute PreviousAttribute
        {
            get
            {
                if (parent == null) return null;
                HAttribute a = ((HElement)parent).lastAttribute;
                while (a.nextAttribute != this)
                    a = a.nextAttribute;
                return a != ((HElement)parent).lastAttribute ? a : null;
            }
        }

        #endregion

    }

}
