using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests
{
    public class HContainerTest
    {

        [Fact]
        public void TestAdd()
        {
            HContainer container = new HDocument();

            // Nothing appening
            container.Add(null);

            
            Assert.Throws<NotImplementedException>(() => container.Add(123.45));

            Assert.Throws<NotImplementedException>(() => container.Add(DateTime.Now));

        }

        [Fact]
        public void TestAddString()
        {
            HContainer container = new HElement("test");

            Assert.Null(container.FirstNode);
            container.Add("String 1");

            Assert.IsType<HText>(container.FirstNode);
            Assert.Equal("String 1", ((HText)container.FirstNode).Value);

            container.Add(" - String 2");

            Assert.IsType<HText>(container.FirstNode);
            Assert.Equal("String 1 - String 2", ((HText)container.FirstNode).Value);

        }

        [Fact]
        public void TestAddAttribute()
        {
            HContainer container = new HDocument();

            // Nothing appening for HContainer
            container.Add(new HAttribute("attr", "value"));

        }

        [Fact]
        public void TestAddNode()
        {
            HContainer container = new HDocument();

            Assert.Throws<NotImplementedException>(() => container.Add(new HElement("parent")));

        }

        [Fact]
        public void TestAddArray()
        {
            HContainer container = new HDocument();

            Assert.Throws<NotImplementedException>(() => container.Add(new object[] { new HElement("parent"), "String", 123, null, DateTime.Now }));
            Assert.Throws<NotImplementedException>(() => container.Add(new HElement("parent"), "String", 123, null, DateTime.Now));

        }

        [Fact]
        public void TestAddEnumerable()
        {
            HContainer container = new HDocument();

            List<object> list = new List<object>(new object[] { new HElement("parent"), "String", 123, null, DateTime.Now });

            Assert.Throws<NotImplementedException>(() => container.Add(list));

        }

        [Fact]
        public void TestFirstNode()
        {
            HContainer container = new HDocument();
            Assert.Null(container.FirstNode);

        }

        [Fact]
        public void TestLastNode()
        {
            HContainer container = new HDocument();
            Assert.Null(container.LastNode);

        }

    }
}
