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
        #region Attribute

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
        
        #endregion

        #region Css

        [Fact]
        public void TestGetCssElement()
        {
            HElement element = null;
            var style = element.Css();
            Assert.Equal(0, style.Count);

            element = new HElement("div");

            style = element.Css();
            Assert.Equal(0, style.Count);

            element.Attr("id", "test").Attr("style", "");
            style = element.Css();
            Assert.Equal(0, style.Count);

            element.Attr("style", "font-size=12px;color=blue;");
            style = element.Css();
            Assert.Equal(2, style.Count);
            Assert.Equal("12px", style["font-size"]);
            Assert.Equal("blue", style["color"]);

            element.Attr("style", "font-size=12px;other=;empty;color=blue");
            style = element.Css();
            Assert.Equal(4, style.Count);
            Assert.Equal("12px", style["font-size"]);
            Assert.Equal("blue", style["color"]);
            Assert.Equal("", style["other"]);
            Assert.Equal("", style["empty"]);

        }

        [Fact]
        public void TestGetCssElements()
        {
            var element1 = new HElement("div");
            var element2 = new HElement("div").Attr("style", "font-size=12px;color=blue;");
            var element3 = new HElement("div").Attr("style", "font-size=12px;other=;empty;color=blue");

            HElement[] elements = null;

            var style = elements.Css();
            Assert.Equal(0, style.Count);

            elements = new HElement[] { element1, element2, element3 };
            style = elements.Css();
            Assert.Equal(0, style.Count);

            elements = new HElement[] { element2, element1, element3 };
            style = elements.Css();
            Assert.Equal(2, style.Count);
            Assert.Equal("12px", style["font-size"]);
            Assert.Equal("blue", style["color"]);

            elements = new HElement[] { element3, element2, element1 };
            style = elements.Css();
            Assert.Equal(4, style.Count);
            Assert.Equal("12px", style["font-size"]);
            Assert.Equal("blue", style["color"]);
            Assert.Equal("", style["other"]);
            Assert.Equal("", style["empty"]);
        }

        [Fact]
        public void TestGetCssPropertyElement()
        {
            HElement element = null;
            Assert.Equal(String.Empty, element.Css("color"));

            element = new HElement("div");

            Assert.Equal(String.Empty, element.Css("color"));

            element.Attr("id", "test").Attr("style", "");
            Assert.Equal(String.Empty, element.Css("color"));

            element.Attr("style", "font-size=12px;color=blue;");
            Assert.Equal("blue", element.Css("color"));

            element.Attr("style", "font-size=12px;other=;empty;color=blue");
            Assert.Equal("blue", element.Css("color"));
        }

        [Fact]
        public void TestGetCssPropertyElements()
        {
            var element1 = new HElement("div");
            var element2 = new HElement("div").Attr("style", "font-size=12px;color=blue;");
            var element3 = new HElement("div").Attr("style", "font-size=12px;other=;empty;color=blue");

            HElement[] elements = null;

            Assert.Equal("", elements.Css("color"));

            elements = new HElement[] { element1, element2, element3 };
            Assert.Equal("", elements.Css("color"));

            elements = new HElement[] { element2, element1, element3 };
            Assert.Equal("blue", elements.Css("color"));

            elements = new HElement[] { element3, element2, element1 };
            Assert.Equal("blue", elements.Css("color"));

        }

        [Fact]
        public void TestGetCssPropertiesElement()
        {
            HElement element = null;
            Assert.Equal(new String[] { "", "" }, element.Css(new String[] { "color", "font-size" }).ToArray());

            element = new HElement("div");

            Assert.Equal(new String[] { "", "" }, element.Css(new String[] { "color", "font-size" }).ToArray());

            element.Attr("id", "test").Attr("style", "");
            Assert.Equal(new String[] { "", "" }, element.Css(new String[] { "color", "font-size" }).ToArray());

            element.Attr("style", "color=blue;");
            Assert.Equal(new String[] { "blue", "" }, element.Css(new String[] { "color", "font-size" }).ToArray());

            element.Attr("style", "font-size=12px;other=;empty;color=blue");
            Assert.Equal(new String[] { "blue", "12px" }, element.Css(new String[] { "color", "font-size" }).ToArray());

        }

        [Fact]
        public void TestGetCssPropertiesElements()
        {
            var element1 = new HElement("div");
            var element2 = new HElement("div").Attr("style", "color=blue;");
            var element3 = new HElement("div").Attr("style", "font-size=12px;other=;empty;color=blue");

            HElement[] elements = null;

            Assert.Equal(new String[] { "", "" }, elements.Css(new String[] { "color", "font-size" }).ToArray());

            elements = new HElement[] { element1, element2, element3 };
            Assert.Equal(new String[] { "", "" }, elements.Css(new String[] { "color", "font-size" }).ToArray());

            elements = new HElement[] { element2, element1, element3 };
            Assert.Equal(new String[] { "blue", "" }, elements.Css(new String[] { "color", "font-size" }).ToArray());

            elements = new HElement[] { element3, element2, element1 };
            Assert.Equal(new String[] { "blue", "12px" }, elements.Css(new String[] { "color", "font-size" }).ToArray());

        }

        [Fact]
        public void TestSetCssPropertyElement()
        {
            HElement element = null;
            Assert.Equal(null, element.Css("color", "blue").Attr("style"));

            Assert.Equal(
                "color=blue;font-size=12px;font-name=Arial", 
                new HElement("div")
                    .Css("color", "blue")
                    .Css("fontSize", "12px")
                    .Css("font-name", "Arial")
                    .Attr("style")
            );

            Assert.Equal(
                "color=blue;font-size=24px",
                new HElement("div")
                    .Css("color", "blue")
                    .Css("fontSize", "12px")
                    .Css("font-name", "Arial")
                    .Css("font-Size", "24px")
                    .Css("font-name", "")
                    .Attr("style")
            );

        }

        [Fact]
        public void TestSetCssPropertyElements()
        {
            var element1 = new HElement("div");
            var element2 = new HElement("div").Attr("style", "color=blue;");
            var element3 = new HElement("div").Attr("style", "font-size=12px;other=;empty;color=blue");

            HElement[] elements = null;

            Assert.Equal(
                null, 
                    elements
                        .Css("font-name", "Arial")
                        .Css("font-Size", "24px")
                        .Css("font-name", "")
                        );

            elements = new HElement[] { element1, element2, element3 };
            Assert.Equal(
                new String[] { "font-size=24px", "color=blue;font-size=24px", "font-size=24px;color=blue" },
                    elements
                        .Css("font-name", "Arial")
                        .Css("font-Size", "24px")
                        .Css("font-name", "")
                        .Select(e => e.Attr("style"))
                        .ToArray());

        }

        #endregion

    }
}
