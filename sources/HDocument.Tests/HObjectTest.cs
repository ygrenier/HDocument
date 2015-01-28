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
            var oMock = new Mock<HObject>();
            var obj = oMock.Object;

            Assert.Null(obj.Parent);
        }

        [Fact]
        public void TestDocument()
        {
            var oMock = new Mock<HObject>();
            var obj = oMock.Object;

            Assert.Null(obj.Document);
        }

    }

}
