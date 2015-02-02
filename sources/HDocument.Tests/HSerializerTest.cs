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
                new HXmlDeclaration(),
                new HDocumentType(StandardDoctype.XHtml10Transitional),
                new HComment("This is the root node"),
                new HElement(
                    "html",
                    "\r\n",
                    new HComment("Start the header"),
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
            expected.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            expected.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\" >");
            expected.Append("<!--This is the root node-->");
            expected.Append("<html>\r\n");
            expected.Append("<!--Start the header-->");
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
    }
}
