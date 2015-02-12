using HDoc.Parser;
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
            Assert.Equal(new ParsePosition(0, 0, 0), parser.ReadPosition);
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
            Assert.Equal(new ParsePosition(), pres.Position);
            Assert.Equal(new ParsePosition(7, 0, 7), parser.ReadPosition);
            Assert.IsType<ParsedText>(pres);
            Assert.Equal(ParsedTokenType.Text, pres.TokenType);
            Assert.Equal("Content", ((ParsedContent)pres).Text);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(7, 0, 7), parser.ReadPosition);
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
            Assert.Equal(new ParsePosition(), pres.Position);
            Assert.Equal(new ParsePosition(5, 0, 5), parser.ReadPosition);
            Assert.IsType<ParsedText>(pres);
            Assert.Equal(ParsedTokenType.Text, pres.TokenType);
            Assert.Equal("Start", ((ParsedContent)pres).Text);

            pres = parser.Parse();
            Assert.IsType<ParsedComment>(pres);
            Assert.Equal(new ParsePosition(5, 0, 5), pres.Position);
            Assert.Equal(new ParsePosition(22, 0, 22), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.Comment, pres.TokenType);
            Assert.Equal("Comments", ((ParsedContent)pres).Text);

            pres = parser.Parse();
            Assert.IsType<ParsedText>(pres);
            Assert.Equal(new ParsePosition(22, 0, 22), pres.Position);
            Assert.Equal(new ParsePosition(25, 0, 25), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.Text, pres.TokenType);
            Assert.Equal("End", ((ParsedContent)pres).Text);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(25, 0, 25), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Comments with false end
            reader = new StringReader("Start<!-- Comments with -- as text \n and false -> end --->End");
            parser = new HParser(reader);

            pres = parser.Parse();  // Pass Start text
            pres = parser.Parse();
            Assert.IsType<ParsedComment>(pres);
            Assert.Equal(new ParsePosition(5, 0, 5), pres.Position);
            Assert.Equal(new ParsePosition(58, 1, 22), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.Comment, pres.TokenType);
            Assert.Equal("Comments with -- as text \n and false -> end -", ((ParsedContent)pres).Text);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(58, 1, 22), pres.Position);
            Assert.Equal(new ParsePosition(61, 1, 25), parser.ReadPosition);
            pres = parser.Parse();
            Assert.Equal(new ParsePosition(61, 1, 25), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Comments non closed
            reader = new StringReader("Start<!-- Comments non closed ->End");
            parser = new HParser(reader);

            pres = parser.Parse();  // Pass Start text
            // Second parse failed
            var pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("End of file unexpected, comment not closed.", pex.Message);
            Assert.Equal(new ParsePosition(35, 0, 35), pex.Position);

            // The next parse returns the comment non closed
            pres = parser.Parse();
            Assert.IsType<ParsedComment>(pres);
            Assert.Equal(new ParsePosition(5, 0, 5), pres.Position);
            Assert.Equal(new ParsePosition(35, 0, 35), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.Comment, pres.TokenType);
            Assert.Equal("Comments non closed ->End", ((ParsedContent)pres).Text);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(35, 0, 35), parser.ReadPosition);
            pres = parser.Parse();
            Assert.Equal(new ParsePosition(35, 0, 35), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Invalid start comments
            reader = new StringReader("Start<!- Invalid stat comment -->End");
            parser = new HParser(reader);

            pres = parser.Parse();  // Pass Start text
            // Second parse failed
            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Comments need to start with '<!--'.", pex.Message);
            Assert.Equal(new ParsePosition(9, 0, 9), pex.Position);

            // The next parse returns false start comment as text
            pres = parser.Parse();
            Assert.IsType<ParsedText>(pres);
            Assert.Equal(new ParsePosition(5, 0, 5), pres.Position);
            Assert.Equal(new ParsePosition(9, 0, 9), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.Text, pres.TokenType);
            Assert.Equal("<!- ", ((ParsedContent)pres).Text);

            pres = parser.Parse();
            Assert.IsType<ParsedText>(pres);
            Assert.Equal(new ParsePosition(9, 0, 9), pres.Position);
            Assert.Equal(new ParsePosition(36, 0, 36), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.Text, pres.TokenType);
            Assert.Equal("Invalid stat comment -->End", ((ParsedContent)pres).Text);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(36, 0, 36), parser.ReadPosition);
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
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(4, 0, 4), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(4, 0, 4), pres.Position);
            Assert.Equal(new ParsePosition(5, 0, 5), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.CloseTag, pres.TokenType);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(5, 0, 5), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // With spaces
            reader = new StringReader("<  div  >");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(6, 0, 6), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(8, 0, 8), pres.Position);
            Assert.Equal(new ParsePosition(9, 0, 9), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.CloseTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(9, 0, 9), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Auto closed tag
            reader = new StringReader("<div />");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(4, 0, 4), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(5, 0, 5), pres.Position);
            Assert.Equal(new ParsePosition(7, 0, 7), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.AutoClosedTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(7, 0, 7), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Auto closed tag with spaces
            reader = new StringReader("<  div />");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(6, 0, 6), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(7, 0, 7), pres.Position);
            Assert.Equal(new ParsePosition(9, 0, 9), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.AutoClosedTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(9, 0, 9), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Invalid char after open a tag
            reader = new StringReader("<  >");
            parser = new HParser(reader);

            var pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Invalid tag name. Need to start with an alphanumeric", pex.Message);
            Assert.Equal(new ParsePosition(4, 0, 4), pex.Position);

            pres = parser.Parse();
            Assert.IsType<ParsedText>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(4, 0, 4), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.Text, pres.TokenType);
            Assert.Equal("<  >", ((ParsedContent)pres).Text);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(4, 0, 4), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // End of file
            reader = new StringReader("<  div  ");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(6, 0, 6), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Unexpected end of file. Tag not closed.", pex.Message);
            Assert.Equal(new ParsePosition(8, 0, 8), pex.Position);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(8, 0, 8), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // End of file
            reader = new StringReader("<  div");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(6, 0, 6), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("End of file unexpected.", pex.Message);
            Assert.Equal(new ParsePosition(6, 0, 6), pex.Position);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(6, 0, 6), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Auto closed tag with spaces at the end
            reader = new StringReader("<div / >");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(4, 0, 4), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Invalid auto closed tag, '/' need to be follow by '>'.", pex.Message);
            Assert.Equal(new ParsePosition(5, 0, 5), pex.Position);
            
            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(5, 0, 5), pres.Position);
            Assert.Equal(new ParsePosition(8, 0, 8), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.AutoClosedTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(8, 0, 8), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Auto closed tag with invalid char at the end
            reader = new StringReader("<div /?");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(4, 0, 4), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Invalid char after '/'. End of auto closed tag expected.", pex.Message);
            Assert.Equal(new ParsePosition(5, 0, 5), pex.Position);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(5, 0, 5), pres.Position);
            Assert.Equal(new ParsePosition(7, 0, 7), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.AutoClosedTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(7, 0, 7), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Auto closed tag with end of file at the end
            reader = new StringReader("<div /");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(4, 0, 4), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Invalid char after '/'. End of auto closed tag expected.", pex.Message);
            Assert.Equal(new ParsePosition(5, 0, 5), pex.Position);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(5, 0, 5), pres.Position);
            Assert.Equal(new ParsePosition(6, 0, 6), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.AutoClosedTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(6, 0, 6), parser.ReadPosition);
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
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(4, 0, 4), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.IsType<ParsedAttribute>(pres);
            Assert.Equal(new ParsePosition(5, 0, 5), pres.Position);
            Assert.Equal(new ParsePosition(12, 0, 12), parser.ReadPosition);
            ParsedAttribute pAttr = (ParsedAttribute)pres;
            Assert.Equal("attr1", pAttr.Name);
            Assert.Equal(null, pAttr.Value);
            Assert.Equal('\0', pAttr.Quote);

            pres = parser.Parse();
            Assert.IsType<ParsedAttribute>(pres);
            Assert.Equal(new ParsePosition(12, 0, 12), pres.Position);
            Assert.Equal(new ParsePosition(26, 0, 26), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.Attribute, pres.TokenType);
            pAttr = (ParsedAttribute)pres;
            Assert.Equal("attr2", pAttr.Name);
            Assert.Equal("value2", pAttr.Value);
            Assert.Equal('\0', pAttr.Quote);

            pres = parser.Parse();
            Assert.IsType<ParsedAttribute>(pres);
            Assert.Equal(new ParsePosition(27, 0, 27), pres.Position);
            Assert.Equal(new ParsePosition(45, 0, 45), parser.ReadPosition);
            pAttr = (ParsedAttribute)pres;
            Assert.Equal("attr3", pAttr.Name);
            Assert.Equal("val&ue", pAttr.Value);
            Assert.Equal('"', pAttr.Quote);

            pres = parser.Parse();
            Assert.IsType<ParsedAttribute>(pres);
            Assert.Equal(new ParsePosition(46, 0, 46), pres.Position);
            Assert.Equal(new ParsePosition(61, 0, 61), parser.ReadPosition);
            pAttr = (ParsedAttribute)pres;
            Assert.Equal("attr4", pAttr.Name);
            Assert.Equal("value", pAttr.Value);
            Assert.Equal('\'', pAttr.Quote);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(62, 0, 62), pres.Position);
            Assert.Equal(new ParsePosition(63, 0, 63), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.CloseTag, pres.TokenType);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(63, 0, 63), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Attribute without value
            reader = new StringReader("<div attr1 = >");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(4, 0, 4), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            var pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Attribute value expected.", pex.Message);
            Assert.Equal(new ParsePosition(13, 0, 13), pex.Position);

            pres = parser.Parse();
            Assert.IsType<ParsedAttribute>(pres);
            Assert.Equal(new ParsePosition(5, 0, 5), pres.Position);
            Assert.Equal(new ParsePosition(13, 0, 13), parser.ReadPosition);
            pAttr = (ParsedAttribute)pres;
            Assert.Equal("attr1", pAttr.Name);
            Assert.Equal(null, pAttr.Value);
            Assert.Equal('\0', pAttr.Quote);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(13, 0, 13), pres.Position);
            Assert.Equal(new ParsePosition(14, 0, 14), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.CloseTag, pres.TokenType);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(14, 0, 14), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Attribute with value unclosed
            reader = new StringReader("<div attr1 = \"value ><div attr2=\"value\" >");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(4, 0, 4), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.IsType<ParsedAttribute>(pres);
            Assert.Equal(new ParsePosition(5, 0, 5), pres.Position);
            Assert.Equal(new ParsePosition(33, 0, 33), parser.ReadPosition);
            pAttr = (ParsedAttribute)pres;
            Assert.Equal("attr1", pAttr.Name);
            Assert.Equal("value ><div attr2=", pAttr.Value);
            Assert.Equal('"', pAttr.Quote);

            pres = parser.Parse();
            Assert.IsType<ParsedAttribute>(pres);
            Assert.Equal(new ParsePosition(33, 0, 33), pres.Position);
            Assert.Equal(new ParsePosition(38, 0, 38), parser.ReadPosition);
            pAttr = (ParsedAttribute)pres;
            Assert.Equal("value", pAttr.Name);
            Assert.Equal(null, pAttr.Value);
            Assert.Equal('\0', pAttr.Quote);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Unexpected character.", pex.Message);
            Assert.Equal(new ParsePosition(38, 0, 38), pex.Position);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(40, 0, 40), pres.Position);
            Assert.Equal(new ParsePosition(41, 0, 41), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.CloseTag, pres.TokenType);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(41, 0, 41), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Attribute with end of file
            reader = new StringReader("<div attr1 = \"value ");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(4, 0, 4), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Unexpected end of file. Attribute is not closed.", pex.Message);
            Assert.Equal(new ParsePosition(20, 0, 20), pex.Position);

            pres = parser.Parse();
            Assert.IsType<ParsedAttribute>(pres);
            Assert.Equal(new ParsePosition(5, 0, 5), pres.Position);
            Assert.Equal(new ParsePosition(20, 0, 20), parser.ReadPosition);
            pAttr = (ParsedAttribute)pres;
            Assert.Equal("attr1", pAttr.Name);
            Assert.Equal("value ", pAttr.Value);
            Assert.Equal('"', pAttr.Quote);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("End of file unexpected.", pex.Message);
            Assert.Equal(new ParsePosition(20, 0, 20), pex.Position);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(20, 0, 20), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Invalid Attribute name
            reader = new StringReader("<div ' >");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(4, 0, 4), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Unexpected character.", pex.Message);
            Assert.Equal(new ParsePosition(5, 0, 5), pex.Position);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(7, 0, 7), pres.Position);
            Assert.Equal(new ParsePosition(8, 0, 8), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.CloseTag, pres.TokenType);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(8, 0, 8), parser.ReadPosition);
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
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(6, 0, 6), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.EndTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(6, 0, 6), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // With spaces
            reader = new StringReader("</  div  >");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.Same(pres, parser.LastParsed);
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(10, 0, 10), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.EndTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(10, 0, 10), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // With attribute : Failed
            reader = new StringReader("</div attr=val>");
            parser = new HParser(reader);

            var pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("End tag can't contains attribute.", pex.Message);
            Assert.Equal(new ParsePosition(7, 0, 7), pex.Position);

            pres = parser.Parse();
            Assert.Same(pres, parser.LastParsed);
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(15, 0, 15), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.EndTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(15, 0, 15), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Invalid char
            reader = new StringReader("</div '>");
            parser = new HParser(reader);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Unexpected char. End tag not closed.", pex.Message);
            Assert.Equal(new ParsePosition(7, 0, 7), pex.Position);

            pres = parser.Parse();
            Assert.Same(pres, parser.LastParsed);
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(8, 0, 8), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.EndTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(8, 0, 8), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // End of stream
            reader = new StringReader("</div ");
            parser = new HParser(reader);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Unexpected end of stream.", pex.Message);
            Assert.Equal(new ParsePosition(6, 0, 6), pex.Position);

            pres = parser.Parse();
            Assert.Same(pres, parser.LastParsed);
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(6, 0, 6), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.EndTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(6, 0, 6), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Not close
            reader = new StringReader("</div <div>");
            parser = new HParser(reader);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("Unexpected char. End tag not closed.", pex.Message);
            Assert.Equal(new ParsePosition(7, 0, 7), pex.Position);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(6, 0, 6), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.EndTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(6, 0, 6), pres.Position);
            Assert.Equal(new ParsePosition(10, 0, 10), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.OpenTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(10, 0, 10), pres.Position);
            Assert.Equal(new ParsePosition(11, 0, 11), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.CloseTag, pres.TokenType);
            Assert.Equal("div", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(11, 0, 11), parser.ReadPosition);
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
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(6, 0, 6), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.OpenProcessInstruction, pres.TokenType);
            Assert.Equal("xml", ((ParsedTag)pres).TagName);

            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(7, 0, 7), pres.Position);
            Assert.Equal(new ParsePosition(9, 0, 9), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.CloseProcessInstruction, pres.TokenType);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(9, 0, 9), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

        }

        [Fact]
        public void TestParse_Doctype()
        {
            // HTML 5
            StringReader reader = new StringReader("<!DOCTYPE html>");
            HParser parser = new HParser(reader);

            var pres = parser.Parse();
            Assert.Same(pres, parser.LastParsed);
            Assert.IsType<ParsedDoctype>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(15, 0, 15), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.Doctype, pres.TokenType);
            var dt = (ParsedDoctype)pres;
            Assert.Equal(1, dt.Values.Length);
            Assert.Equal(new string[] { "html" }, dt.Values);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(15, 0, 15), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // Full doctype
            reader = new StringReader("<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.0//EN\" \"http://www.w3.org/TR/REC-html40/strict.dtd\"  >");
            parser = new HParser(reader);

            pres = parser.Parse();
            Assert.Same(pres, parser.LastParsed);
            Assert.IsType<ParsedDoctype>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(96, 0, 96), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.Doctype, pres.TokenType);
            dt = (ParsedDoctype)pres;
            Assert.Equal(4, dt.Values.Length);
            Assert.Equal(new string[] { "html", "PUBLIC", "-//W3C//DTD HTML 4.0//EN", "http://www.w3.org/TR/REC-html40/strict.dtd" }, dt.Values);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(96, 0, 96), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

        }

        [Fact]
        public void TestParse_Doctype_InvalidDoctypeTag()
        {
            // Invalid doctype
            var reader = new StringReader("<!DOCTPE html PUBLIC \"-//W3C//DTD HTML 4.0//EN\" \"http://www.w3.org/TR/REC-html40/strict.dtd\"  >");
            var parser = new HParser(reader);

            var pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("DOCTYPE expected.", pex.Message);
            Assert.Equal(new ParsePosition(0, 0, 0), pex.Position);

            var pres = parser.Parse();
            Assert.Same(pres, parser.LastParsed);
            Assert.IsType<ParsedText>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(9, 0, 9), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.Text, pres.TokenType);
            Assert.Equal("<!DDOCTPE ", ((ParsedText)pres).Text);

            pres = parser.Parse();
            Assert.Same(pres, parser.LastParsed);
            Assert.IsType<ParsedText>(pres);
            Assert.Equal(new ParsePosition(9, 0, 9), pres.Position);
            Assert.Equal(new ParsePosition(95, 0, 95), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.Text, pres.TokenType);
            Assert.Equal("html PUBLIC \"-//W3C//DTD HTML 4.0//EN\" \"http://www.w3.org/TR/REC-html40/strict.dtd\"  >", ((ParsedText)pres).Text);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(95, 0, 95), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

        }

        [Fact]
        public void TestParse_Doctype_EOF()
        {
            var reader = new StringReader("<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.0//EN\" \"http://www.w3.org/TR/REC-html40/strict.dtd\"  ");
            var parser = new HParser(reader);

            var pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("End of file unexpected, doctype not closed.", pex.Message);
            Assert.Equal(new ParsePosition(95, 0, 95), pex.Position);

            var pres = parser.Parse();
            Assert.Same(pres, parser.LastParsed);
            Assert.IsType<ParsedDoctype>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(95, 0, 95), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.Doctype, pres.TokenType);
            var dt = (ParsedDoctype)pres;
            Assert.Equal(4, dt.Values.Length);
            Assert.Equal(new string[] { "html", "PUBLIC", "-//W3C//DTD HTML 4.0//EN", "http://www.w3.org/TR/REC-html40/strict.dtd" }, dt.Values);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(95, 0, 95), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

            // EOF in a value
            reader = new StringReader("<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.0//EN\" \"http://www.w3.org/TR/REC-html40/strict");
            parser = new HParser(reader);

            pex = Assert.Throws<ParseError>(() => parser.Parse());
            Assert.Equal("End of file unexpected, doctype not closed.", pex.Message);
            Assert.Equal(new ParsePosition(88, 0, 88), pex.Position);

            pres = parser.Parse();
            Assert.Same(pres, parser.LastParsed);
            Assert.IsType<ParsedDoctype>(pres);
            Assert.Equal(new ParsePosition(0, 0, 0), pres.Position);
            Assert.Equal(new ParsePosition(88, 0, 88), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.Doctype, pres.TokenType);
            dt = (ParsedDoctype)pres;
            Assert.Equal(4, dt.Values.Length);
            Assert.Equal(new string[] { "html", "PUBLIC", "-//W3C//DTD HTML 4.0//EN", "http://www.w3.org/TR/REC-html40/strict" }, dt.Values);

            pres = parser.Parse();
            Assert.Equal(new ParsePosition(88, 0, 88), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);
        }

        [Fact]
        public void TestParseContentText()
        {
            StringReader reader = new StringReader(@"
$('<div></div>').append();
</script>
</body>
</html>
");
            HParser parser = new HParser(reader);

            var tres = parser.ParseContentText("script");
            Assert.Same(tres, parser.LastParsed);
            Assert.Equal(new ParsePosition(), tres.Position);
            Assert.Equal(new ParsePosition(38, 2, 8), parser.ReadPosition);
            Assert.Equal("\r\n$('<div></div>').append();\r\n", tres.Text);

            var pres = parser.Parse();
            Assert.Same(pres, parser.LastParsed);
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(30, 2, 0), pres.Position);
            Assert.Equal(new ParsePosition(38, 2, 8), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.EndTag, pres.TokenType);
            Assert.Equal("script", ((ParsedTag)pres).TagName);

            pres = parser.Parse();  // Pass text
            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(41, 3, 0), pres.Position);
            Assert.Equal(new ParsePosition(48, 3, 7), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.EndTag, pres.TokenType);
            Assert.Equal("body", ((ParsedTag)pres).TagName);

            pres = parser.Parse();  // Pass text
            pres = parser.Parse();
            Assert.IsType<ParsedTag>(pres);
            Assert.Equal(new ParsePosition(50, 4, 0), pres.Position);
            Assert.Equal(new ParsePosition(57, 4, 7), parser.ReadPosition);
            Assert.Equal(ParsedTokenType.EndTag, pres.TokenType);
            Assert.Equal("html", ((ParsedTag)pres).TagName);

            pres = parser.Parse();  // Pass text
            pres = parser.Parse();
            Assert.Equal(new ParsePosition(59, 5, 0), parser.ReadPosition);
            Assert.Null(pres);
            Assert.True(parser.EOF);

        }

    }
}
