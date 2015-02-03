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

            
            //Assert.Throws<NotImplementedException>(() => container.Add(123.45));

            //Assert.Throws<NotImplementedException>(() => container.Add(DateTime.Now));

        }

        [Fact]
        public void TestAddString()
        {
            HContainer container = new HElement("test");

            container.Add("String 1");
            container.Add(" - String 2");

            Assert.IsType<HText>(container.FirstNode);
            Assert.Equal("String 1 - String 2", ((HText)container.FirstNode).Value);

            container.Add(" - String 3");

            Assert.IsType<HText>(container.FirstNode);
            Assert.Equal("String 1 - String 2 - String 3", ((HText)container.FirstNode).Value);

            container = new HElement("test");
            container.Add("Content");
            container.Add(new HText("Other content"));
            Assert.Equal(2, container.Nodes().Count());
        }

        [Fact]
        public void TestAddAttribute()
        {
            HContainer container = new HDocument();

            // Nothing appening for HContainer
            container.Add(new HAttribute("attr", "value"));

            Assert.Null(container.FirstNode);

        }

        [Fact]
        public void TestAddNode()
        {
            HContainer container = new HElement("parent");

            HNode node1 = new HElement("test");
            container.Add(node1);

            HNode node2 = new HElement("test2");
            container.Add(node2);

            Assert.Same(node1, container.FirstNode);
            Assert.Same(node2, container.LastNode);

        }

        [Fact]
        public void TestAddNodeAlreadyAssigned()
        {
            HContainer container = new HElement("parent");
            HNode node1 = new HElement("test");
            container.Add(node1);

            HContainer container2 = new HElement("OtherParent");
            container2.Add(node1);

            Assert.IsType<HElement>(container.FirstNode);
            Assert.IsType<HElement>(container2.FirstNode);

            Assert.Same(node1, container.FirstNode);
            Assert.NotSame(node1, container2.FirstNode);

        }

        [Fact]
        public void TestAddParentAsNode()
        {
            HContainer parent = new HElement("parent");
            HElement child1 = new HElement("child1");
            parent.Add(child1);
            HElement child2 = new HElement("child2");
            child1.Add(child2);

            child2.Add(parent);

            Assert.IsType<HElement>(child2.FirstNode);
            HElement node = (HElement)child2.FirstNode;
            Assert.Equal("parent", node.Name);
            Assert.NotSame(parent, node);

            node = (HElement)node.FirstNode;
            Assert.Equal("child1", node.Name);
            Assert.NotSame(child1, node);

            node = (HElement)node.FirstNode;
            Assert.Equal("child2", node.Name);
            Assert.NotSame(child2, node);

            Assert.Null(node.FirstNode);
        }

        [Fact]
        public void TestAddArray()
        {
            // params 
            HContainer container = new HElement("parent");

            HNode node1 = new HElement("test");
            HNode node2 = new HElement("test2");

            container.Add(node1, node2);

            Assert.Same(node1, container.FirstNode);
            Assert.Same(node2, container.LastNode);

            // direct array
            container = new HElement("parent");

            node1 = new HElement("test");
            node2 = new HElement("test2");

            container.Add(new HNode[] { node1, node2 });

            Assert.Same(node1, container.FirstNode);
            Assert.Same(node2, container.LastNode);

            // mixed content
            container = new HElement("parent");
            var dt = DateTime.Now;
            container.Add(new HElement("parent"), "String", 123, null, dt);

            var node = container.FirstNode;
            Assert.IsType<HElement>(node);
            Assert.Equal("parent", ((HElement)node).Name);

            node = node.NextNode;
            Assert.IsType<HText>(node);
            Assert.Equal(String.Format("{0}{1}{2}", "String", 123, dt), ((HText)node).Value);

            node = node.NextNode;
            Assert.Null(node);
        }

        [Fact]
        public void TestAddEnumerable()
        {

            var container = new HElement("parent");
            var dt = DateTime.Now;
            container.Add(new List<Object>(new object[] { new HElement("parent"), "String", 123, null, dt }));

            var node = container.FirstNode;
            Assert.IsType<HElement>(node);
            Assert.Equal("parent", ((HElement)node).Name);

            node = node.NextNode;
            Assert.IsType<HText>(node);
            Assert.Equal(String.Format("{0}{1}{2}", "String", 123, dt), ((HText)node).Value);

            node = node.NextNode;
            Assert.Null(node);

        }

        [Fact]
        public void TestClone()
        {
            var container = new HElement("parent");
            container.Add(new HElement("child1"));
            container.Add(new HElement("child2", "Content"));
            container.Add(new HElement("child3", new HText("Content 1"), new HText("Content 2")));

            // Adding this container itself to a clone
            container.Add(container);

            var nodes = container.Nodes().ToArray();
            Assert.Equal(4, nodes.Length);
            
        }

        [Fact]
        public void TestFirstNode()
        {
            HContainer container = new HElement("Test");
            Assert.Null(container.FirstNode);

        }

        [Fact]
        public void TestLastNode()
        {
            HContainer container = new HElement("Test");
            Assert.Null(container.LastNode);

            var node = new HText("Content");
            container.Add(node);
            Assert.Same(node, container.LastNode);

            container.Add(new HText("Other content"));
            Assert.NotSame(node, container.LastNode);

            // If content is an empty string, LastNode returns null
            container = new HElement("Test");
            container.Add("");
            Assert.Null(container.LastNode);

        }

        [Fact]
        public void TestNodes()
        {
            
            HContainer container = new HElement("Test");
            Assert.False(container.HasNodes);
            HNode[] nodes = container.Nodes().ToArray();
            Assert.Equal(0, nodes.Length);

            container.Add("Content 1");
            Assert.True(container.HasNodes);
            container.Add(new HElement("test"));
            container.Add("Content 2");
            container.Add(new HElement("element"));
            container.Add("Content 3");

            nodes = container.Nodes().ToArray();
            Assert.Equal(5, nodes.Length);

            Assert.IsType<HText>(nodes[0]);
            Assert.IsType<HElement>(nodes[1]);
            Assert.IsType<HText>(nodes[2]);
            Assert.IsType<HElement>(nodes[3]);
            Assert.IsType<HText>(nodes[4]);

        }

        [Fact]
        public void TestElements()
        {

            HContainer container = new HElement("Test");
            Assert.False(container.HasElements);
            HElement[] elements = container.Elements().ToArray();
            Assert.Equal(0, elements.Length);

            container.Add("Content 1");
            Assert.False(container.HasElements);
            container.Add(new HElement("test"));
            Assert.True(container.HasElements);
            container.Add("Content 2");
            container.Add(new HElement("element"));
            container.Add("Content 3");

            elements = container.Elements().ToArray();
            Assert.Equal(2, elements.Length);

        }

        [Fact]
        public void TestElementsWithName()
        {

            HContainer container = new HElement("Test");
            HElement[] elements = container.Elements("Test").ToArray();
            Assert.Equal(0, elements.Length);

            container.Add("Content 1");
            container.Add(new HElement("test"));
            container.Add("Content 2");
            container.Add(new HElement("element"));
            container.Add("Content 3");

            elements = container.Elements("Test").ToArray();
            Assert.Equal(1, elements.Length);

        }

    }
}
