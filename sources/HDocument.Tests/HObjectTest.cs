using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests
{

    public class HObjectTest
    {

        [Fact]
        public void TestParent()
        {
            HObject obj = new HText("test");

            Assert.Null(obj.Parent);

            HElement p = new HElement("parent", obj);
            Assert.Same(p, obj.Parent);

        }

        [Fact]
        public void TestDocument()
        {
            HObject n = new HText("test");
            Assert.Null(n.Document);

            HObject p = new HElement("parent", n);
            Assert.Null(n.Document);
            Assert.Null(p.Document);

            HDocument doc = new HDocument(p);
            Assert.Same(doc, n.Document);
            Assert.Same(doc, p.Document);
            Assert.Same(doc, doc.Document);
        }


    }

}
