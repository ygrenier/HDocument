﻿using System;
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

        [Fact]
        public void TestAddClassByCallback()
        {
            var elements = new HElement[]{
                new HElement("p", "First paragraph."),
                null,
                new HElement("p", new HAttribute("class", "selected class3"), "Second paragraph is selected.")
            };

            elements
                .AddClass((elm, idx) => String.Format("index-{0}", idx))
                ;

            Assert.Equal("index-0", elements[0].Attribute("class").Value);
            Assert.Equal("selected class3 index-2", elements[2].Attribute("class").Value);

        }

        [Fact]
        public void TestRemoveClassAll()
        {
            var elements = new HElement[]{
                new HElement("p", "First paragraph."),
                null,
                new HElement("p", new HAttribute("class", "selected class3"), "Second paragraph is selected.")
            };

            elements.RemoveClass();

            Assert.Null(elements[0].Attribute("class"));
            Assert.Null(elements[2].Attribute("class"));

        }

        [Fact]
        public void TestRemoveClass()
        {
            var elements = new HElement[]{
                new HElement("p","First paragraph."),
                null,
                new HElement("p", new HAttribute("class", "selected class3"), "Second paragraph is selected.")
            };

            elements
                .AddClass("class2")
                .AddClass("Selected")
                .RemoveClass("class2 selected")
                ;


            Assert.Null(elements[0].Attribute("class"));
            Assert.Equal("class3", elements[2].Attribute("class").Value);

        }

        [Fact]
        public void TestRemoveClassByCallback()
        {
            var elements = new HElement[]{
                new HElement("p",new HAttribute("class", "class0"),  "First paragraph."),
                null,
                new HElement("p", new HAttribute("class", "selected class3"), "Second paragraph is selected.")
            };

            elements
                .AddClass("class2")
                .AddClass("Selected")
                .RemoveClass((elm, idx) => String.Format("class{0} selected", idx))
                ;


            Assert.Equal("class2", elements[0].Attribute("class").Value);
            Assert.Equal("class3", elements[2].Attribute("class").Value);

        }

        [Fact]
        public void TestToggleClass()
        {
            var elements = new HElement[]{
                new HElement("p","First paragraph."),
                null,
                new HElement("p", new HAttribute("class", "selected class3"), "Second paragraph is selected.")
            };

            elements
                .ToggleClass("class1 selected")
                ;

            Assert.Equal("class1 selected", elements[0].Attribute("class").Value);
            Assert.Equal("class3 class1", elements[2].Attribute("class").Value);

            elements
                .ToggleClass("class1 selected", true)
                ;

            Assert.Equal("class1 selected", elements[0].Attribute("class").Value);
            Assert.Equal("class3 class1 selected", elements[2].Attribute("class").Value);

            elements
                .ToggleClass("class1 selected", false)
                ;

            Assert.Null(elements[0].Attribute("class"));
            Assert.Equal("class3", elements[2].Attribute("class").Value);

            elements[0].ToggleClass("class1");
            Assert.Equal("class1", elements[0].Attribute("class").Value);

            elements[0].ToggleClass("class1", true);
            Assert.Equal("class1", elements[0].Attribute("class").Value);

            elements[0].ToggleClass("class1", false);
            Assert.Null(elements[0].Attribute("class"));

        }

        [Fact]
        public void TestToggleClassByCallback()
        {
            var elements = new HElement[]{
                new HElement("p",new HAttribute("class", "class0"),  "First paragraph."),
                null,
                new HElement("p", new HAttribute("class", "selected class3"), "Second paragraph is selected.")
            };

            elements
                .AddClass("class2")
                .AddClass("Selected")
                .ToggleClass((elm, idx) => String.Format("class{0} selected", idx))
                ;

            Assert.Equal("class2", elements[0].Attribute("class").Value);
            Assert.Equal("class3", elements[2].Attribute("class").Value);

            elements
                .ToggleClass((elm, idx) => String.Format("class{0} selected", idx))
                ;

            Assert.Equal("class2 class0 selected", elements[0].Attribute("class").Value);
            Assert.Equal("class3 class2 selected", elements[2].Attribute("class").Value);

            elements
                .ToggleClass((elm, idx) => String.Format("class{0} selected", idx), true)
                ;

            Assert.Equal("class2 class0 selected", elements[0].Attribute("class").Value);
            Assert.Equal("class3 class2 selected", elements[2].Attribute("class").Value);

            elements
                .ToggleClass((elm, idx) => String.Format("class{0} selected", idx), false)
                ;

            Assert.Equal("class2", elements[0].Attribute("class").Value);
            Assert.Equal("class3", elements[2].Attribute("class").Value);
        }

    }
}
