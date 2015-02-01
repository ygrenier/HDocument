using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests
{
    public class HDocumentTypeTest
    {

        [Fact]
        public void TestCreate()
        {
            // Create default => HTML5
            var dt = new HDocumentType();
            Assert.Equal("html", dt.RootElement);
            Assert.Equal(null, dt.KindDoctype);
            Assert.Equal(null, dt.FPI);
            Assert.Equal(null, dt.Uri);
            Assert.Equal(StandardDoctype.Html5, dt.StandardType);

            // Create an HTML5 by elements
            dt = new HDocumentType("HTML", null, null, null);
            Assert.Equal("HTML", dt.RootElement);
            Assert.Equal(null, dt.KindDoctype);
            Assert.Equal(null, dt.FPI);
            Assert.Equal(null, dt.Uri);
            Assert.Equal(StandardDoctype.Html5, dt.StandardType);

            // Create a standard by elements
            dt = new HDocumentType("html", "-//W3C//DTD HTML 4.01//EN", "http://www.w3.org/TR/html4/strict.dtd", "public");
            Assert.Equal("html", dt.RootElement);
            Assert.Equal("public", dt.KindDoctype);
            Assert.Equal("-//W3C//DTD HTML 4.01//EN", dt.FPI);
            Assert.Equal("http://www.w3.org/TR/html4/strict.dtd", dt.Uri);
            Assert.Equal(StandardDoctype.Html401Strict, dt.StandardType);

            // Create a non standard by elements
            dt = new HDocumentType("html", "-//W3C//DTD HTML 4.01//EN", "Unknown",null);
            Assert.Equal("html", dt.RootElement);
            Assert.Equal(null, dt.KindDoctype);
            Assert.Equal("-//W3C//DTD HTML 4.01//EN", dt.FPI);
            Assert.Equal("Unknown", dt.Uri);
            Assert.Equal(null, dt.StandardType);

            // Create from standard
            dt = new HDocumentType(StandardDoctype.Html401Transitional);
            Assert.Equal("HTML", dt.RootElement);
            Assert.Equal("PUBLIC", dt.KindDoctype);
            Assert.Equal("-//W3C//DTD HTML 4.01 Transitional//EN", dt.FPI);
            Assert.Equal("http://www.w3.org/TR/html4/loose.dtd", dt.Uri);
            Assert.Equal(StandardDoctype.Html401Transitional, dt.StandardType);

            // Root element is required
            Assert.Throws<ArgumentNullException>(() => new HDocumentType(null, null, null, null));

        }

        [Fact]
        public void TestCreateFromOther()
        {
            var dt = new HDocumentType(StandardDoctype.Html401Transitional);

            var anotherDt = new HDocumentType(dt);
            Assert.Equal("HTML", anotherDt.RootElement);
            Assert.Equal("PUBLIC", anotherDt.KindDoctype);
            Assert.Equal("-//W3C//DTD HTML 4.01 Transitional//EN", anotherDt.FPI);
            Assert.Equal("http://www.w3.org/TR/html4/loose.dtd", anotherDt.Uri);
            Assert.Equal(StandardDoctype.Html401Transitional, anotherDt.StandardType);

            Assert.Throws<ArgumentNullException>(() => new HDocumentType(null));

        }

        [Fact]
        public void TestClone()
        {
            var doc = new HDocument(
                    new HDocumentType(StandardDoctype.Html5),
                    new HElement("root")
                );

            var newDoc = new HDocument(doc);
            var nodes = newDoc.Nodes().ToArray();
            Assert.IsType<HDocumentType>(nodes[0]);
            Assert.Equal(StandardDoctype.Html5, ((HDocumentType)nodes[0]).StandardType);
        }

    }
}
