using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests
{
    public class HXmlDeclarationTest
    {


        [Fact]
        public void TestCreate()
        {
            // Create default 
            var xd = new HXmlDeclaration();
            Assert.Equal("1.0", xd.Version);
            Assert.Equal("utf-8", xd.Encoding);
            Assert.Equal(null, xd.Standalone);

            // Create by elements
            xd = new HXmlDeclaration("version", "encoding", "standalone");
            Assert.Equal("version", xd.Version);
            Assert.Equal("encoding", xd.Encoding);
            Assert.Equal("standalone", xd.Standalone);

        }

        [Fact]
        public void TestCreateFromOther()
        {
            var xd = new HXmlDeclaration("version", "encoding", "standalone");

            var anotherXd = new HXmlDeclaration(xd);
            Assert.Equal("version", anotherXd.Version);
            Assert.Equal("encoding", anotherXd.Encoding);
            Assert.Equal("standalone", anotherXd.Standalone);

            Assert.Throws<ArgumentNullException>(() => new HXmlDeclaration(null));

        }

        [Fact]
        public void TestClone()
        {
            var doc = new HDocument(
                    new HXmlDeclaration("version", "encoding", "standalone"),
                    new HElement("root")
                );

            var newDoc = new HDocument(doc);
            var nodes = newDoc.Nodes().ToArray();
            Assert.IsType<HXmlDeclaration>(nodes[0]);
            var xd = (HXmlDeclaration)nodes[0];
            Assert.Equal("version", xd.Version);
            Assert.Equal("encoding", xd.Encoding);
            Assert.Equal("standalone", xd.Standalone);
        }


    }
}
