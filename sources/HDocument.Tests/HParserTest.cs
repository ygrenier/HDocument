using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests
{
    public class HParserTest
    {

        [Fact]
        public void TestCreate()
        {
            StringReader reader = new StringReader("<h1></h1>");
            HParser parser = new HParser(reader);
            Assert.Same(reader, parser.Reader);
            Assert.Null(parser.LastParsed);
            Assert.False(parser.EOF);

            Assert.Throws<ArgumentNullException>(() => new HParser(null));
        }

        [Fact]
        public void TestParse_Text()
        {
            StringReader reader = new StringReader("Content");
            HParser parser = new HParser(reader);

            var pres = parser.Parse();
            Assert.Same(pres, parser.LastParsed);
            Assert.IsType<HParser.ParsedText>(pres);
            Assert.Equal(HParser.ParsedTokenType.Text, pres.TokenType);
            Assert.Equal("Content", ((HParser.ParsedContent)pres).Text);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);
        }

        [Fact]
        public void TestParse_Comment()
        {
            StringReader reader = new StringReader("Start<!-- Comments -->End");
            HParser parser = new HParser(reader);

            var pres = parser.Parse();
            Assert.Same(pres, parser.LastParsed);
            Assert.IsType<HParser.ParsedText>(pres);
            Assert.Equal(HParser.ParsedTokenType.Text, pres.TokenType);
            Assert.Equal("Start", ((HParser.ParsedContent)pres).Text);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedComment>(pres);
            Assert.Equal(HParser.ParsedTokenType.Comment, pres.TokenType);
            Assert.Equal("Comments", ((HParser.ParsedContent)pres).Text);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedText>(pres);
            Assert.Equal(HParser.ParsedTokenType.Text, pres.TokenType);
            Assert.Equal("End", ((HParser.ParsedContent)pres).Text);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Comments with false end
            reader = new StringReader("Start<!-- Comments with -- as text and false -> end --->End");
            parser = new HParser(reader);

            pres = parser.Parse();  // Pass Start text
            pres = parser.Parse();
            Assert.IsType<HParser.ParsedComment>(pres);
            Assert.Equal(HParser.ParsedTokenType.Comment, pres.TokenType);
            Assert.Equal("Comments with -- as text and false -> end -", ((HParser.ParsedContent)pres).Text);

            pres = parser.Parse();
            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Comments non closed
            reader = new StringReader("Start<!-- Comments non closed ->End");
            parser = new HParser(reader);

            pres = parser.Parse();  // Pass Start text
            // Second parse failed
            var pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("End of file unexpected, comment not closed.", pex.Message);
            // The next parse returns the comment non closed
            pres = parser.Parse();
            Assert.IsType<HParser.ParsedComment>(pres);
            Assert.Equal(HParser.ParsedTokenType.Comment, pres.TokenType);
            Assert.Equal("Comments non closed ->End", ((HParser.ParsedContent)pres).Text);

            pres = parser.Parse();
            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Invalid start comments
            reader = new StringReader("Start<!- Invalid stat comment -->End");
            parser = new HParser(reader);

            pres = parser.Parse();  // Pass Start text
            // Second parse failed
            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Comments need to start with '<!--'.", pex.Message);

            // The next parse returns false start comment as text
            pres = parser.Parse();
            Assert.IsType<HParser.ParsedText>(pres);
            Assert.Equal(HParser.ParsedTokenType.Text, pres.TokenType);
            Assert.Equal("<!- ", ((HParser.ParsedContent)pres).Text);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedText>(pres);
            Assert.Equal(HParser.ParsedTokenType.Text, pres.TokenType);
            Assert.Equal("Invalid stat comment -->End", ((HParser.ParsedContent)pres).Text);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

        }

        [Fact]
        public void TestParse_Tag()
        {
            // Open tag
            StringReader reader = new StringReader("<div>");
            HParser parser = new HParser(reader);

            var pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.CloseTag, pres.TokenType);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // With spaces
            reader = new StringReader("<  div  >");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.CloseTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Auto closed tag
            reader = new StringReader("<div />");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.AutoClosedTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Auto closed tag with spaces
            reader = new StringReader("<  div />");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.AutoClosedTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Invalid char after open a tag
            reader = new StringReader("<  >");
            parser = new HParser(reader);

            var pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Invalid tag name. Need to start with an alphanumeric", pex.Message);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedText>(pres);
            Assert.Equal(HParser.ParsedTokenType.Text, pres.TokenType);
            Assert.Equal("<  >", ((HParser.ParsedContent)pres).Text);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // End of file
            reader = new StringReader("<  div  ");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Unexpected end of file. Tag not closed.", pex.Message);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // End of file
            reader = new StringReader("<  div");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("End of file unexpected.", pex.Message);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Auto closed tag with spaces at the end
            reader = new StringReader("<div / >");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Invalid auto closed tag, '/' need to be follow by '>'.", pex.Message);
            
            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.AutoClosedTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Auto closed tag with invalid char at the end
            reader = new StringReader("<div /?");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Invalid char after '/'. End of auto closed tag expected.", pex.Message);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.AutoClosedTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Auto closed tag with end of file at the end
            reader = new StringReader("<div /");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Invalid char after '/'. End of auto closed tag expected.", pex.Message);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.AutoClosedTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

        }

        [Fact]
        public void TestParse_TagAttribute()
        {
            // Attributes
            StringReader reader = new StringReader("<div attr1  attr2 = value2 attr3=\"val&amp;ue\" attr4 = 'value' >");
            HParser parser = new HParser(reader);

            var pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedAttribute>(pres);
            HParser.ParsedAttribute pAttr = (HParser.ParsedAttribute)pres;
            Assert.Equal("attr1", pAttr.Name);
            Assert.Equal(null, pAttr.Value);
            Assert.Equal('\0', pAttr.Quote);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedAttribute>(pres);
            pAttr = (HParser.ParsedAttribute)pres;
            Assert.Equal("attr2", pAttr.Name);
            Assert.Equal("value2", pAttr.Value);
            Assert.Equal('\0', pAttr.Quote);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedAttribute>(pres);
            pAttr = (HParser.ParsedAttribute)pres;
            Assert.Equal("attr3", pAttr.Name);
            Assert.Equal("val&ue", pAttr.Value);
            Assert.Equal('"', pAttr.Quote);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedAttribute>(pres);
            pAttr = (HParser.ParsedAttribute)pres;
            Assert.Equal("attr4", pAttr.Name);
            Assert.Equal("value", pAttr.Value);
            Assert.Equal('\'', pAttr.Quote);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.CloseTag, pres.TokenType);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Attribute without value
            reader = new StringReader("<div attr1 = >");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            var pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Attribute value expected.", pex.Message);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedAttribute>(pres);
            pAttr = (HParser.ParsedAttribute)pres;
            Assert.Equal("attr1", pAttr.Name);
            Assert.Equal(null, pAttr.Value);
            Assert.Equal('\0', pAttr.Quote);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.CloseTag, pres.TokenType);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Attribute with value unclosed
            reader = new StringReader("<div attr1 = \"value ><div attr2=\"value\" >");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedAttribute>(pres);
            pAttr = (HParser.ParsedAttribute)pres;
            Assert.Equal("attr1", pAttr.Name);
            Assert.Equal("value ><div attr2=", pAttr.Value);
            Assert.Equal('"', pAttr.Quote);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedAttribute>(pres);
            pAttr = (HParser.ParsedAttribute)pres;
            Assert.Equal("value", pAttr.Name);
            Assert.Equal(null, pAttr.Value);
            Assert.Equal('\0', pAttr.Quote);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Unexpected character.", pex.Message);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.CloseTag, pres.TokenType);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Attribute with end of file
            reader = new StringReader("<div attr1 = \"value ");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Unexpected end of file. Attribute is not closed.", pex.Message);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedAttribute>(pres);
            pAttr = (HParser.ParsedAttribute)pres;
            Assert.Equal("attr1", pAttr.Name);
            Assert.Equal("value ", pAttr.Value);
            Assert.Equal('"', pAttr.Quote);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("End of file unexpected.", pex.Message);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Invalid Attribute name
            reader = new StringReader("<div ' >");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Unexpected character.", pex.Message);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.CloseTag, pres.TokenType);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

        }

        [Fact]
        public void TestParse_EndTag()
        {
            // Normal
            StringReader reader = new StringReader("</div>");
            HParser parser = new HParser(reader);

            var pres = parser.Parse();
            Assert.Same(pres, parser.LastParsed);
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.EndTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // With spaces
            reader = new StringReader("</  div  >");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.Same(pres, parser.LastParsed);
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.EndTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // With attribute : Failed
            reader = new StringReader("</div attr=val>");
            parser = new HParser(reader);

            var pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("End tag can't contains attribute.", pex.Message);

            pres = parser.Parse();
            Assert.Same(pres, parser.LastParsed);
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.EndTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Invalid char
            reader = new StringReader("</div '>");
            parser = new HParser(reader);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Unexpected char. End tag not closed.", pex.Message);

            pres = parser.Parse();
            Assert.Same(pres, parser.LastParsed);
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.EndTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // End of stream
            reader = new StringReader("</div ");
            parser = new HParser(reader);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Unexpected end of stream.", pex.Message);

            pres = parser.Parse();
            Assert.Same(pres, parser.LastParsed);
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.EndTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Not close
            reader = new StringReader("</div <div>");
            parser = new HParser(reader);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Unexpected char. End tag not closed.", pex.Message);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.EndTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.CloseTag, pres.TokenType);
            Assert.Equal("div", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

        }

        [Fact]
        public void TestParse_ProcessInstruction()
        {
            StringReader reader = new StringReader("<? xml ?>");
            HParser parser = new HParser(reader);

            var pres = parser.Parse();
            Assert.Same(pres, parser.LastParsed);
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.OpenProcessInstruction, pres.TokenType);
            Assert.Equal("xml", ((HParser.ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.IsType<HParser.ParsedTag>(pres);
            Assert.Equal(HParser.ParsedTokenType.CloseProcessInstruction, pres.TokenType);

            pres = parser.Parse();
            Assert.Null(pres);
            Assert.True(parser.EOF);

        }

    }
}
