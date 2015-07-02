using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests
{
    public class ParseErrorTest
    {
        [Fact]
        public void TestCreate()
        {
            var error = new ParseError("Message");
            Assert.Equal("Message", error.Message);
            Assert.Equal("-1 (L:-1 / C:-1)", error.Position.ToString());

            error = new ParseError("Message", new HDoc.Parser.ParsePosition(10, 2, 4));
            Assert.Equal("Message", error.Message);
            Assert.Equal("10 (L:2 / C:4)", error.Position.ToString());
        }
    }
}
