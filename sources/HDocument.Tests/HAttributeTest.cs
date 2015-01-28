using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests
{
    public class HAttributeTest
    {

        [Fact]
        public void TestCreate()
        {
            var attr = new HAttribute();
            Assert.Null(attr.Name);
            Assert.Null(attr.Value);
        }

        [Fact]
        public void TestValue()
        {
            var attr = new HAttribute();
            
            Assert.Null(attr.Value);

            String value = "test";
            attr.Value = value;
            Assert.Equal(value, attr.Value);

            value = "";
            attr.Value = value;
            Assert.Equal(value, attr.Value);

            Assert.Throws<ArgumentNullException>(() => attr.Value = null);

        }

    }
}
