using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDoc
{

    /// <summary>
    /// Doctype node
    /// </summary>
    public class HDocumentType : HNode
    {

        #region Standard doctype management

        static IDictionary<StandardDoctype, Tuple<String, String, String, String>> StandardDefinitions = new Dictionary<StandardDoctype, Tuple<String, String, String, String>>() {
            { StandardDoctype.Html5, new Tuple<String,String,String,String>("html", null, null, null) },
            { StandardDoctype.Html401Strict, new Tuple<String,String,String,String>("HTML", "PUBLIC", "-//W3C//DTD HTML 4.01//EN", "http://www.w3.org/TR/html4/strict.dtd") },
            { StandardDoctype.Html401Transitional, new Tuple<String,String,String,String>("HTML", "PUBLIC", "-//W3C//DTD HTML 4.01 Transitional//EN", "http://www.w3.org/TR/html4/loose.dtd") },
            { StandardDoctype.Html401Frameset, new Tuple<String,String,String,String>("HTML", "PUBLIC", "-//W3C//DTD HTML 4.01 Frameset//EN", "http://www.w3.org/TR/html4/frameset.dtd") },
            { StandardDoctype.XHtml10Strict, new Tuple<String,String,String,String>("html", "PUBLIC", "-//W3C//DTD XHTML 1.0 Strict//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd") },
            { StandardDoctype.XHtml10Transitional, new Tuple<String,String,String,String>("html", "PUBLIC", "-//W3C//DTD XHTML 1.0 Transitional//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd") },
            { StandardDoctype.XHtml10Frameset, new Tuple<String,String,String,String>("html", "PUBLIC", "-//W3C//DTD XHTML 1.0 Frameset//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd") },
            { StandardDoctype.XHtml10Basic, new Tuple<String,String,String,String>("html", "PUBLIC", "-//W3C//DTD XHTML Basic 1.0//EN", "http://www.w3.org/TR/xhtml-basic/xhtml-basic10.dtd") },
            { StandardDoctype.XHtml11DTD, new Tuple<String,String,String,String>("html", "PUBLIC", "-//W3C//DTD XHTML 1.1//EN", "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd") },
            { StandardDoctype.XHtml11Basic, new Tuple<String,String,String,String>("html", "PUBLIC", "-//W3C//DTD XHTML Basic 1.1//EN", "http://www.w3.org/TR/xhtml-basic/xhtml-basic11.dtd") },
            { StandardDoctype.Html20, new Tuple<String,String,String,String>("html", "PUBLIC", "-//IETF//DTD HTML 2.0//EN", null) },
            { StandardDoctype.Html32, new Tuple<String,String,String,String>("html", "PUBLIC", "-//W3C//DTD HTML 3.2 Final//EN", null) },
        };

        static bool MatchValue(String a, String b)
        {
            if (String.IsNullOrEmpty(a) && String.IsNullOrEmpty(b)) return true;
            return String.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        static StandardDoctype? FindStandardDoctype(String rootElement, String kind, String fpi, String uri)
        {
            foreach (var sdt in StandardDefinitions)
            {
                if (
                    MatchValue(rootElement, sdt.Value.Item1)
                    && MatchValue(kind, sdt.Value.Item2)
                    && MatchValue(fpi, sdt.Value.Item3)
                    && MatchValue(uri, sdt.Value.Item4)
                    )
                    return sdt.Key;
            }
            return null;
        }

        #endregion

        #region Constants
        /// <summary>
        /// Public kind of DTD keyword
        /// </summary>
        public const String PublicKind = "PUBLIC";
        /// <summary>
        /// System kind of DTD keyword
        /// </summary>
        public const String SystemKind = "SYSTEM";
        #endregion

        /// <summary>
        /// Create a new HTML5 doctype
        /// </summary>
        public HDocumentType()
            : this("html", null, null, null)
        {
        }

        /// <summary>
        /// Create a new document type node
        /// </summary>
        public HDocumentType(String rootElement, String fpi, String uri, String kind)
        {
            if (String.IsNullOrWhiteSpace(rootElement)) throw new ArgumentNullException("rootElement");
            this.RootElement = rootElement;
            this.KindDoctype = kind;
            this.FPI = fpi;
            this.Uri = uri;
            this.StandardType = FindStandardDoctype(this.RootElement, this.KindDoctype, this.FPI, this.Uri);
        }

        /// <summary>
        /// Create a new document from a standard document type
        /// </summary>
        public HDocumentType(StandardDoctype doctype)
        {
            this.StandardType = doctype;
            var sdef = StandardDefinitions[doctype];
            this.RootElement = sdef.Item1;
            this.KindDoctype = sdef.Item2;
            this.FPI = sdef.Item3;
            this.Uri = sdef.Item4;
        }

        /// <summary>
        /// Create a new document type from another
        /// </summary>
        public HDocumentType(HDocumentType other)
        {
            if (other == null) throw new ArgumentNullException("other");
            this.RootElement = other.RootElement;
            this.KindDoctype = other.KindDoctype;
            this.FPI = other.FPI;
            this.Uri = other.Uri;
            this.StandardType = other.StandardType;
        }

        internal override HNode CloneNode()
        {
            return new HDocumentType(this);
        }

        /// <summary>
        /// Root element
        /// </summary>
        public String RootElement { get; set; }

        /// <summary>
        /// Kind of DTD (PUBLIC or SYSTEM)
        /// </summary>
        public String KindDoctype { get; set; }

        /// <summary>
        /// Format Public Identifier
        /// </summary>
        public String FPI { get; set; }

        /// <summary>
        /// DTD Uri
        /// </summary>
        public String Uri { get; set; }

        /// <summary>
        /// Indicate the standard document type or null
        /// </summary>
        public StandardDoctype? StandardType { get; private set; }
    }

    /// <summary>
    /// Standard Doctype
    /// </summary>
    public enum StandardDoctype
    {
        /// <summary>
        /// HTML 5
        /// </summary>
        Html5,
        /// <summary>
        /// HTML 4.01 Strict
        /// </summary>
        Html401Strict,
        /// <summary>
        /// HTML 4.01 Transitional
        /// </summary>
        Html401Transitional,
        /// <summary>
        /// HTML 4.01 Frameset
        /// </summary>
        Html401Frameset,

        /// <summary>
        /// XHTML 1.0 Strict
        /// </summary>
        XHtml10Strict,
        /// <summary>
        /// XHTML 1.0 Transitional
        /// </summary>
        XHtml10Transitional,
        /// <summary>
        /// XHTML 1.0 Frameset
        /// </summary>
        XHtml10Frameset,
        /// <summary>
        /// XHTML 1.0 Basic
        /// </summary>
        XHtml10Basic,

        /// <summary>
        /// XHTML 1.1 - DTD
        /// </summary>
        XHtml11DTD,

        /// <summary>
        /// XHTML Basic 1.1
        /// </summary>
        XHtml11Basic,

        /// <summary>
        /// HTML 2.0
        /// </summary>
        Html20,
        /// <summary>
        /// HTML 3.2
        /// </summary>
        Html32,

//MathML Doctype Declarations
//MathML 2.0 - DTD: <!DOCTYPE math PUBLIC "-//W3C//DTD MathML 2.0//EN"	
//    "http://www.w3.org/Math/DTD/mathml2/mathml2.dtd">
//MathML 1.01 - DTD: <!DOCTYPE math SYSTEM 
//    "http://www.w3.org/Math/DTD/mathml1/mathml.dtd">
//Compound documents doctype declarations
//XHTML + MathML + SVG - DTD: <!DOCTYPE html PUBLIC
//    "-//W3C//DTD XHTML 1.1 plus MathML 2.0 plus SVG 1.1//EN"
//    "http://www.w3.org/2002/04/xhtml-math-svg/xhtml-math-svg.dtd">
//XHTML + MathML + SVG Profile (XHTML as the host language) - DTD: <!DOCTYPE html PUBLIC
//    "-//W3C//DTD XHTML 1.1 plus MathML 2.0 plus SVG 1.1//EN"
//    "http://www.w3.org/2002/04/xhtml-math-svg/xhtml-math-svg.dtd">
//XHTML + MathML + SVG Profile (Using SVG as the host) - DTD: <!DOCTYPE svg:svg PUBLIC
//    "-//W3C//DTD XHTML 1.1 plus MathML 2.0 plus SVG 1.1//EN"
//    "http://www.w3.org/2002/04/xhtml-math-svg/xhtml-math-svg.dtd">
//Optional doctype declarations
//Beyond the specificities of (X)HTML processing, Doctype declarations in XML languages are only useful to declare named entities and to facilitate the validation of documents based on DTDs. This means that in many XML languages, doctype declarations are not necessarily useful.
//The list below is provided only if you actually need to declare a doctype for these types of documents.
//SVG 1.1 Full - DTD: <!DOCTYPE svg PUBLIC "-//W3C//DTD SVG 1.1//EN"
//    "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd">
//SVG 1.0 - DTD: <!DOCTYPE svg PUBLIC "-//W3C//DTD SVG 1.0//EN"
//    "http://www.w3.org/TR/2001/REC-SVG-20010904/DTD/svg10.dtd">
//SVG 1.1 Basic - DTD: <!DOCTYPE svg PUBLIC "-//W3C//DTD SVG 1.1 Basic//EN"
//    "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11-basic.dtd">
//SVG 1.1 Tiny - DTD: <!DOCTYPE svg PUBLIC "-//W3C//DTD SVG 1.1 Tiny//EN"
//    "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11-tiny.dtd">

    }

}
