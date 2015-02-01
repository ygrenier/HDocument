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

        [Fact]
        public void TestHtmlDecode()
        {
            Assert.Equal(null, HEntity.HtmlDecode(null));
            Assert.Equal("", HEntity.HtmlDecode(""));
            Assert.Equal(" \t ", HEntity.HtmlDecode(" \t "));

            Assert.Equal("test", HEntity.HtmlDecode("test"));

            // named entities
            Assert.Equal("&", HEntity.HtmlDecode("&amp;"));
            Assert.Equal("&test", HEntity.HtmlDecode("&amp;test"));
            Assert.Equal("test&", HEntity.HtmlDecode("test&amp;"));
            Assert.Equal("te&st", HEntity.HtmlDecode("te&amp;st"));

            // Unknown entity
            Assert.Equal("&test;", HEntity.HtmlDecode("&test;"));

            // Numeric entities
            Assert.Equal("&", HEntity.HtmlDecode("&#X26;"));
            Assert.Equal("&test", HEntity.HtmlDecode("&#x26;test"));
            Assert.Equal("test&", HEntity.HtmlDecode("test&#38;"));
            Assert.Equal("te&st", HEntity.HtmlDecode("te&#38;st"));

            // Invalid numeric
            Assert.Equal("&#X2Z6;", HEntity.HtmlDecode("&#X2Z6;"));
            Assert.Equal("&#3d8;test", HEntity.HtmlDecode("&#3d8;test"));

            // Non closed entities
            Assert.Equal("test&amp test&test", HEntity.HtmlDecode("test&amp test&amp;test"));

        }

    }
}
