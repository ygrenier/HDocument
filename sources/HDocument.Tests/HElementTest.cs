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
            Assert.Throws<ArgumentNullException>(() => new HElement(null));

        }

    }

}
