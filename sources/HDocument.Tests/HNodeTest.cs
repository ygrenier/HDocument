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

    }
}
