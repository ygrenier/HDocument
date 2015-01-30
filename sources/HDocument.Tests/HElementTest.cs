using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests
{

    public class HElementTest
    {

        [Fact]
        public void TestCreate()
        {
            String name = "test";

            var elm = new HElement(name);
            Assert.Equal(name, elm.Name);

            Assert.Throws<ArgumentNullException>(() => new HElement(" "));
            Assert.Throws<ArgumentNullException>(() => new HElement((String)null));

        }

        [Fact]
        public void TestCreateFromOther()
        {
            var elm = new HElement("test");
            elm.Add(
                new HAttribute("attr1", "val1"),
                new HAttribute("attr3", "val3")
                );

            var newElm = new HElement(elm);
            Assert.Equal(2, elm.Attributes().Count());
            Assert.Equal(0, elm.Attributes("Attr2").Count());

        }

        [Fact]
        public void TestAttributes()
        {
            // Attributes are empty
            var elm = new HElement("test");

            Assert.Equal(0, elm.Attributes().Count());
            Assert.Null(elm.Attribute(null));
            Assert.Null(elm.Attribute(""));
            Assert.Null(elm.Attribute(" "));
            Assert.Null(elm.Attribute("attr2"));

            // Adding two attributes
            elm.Add(
                new HAttribute("attr1", "val1"),
                new HAttribute("attr3", "val3")
                );
            Assert.Equal(2, elm.Attributes().Count());
            Assert.Equal(0, elm.Attributes("Attr2").Count());
            Assert.Null(elm.Attribute(null));
            Assert.Null(elm.Attribute(""));
            Assert.Null(elm.Attribute(" "));
            Assert.Null(elm.Attribute("attr2"));

            // Adding a third attribute
            elm.Add(new HAttribute("attr2", "val2"));

            Assert.Equal(3, elm.Attributes().Count());
            Assert.Equal(1, elm.Attributes("Attr2").Count());
            Assert.Null(elm.Attribute(null));
            Assert.Null(elm.Attribute(""));
            Assert.Null(elm.Attribute(" "));
            Assert.NotNull(elm.Attribute("attr2"));
            Assert.NotNull(elm.Attribute("Attr2"));

            // Can't adding an attribute with a name already defined
            var ioex = Assert.Throws<InvalidOperationException>(() => elm.Add(new HAttribute("attr2", "value")));
            Assert.Equal("The attribute 'attr2' already defined", ioex.Message);

            // Test adding an attribute already defined in another element
            var otherElement = new HElement("OtherTest");
            var a1=elm.Attribute("Attr2");
            otherElement.Add(a1);
            var a2 = otherElement.Attribute("attr2");
            Assert.NotSame(a1, a2);
            Assert.Equal(a1.Name, a2.Name);
            Assert.Equal(a1.Value, a2.Value);

        }

        [Fact]
        public void TestHasAttributes()
        {
            var elm = new HElement("test");
            Assert.False(elm.HasAttributes);

            elm.Add(new HAttribute("attr1", "val1"));
            Assert.True(elm.HasAttributes);
        }

        [Fact]
        public void TestFirstAttribute()
        {
            var elm = new HElement("test");
            Assert.Null(elm.FirstAttribute);

            elm.Add(
                new HAttribute("attr1", "val1"),
                new HAttribute("attr2", "val2"),
                new HAttribute("attr3", "val3")
                );
            var fa = elm.FirstAttribute;
            Assert.NotNull(fa);
            Assert.Equal("attr1", fa.Name);
            Assert.Equal("val1", fa.Value);
        }

        [Fact]
        public void TestLastAttribute()
        {
            var elm = new HElement("test");
            Assert.Null(elm.LastAttribute);

            elm.Add(
                new HAttribute("attr1", "val1"),
                new HAttribute("attr2", "val2"),
                new HAttribute("attr3", "val3")
                );
            var la = elm.LastAttribute;
            Assert.NotNull(la);
            Assert.Equal("attr3", la.Name);
            Assert.Equal("val3", la.Value);
        }

    }

}
