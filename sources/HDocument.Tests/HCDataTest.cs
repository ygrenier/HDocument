using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests
{
    public class HCDataTest
    {

        [Fact]
        public void TestCreate()
        {
            HCData cdata = new HCData("Test");
            Assert.Equal("Test", cdata.Value);
        }

        [Fact]
        public void TestCreateFromOther()
        {
            HCData cdata = new HCData("Test");
            Assert.Equal("Test", cdata.Value);

            HCData ncdata = new HCData(cdata);
            Assert.Equal("Test", ncdata.Value);

        }

        [Fact]
        public void TestClone()
        {
            HCData cdata = new HCData("Test");

            HContainer parent = new HElement("test");

            parent.Add(cdata);
            Assert.Same(cdata, parent.FirstNode);
            Assert.Same(cdata, parent.LastNode);

            HContainer otherParent = new HElement("test");
            // Do clone
            otherParent.Add(cdata);

            Assert.IsType<HCData>(otherParent.FirstNode);
            Assert.NotSame(otherParent.FirstNode, parent.FirstNode);



        }

    }
}
