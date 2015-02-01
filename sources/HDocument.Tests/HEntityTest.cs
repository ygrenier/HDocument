using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests
{
    public class HEntityTest
    {

        [Fact]
        public void TestFindByName()
        {
            var entity = HEntity.FindEntityByName("amp");
            Assert.Equal("amp", entity.Name);

            entity = HEntity.FindEntityByName("acE");
            Assert.Equal("acE", entity.Name);

            Assert.Null(HEntity.FindEntityByName("unknown"));
            Assert.Null(HEntity.FindEntityByName(" "));
            Assert.Null(HEntity.FindEntityByName(""));
            Assert.Null(HEntity.FindEntityByName(null));

            foreach (var ent in HEntity.GetEntities())
            {
                entity = HEntity.FindEntityByName(ent.Name);
                Assert.Same(ent, entity);
            }
        }

        [Fact]
        public void TestFindByChars_String()
        {
            var entity = HEntity.FindEntityByChars("&");
            Assert.Equal("amp", entity.Name);

            entity = HEntity.FindEntityByChars("\u223E\u0333");
            Assert.Equal("acE", entity.Name);

            Assert.Null(HEntity.FindEntityByChars("unknown"));
            Assert.Null(HEntity.FindEntityByChars(" "));
            Assert.Null(HEntity.FindEntityByChars(""));
            Assert.Null(HEntity.FindEntityByChars((String)null));

            foreach (var ent in HEntity.GetEntities())
            {
                entity = HEntity.FindEntityByChars(ent.ToString());
                //Assert.Equal(ent.Name, entity.Name);
                Assert.Equal(ent.Characters, entity.Characters);
            }
        }

        [Fact]
        public void TestFindByChars_Chars()
        {
            var entity = HEntity.FindEntityByChars('&');
            Assert.Equal("amp", entity.Name);

            entity = HEntity.FindEntityByChars('\u223E', '\u0333');
            Assert.Equal("acE", entity.Name);

            Assert.Null(HEntity.FindEntityByChars('a'));
            Assert.Null(HEntity.FindEntityByChars('&', '&'));
            Assert.Null(HEntity.FindEntityByChars('a', '&'));
            Assert.Null(HEntity.FindEntityByChars('\u223E', 'a'));
            Assert.Null(HEntity.FindEntityByChars('\u223E', '\u0333', 'a'));
            Assert.Null(HEntity.FindEntityByChars((char[])null));

            foreach (var ent in HEntity.GetEntities())
            {
                entity = HEntity.FindEntityByChars(ent.ToString());
                //Assert.Equal(ent.Name, entity.Name);
                Assert.Equal(ent.Characters, entity.Characters);
            }
        }

    }
}
