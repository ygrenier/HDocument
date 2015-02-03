using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests
{
    public class HSerializerTest
    {

        [Fact]
        public void TestSerialize()
        {
            StringBuilder html=new StringBuilder();
            var writer = new StringWriter(html);

            var doc = new HDocument(
                new HDocumentType(),
                new HElement(
                    "html",
                    new HElement(
                        "header",
                        new HElement("title", "Title of the document")
                        ),
                        new HElement(
                            "body",
                            new HElement(
                                "div",
                                new HAttribute("class", "container"),
                                "This is the content of the container.",
                                new HElement("br"),
                                "\r\nTexte avec des lettres échappées : &é~\"'èçàêëù!µ."
                                )
                            )
                    )
                );

            var serializer = new HSerializer();
            serializer.Serialize(doc, writer);

            var expected = new StringBuilder();
            expected.AppendLine("<!DOCTYPE html>");
            expected.Append("<html>");
            expected.Append("<header>");
            expected.Append("<title>Title of the document</title>");
            expected.Append("</header>");
            expected.Append("<body>");
            expected.Append("<div class=\"container\">");
            expected.Append("This is the content of the container.");
            expected.Append("<br />\r\n");
            expected.Append("Texte avec des lettres &eacute;chapp&eacute;es : &amp;&eacute;~&quot;'&egrave;&ccedil;&agrave;&ecirc;&euml;&ugrave;!&micro;.");
            expected.Append("</div>");
            expected.Append("</body>");
            expected.Append("</html>");
            Assert.Equal(expected.ToString(), html.ToString());

            Assert.Throws<ArgumentNullException>(() => serializer.Serialize(null, null));
            Assert.Throws<ArgumentNullException>(() => serializer.Serialize(doc, null));

        }

        #region XmlDeclaration

        [Fact]
        public void TestXmlDeclaration()
        {
            var doc = new HDocument(
                new HXmlDeclaration(),
                new HElement("html")
                );

            StringBuilder html = new StringBuilder();
            var writer = new StringWriter(html);
            var serializer = new HSerializer();
            serializer.Serialize(doc, writer);

            var expected = new StringBuilder();
            expected.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            expected.Append("<html></html>");
            Assert.Equal(expected.ToString(), html.ToString());

        }

        [Fact]
        public void TestXmlDeclaration_EmptyDeclaration()
        {
            var doc = new HDocument(
                new HXmlDeclaration(null, null, null),
                new HElement("html")
                );

            StringBuilder html = new StringBuilder();
            var writer = new StringWriter(html);
            var serializer = new HSerializer();
            serializer.Serialize(doc, writer);

            var expected = new StringBuilder();
            expected.AppendLine("<?xml ?>");
            expected.Append("<html></html>");
            Assert.Equal(expected.ToString(), html.ToString());

        }

        [Fact]
        public void TestXmlDeclaration_Full()
        {
            var doc = new HDocument(
                new HXmlDeclaration("version", "encoding", "standalone"),
                new HElement("html")
                );

            StringBuilder html = new StringBuilder();
            var writer = new StringWriter(html);
            var serializer = new HSerializer();
            serializer.Serialize(doc, writer);

            var expected = new StringBuilder();
            expected.AppendLine("<?xml version=\"version\" encoding=\"encoding\" standalone=\"standalone\" ?>");
            expected.Append("<html></html>");
            Assert.Equal(expected.ToString(), html.ToString());

        }

        #endregion

        #region Doctype

        [Fact]
        public void TestDocumentType()
        {
            StringBuilder html = new StringBuilder();
            var writer = new StringWriter(html);

            var doc = new HDocument(
                new HDocumentType(),
                new HElement("html")
                );

            var serializer = new HSerializer();
            serializer.Serialize(doc, writer);

            var expected = new StringBuilder();
            expected.AppendLine("<!DOCTYPE html>");
            expected.Append("<html></html>");
            Assert.Equal(expected.ToString(), html.ToString());

        }

        [Fact]
        public void TestDocumentType_Full()
        {
            StringBuilder html = new StringBuilder();
            var writer = new StringWriter(html);

            var doc = new HDocument(
                new HDocumentType(StandardDoctype.XHtml10Transitional),
                new HElement("html")
                );

            var serializer = new HSerializer();
            serializer.Serialize(doc, writer);

            var expected = new StringBuilder();
            expected.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            expected.Append("<html></html>");
            Assert.Equal(expected.ToString(), html.ToString());

        }

        #endregion

        #region Comments

        [Fact]
        public void TestComments()
        {
            StringBuilder html = new StringBuilder();
            var writer = new StringWriter(html);

            var doc = new HDocument(
                new HComment("First comment before doctype"),
                new HDocumentType(),
                new HComment("First second comments before <html>"),
                new HElement(
                    "html",
                    new HComment("Another comments \n with multiple lines")
                    )
                );

            var serializer = new HSerializer();
            serializer.Serialize(doc, writer);

            var expected = new StringBuilder();
            expected.Append("<!-- First comment before doctype -->");
            expected.AppendLine("<!DOCTYPE html>");
            expected.Append("<!-- First second comments before &lt;html&gt; -->");
            expected.Append("<html>");
            expected.Append("<!-- Another comments \n with multiple lines -->");
            expected.Append("</html>");
            Assert.Equal(expected.ToString(), html.ToString());

        }

        #endregion

        #region CData

        [Fact]
        public void TestCData()
        {
            StringBuilder html = new StringBuilder();
            var writer = new StringWriter(html);

            var doc = new HDocument(
                new HDocumentType(),
                new HElement(
                    "html",
                    new HCData("A content <CDATA> \n with multiple lines and é accented")
                    )
                );

            var serializer = new HSerializer();
            serializer.Serialize(doc, writer);

            var expected = new StringBuilder();
            expected.AppendLine("<!DOCTYPE html>");
            expected.Append("<html>");
            expected.Append("<![CDATA[A content <CDATA> \n with multiple lines and é accented]]>");
            expected.Append("</html>");
            Assert.Equal(expected.ToString(), html.ToString());

        }

        #endregion

        #region Text

        [Fact]
        public void TestText()
        {
            StringBuilder html = new StringBuilder();
            var writer = new StringWriter(html);

            var doc = new HDocument(
                new HDocumentType(),
                new HElement(
                    "html",
                    new HText("A content <text> \n with multiple lines and é accented")
                    )
                );

            var serializer = new HSerializer();
            serializer.Serialize(doc, writer);

            var expected = new StringBuilder();
            expected.AppendLine("<!DOCTYPE html>");
            expected.Append("<html>");
            expected.Append("A content &lt;text&gt; \n with multiple lines and &eacute; accented");
            expected.Append("</html>");
            Assert.Equal(expected.ToString(), html.ToString());

        }

        #endregion

        #region Raw Text

        [Fact]
        public void TestRawText()
        {
            StringBuilder html = new StringBuilder();
            var writer = new StringWriter(html);

            var doc = new HDocument(
                new HDocumentType(),
                new HElement(
                    "html",
                    new HElement("script", "A content <text> \n with multiple lines and é accented"),
                    new HElement("div", "A content <text> \n with multiple lines and é accented")
                    )
                );

            var serializer = new HSerializer();
            serializer.Serialize(doc, writer);

            var expected = new StringBuilder();
            expected.AppendLine("<!DOCTYPE html>");
            expected.Append("<html>");
            expected.Append("<script>A content <text> \n with multiple lines and é accented</script>");
            expected.Append("<div>A content &lt;text&gt; \n with multiple lines and &eacute; accented</div>");
            expected.Append("</html>");
            Assert.Equal(expected.ToString(), html.ToString());

        }

        #endregion

        #region Escapable Raw Text

        [Fact]
        public void TestEscapableRawText()
        {
            var doc = new HDocument(
                new HDocumentType(),
                new HElement(
                    "html",
                    new HElement("textarea", "A content <text> \n with multiple lines and é accented"),
                    new HElement("div", "A content <text> \n with multiple lines and é accented")
                    )
                );

            var serializer = new HSerializer();
            String html = serializer.Serialize(doc);

            var expected = new StringBuilder();
            expected.AppendLine("<!DOCTYPE html>");
            expected.Append("<html>");
            expected.Append("<textarea>A content &lt;text&gt; \n with multiple lines and &eacute; accented</textarea>");
            expected.Append("<div>A content &lt;text&gt; \n with multiple lines and &eacute; accented</div>");
            expected.Append("</html>");
            Assert.Equal(expected.ToString(), html);

        }

        #endregion

    }
}
