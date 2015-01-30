using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests
{

    public class HElementTest
    {

        [Fact]
        public void TestCreate()
        {
            String name = "test";

            var elm = new HElement(name);
            Assert.Equal(name, elm.Name);

            Assert.Throws<ArgumentNullException>(() => new HElement(" "));
            Assert.Throws<ArgumentNullException>(() => new HElement((String)null));

        }

        [Fact]
        public void TestHasAttributes()
        {
            var elm = new HElement("test");
            Assert.False(elm.HasAttributes);
        }

        [Fact]
        public void TestFirstAttribute()
        {
            var elm = new HElement("test");
            Assert.Null(elm.FirstAttribute);
        }

        [Fact]
        public void TestLastAttribute()
        {
            var elm = new HElement("test");
            Assert.Null(elm.LastAttribute);
        }

    }

}
