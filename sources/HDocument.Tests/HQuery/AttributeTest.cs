using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests.HQuery
{
    public class AttributeTest
    {
        [Fact]
        public void TestAttribute()
        {
            var element = new HElement("div");

            // The value is null so the attribute is not created
            element.Attribute("style", null);
            Assert.Null(element.Attribute("Style"));

            // Create the attribute
            element.Attribute("style", "font-size:red");
            Assert.Equal("font-size:red", element.Attribute("Style").Value);

            // Update the attribute
            element.Attribute("style", "font-size:blue");
            Assert.Equal("font-size:blue", element.Attribute("Style").Value);

            // Delete the attribute
            element.Attribute("style", "");
            Assert.Null(element.Attribute("Style"));

        }

        [Fact]
        public void TestAttrElement()
        {
            var element = new HElement("div");

            // The value is null so the attribute is not created
            element.Attr("style", (String)null);
            Assert.Null(element.Attr("Style"));

            // Create the attribute
            element.Attr("style", "font-size:red");
            Assert.Equal("font-size:red", element.Attr("Style"));

            // Update the attribute
            element.Attr("style", "font-size:blue");
            Assert.Equal("font-size:blue", element.Attr("Style"));

            // Delete the attribute
            element.Attr("style", "");
            Assert.Null(element.Attr("Style"));
        }

        [Fact]
        public void TestAttrElements()
        {
            var elements = new HElement[]{
                new HElement("div", new HAttribute("class", "one")),
                new HElement("div"),
                new HElement("div", new HAttribute("style", "font-size:red"))
            };

            Assert.Equal("one", elements.Attr("class"));
            Assert.Equal(null, elements.Attr("style"));
            Assert.Equal(null, elements.Attr("id"));

            // Remove all style attributes
            elements.Attr("style", ""); 
            Assert.Null(elements[0].Attribute("style"));
            Assert.Null(elements[1].Attribute("style"));
            Assert.Null(elements[2].Attribute("style"));

            // Create the attribute
            elements.Attr("style", "font-size:red");
            Assert.Equal("font-size:red", elements[0].Attr("style"));
            Assert.Equal("font-size:red", elements[1].Attr("style"));
            Assert.Equal("font-size:red", elements[2].Attr("style"));

            elements = null;
            Assert.Equal(null, elements.Attr("id"));

        }

        class TestPoco
        {
            public int data_int;
            public string data_str { get; set; }
        }

        [Fact]
        public void TestAttrPoco()
        {
            var elements = new HElement[]{
                new HElement("div", new HAttribute("class", "one")),
                new HElement("div"),
                new HElement("div", new HAttribute("style", "font-size:red"))
            };

            elements.Attr(new { style = "", @class="two" });
            Assert.Null(elements[0].Attribute("style"));
            Assert.Null(elements[1].Attribute("style"));
            Assert.Null(elements[2].Attribute("style"));
            Assert.Equal("two", elements.Attr("class"));

            // Create the attribute
            elements.Attr(new { style = "font-size:red", data_value = "data" });
            Assert.Equal("font-size:red", elements[0].Attr("style"));
            Assert.Equal("font-size:red", elements[1].Attr("style"));
            Assert.Equal("font-size:red", elements[2].Attr("style"));
            Assert.Equal("data", elements.Attr("data-value"));

            elements.Attr(new TestPoco() { data_int = 123, data_str = "str" });
            Assert.Equal("123", elements.Attr("data-int"));
            Assert.Equal("str", elements.Attr("data-str"));

        }

        [Fact]
        public void TestAttrByCallback()
        {
            var elements = new HElement[]{
                new HElement("div", new HAttribute("class", "one")),
                new HElement("div"),
                new HElement("div", new HAttribute("style", "font-size:red"))
            };

            elements.Attr("style", (elm, idx) => String.Format("color-{0}", idx));
            Assert.Equal("color-0", elements[0].Attr("style"));
            Assert.Equal("color-1", elements[1].Attr("style"));
            Assert.Equal("color-2", elements[2].Attr("style"));

        }

    }
}
