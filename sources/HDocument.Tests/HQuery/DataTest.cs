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

    }
}
