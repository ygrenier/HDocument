using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests
{
    public class HTextTest
    {
        [Fact]
        public void TestCreate()
        {
            var hTxt = new HText("Content");
            Assert.Equal("Content", hTxt.Value);

            hTxt = new HText("");
            Assert.Equal("", hTxt.Value);

            Assert.Throws<ArgumentNullException>(() => new HText((String)null));
        }

        [Fact]
        public void TestCreateFromOther()
        {
            var hOtherTxt = new HText("Content");

            var hTxt = new HText(hOtherTxt);
            Assert.Equal("Content", hTxt.Value);

            Assert.Throws<ArgumentNullException>(() => new HText((HText)null));
        }

        [Fact]
        public void TestValue()
        {
            var hTxt = new HText("Content");
            Assert.Equal("Content", hTxt.Value);
            hTxt.Value = "Other content";
            Assert.Equal("Other content", hTxt.Value);

            Assert.Throws<ArgumentNullException>(() => hTxt.Value = null);
        }

        [Fact]
        public void TestClone()
        {
            HText txt = new HText("Test");

            HContainer parent = new HElement("test");

            parent.Add(txt);
            Assert.Same(txt, parent.FirstNode);
            Assert.Same(txt, parent.LastNode);

            HContainer otherParent = new HElement("test");
            // Do clone
            otherParent.Add(txt);

            Assert.IsType<HText>(otherParent.FirstNode);
            Assert.NotSame(otherParent.FirstNode, parent.FirstNode);
        }

    }
}
