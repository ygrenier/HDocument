using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests
{

    public class HObjectTest
    {

        [Fact]
        public void TestParent()
        {
            HObject obj = new HText("test");

            Assert.Null(obj.Parent);

            HElement p = new HElement("parent", obj);
            Assert.Same(p, obj.Parent);

        }

        [Fact]
        public void TestDocument()
        {
            HObject n = new HText("test");
            Assert.Null(n.Document);

            HObject p = new HElement("parent", n);
            Assert.Null(n.Document);
            Assert.Null(p.Document);

            HDocument doc = new HDocument(p);
            Assert.Same(doc, n.Document);
            Assert.Same(doc, p.Document);
            Assert.Same(doc, doc.Document);
        }

        [Fact]
        public void TestParents()
        {
            var t1 = new HText("text 1");

            Assert.Equal(0, t1.Parents().Count());
            Assert.Equal(0, t1.Parents("div").Count());

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
                                        t1
                                        )
                                    )
                                )
                            ),
                        new HElement(
                            "div",
                            new HAttribute("id", "div2"),
                            new HElement(
                                "p"
                                )
                            )
                        )
                    )
                );

            // Get all parents
            Assert.Equal(new String[] { "p@p2", "div@div3", "p@p1", "div@div1", "body", "html" }, t1.Parents().Select(h => {
                var id = h.Attribute("id");
                return id != null ? String.Format("{0}@{1}", h.Name, id.Value) : h.Name;
            }).ToArray());

            // Get parents filtered
            Assert.Equal(new String[] { "p@p2", "p@p1" }, t1.Parents("p").Select(h => {
                var id = h.Attribute("id");
                return id != null ? String.Format("{0}@{1}", h.Name, id.Value) : h.Name;
            }).ToArray());
            Assert.Equal(new String[] { "div@div3", "div@div1" }, t1.Parents("div").Select(h => {
                var id = h.Attribute("id");
                return id != null ? String.Format("{0}@{1}", h.Name, id.Value) : h.Name;
            }).ToArray());
            Assert.Equal(new String[] { }, t1.Parents("strong").Select(h => {
                var id = h.Attribute("id");
                return id != null ? String.Format("{0}@{1}", h.Name, id.Value) : h.Name;
            }).ToArray());

        }

    }

}
