using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests
{
    public class HNodeTest
    {

        [Fact]
        public void TestNextNode()
        {
            HNode n1 = new HText("test 1");
            Assert.Null(n1.NextNode);

            HElement parent = new HElement("parent");
            parent.Add(n1);
            Assert.Null(n1.NextNode);

            HNode n2 = new HText("test 2");
            parent.Add(n2);
            Assert.Same(n2, n1.NextNode);
            Assert.Null(n2.NextNode);

        }

        [Fact]
        public void TestPreviousNode()
        {
            HNode n1 = new HText("test 1");
            Assert.Null(n1.PreviousNode);

            HElement parent = new HElement("parent");
            parent.Add(n1);
            Assert.Null(n1.PreviousNode);

            HNode n2 = new HText("test 2");
            parent.Add(n2);
            Assert.Null(n1.PreviousNode);
            Assert.Same(n1, n2.PreviousNode);

        }

        [Fact]
        public void TestAddBefore()
        {
            HElement parent = new HElement("parent");

            HNode n1 = new HElement("test-1");
            parent.Add(n1);
            Assert.Equal(1, parent.Nodes().Count());

            HNode n2 = new HElement("test-2");
            n1.AddBefore(n2);
            Assert.Equal(2, parent.Nodes().Count());
            n1.AddBefore("1");
            Assert.Equal(3, parent.Nodes().Count());
            n2.AddBefore("2");
            Assert.Equal(4, parent.Nodes().Count());

            var nodes = parent.Nodes().ToArray();
            Assert.Equal(4, nodes.Length);
            Assert.IsType<HText>(nodes[0]);
            Assert.IsType<HElement>(nodes[1]);
            Assert.IsType<HText>(nodes[2]);
            Assert.IsType<HElement>(nodes[3]);

            n1 = new HElement("test-3");
            var ioe = Assert.Throws<InvalidOperationException>(() => n1.AddBefore(null));
            Assert.Equal("No parent found.", ioe.Message);
        }

        [Fact]
        public void TestAddAfter()
        {
            HElement parent = new HElement("parent");

            HNode n1 = new HElement("test-1");
            parent.Add(n1);
            Assert.Equal(1, parent.Nodes().Count());

            HNode n2 = new HElement("test-2");
            n1.AddAfter(n2);
            Assert.Equal(2, parent.Nodes().Count());
            n1.AddAfter("1");
            Assert.Equal(3, parent.Nodes().Count());
            n2.AddAfter("2");
            Assert.Equal(4, parent.Nodes().Count());

            var nodes = parent.Nodes().ToArray();
            Assert.Equal(4, nodes.Length);
            Assert.IsType<HElement>(nodes[0]);
            Assert.IsType<HText>(nodes[1]);
            Assert.IsType<HElement>(nodes[2]);
            Assert.IsType<HText>(nodes[3]);

            n1 = new HElement("test-3");
            var ioe = Assert.Throws<InvalidOperationException>(() => n1.AddAfter(null));
            Assert.Equal("No parent found.", ioe.Message);
        }

    }
}
