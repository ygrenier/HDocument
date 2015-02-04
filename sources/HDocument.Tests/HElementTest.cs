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
        public void TestCreateWithContent()
        {
            String name = "test";
            var elm = new HElement(name, null);
            Assert.Equal(0, elm.Nodes().Count());

            elm = new HElement(name, "a");
            Assert.Equal(1, elm.Nodes().Count());

        }

        [Fact]
        public void TestCreateWithContents()
        {
            String name = "test";
            var elm = new HElement(name, null, null);
            Assert.Equal(0, elm.Nodes().Count());

            elm = new HElement(name, "a", "b");
            Assert.Equal(1, elm.Nodes().Count());

            elm = new HElement(name, new HText("a"), new HText("b"));
            Assert.Equal(2, elm.Nodes().Count());

        }

        [Fact]
        public void TestCreateFromOther()
        {
            var elm = new HElement("test", "content");
            elm.Add(
                new HAttribute("attr1", "val1"),
                new HAttribute("attr3", "val3")
                );
            Assert.Equal(1, elm.Nodes().Count());

            var newElm = new HElement(elm);
            Assert.Equal(2, elm.Attributes().Count());
            Assert.Equal(0, elm.Attributes("Attr2").Count());
            Assert.Equal(1, elm.Nodes().Count());
        }

        [Fact]
        public void TestClone()
        {
            var parent = new HElement("parent");

            var elm = new HElement("test", "content");
            elm.Add(
                new HAttribute("attr1", "val1"),
                new HAttribute("attr3", "val3")
                );
            Assert.Equal(1, elm.Nodes().Count());
            parent.Add(elm);

            var newElm = new HElement("test", elm);
            var c = newElm.Elements().First();
            Assert.Equal(2, c.Attributes().Count());
            Assert.Equal(0, c.Attributes("Attr2").Count());
            Assert.Equal(1, c.Nodes().Count());
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

        [Fact]
        public void TestCantAddDocumentInElement()
        {
            var elm = new HElement("test");
            var doc = new HDocument();

            var ex = Assert.Throws<ArgumentException>(() => elm.Add(doc));
            Assert.Equal("Can't add a document in a element.", ex.Message);
        }

        [Fact]
        public void TestCantAddDocumentTypeInElement()
        {
            var elm = new HElement("test");
            var dt = new HDocumentType();

            var ex = Assert.Throws<ArgumentException>(() => elm.Add(dt));
            Assert.Equal("Can't add a document type in a element.", ex.Message);
        }

        [Fact]
        public void TestRemoveAttribute()
        {
            var attr1 = new HAttribute("attr1", "value1");
            var attr2 = new HAttribute("attr2", "value2");
            var attr3 = new HAttribute("attr3", "value3");
            var attr4 = new HAttribute("attr4", "value4");

            // Create parent
            var elm = new HElement("test", attr1, attr2, attr3, attr4);
            Assert.Same(attr1, elm.FirstAttribute);
            Assert.Same(attr4, elm.LastAttribute);

            Assert.Same(elm, attr1.Parent);
            Assert.Null(attr1.PreviousAttribute);
            Assert.Same(attr2, attr1.NextAttribute);

            Assert.Same(elm, attr2.Parent);
            Assert.Same(attr1, attr2.PreviousAttribute);
            Assert.Same(attr3, attr2.NextAttribute);

            Assert.Same(elm, attr3.Parent);
            Assert.Same(attr2, attr3.PreviousAttribute);
            Assert.Same(attr4, attr3.NextAttribute);

            Assert.Same(elm, attr4.Parent);
            Assert.Same(attr3, attr4.PreviousAttribute);
            Assert.Null(attr4.NextAttribute);

            // Remove attr2
            attr2.Remove();
            Assert.Same(attr1, elm.FirstAttribute);
            Assert.Same(attr4, elm.LastAttribute);

            Assert.Same(elm, attr1.Parent);
            Assert.Null(attr1.PreviousAttribute);
            Assert.Same(attr3, attr1.NextAttribute);

            Assert.Null(attr2.Parent);
            Assert.Null(attr2.PreviousAttribute);
            Assert.Null(attr2.NextAttribute);

            Assert.Same(elm, attr3.Parent);
            Assert.Same(attr1, attr3.PreviousAttribute);
            Assert.Same(attr4, attr3.NextAttribute);

            Assert.Same(elm, attr4.Parent);
            Assert.Same(attr3, attr4.PreviousAttribute);
            Assert.Null(attr4.NextAttribute);

            var attrs = elm.Attributes().ToArray();
            Assert.Equal(new HAttribute[] { attr1, attr3, attr4 }, attrs);

            // Remove attr4
            attr4.Remove();
            Assert.Same(attr1, elm.FirstAttribute);
            Assert.Same(attr3, elm.LastAttribute);

            Assert.Same(elm, attr1.Parent);
            Assert.Null(attr1.PreviousAttribute);
            Assert.Same(attr3, attr1.NextAttribute);

            Assert.Same(elm, attr3.Parent);
            Assert.Same(attr1, attr3.PreviousAttribute);
            Assert.Null(attr3.NextAttribute);

            Assert.Null(attr4.Parent);
            Assert.Null(attr4.PreviousAttribute);
            Assert.Null(attr4.NextAttribute);

            attrs = elm.Attributes().ToArray();
            Assert.Equal(new HAttribute[] { attr1, attr3}, attrs);

            // Remove attr1
            attr1.Remove();
            Assert.Same(attr3, elm.FirstAttribute);
            Assert.Same(attr3, elm.LastAttribute);

            Assert.Null(attr1.Parent);
            Assert.Null(attr1.PreviousAttribute);
            Assert.Null(attr1.NextAttribute);

            Assert.Same(elm, attr3.Parent);
            Assert.Null(attr3.PreviousAttribute);
            Assert.Null(attr3.NextAttribute);

            attrs = elm.Attributes().ToArray();
            Assert.Equal(new HAttribute[] { attr3 }, attrs);

            // Remove attr3
            attr3.Remove();
            Assert.Null(elm.FirstAttribute);
            Assert.Null(elm.LastAttribute);

            Assert.Null(attr3.Parent);
            Assert.Null(attr3.PreviousAttribute);
            Assert.Null(attr3.NextAttribute);

            attrs = elm.Attributes().ToArray();
            Assert.Equal(new HAttribute[] { }, attrs);

            // Fail to remove a detached attribute
            var ioe = Assert.Throws<InvalidOperationException>(() => attr1.Remove());
            Assert.Equal("No parent found.", ioe.Message);

        }

        [Fact]
        public void TestRemoveAttributes()
        {
            var attr1 = new HAttribute("attr1", "value1");
            var attr2 = new HAttribute("attr2", "value2");
            var attr3 = new HAttribute("attr3", "value3");
            var attr4 = new HAttribute("attr4", "value4");

            // Create parent
            var elm = new HElement("test", attr1, attr2, attr3, attr4);
            Assert.Same(attr1, elm.FirstAttribute);
            Assert.Same(attr4, elm.LastAttribute);

            Assert.Same(elm, attr1.Parent);
            Assert.Null(attr1.PreviousAttribute);
            Assert.Same(attr2, attr1.NextAttribute);

            Assert.Same(elm, attr2.Parent);
            Assert.Same(attr1, attr2.PreviousAttribute);
            Assert.Same(attr3, attr2.NextAttribute);

            Assert.Same(elm, attr3.Parent);
            Assert.Same(attr2, attr3.PreviousAttribute);
            Assert.Same(attr4, attr3.NextAttribute);

            Assert.Same(elm, attr4.Parent);
            Assert.Same(attr3, attr4.PreviousAttribute);
            Assert.Null(attr4.NextAttribute);

            // Remove all attributes
            elm.RemoveAttributes();

            Assert.Null(elm.FirstAttribute);
            Assert.Null(elm.LastAttribute);

            Assert.Null(attr1.Parent);
            Assert.Null(attr1.PreviousAttribute);
            Assert.Null(attr1.NextAttribute);

            Assert.Null(attr2.Parent);
            Assert.Null(attr2.PreviousAttribute);
            Assert.Null(attr2.NextAttribute);

            Assert.Null(attr3.Parent);
            Assert.Null(attr3.PreviousAttribute);
            Assert.Null(attr3.NextAttribute);

            Assert.Null(attr4.Parent);
            Assert.Null(attr4.PreviousAttribute);
            Assert.Null(attr4.NextAttribute);

            Assert.Equal(0, elm.Attributes().Count());

            // Check no exception
            elm.RemoveAttributes();
            Assert.Equal(0, elm.Attributes().Count());

        }

    }

}
