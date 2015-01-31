using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests
{
    public class HDocumentTest
    {
        [Fact]
        public void TestCreateEmpty()
        {
            var hDoc = new HDocument();
            Assert.Same(hDoc, hDoc.Document);
            Assert.Null(hDoc.Parent);
            Assert.Equal(0, hDoc.Nodes().Count());
        }

        [Fact]
        public void TestCreateWithContent()
        {
            var hDoc = new HDocument("test");
            Assert.Same(hDoc, hDoc.Document);
            Assert.Null(hDoc.Parent);
            Assert.Equal(1, hDoc.Nodes().Count());
        }

        [Fact]
        public void TestCreateFromOther()
        {
            var hDoc = new HDocument("test");
            Assert.Same(hDoc, hDoc.Document);
            Assert.Null(hDoc.Parent);
            Assert.Equal(1, hDoc.Nodes().Count());

            var nDoc = new HDocument(hDoc);
            Assert.Same(nDoc, nDoc.Document);
            Assert.Null(nDoc.Parent);
            Assert.Equal(1, nDoc.Nodes().Count());

        }

        [Fact]
        public void TestRoot()
        {
            var hDoc = new HDocument();

            Assert.Null(hDoc.Root);

            hDoc.Add("  ");
            Assert.Null(hDoc.Root);

            hDoc.Add(new HText("  "));
            Assert.Null(hDoc.Root);

            var root = new HElement("root");
            hDoc.Add(root);
            Assert.Same(root, hDoc.Root);

        }

    }
}
