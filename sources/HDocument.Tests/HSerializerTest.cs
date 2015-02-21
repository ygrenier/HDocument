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

        #region Serialization

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
            expected.Append("<!DOCTYPE html>");
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

        [Fact]
        public void TestSerialize_DirectToString()
        {
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
            String html = serializer.Serialize(doc);

            var expected = new StringBuilder();
            expected.Append("<!DOCTYPE html>");
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

            Assert.Throws<ArgumentNullException>(() => serializer.Serialize(null));

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
            expected.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
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
            expected.Append("<?xml ?>");
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
            expected.Append("<?xml version=\"version\" encoding=\"encoding\" standalone=\"standalone\" ?>");
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
            expected.Append("<!DOCTYPE html>");
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
            expected.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
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
            expected.Append("<!DOCTYPE html>");
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
            expected.Append("<!DOCTYPE html>");
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
            expected.Append("<!DOCTYPE html>");
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
            expected.Append("<!DOCTYPE html>");
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
            expected.Append("<!DOCTYPE html>");
            expected.Append("<html>");
            expected.Append("<textarea>A content &lt;text&gt; \n with multiple lines and &eacute; accented</textarea>");
            expected.Append("<div>A content &lt;text&gt; \n with multiple lines and &eacute; accented</div>");
            expected.Append("</html>");
            Assert.Equal(expected.ToString(), html);

        }

        #endregion

        #endregion

        #region Deserialization

        [Fact]
        public void TestDeserializeDocument()
        {
            var serializer = new HSerializer();

            var hdoc = serializer.DeserializeDocument(new StringReader("<html><body><h1>Document</h1><p>Content &amp; more.</p></body></html>"));

            Assert.Same(Encoding.UTF8, hdoc.Encoding);

            // Document with one root
            Assert.Equal(1, hdoc.Nodes().Count());
            Assert.Equal("html", hdoc.Root.Name);

            // Root with one body
            Assert.Equal(1, hdoc.Root.Nodes().Count());
            HElement body = hdoc.Root.FirstNode as HElement;
            Assert.NotNull(body);
            Assert.Equal("body", body.Name);

            // Body contains two elements
            Assert.Equal(2, body.Nodes().Count());
            var elms = body.Elements().ToArray();
            Assert.Equal(2, elms.Length);

            // First h1
            Assert.Equal("h1", elms[0].Name);
            Assert.Equal(1, elms[0].Nodes().Count());
            Assert.IsType<HText>(elms[0].FirstNode);
            Assert.Equal("Document", ((HText)elms[0].FirstNode).Value);

            // Second p
            Assert.Equal("p", elms[1].Name);
            Assert.Equal(1, elms[1].Nodes().Count());
            Assert.IsType<HText>(elms[1].FirstNode);
            Assert.Equal("Content & more.", ((HText)elms[1].FirstNode).Value);

            // Test from stream
            using (var ms = new MemoryStream(Encoding.ASCII.GetBytes("<html><body><h1>Document</h1><p>Content &amp; more.</p></body></html>")))
            {
                hdoc = serializer.DeserializeDocument(new StreamReader(ms, Encoding.ASCII));
                Assert.Same(Encoding.ASCII, hdoc.Encoding);
                Assert.Equal(6, hdoc.DescendantNodes().Count());
                Assert.Equal(0, hdoc.ParseErrors.Length);
            }

            Assert.Throws<ArgumentNullException>(() => serializer.DeserializeDocument(null));

        }

        [Fact]
        public void TestDeserializeDocument_WithErrors()
        {
            var serializer = new HSerializer();

            var hdoc = serializer.DeserializeDocument(new StringReader("<html><body><h1>Document</h1><p class=>Content &amp; more.</p></body></html>"));

            Assert.Same(Encoding.UTF8, hdoc.Encoding);

            // Document with one root
            Assert.Equal(1, hdoc.Nodes().Count());
            Assert.Equal("html", hdoc.Root.Name);

            // Root with one body
            Assert.Equal(1, hdoc.Root.Nodes().Count());
            HElement body = hdoc.Root.FirstNode as HElement;
            Assert.NotNull(body);
            Assert.Equal("body", body.Name);

            // Body contains two elements
            Assert.Equal(2, body.Nodes().Count());
            var elms = body.Elements().ToArray();
            Assert.Equal(2, elms.Length);

            // First h1
            Assert.Equal("h1", elms[0].Name);
            Assert.Equal(1, elms[0].Nodes().Count());
            Assert.IsType<HText>(elms[0].FirstNode);
            Assert.Equal("Document", ((HText)elms[0].FirstNode).Value);

            // Second p
            Assert.Equal("p", elms[1].Name);
            Assert.Equal(1, elms[1].Nodes().Count());
            Assert.IsType<HText>(elms[1].FirstNode);
            Assert.Equal("Content & more.", ((HText)elms[1].FirstNode).Value);

            // Check errors
            Assert.Equal(1, hdoc.ParseErrors.Length);
            Assert.Equal("Attribute value expected.", hdoc.ParseErrors[0].Message);

        }

        [Fact]
        public void TestDeserialize()
        {
            var serializer = new HSerializer();

            StringBuilder html = new StringBuilder();
            html.Append("<p class=first>Start<div>Begin<span>Content</span>End</div>Stop</p>");
            html.Append("Title : <h1 class=\"next\">Document</h1>");
            html.Append("Content : <p class='last'>Content &amp; more.</p>The end !");

            var nodes = serializer.Deserialize(new StringReader(html.ToString()));

            var gEnum = nodes.GetEnumerator();

            // p
            Assert.True(gEnum.MoveNext());
            Assert.IsType<HElement>(gEnum.Current);
            var elm = (HElement)gEnum.Current;
            Assert.Equal("p", elm.Name);
            Assert.Equal(1, elm.Attributes().Count());
            Assert.Equal(7, elm.DescendantNodes().Count());

            // Text
            Assert.True(gEnum.MoveNext());
            Assert.IsType<HText>(gEnum.Current);
            Assert.Equal("Title : ", ((HText)gEnum.Current).Value);

            // h1
            Assert.True(gEnum.MoveNext());
            Assert.IsType<HElement>(gEnum.Current);
            elm = (HElement)gEnum.Current;
            Assert.Equal("h1", elm.Name);
            Assert.Equal(1, elm.Attributes().Count());
            Assert.Equal(1, elm.DescendantNodes().Count());

            // Text
            Assert.True(gEnum.MoveNext());
            Assert.IsType<HText>(gEnum.Current);
            Assert.Equal("Content : ", ((HText)gEnum.Current).Value);

            // p
            Assert.True(gEnum.MoveNext());
            Assert.IsType<HElement>(gEnum.Current);
            elm = (HElement)gEnum.Current;
            Assert.Equal("p", elm.Name);
            Assert.Equal(1, elm.Attributes().Count());
            Assert.Equal(1, elm.DescendantNodes().Count());

            // Text
            Assert.True(gEnum.MoveNext());
            Assert.IsType<HText>(gEnum.Current);
            Assert.Equal("The end !", ((HText)gEnum.Current).Value);

            // End
            Assert.False(gEnum.MoveNext());
        }

        [Fact]
        public void TestDeserializeTestPage1()
        {
            String pageContent;
            using (var pageStream = this.GetType().Assembly.GetManifestResourceStream("HDoc.Tests.Resources.TestPage1.html"))
            using (var reader = new StreamReader(pageStream))
                pageContent = reader.ReadToEnd();

            using (var pageStream = this.GetType().Assembly.GetManifestResourceStream("HDoc.Tests.Resources.TestPage1.html"))
            using (var reader = new StreamReader(pageStream))
            {
                var serializer = new HSerializer();
                var doc = serializer.DeserializeDocument(reader);

                Assert.Same(Encoding.UTF8, doc.Encoding);
                Assert.Null(doc.XmlDeclaration);
                Assert.Equal(StandardDoctype.Html5, doc.DocumentType.StandardType);

                var nodes = doc.Nodes().ToArray();
                Assert.Equal(3, nodes.Length);
                Assert.IsType<HDocumentType>(nodes[0]);
                Assert.IsType<HText>(nodes[1]);
                Assert.IsType<HElement>(nodes[2]);

                nodes = doc.DescendantNodes().ToArray();
                Assert.Equal(129, nodes.Length);

                var elms = doc.Descendants().ToArray();
                Assert.Equal(46, elms.Length);

                // Correct false HTML source
                String tmp = pageContent
                    .Replace("src=\"http://placekitten.com/g/64/64\">", "src=\"http://placekitten.com/g/64/64\" />")
                    .Replace("a = b < c;", "a = b &lt; c;")
                    .Replace("s = \"&lt;html&gt;\";", "s = &quot;&lt;html&gt;&quot;;")
                    .Replace("français", "fran&ccedil;ais")
                    ;
                Assert.Equal(tmp, serializer.Serialize(doc));
            }
        }

        #endregion

    }
}
