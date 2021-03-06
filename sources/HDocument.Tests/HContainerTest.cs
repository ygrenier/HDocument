﻿using Moq;
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
            HContainer container = new HElement("div");

            // Nothing appening
            container.Add(null);

            Assert.Equal(0, container.Nodes().Count());

            container.Add(123.45);
            String s = (123.45).ToString();
            Assert.Equal(1, container.Nodes().Count());
            Assert.Equal("<div>" + s + "</div>", container.ToString());

            container.Add(null);
            Assert.Equal(1, container.Nodes().Count());
            Assert.Equal("<div>" + s + "</div>", container.ToString());

            container.Add("Other Content");
            Assert.Equal(1, container.Nodes().Count());
            Assert.Equal("<div>" + s + "Other Content</div>", container.ToString());

            container.Add(new HElement("span", "SPAN"));
            Assert.Equal(2, container.Nodes().Count());
            Assert.Equal("<div>" + s + "Other Content<span>SPAN</span></div>", container.ToString());

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

        [Fact]
        public void TestRemoveNodes()
        {
            HNode node1 = new HText("node 1");
            HNode node2 = new HElement("node2", "value2");
            HNode node3 = new HText("node 3");
            HNode node4 = new HElement("node4", "value4");

            // Create parent
            var elm = new HElement("test", node1, node2, node3, node4);
            Assert.Same(node1, elm.FirstNode);
            Assert.Same(node4, elm.LastNode);

            Assert.Same(elm, node1.Parent);
            Assert.Null(node1.PreviousNode);
            Assert.Same(node2, node1.NextNode);

            Assert.Same(elm, node2.Parent);
            Assert.Same(node1, node2.PreviousNode);
            Assert.Same(node3, node2.NextNode);

            Assert.Same(elm, node3.Parent);
            Assert.Same(node2, node3.PreviousNode);
            Assert.Same(node4, node3.NextNode);

            Assert.Same(elm, node4.Parent);
            Assert.Same(node3, node4.PreviousNode);
            Assert.Null(node4.NextNode);

            // Remove all nodes
            elm.RemoveNodes();

            Assert.Null(elm.FirstNode);
            Assert.Null(elm.LastNode);

            Assert.Null(node1.Parent);
            Assert.Null(node1.PreviousNode);
            Assert.Null(node1.NextNode);
                        
            Assert.Null(node2.Parent);
            Assert.Null(node2.PreviousNode);
            Assert.Null(node2.NextNode);
                        
            Assert.Null(node3.Parent);
            Assert.Null(node3.PreviousNode);
            Assert.Null(node3.NextNode);
                        
            Assert.Null(node4.Parent);
            Assert.Null(node4.PreviousNode);
            Assert.Null(node4.NextNode);

            Assert.Equal(0, elm.Nodes().Count());

            // Create new element with string content
            elm = new HElement("test", "content");
            elm.RemoveNodes();
            Assert.Null(elm.FirstNode);
            Assert.Null(elm.LastNode);
            Assert.Equal(0, elm.Nodes().Count());

            // Check no exception
            elm.RemoveNodes();
            Assert.Equal(0, elm.Nodes().Count());
        }

        [Fact]
        public void TestDescendantNodes()
        {
            var t1 = new HText("text 1");

            var doc = new HDocument(
                new HElement(
                    "html",
                    new HElement(
                        "body",
                        new HElement(
                            "div",
                            new HAttribute("id", "div1"),
                            new HElement(
                                "p",
                                new HAttribute("id", "p1"),
                                new HElement(
                                    "div",
                                    new HAttribute("id", "div3"),
                                    new HElement(
                                        "p",
                                        new HAttribute("id", "p2"),
                                        new HText("text 1"),
                                        new HText("text 2")
                                        )
                                    )
                                )
                            ),
                        new HElement(
                            "div",
                            new HAttribute("id", "div2"),
                            new HElement(
                                "p",
                                new HText("text 3")
                                )
                            )
                        )
                    )
                );

            // Get all document nodes
            Assert.Equal(new String[] { 
                "<html>", "<body>", "<div>", "<p>", "<div>", "<p>", "HText", "HText", "<div>", "<p>", "HText" 
            }, doc.DescendantNodes().Select(n => {
                if (n is HElement)
                    return String.Format("<{0}>", ((HElement)n).Name);
                else
                    return n.GetType().Name;
            }).ToArray());

        }

        [Fact]
        public void TestDescendants()
        {
            var t1 = new HText("text 1");

            var doc = new HDocument(
                new HElement(
                    "html",
                    new HElement(
                        "body",
                        new HElement(
                            "div",
                            new HAttribute("id", "div1"),
                            new HElement(
                                "p",
                                new HAttribute("id", "p1"),
                                new HElement(
                                    "div",
                                    new HAttribute("id", "div3"),
                                    new HElement(
                                        "p",
                                        new HAttribute("id", "p2"),
                                        new HText("text 1"),
                                        new HText("text 2")
                                        )
                                    )
                                )
                            ),
                        new HElement(
                            "div",
                            new HAttribute("id", "div2"),
                            new HElement(
                                "p",
                                new HText("text 3")
                                )
                            )
                        )
                    )
                );

            // Get all document elements
            Assert.Equal(new String[] { 
                "<html>", "<body>", "<div@div1>", "<p@p1>", "<div@div3>", "<p@p2>", "<div@div2>", "<p>"
            }, doc.Descendants().Select(n => {
                var attr = n.Attribute("id");
                if (attr != null)
                    return String.Format("<{0}@{1}>", ((HElement)n).Name, attr.Value);
                else
                    return String.Format("<{0}>", ((HElement)n).Name);
            }).ToArray());

            // Get document div
            Assert.Equal(new String[] { 
                "<div@div1>", "<div@div3>", "<div@div2>"
            }, doc.Descendants("Div").Select(n => {
                var attr = n.Attribute("id");
                if (attr != null)
                    return String.Format("<{0}@{1}>", ((HElement)n).Name, attr.Value);
                else
                    return String.Format("<{0}>", ((HElement)n).Name);
            }).ToArray());

        }

    }
}
