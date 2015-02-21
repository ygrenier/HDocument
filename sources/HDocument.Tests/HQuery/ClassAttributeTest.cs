using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests.HQuery
{
    public class ClassAttributeTest
    {
        [Fact]
        public void TestGetClasses()
        {
            Assert.Equal(new String[] { }, ClassAttributeExtensions.GetClasses(null));

            var element = new HElement("div");
            Assert.Equal(new String[] { }, element.GetClasses());

            var clsAttr = new HAttribute("class");
            element.Add(clsAttr);

            Assert.Equal(new String[] { }, element.GetClasses());

            clsAttr.Value = "    ";
            Assert.Equal(new String[] { }, element.GetClasses());

            clsAttr.Value = "  class1  ";
            Assert.Equal(new String[] { "class1" }, element.GetClasses());

            clsAttr.Value = "  class1 class-2 ";
            Assert.Equal(new String[] { "class1", "class-2" }, element.GetClasses());

        }

        [Fact]
        public void TestHasClass()
        {
            var elements = new HElement[]{
                new HElement("p", "First paragraph."),
                null,
                new HElement("p", new HAttribute("class", "selected"), "Second paragraph is selected.")
            };
            Assert.False(elements[0].HasClass(null));
            Assert.False(elements[0].HasClass("selected"));
            Assert.False(elements[1].HasClass("selected"));
            Assert.True(elements[2].HasClass("selected"));
            Assert.False(elements.HasClass(null));
            Assert.False(elements.HasClass("not-selected"));
            Assert.True(elements.HasClass("selected"));
        }

        [Fact]
        public void TestAddClass()
        {
            var elements = new HElement[]{
                new HElement("p", "First paragraph."),
                null,
                new HElement("p", new HAttribute("class", "selected class3"), "Second paragraph is selected.")
            };

            elements
                .AddClass("class1")
                .AddClass("Selected")
                ;
            elements[0].AddClass("more");

            Assert.Equal("class1 Selected more", elements[0].Attribute("class").Value);
            Assert.Equal("selected class3 class1", elements[2].Attribute("class").Value);

        }

    }
}
