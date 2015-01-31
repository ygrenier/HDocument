using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests
{
    public class HCommentTest
    {
        [Fact]
        public void TestCreate()
        {
            var hComment = new HComment("Content");
            Assert.Equal("Content", hComment.Value);

            hComment = new HComment("");
            Assert.Equal("", hComment.Value);

            Assert.Throws<ArgumentNullException>(() => new HComment((String)null));
        }

        [Fact]
        public void TestCreateFromOther()
        {
            var hOtherComment = new HComment("Content");

            var hComment = new HComment(hOtherComment);
            Assert.Equal("Content", hComment.Value);

            Assert.Throws<ArgumentNullException>(() => new HComment((HComment)null));
        }

        [Fact]
        public void TestValue()
        {
            var hComment = new HComment("Content");
            Assert.Equal("Content", hComment.Value);
            hComment.Value = "Other content";
            Assert.Equal("Other content", hComment.Value);

            Assert.Throws<ArgumentNullException>(() => hComment.Value = null);
        }

        [Fact]
        public void TestClone()
        {
            HComment hComment = new HComment("Test");

            HContainer parent = new HElement("test");

            parent.Add(hComment);
            Assert.Same(hComment, parent.FirstNode);
            Assert.Same(hComment, parent.LastNode);

            HContainer otherParent = new HElement("test");
            // Do clone
            otherParent.Add(hComment);

            Assert.IsType<HComment>(otherParent.FirstNode);
            Assert.NotSame(otherParent.FirstNode, parent.FirstNode);
        }

    }
}
