using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests.HQuery
{
    public class ElementTest
    {
        #region After()

        [Fact]
        public void TestAfterElement()
        {
            var element1 = new HElement("h1");
            var element2 = new HElement("h2");
            var container = new HElement("div", element1, element2).AddClass("container");

            element1.After(new HElement("p"));

            Assert.Equal(new String[] { "h1", "p", "h2" }, container.Elements().Select(e => e.Name));

            var ex = Assert.Throws<InvalidOperationException>(() => container.After(new HElement("p")));
            Assert.Equal("No parent found.", ex.Message);
        }

        [Fact]
        public void TestAfterElements()
        {
            var element1 = new HElement("h1");
            var element2 = new HElement("h2");
            var container = new HElement("div", element1, element2).AddClass("container");

            container.Elements().After(new HElement("p"));

            Assert.Equal(new String[] { "h1", "p", "h2", "p" }, container.Elements().Select(e => e.Name));

        }

        [Fact]
        public void TestAfterElementsByCallback()
        {
            var element1 = new HElement("h1");
            var element2 = new HElement("h2");
            var container = new HElement("div", element1, element2).AddClass("container");

            container.Elements().After((e, i) => new HElement[] { new HElement(e.Name + "-" + i.ToString()) });

            Assert.Equal(new String[] { "h1", "h1-0", "h2", "h2-1" }, container.Elements().Select(e => e.Name));

        }

        #endregion

        #region Before()

        [Fact]
        public void TestBeforeElement()
        {
            var element1 = new HElement("h1");
            var element2 = new HElement("h2");
            var container = new HElement("div", element1, element2).AddClass("container");

            element1.Before(new HElement("p"));

            Assert.Equal(new String[] { "p", "h1", "h2" }, container.Elements().Select(e => e.Name));

            var ex = Assert.Throws<InvalidOperationException>(() => container.Before(new HElement("p")));
            Assert.Equal("No parent found.", ex.Message);
        }

        [Fact]
        public void TestBeforeElements()
        {
            var element1 = new HElement("h1");
            var element2 = new HElement("h2");
            var container = new HElement("div", element1, element2).AddClass("container");

            container.Elements().Before(new HElement("p"));

            Assert.Equal(new String[] { "p", "h1", "p", "h2" }, container.Elements().Select(e => e.Name));

        }

        [Fact]
        public void TestBeforeElementsByCallback()
        {
            var element1 = new HElement("h1");
            var element2 = new HElement("h2");
            var container = new HElement("div", element1, element2).AddClass("container");

            container.Elements().Before((e, i) => new HElement[] { new HElement(e.Name + "-" + i.ToString()) });

            Assert.Equal(new String[] { "h1-0", "h1", "h2-1", "h2" }, container.Elements().Select(e => e.Name));

        }

        #endregion

        #region Empty()

        [Fact]
        public void TestEmptyElement()
        {
            var element = new HElement("div", new HAttribute("attr", "value"), new HElement("p"), new HElement("div"));
            Assert.Same(element, element.Empty());
            Assert.Equal(false, element.HasNodes);
            Assert.Equal(true, element.HasAttributes);

            element = null;
            Assert.Null(element.Empty());
        }

        [Fact]
        public void TestEmptyElements()
        {
            var element1 = new HElement("div", new HAttribute("attr", "value"), new HElement("p"), new HElement("div"));
            var element2 = new HElement("div", new HAttribute("attr", "value"), new HElement("p"), new HElement("div"), new HElement("p"), new HElement("div"));

            var elements = new HElement[] { element1, null, element2 };
            Assert.Same(elements, elements.Empty());
            Assert.Equal(false, element1.HasNodes);
            Assert.Equal(true, element1.HasAttributes);
            Assert.Equal(false, element2.HasNodes);
            Assert.Equal(true, element2.HasAttributes);

            elements = null;
            Assert.Null(elements.Empty());
        }

        #endregion

        #region Clone()

        [Fact]
        public void TestCloneElements()
        {
            var element1 = new HElement("div", new HAttribute("attr", "value"), new HElement("p"), new HElement("div"));
            var element2 = new HElement("div", new HAttribute("attr", "value"), new HElement("p"), new HElement("div"), new HElement("p"), new HElement("div"));

            var elements = new HElement[] { element1, null, element2 };

            var clones = ((IEnumerable<HElement>)elements).Clone().ToArray();
            Assert.NotSame(elements[0], clones[0]);
            Assert.NotSame(elements[2], clones[2]);

            Assert.Null(clones[1]);
            Assert.Equal(1, clones[0].Attributes().Count());
            Assert.Equal(2, clones[0].Elements().Count());
            Assert.Equal(1, clones[2].Attributes().Count());
            Assert.Equal(4, clones[2].Elements().Count());

            elements = null;
            Assert.Null(((IEnumerable<HElement>)elements).Clone());
        }

        #endregion

        #region Append()

        [Fact]
        public void TestAppendElement()
        {
            var element = new HElement("div", new HElement("p"));

            Assert.Same(element, element.Append(new HElement("span"), "test", null, new HAttribute("attr", "value")));
            Assert.Equal(3, element.Nodes().Count());
            Assert.Equal("value", element.Attr("attr"));

            element = null;
            Assert.Null(element.Append(new HElement("span"), "test", null, new HAttribute("attr", "value")));

        }

        [Fact]
        public void TestAppendElements()
        {
            var element1 = new HElement("div", new HElement("p"));
            var element2 = new HElement("div", new HElement("p"), new HElement("div"), new HAttribute("attr1", "val1"));

            var elements = new HElement[] { element1, null, element2 };
            Assert.Same(elements, elements.Append(new HElement("span"), "test", null, new HAttribute("attr", "value")));
            Assert.Equal(3, element1.Nodes().Count());
            Assert.Equal("value", element1.Attr("attr"));
            Assert.Equal(4, element2.Nodes().Count());
            Assert.Equal("value", element2.Attr("attr"));

            elements = null;
            Assert.Null(elements.Append(new HElement("span"), "test", null, new HAttribute("attr", "value")));

        }

        [Fact]
        public void TestAppendElementsByCallback()
        {
            var element1 = new HElement("div", new HElement("p"));
            var element2 = new HElement("div", new HElement("p"), new HElement("div"), new HAttribute("attr1", "val1"));

            var elements = new HElement[] { element1, null, element2 };
            Assert.Same(elements, elements.Append((e, i) => {
                return new object[]{
                    new HAttribute("a"+i.ToString(), i.ToString()),
                    "Content"
                };
            }));
            Assert.Equal(2, element1.Nodes().Count());
            Assert.Equal("0", element1.Attr("a0"));
            Assert.Equal(3, element2.Nodes().Count());
            Assert.Equal("2", element2.Attr("a2"));

            elements = null;
            Assert.Null(elements.Append((e, i) => {
                return new object[]{
                    new HAttribute("a"+i.ToString(), i.ToString()),
                    "Content"
                };
            }));

        }

        #endregion

        #region AppendTo()

        [Fact]
        public void TestAppendToElement()
        {
            var content = new HElement("span", "Content");
            var element = new HElement("div", "First Content");

            Assert.Same(content, content.AppendTo(element));
            Assert.Equal("<div>First Content<span>Content</span></div>", element.ToString());

            Assert.Same(content, content.AppendTo(null));

            content = null;
            Assert.Null(content.AppendTo(element));

        }

        [Fact]
        public void TestAppendToElements()
        {
            var content1 = new HElement("span", "Content1");
            var content2 = new HElement("span", "Content2");
            var element = new HElement("div", "First Content");

            var contents = new HNode[] { content1, null, content2 };
            Assert.Same(contents, contents.AppendTo(element));
            Assert.Equal("<div>First Content<span>Content1</span><span>Content2</span></div>", element.ToString());

            Assert.Same(contents, contents.AppendTo(null));

            contents = null;
            Assert.Null(contents.AppendTo(element));

        }

        #endregion

        #region Prepend()

        [Fact]
        public void TestPrependElement()
        {
            var element = new HElement("div", new HElement("p"));

            Assert.Same(element, element.Prepend(new HElement("span"), "test", null, new HAttribute("attr", "value")));
            Assert.Equal(3, element.Nodes().Count());
            Assert.Equal("value", element.Attr("attr"));
            Assert.Equal("<div attr=\"value\"><span></span>test<p></p></div>", element.ToString());

            element = null;
            Assert.Null(element.Prepend(new HElement("span"), "test", null, new HAttribute("attr", "value")));

        }

        [Fact]
        public void TestPrependElements()
        {
            var element1 = new HElement("div", new HElement("p"));
            var element2 = new HElement("div", new HElement("p"), new HElement("div"), new HAttribute("attr1", "val1"));

            var elements = new HElement[] { element1, null, element2 };
            Assert.Same(elements, elements.Prepend(new HElement("span"), "test", null, new HAttribute("attr", "value")));
            Assert.Equal(3, element1.Nodes().Count());
            Assert.Equal("value", element1.Attr("attr"));
            Assert.Equal(4, element2.Nodes().Count());
            Assert.Equal("value", element2.Attr("attr"));

            elements = null;
            Assert.Null(elements.Prepend(new HElement("span"), "test", null, new HAttribute("attr", "value")));

        }

        [Fact]
        public void TestPrependElementsByCallback()
        {
            var element1 = new HElement("div", new HElement("p"));
            var element2 = new HElement("div", new HElement("p"), new HElement("div"), new HAttribute("attr1", "val1"));

            var elements = new HElement[] { element1, null, element2 };
            Assert.Same(elements, elements.Prepend((e, i) => {
                return new object[]{
                    new HAttribute("a"+i.ToString(), i.ToString()),
                    "Content"
                };
            }));
            Assert.Equal(2, element1.Nodes().Count());
            Assert.Equal("0", element1.Attr("a0"));
            Assert.Equal(3, element2.Nodes().Count());
            Assert.Equal("2", element2.Attr("a2"));

            elements = null;
            Assert.Null(elements.Prepend((e, i) => {
                return new object[]{
                    new HAttribute("a"+i.ToString(), i.ToString()),
                    "Content"
                };
            }));

        }

        #endregion

        #region PrependTo()

        [Fact]
        public void TestPrependToElement()
        {
            var content = new HElement("span", "Content");
            var element = new HElement("div", "First Content");

            Assert.Same(content, content.PrependTo(element));
            Assert.Equal("<div><span>Content</span>First Content</div>", element.ToString());

            Assert.Same(content, content.PrependTo(null));

            content = null;
            Assert.Null(content.PrependTo(element));

        }

        [Fact]
        public void TestPrependToElements()
        {
            var content1 = new HElement("span", "Content1");
            var content2 = new HElement("span", "Content2");
            var element = new HElement("div", "First Content");

            var contents = new HNode[] { content1, null, content2 };
            Assert.Same(contents, contents.PrependTo(element));
            Assert.Equal("<div><span>Content1</span><span>Content2</span>First Content</div>", element.ToString());

            Assert.Same(contents, contents.PrependTo(null));

            contents = null;
            Assert.Null(contents.PrependTo(element));

        }

        #endregion

        #region Children()

        [Fact]
        public void TestChildrenElement()
        {
            var element = new HElement("div", "Content Before", new HElement("p", "Content"), "Content After");

            Assert.Equal(1, element.Children().Count());

            element = null;
            Assert.Equal(0, element.Children().Count());

        }

        [Fact]
        public void TestChildrenElements()
        {
            var element1 = new HElement("div", "Content Before", new HElement("p", "Content"), "Content After");
            var element2 = new HElement("div", "Content Before", new HElement("p", "Content"), "Content After", new HElement("p", "Content"));

            var elements = new HElement[] { element1, null, element2 };
            Assert.Equal(3, elements.Children().Count());

            elements = null;
            Assert.Equal(0, elements.Children().Count());

        }

        #endregion

        #region Contents()

        [Fact]
        public void TestContentsElement()
        {
            var element = new HElement("div", "Content Before", new HElement("p", "Content"), "Content After");

            Assert.Equal(3, element.Contents().Count());

            element = null;
            Assert.Equal(0, element.Contents().Count());

        }

        [Fact]
        public void TestContentsElements()
        {
            var element1 = new HElement("div", "Content Before", new HElement("p", "Content"), "Content After");
            var element2 = new HElement("div", "Content Before", new HElement("p", "Content"), "Content After", new HElement("p", "Content"));

            var elements = new HElement[] { element1, null, element2 };
            Assert.Equal(7, elements.Contents().Count());

            elements = null;
            Assert.Equal(0, elements.Contents().Count());

        }

        #endregion

        #region FirstElement()

        [Fact]
        public void TestFirstElement()
        {
            var element1 = new HElement("div", "Content");
            var element2 = new HElement("div", "Content2");

            var elements=new HElement[]{null, element1,element2};
            Assert.Same(element1, elements.FirstElement().Single());

            elements = new HElement[] { null };
            Assert.Equal(0, elements.FirstElement().Count());

            elements = null;
            Assert.Equal(0, elements.FirstElement().Count());

        }

        #endregion

        #region LastElement()

        [Fact]
        public void TestLastElement()
        {
            var element1 = new HElement("div", "Content");
            var element2 = new HElement("div", "Content2");

            var elements = new HElement[] { element1, element2, null };
            Assert.Same(element2, elements.LastElement().Single());

            elements = new HElement[] { null };
            Assert.Equal(0, elements.LastElement().Count());

            elements = null;
            Assert.Equal(0, elements.LastElement().Count());

        }

        #endregion

        #region Html()

        [Fact]
        public void TestGetHtmlElement()
        {
            var element = new HElement("div", new HElement("span", "Content"));

            Assert.Equal("<span>Content</span>", element.Html());

            element = null;
            Assert.Equal("", element.Html());

        }

        [Fact]
        public void TestGetHtmlElements()
        {
            var element1 = new HElement("div", new HElement("span", "Content 1"));
            var element2 = new HElement("div", new HElement("span", "Content 2"));

            var elements = new HElement[] { element1, null, element2 };
            Assert.Equal("<span>Content 1</span>", elements.Html());

            elements = new HElement[] { null, element1, element2 };
            Assert.Equal("<span>Content 1</span>", elements.Html());

            elements = null;
            Assert.Equal("", elements.Html());

        }

        [Fact]
        public void TestSetHtmlElement()
        {
            var element = new HElement("div", new HElement("span", "Content"));

            Assert.Same(element, element.Html("<strong>New Content</strong>"));
            Assert.Equal("<strong>New Content</strong>", element.Html());
        }

        [Fact]
        public void TestSetHtmlElements()
        {
            var element1 = new HElement("div", new HElement("span", "Content 1"));
            var element2 = new HElement("div", new HElement("span", "Content 2"));

            var elements = new HElement[] { element1, element2 };
            Assert.Same(elements, elements.Html("<strong>New Content</strong>"));
            Assert.Equal("<strong>New Content</strong>", element1.Html());
            Assert.Equal("<strong>New Content</strong>", element2.Html());
        }

        [Fact]
        public void TestSetHtmlElementsByCallback()
        {
            var element1 = new HElement("div", new HElement("span", "Content 1"));
            var element2 = new HElement("div", new HElement("span", "Content 2"));

            var elements = new HElement[] { element1, null, element2 };
            Assert.Same(elements, elements.Html((e, i) => {
                return String.Format("<strong>New Content {0}</strong>", i);
            }));
            Assert.Equal("<strong>New Content 0</strong>", element1.Html());
            Assert.Equal("<strong>New Content 2</strong>", element2.Html());
        }

        #endregion

        #region InsertAfter()

        [Fact]
        public void TestInsertAfterElement()
        {
            var n1 = new HText("Content 1");
            var n2 = new HElement("span", "Content 2");
            var n3 = new HText("Content 3");
            var element = new HElement("div", n1, n2, n3);
            var n4 = new HText("Insertion");

            Assert.Same(n4, n4.InsertAfter(n2));
            Assert.Equal("<div>Content 1<span>Content 2</span>InsertionContent 3</div>", element.ToString());
        }

        [Fact]
        public void TestInsertAfterElements()
        {
            var n1 = new HText("Content 1");
            var n2 = new HElement("span", "Content 2");
            var n3 = new HText("Content 3");
            var element = new HElement("div", n1, n2, n3);
            var n4 = new HText("Insertion1");
            var n5 = new HText("Insertion2");

            var elements = new HNode[] { n4, n5 };
            Assert.Same(elements, elements.InsertAfter(n2));
            Assert.Equal("<div>Content 1<span>Content 2</span>Insertion1Insertion2Content 3</div>", element.ToString());
        }

        #endregion

        #region InsertBefore()

        [Fact]
        public void TestInsertBeforeElement()
        {
            var n1 = new HText("Content 1");
            var n2 = new HElement("span", "Content 2");
            var n3 = new HText("Content 3");
            var element = new HElement("div", n1, n2, n3);
            var n4 = new HText("Insertion");

            Assert.Same(n4, n4.InsertBefore(n2));
            Assert.Equal("<div>Content 1Insertion<span>Content 2</span>Content 3</div>", element.ToString());
        }

        [Fact]
        public void TestInsertBeforeElements()
        {
            var n1 = new HText("Content 1");
            var n2 = new HElement("span", "Content 2");
            var n3 = new HText("Content 3");
            var element = new HElement("div", n1, n2, n3);
            var n4 = new HText("Insertion1");
            var n5 = new HText("Insertion2");

            var elements = new HNode[] { n4, n5 };
            Assert.Same(elements, elements.InsertBefore(n2));
            Assert.Equal("<div>Content 1Insertion1Insertion2<span>Content 2</span>Content 3</div>", element.ToString());
        }

        #endregion

        #region Next()

        [Fact]
        public void TestNext()
        {
            var n1 = new HText("Content 1");
            var n2 = new HElement("span", "Content 2");
            var n3 = new HElement("span", "Content 3");
            var n4 = new HText("Content 4");
            var element = new HElement("div", n1, n2, n3, n4);

            Assert.Equal(new HNode[] { n3, n4 }, new HNode[] { n2, n3, n4 }.Next().ToArray());
            Assert.Equal(new HNode[] { }, ((HNode[])null).Next().ToArray());
        }

        #endregion

        #region NextAll()

        [Fact]
        public void TestNextAll()
        {
            var n1 = new HText("Content 1");
            var n2 = new HElement("span", "Content 2");
            var n3 = new HElement("span", "Content 3");
            var n4 = new HText("Content 4");
            var element = new HElement("div", n1, n2, n3, n4);

            Assert.Equal(new HNode[] { n3, n4 }, n2.NextAll().ToArray());
            Assert.Equal(new HNode[] { n2, n3, n4 }, new HNode[] { n1, n3, n4 }.NextAll().ToArray());

            Assert.Equal(new HNode[] { }, ((HNode[])null).NextAll().ToArray());
        }

        #endregion

        #region Prev()

        [Fact]
        public void TestPrev()
        {
            var n1 = new HText("Content 1");
            var n2 = new HElement("span", "Content 2");
            var n3 = new HElement("span", "Content 3");
            var n4 = new HText("Content 4");
            var element = new HElement("div", n1, n2, n3, n4);

            Assert.Equal(new HNode[] { n1, n3 }, new HNode[] { n1, n2, n4 }.Prev().ToArray());
            Assert.Equal(new HNode[] { }, ((HNode[])null).Prev().ToArray());
        }

        #endregion

        #region PrevAll()

        [Fact]
        public void TestPrevAll()
        {
            var n1 = new HText("Content 1");
            var n2 = new HElement("span", "Content 2");
            var n3 = new HElement("span", "Content 3");
            var n4 = new HText("Content 4");
            var element = new HElement("div", n1, n2, n3, n4);

            Assert.Equal(new HNode[] { n2, n1 }, n3.PrevAll().ToArray());
            Assert.Equal(new HNode[] { n2, n1, n3 }, new HNode[] { n1, n3, n4 }.PrevAll().ToArray());

            Assert.Equal(new HNode[] { }, ((HNode[])null).PrevAll().ToArray());
        }

        #endregion

        #region Parent()

        [Fact]
        public void TestParent()
        {
            var n1 = new HText("Content");
            var n2 = new HElement("span", n1);
            var n3 = new HElement("span", "Another content");
            var n4 = new HElement("div", n2, n3);

            Assert.Equal(new HElement[] { n2 }, new HNode[] { n1 }.Parent());
            Assert.Equal(new HElement[] { n4 }, new HNode[] { n3 }.Parent());
            Assert.Equal(new HElement[] { }, new HNode[] { n4 }.Parent());
            Assert.Equal(new HElement[] { n4, n2 }, new HNode[] { n3, n1, n2 }.Parent());
            Assert.Equal(new HElement[] { }, ((HNode[])null).Parent());
        }

        #endregion

        #region Parents()

        [Fact]
        public void TestParents()
        {
            var n1 = new HText("Content");
            var n2 = new HElement("span", n1);
            var n3 = new HElement("span", "Another content");
            var n4 = new HElement("div", n2, n3);

            Assert.Equal(new HElement[] { n2, n4 }, new HNode[] { n1 }.Parents());
            Assert.Equal(new HElement[] { n4 }, new HNode[] { n3 }.Parents());
            Assert.Equal(new HElement[] { }, new HNode[] { n4 }.Parents());
            Assert.Equal(new HElement[] { n4, n2 }, new HNode[] { n3, n1, n2 }.Parents());
            Assert.Equal(new HElement[] { }, ((HNode[])null).Parents());
        }

        #endregion

        #region Remove()

        [Fact]
        public void TestRemove()
        {
            var n1 = new HText("Content");
            var n2 = new HElement("span", n1);
            var n3 = new HElement("span", "Another content");
            var n4 = new HElement("div", n2, n3);

            var elements = new HNode[] { n3, n1 };
            Assert.Same(elements, elements.Remove());
            Assert.Equal("<div><span></span></div>", n4.ToString());

            elements = null;
            Assert.Null(elements.Remove());

        }

        #endregion

        #region ReplaceWith()

        [Fact]
        public void TestReplaceWith()
        {
            var n1 = new HText("Content");
            var n2 = new HElement("span", n1);
            var n3 = new HElement("span", "Another content");
            var n4 = new HElement("div", n2, n3);

            Assert.Same(n2, n2.ReplaceWith(new HElement("div", "Div Content"), new HElement("p", "P content")));
            Assert.Equal("<div><div>Div Content</div><p>P content</p><span>Another content</span></div>", n4.ToString());

            Assert.Same(n3, n3.ReplaceWith(null));
            Assert.Equal("<div><div>Div Content</div><p>P content</p></div>", n4.ToString());

            n2 = null;
            Assert.Null(n2.ReplaceWith(new HElement("div", "Div Content"), new HElement("p", "P content")));
            Assert.Equal("<div><div>Div Content</div><p>P content</p></div>", n4.ToString());

        }

        [Fact]
        public void TestReplaceWithElements()
        {
            var n1 = new HText("Content");
            var n2 = new HElement("span", n1);
            var n3 = new HElement("span", "Another content");
            var n4 = new HElement("div", n2, n3);

            IEnumerable<HElement> elements = new HElement[] { n2, n3 };
            Assert.Same(elements, elements.ReplaceWith(new HElement("div", "Div Content"), new HElement("p", "P content")));
            Assert.Equal("<div><div>Div Content</div><p>P content</p><div>Div Content</div><p>P content</p></div>", n4.ToString());

            n4.Descendants("div").ReplaceWith(null, null, null);
            Assert.Equal("<div><p>P content</p><p>P content</p></div>", n4.ToString());

            elements = null;
            Assert.Null(elements.ReplaceWith(new HElement("div", "Div Content"), new HElement("p", "P content")));
            Assert.Equal("<div><p>P content</p><p>P content</p></div>", n4.ToString());

        }

        [Fact]
        public void TestReplaceWithElementsByCallback()
        {
            var n1 = new HText("Content");
            var n2 = new HElement("span", n1);
            var n3 = new HElement("span", "Another content");
            var n4 = new HElement("div", n2, n3);

            var elements = new HElement[] { n2, n3 };
            Assert.Same(elements, elements.ReplaceWith((e, i) => {
                return new HNode[] { new HElement("div", "Div Content " + i.ToString()), new HElement("span", "Span Content " + i.ToString()) };
            }));
            Assert.Equal("<div><div>Div Content 0</div><span>Span Content 0</span><div>Div Content 1</div><span>Span Content 1</span></div>", n4.ToString());

            n4.Descendants("div").ReplaceWith((e, i) => null);
            Assert.Equal("<div><span>Span Content 0</span><span>Span Content 1</span></div>", n4.ToString());

            elements = null;
            Assert.Null(elements.ReplaceWith((e, i) => {
                return new HNode[] { new HElement("div", "Div Content " + i.ToString()), new HElement("span", "Span Content " + i.ToString()) };
            }));
            Assert.Equal("<div><span>Span Content 0</span><span>Span Content 1</span></div>", n4.ToString());

        }

        #endregion

        #region ReplaceAll

        [Fact]
        public void TestReplaceAll()
        {
            var n1 = new HText("Content");
            var n2 = new HElement("span", n1);
            var n3 = new HElement("span", "Another content");
            var n4 = new HElement("div", n2, n3);

            var elements = new HElement[] { new HElement("p", "p 1"), null, new HElement("div", "div 2") };
            Assert.Same(elements, elements.ReplaceAll(n3, n2));
            Assert.Equal("<div><p>p 1</p><div>div 2</div><p>p 1</p><div>div 2</div></div>", n4.ToString());

            elements = null;
            Assert.Null(elements.ReplaceAll(n3, n2));
            Assert.Equal("<div><p>p 1</p><div>div 2</div><p>p 1</p><div>div 2</div></div>", n4.ToString());

        }

        #endregion

        #region Wrap()

        [Fact]
        public void TestWrapElement()
        {
            var n1 = new HElement("span", "Content");
            var n2 = new HElement("div", n1);

            Assert.Same(n1, n1.Wrap(new HElement("strong", new HElement("em"))));
            Assert.Equal("<div><strong><em><span>Content</span></em></strong></div>", n2.ToString());

            n1 = null;
            Assert.Null(n1.Wrap(new HElement("strong", new HElement("em"))));
            Assert.Equal("<div><strong><em><span>Content</span></em></strong></div>", n2.ToString());

        }

        [Fact]
        public void TestWrapElements()
        {
            var n1 = new HElement("span", "Content 1");
            var n2 = new HElement("span", "Content 2");
            var n3 = new HElement("div", n1, n2);
            Assert.Equal("<div><span>Content 1</span><span>Content 2</span></div>", n3.ToString());

            var elements = new HElement[] { n1, n2 };
            Assert.Same(elements, elements.Wrap(new HElement("strong", new HElement("em"))));
            Assert.Equal("<div><strong><em><span>Content 1</span></em></strong><strong><em><span>Content 2</span></em></strong></div>", n3.ToString());

            elements = null;
            Assert.Null(elements.Wrap(new HElement("strong", new HElement("em"))));
            Assert.Equal("<div><strong><em><span>Content 1</span></em></strong><strong><em><span>Content 2</span></em></strong></div>", n3.ToString());

        }

        [Fact]
        public void TestWrapElementsByCallback()
        {
            var n1 = new HElement("span", "Content 1");
            var n2 = new HElement("span", "Content 2");
            var n3 = new HElement("div", n1, n2);
            Assert.Equal("<div><span>Content 1</span><span>Content 2</span></div>", n3.ToString());

            var elements = new HElement[] { n1, n2 };
            Assert.Same(elements, elements.Wrap((e, i) => new HElement("strong", "C" + i.ToString(), new HElement("em"))));
            Assert.Equal("<div><strong>C0<em><span>Content 1</span></em></strong><strong>C1<em><span>Content 2</span></em></strong></div>", n3.ToString());

            elements = null;
            Assert.Null(elements.Wrap((e, i) => new HElement("strong", "C" + i.ToString(), new HElement("em"))));
            Assert.Equal("<div><strong>C0<em><span>Content 1</span></em></strong><strong>C1<em><span>Content 2</span></em></strong></div>", n3.ToString());

        }

        #endregion

        #region WrapAll()

        [Fact]
        public void TestWrapAll()
        {
            var n1 = new HElement("span", "Content 1");
            var n2 = new HElement("span", "Content 2");
            var n3 = new HElement("span", "Content 3");
            var n4 = new HElement("span", "Content 4");
            var root = new HElement("div", n1, n2, n3, n4);

            var elements = new HElement[] { n1, n3, n4 };
            Assert.Same(elements, elements.WrapAll(new HElement("p", new HElement("a"))));
            Assert.Equal("<div><p><a><span>Content 1</span><span>Content 3</span><span>Content 4</span></a></p><span>Content 2</span></div>", root.ToString());

        }

        #endregion

        #region WrapInner()

        [Fact]
        public void TestWrapInnerElement()
        {
            var n1 = new HElement("span", "Content");
            var n2 = new HElement("span");
            var root = new HElement("div", n1, n2);

            Assert.Same(n1, n1.WrapInner(new HElement("strong", new HElement("em"))));
            Assert.Equal("<div><span><strong><em>Content</em></strong></span><span></span></div>", root.ToString());

            Assert.Same(n2, n2.WrapInner(new HElement("p", new HElement("a"))));
            Assert.Equal("<div><span><strong><em>Content</em></strong></span><span><p><a></a></p></span></div>", root.ToString());

            n1 = null;
            Assert.Null(n1.WrapInner(new HElement("strong", new HElement("em"))));
            Assert.Equal("<div><span><strong><em>Content</em></strong></span><span><p><a></a></p></span></div>", root.ToString());

        }

        [Fact]
        public void TestWrapInnerElements()
        {
            var n1 = new HElement("span", "Content 1");
            var n2 = new HElement("span", "Content 2");
            var n3 = new HElement("div", n1, n2);
            Assert.Equal("<div><span>Content 1</span><span>Content 2</span></div>", n3.ToString());

            var elements = new HElement[] { n1, n2 };
            Assert.Same(elements, elements.WrapInner(new HElement("strong", new HElement("em"))));
            Assert.Equal("<div><span><strong><em>Content 1</em></strong></span><span><strong><em>Content 2</em></strong></span></div>", n3.ToString());

            elements = null;
            Assert.Null(elements.WrapInner(new HElement("strong", new HElement("em"))));
            Assert.Equal("<div><span><strong><em>Content 1</em></strong></span><span><strong><em>Content 2</em></strong></span></div>", n3.ToString());

        }

        [Fact]
        public void TestWrapInnerElementsByCallback()
        {
            var n1 = new HElement("span", "Content 1");
            var n2 = new HElement("span", "Content 2");
            var n3 = new HElement("div", n1, n2);
            Assert.Equal("<div><span>Content 1</span><span>Content 2</span></div>", n3.ToString());

            var elements = new HElement[] { n1, n2 };
            Assert.Same(elements, elements.WrapInner((e, i) => new HElement("strong", "C" + i.ToString(), new HElement("em"))));
            Assert.Equal("<div><span><strong>C0<em>Content 1</em></strong></span><span><strong>C1<em>Content 2</em></strong></span></div>", n3.ToString());

            elements = null;
            Assert.Null(elements.WrapInner((e, i) => new HElement("strong", "C" + i.ToString(), new HElement("em"))));
            Assert.Equal("<div><span><strong>C0<em>Content 1</em></strong></span><span><strong>C1<em>Content 2</em></strong></span></div>", n3.ToString());

        }

        #endregion

        #region Unwrap

        [Fact]
        public void TestUnwrapElement()
        {
            var n1 = new HElement("span", "Content");
            var root = new HElement("div", new HElement("strong", new HElement("em", n1)));

            Assert.Equal("<div><strong><em><span>Content</span></em></strong></div>", root.ToString());

            Assert.Same(n1, n1.Unwrap());
            Assert.Equal("<div><strong><span>Content</span></strong></div>", root.ToString());

            n1 = null;
            Assert.Null(n1.Unwrap());

        }

        [Fact]
        public void TestUnwrapElements()
        {
            var n1 = new HElement("span", "Content");
            var n2 = new HText("Content");
            var root = new HElement("div", new HElement("a", new HElement("strong", new HElement("em", n1))), new HElement("div", n2));

            Assert.Equal("<div><a><strong><em><span>Content</span></em></strong></a><div>Content</div></div>", root.ToString());

            IEnumerable<HNode> elements = new HNode[] { n1, n2 };
            Assert.Same(elements, elements.Unwrap());
            Assert.Equal("<div><a><strong><span>Content</span></strong></a>Content</div>", root.ToString());

            elements = null;
            Assert.Null(elements.Unwrap());

        }

        #endregion

        #region Siblings()

        [Fact]
        public void TestSiblingsElement()
        {
            var n1 = new HElement("div", "Content 1");
            var n2 = new HElement("div", "Content 2");
            var n3 = new HElement("div", "Content 3");
            var n4 = new HElement("div", "Content 4");
            var n5 = new HElement("div", "Content 5");
            var root = new HElement("div", n1, n2, n3, n4, n5);

            Assert.Equal(new HElement[] { n1, n3, n4, n5 }, n2.Siblings().ToArray());

            n2 = null;
            Assert.Equal(0, n2.Siblings().Count());
        }

        [Fact]
        public void TestSiblingsElements()
        {
            var n1 = new HElement("div", "Content 1");
            var n2 = new HElement("div", "Content 2");
            var n3 = new HElement("div", "Content 3");
            var n4 = new HElement("div", "Content 4");
            var n5 = new HElement("div", "Content 5");
            var root = new HElement("div", n1, n2, n3, n4, n5);

            var elements = new HElement[] { n2, n4 };
            Assert.Equal(new HElement[] { n1, n3, n4, n5, n2 }, elements.Siblings().ToArray());

            elements = null;
            Assert.Equal(0, elements.Siblings().Count());
        }

        #endregion

        #region Slice()

        [Fact]
        public void TestSlice()
        {
            var n1 = new HElement("div", "Content 1");
            var n2 = new HElement("div", "Content 2");
            var n3 = new HElement("div", "Content 3");
            var n4 = new HElement("div", "Content 4");
            var n5 = new HElement("div", "Content 5");
            var root = new HElement("div", n1, n2, n3, n4, n5);

            IEnumerable<HElement> elements = root.Elements().ToArray();
            Assert.Equal(new HElement[] { n1, n2, n3, n4, n5 }, elements.Slice(0).ToArray());
            Assert.Equal(new HElement[] { n3, n4, n5 }, elements.Slice(2).ToArray());
            Assert.Equal(new HElement[] { n4, n5 }, elements.Slice(-2).ToArray());

            Assert.Equal(new HElement[] { n1, n2, n3, n4, n5 }, elements.Slice(0, 4).ToArray());
            Assert.Equal(new HElement[] { n3, n4 }, elements.Slice(2, 3).ToArray());
            Assert.Equal(new HElement[] { n3 }, elements.Slice(2, 2).ToArray());
            Assert.Equal(new HElement[] { n4, n5 }, elements.Slice(-2, 4).ToArray());
            Assert.Equal(new HElement[] { n4, n5 }, elements.Slice(-2, -1).ToArray());
            Assert.Equal(new HElement[] { }, elements.Slice(-2, 2).ToArray());

            elements = null;
            Assert.Equal(0, elements.Slice(0).Count());
            Assert.Equal(0, elements.Slice(0, 4).Count());
        }

        #endregion

    }
}
