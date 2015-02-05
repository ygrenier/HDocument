using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HDoc
{

    /// <summary>
    /// HTML Parser
    /// </summary>
    public class HParser
    {
        #region Inner types
        enum ParseState
        {
            InText
        }

        #endregion

        /// <summary>
        /// Create a new parser
        /// </summary>
        public HParser(TextReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            this.Reader = reader;
            this.EOF = false;
        }

        /// <summary>
        /// Current reader
        /// </summary>
        public TextReader Reader { get; private set; }

        /// <summary>
        /// End of the stream
        /// </summary>
        public bool EOF { get; private set; }

    }

}
