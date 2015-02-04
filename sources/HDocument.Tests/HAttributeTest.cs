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
            var attr1 = new HAttribute("attr1", "value1");

            // Create parent
            var elm = new HElement("test", attr1);
            Assert.Same(attr1, elm.FirstAttribute);
            Assert.Same(attr1, elm.LastAttribute);
            Assert.Same(elm, attr1.Parent);
            Assert.Equal(1, elm.Attributes().Count());

            // Remove attribute
            attr1.Remove();
            Assert.Null(elm.FirstAttribute);
            Assert.Null(elm.LastAttribute);
            Assert.Null(attr1.Parent);
            Assert.Equal(0, elm.Attributes().Count());

            // Fail to remove a detached attribute
            var ioe = Assert.Throws<InvalidOperationException>(() => attr1.Remove());
            Assert.Equal("No parent found.", ioe.Message);
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

            var elm = new HElement("parent");
            elm.Add(attr);
            Assert.Null(attr.NextAttribute);

            var attr2 = new HAttribute("attr1", "val1");
            elm.Add(attr2);
            Assert.Same(attr2, attr.NextAttribute);
            Assert.Null(attr2.NextAttribute);

            var attr3 = new HAttribute("attr2", "val2");
            elm.Add(attr3);
            Assert.Same(attr2, attr.NextAttribute);
            Assert.Same(attr3, attr2.NextAttribute);
            Assert.Null(attr3.NextAttribute);

        }

        [Fact]
        public void TestPreviousAttribute()
        {
            var attr = new HAttribute("name", "value");

            Assert.Null(attr.PreviousAttribute);

            var elm = new HElement("parent");
            elm.Add(attr);
            Assert.Null(attr.PreviousAttribute);

            var attr2 = new HAttribute("attr1", "val1");
            elm.Add(attr2);
            Assert.Null(attr.PreviousAttribute);
            Assert.Same(attr, attr2.PreviousAttribute);

            var attr3 = new HAttribute("attr2", "val2");
            elm.Add(attr3);
            Assert.Null(attr.PreviousAttribute);
            Assert.Same(attr, attr2.PreviousAttribute);
            Assert.Same(attr2, attr3.PreviousAttribute);

        }

    }
}
