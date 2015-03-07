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

        #region Prepend()

        [Fact]
        public void TestPrependElement()
        {
            var element = new HElement("div", new HElement("p"));

            Assert.Same(element, element.Prepend(new HElement("span"), "test", null, new HAttribute("attr", "value")));
            Assert.Equal(3, element.Nodes().Count());
            Assert.Equal("value", element.Attr("attr"));

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

        #region AppendTo()

        [Fact]
        public void TestAppendToElement()
        {
            var content = new HElement("span", "Content");
            var element = new HElement("div");

            Assert.Same(content, content.AppendTo(element));
            Assert.Equal("<div><span>Content</span></div>", element.ToString());

            Assert.Same(content, content.AppendTo(null));

            content = null;
            Assert.Null(content.AppendTo(element));

        }

        [Fact]
        public void TestAppendToElements()
        {
            var content1 = new HElement("span", "Content1");
            var content2 = new HElement("span", "Content2");
            var element = new HElement("div");

            var contents = new HNode[] { content1, null, content2 };
            Assert.Same(contents, contents.AppendTo(element));
            Assert.Equal("<div><span>Content1</span><span>Content2</span></div>", element.ToString());

            Assert.Same(contents, contents.AppendTo(null));

            contents = null;
            Assert.Null(contents.AppendTo(element));

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

    }
}
