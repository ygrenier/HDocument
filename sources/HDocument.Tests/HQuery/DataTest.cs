using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests.HQuery
{
    public class DataTest
    {

        [Fact]
        public void TestGetDataElement()
        {
            HElement element = new HElement("div", new HAttribute("data-value1", "value1"));
            Assert.Equal("value1", element.Data("Value1"));
            Assert.Equal(null, element.Data("Value2"));

            element = null;
            Assert.Equal(null, element.Data("Value1"));
            Assert.Equal(null, element.Data("Value2"));
        }

        [Fact]
        public void TestGetDataElements()
        {
            HElement element1 = new HElement("div", new HAttribute("data-value1", "value1"));
            HElement element2 = new HElement("div", new HAttribute("data-value2", "value2"));

            var elements = new HElement[] { element1, element2 };
            Assert.Equal("value1", elements.Data("Value1"));
            Assert.Equal(null, elements.Data("Value2"));

            elements = new HElement[] { null, element1, element2 };
            Assert.Equal(null, elements.Data("Value1"));
            Assert.Equal(null, elements.Data("Value2"));

            elements = null;
            Assert.Equal(null, elements.Data("Value1"));
            Assert.Equal(null, elements.Data("Value2"));

        }

        [Fact]
        public void TestSetDataElement()
        {
            HElement element = new HElement("div", new HAttribute("data-value1", "value1"));
            element.Data("Value1", "NewValue1")
                .Data("Value2", "NewValue2");

            Assert.Equal("NewValue1", element.Data("Value1"));
            Assert.Equal("NewValue2", element.Data("Value2"));

            element = null;
            element.Data("Value1", "NewValue1")
                .Data("Value2", "NewValue2");
        }

        [Fact]
        public void TestSetDataElements()
        {
            HElement element1 = new HElement("div", new HAttribute("data-value1", "value1"));
            HElement element2 = new HElement("div", new HAttribute("data-value3", "value3"));

            var elements = new HElement[] { element1, null, element2 };
            elements.Data("Value1", "NewValue1")
                .Data("Value2", "NewValue2");

            Assert.Equal("NewValue1", element1.Data("Value1"));
            Assert.Equal("NewValue2", element1.Data("Value2"));
            Assert.Equal(null, element1.Data("Value3"));
            Assert.Equal("NewValue1", element2.Data("Value1"));
            Assert.Equal("NewValue2", element2.Data("Value2"));
            Assert.Equal("value3", element2.Data("Value3"));

            elements = null;
            elements.Data("Value1", "NewValue1")
                .Data("Value2", "NewValue2");

        }

        [Fact]
        public void TestSetDataValuesElement()
        {
            HElement element = new HElement("div", new HAttribute("data-value1", "value1"));
            element.Data(new {
                Value1 = "NewValue1",
                Value2 = "NewValue2"
            });

            Assert.Equal("NewValue1", element.Data("Value1"));
            Assert.Equal("NewValue2", element.Data("Value2"));

            element = null;
            element.Data(new {
                Value1 = "NewValue1",
                Value2 = "NewValue2"
            });
        }

        [Fact]
        public void TestSetDataValuesElements()
        {
            HElement element1 = new HElement("div", new HAttribute("data-value1", "value1"));
            HElement element2 = new HElement("div", new HAttribute("data-value3", "value3"));

            var elements = new HElement[] { element1, null, element2 };
            elements.Data(new {
                Value1 = "NewValue1",
                Value2 = "NewValue2"
            });

            Assert.Equal("NewValue1", element1.Data("Value1"));
            Assert.Equal("NewValue2", element1.Data("Value2"));
            Assert.Equal(null, element1.Data("Value3"));
            Assert.Equal("NewValue1", element2.Data("Value1"));
            Assert.Equal("NewValue2", element2.Data("Value2"));
            Assert.Equal("value3", element2.Data("Value3"));

            elements = null;
            elements.Data(new {
                Value1 = "NewValue1",
                Value2 = "NewValue2"
            });

        }

        [Fact]
        public void TestHasDataElement()
        {
            HElement element = new HElement("div", new HAttribute("data-value1", "value1"));

            Assert.Equal(true, element.HasData("Value1"));
            Assert.Equal(false, element.HasData("Value2"));

            element = null;

            Assert.Equal(false, element.HasData("Value1"));
            Assert.Equal(false, element.HasData("Value2"));
        }

        [Fact]
        public void TestHasDataElements()
        {
            HElement element1 = new HElement("div", new HAttribute("data-value1", "value1"));
            HElement element2 = new HElement("div", new HAttribute("data-value2", "value2"));

            var elements = new HElement[] { element1, null, element2 };

            Assert.Equal(true, elements.HasData("Value1"));
            Assert.Equal(false, elements.HasData("Value2"));

            elements = new HElement[] { null, element1, element2 };
            Assert.Equal(false, elements.HasData("Value1"));
            Assert.Equal(false, elements.HasData("Value2"));

            elements = null;

            Assert.Equal(false, elements.HasData("Value1"));
            Assert.Equal(false, elements.HasData("Value2"));
        }

        [Fact]
        public void TestHasAnyDataElement()
        {
            HElement element = new HElement("div");

            Assert.Equal(false, element.HasData());

            element.Add(new HAttribute("data-value1", "value1"));
            Assert.Equal(true, element.HasData());

            element = null;

            Assert.Equal(false, element.HasData());
        }

        [Fact]
        public void TestHasAnyDataElements()
        {
            HElement element1 = new HElement("div");
            HElement element2 = new HElement("div", new HAttribute("data-value2", "value2"));

            var elements = new HElement[] { element1, null, element2 };
            Assert.Equal(false, elements.HasData());

            elements = new HElement[] { element2, element1, null };
            Assert.Equal(true, elements.HasData());

            elements = new HElement[] { null, element1, element2 };
            Assert.Equal(false, elements.HasData());

            elements = null;
            Assert.Equal(false, elements.HasData());

        }

        [Fact]
        public void TestRemoveDataElement()
        {
            HElement element = new HElement("div",
                new HAttribute("data-value1", "value1"),
                new HAttribute("data-value2", "value2"),
                new HAttribute("data-value3", "value3")
                );

            element = element.RemoveData("value1", "value4");

            Assert.Equal(new String[] { "data-value2", "data-value3" }, element.Attributes().Select(a => a.Name).ToArray());

            element = element.RemoveData();

            Assert.Equal(new String[] { }, element.Attributes().Select(a => a.Name).ToArray());

            element = null;
            element = element.RemoveData();
            Assert.Null(element);

        }

        [Fact]
        public void TestRemoveDataElements()
        {
            HElement element1 = new HElement("div",
                new HAttribute("data-value1", "value1"),
                new HAttribute("data-value2", "value2"),
                new HAttribute("data-value3", "value3")
                );
            HElement element2 = new HElement("div",
                new HAttribute("data-value2", "value2"),
                new HAttribute("data-value3", "value3"),
                new HAttribute("data-value4", "value4")
                );

            var elements = new HElement[] { element1, null, element2 };

            elements.RemoveData("value1", "value4");
            Assert.Equal(new String[] { "data-value2", "data-value3", "data-value2", "data-value3" }, elements.Where(e => e != null).SelectMany(e => e.Attributes()).Select(a => a.Name).ToArray());

            elements.RemoveData();

            Assert.Equal(new String[] { }, elements.Where(e => e != null).SelectMany(e => e.Attributes()).Select(a => a.Name).ToArray());

            elements = null;
            elements.RemoveData();

        }

    }
}
