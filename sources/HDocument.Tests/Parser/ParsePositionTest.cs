using HDoc.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests.Parser
{
    
    public class ParsePositionTest
    {
        [Fact]
        public void TestCreate()
        {
            var pos = new ParsePosition();
            Assert.Equal(0, pos.Position);
            Assert.Equal(0, pos.Line);
            Assert.Equal(0, pos.Column);

            pos = new ParsePosition(1,2,3);
            Assert.Equal(1, pos.Position);
            Assert.Equal(2, pos.Line);
            Assert.Equal(3, pos.Column);

        }

        [Fact]
        public void TestIncrement()
        {
            var pos = new ParsePosition(17, 2, 7);
            pos++;

            Assert.Equal(18, pos.Position);
            Assert.Equal(2, pos.Line);
            Assert.Equal(8, pos.Column);

        }

        [Fact]
        public void TestAddPosition()
        {
            var pos = new ParsePosition();

            var pos2 = pos.AddPosition(2);

            Assert.Equal(0, pos.Position);
            Assert.Equal(0, pos.Line);
            Assert.Equal(0, pos.Column);

            Assert.Equal(2, pos2.Position);
            Assert.Equal(0, pos2.Line);
            Assert.Equal(0, pos2.Column);

        }

        [Fact]
        public void TestNexLine()
        {
            var pos = new ParsePosition(17, 2, 7);
            var pos2 = pos.NextLine();
            var pos3 = pos2.NextLine(false);

            Assert.Equal(17, pos.Position);
            Assert.Equal(2, pos.Line);
            Assert.Equal(7, pos.Column);

            Assert.Equal(18, pos2.Position);
            Assert.Equal(3, pos2.Line);
            Assert.Equal(0, pos2.Column);

            Assert.Equal(18, pos3.Position);
            Assert.Equal(4, pos3.Line);
            Assert.Equal(0, pos3.Column);

        }

        [Fact]
        public void TestEquality()
        {
            var pos1 = new ParsePosition(17, 2, 7);
            var pos2 = new ParsePosition(17, 3, 8);
            var pos3 = new ParsePosition(17, 2, 7);
            var pos4 = new ParsePosition(18, 3, 8);

            Assert.False(pos1.Equals(pos2));
            Assert.True(pos1.Equals(pos3));
            Assert.False(pos1.Equals(pos4));

            Assert.False(pos1.Equals((Object)pos2));
            Assert.True(pos1.Equals((Object)pos3));
            Assert.False(pos1.Equals((Object)pos4));
            Assert.False(pos1.Equals(1));
            Assert.False(pos1.Equals("test"));

            Assert.False(pos1 == pos2);
            Assert.True(pos1 == pos3);
            Assert.False(pos1 == pos4);

            Assert.True(pos1 != pos2);
            Assert.False(pos1 != pos3);
            Assert.True(pos1 != pos4);

        }

        [Fact]
        public void TestGetHasCode()
        {
            Assert.Equal(0, new ParsePosition().GetHashCode());
            Assert.Equal(-1, ParsePosition.None.GetHashCode());
            Assert.Equal(20, new ParsePosition(17, 2, 7).GetHashCode());
        }

    }

}
