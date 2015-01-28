using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests
{
    public class HAttributeTest
    {

        [Fact]
        public void TestCreate()
        {
            var attr = new HAttribute("name", "value");
            Assert.Equal("name", attr.Name);
            Assert.Equal("value", attr.Value);

            attr = new HAttribute("name");
            Assert.Equal("name", attr.Name);
            Assert.Equal("", attr.Value);

            Assert.Throws<ArgumentNullException>(() => new HAttribute("name", null));
            Assert.Throws<ArgumentNullException>(() => new HAttribute(" ", null));
            Assert.Throws<ArgumentNullException>(() => new HAttribute(null, null));
        }

        [Fact]
        public void TestCreateFromOther()
        {
            var source = new HAttribute("name", "value");

            var attr = new HAttribute(source);
            Assert.Equal("name", attr.Name);
            Assert.Equal("value", attr.Value);

            Assert.Throws<ArgumentNullException>(() => new HAttribute((HAttribute)null));
        }

        [Fact]
        public void TestValue()
        {
            var attr = new HAttribute("name");
            
            Assert.Equal("", attr.Value);

            String value = "test";
            attr.Value = value;
            Assert.Equal(value, attr.Value);

            value = "";
            attr.Value = value;
            Assert.Equal(value, attr.Value);

            Assert.Throws<ArgumentNullException>(() => attr.Value = null);

        }

        [Fact]
        public void TestRemove()
        {
            var attr = new HAttribute("name");

            Assert.Throws<NotImplementedException>(() => attr.Remove());
        }

        [Fact]
        public void TestOperatorString()
        {
            var attr = new HAttribute("name", "value");

            String val = (string)attr;

            Assert.Equal("value", attr.Value);

        }

        [Fact]
        public void TestNextAttribute()
        {
            var attr = new HAttribute("name", "value");

            Assert.Null(attr.NextAttribute);

        }

        [Fact]
        public void TestPreviousAttribute()
        {
            var attr = new HAttribute("name", "value");

            Assert.Null(attr.PreviousAttribute);

        }

    }
}
