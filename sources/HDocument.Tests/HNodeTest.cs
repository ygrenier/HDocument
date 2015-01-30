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
            HNode n = new HText("test");
            Assert.Null(n.NextNode);
        }

        [Fact]
        public void TestPreviousNode()
        {
            HNode n = new HText("test");
            Assert.Null(n.PreviousNode);
        }

    }
}
