﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HDoc.Tests
{
    public class HDocumentTest
    {
        [Fact]
        public void TestCreateEmpty()
        {
            var hDoc = new HDocument();
            Assert.Same(hDoc, hDoc.Document);
            Assert.Null(hDoc.Parent);
            Assert.Null(hDoc.DocumentType);
            Assert.Null(hDoc.Root);
            Assert.Equal(0, hDoc.Nodes().Count());
        }

        [Fact]
        public void TestCreateWithContent()
        {
            var hDoc = new HDocument(new HElement("root"));
            Assert.Same(hDoc, hDoc.Document);
            Assert.Null(hDoc.Parent);
            Assert.Equal(1, hDoc.Nodes().Count());
            Assert.NotNull(hDoc.Root);
        }

        [Fact]
        public void TestCreateFromOther()
        {
            var hDoc = new HDocument("    ");
            Assert.Same(hDoc, hDoc.Document);
            Assert.Null(hDoc.Parent);
            Assert.Equal(1, hDoc.Nodes().Count());

            var nDoc = new HDocument(hDoc);
            Assert.Same(nDoc, nDoc.Document);
            Assert.Null(nDoc.Parent);
            Assert.Equal(1, nDoc.Nodes().Count());

        }

        [Fact]
        public void TestRoot()
        {
            var hDoc = new HDocument();

            Assert.Null(hDoc.Root);

            hDoc.Add("  ");
            Assert.Null(hDoc.Root);

            hDoc.Add(new HText("  "));
            Assert.Null(hDoc.Root);

            var root = new HElement("root");
            hDoc.Add(root);
            Assert.Same(root, hDoc.Root);

        }

        [Fact]
        public void TestDocumentType()
        {
            var hDoc = new HDocument();

            Assert.Null(hDoc.DocumentType);

            hDoc.Add("  ");
            Assert.Null(hDoc.DocumentType);

            hDoc.Add(new HText("  "));
            Assert.Null(hDoc.DocumentType);

            var dt = new HDocumentType();
            hDoc.Add(dt);
            Assert.Same(dt, hDoc.DocumentType);

        }

        [Fact]
        public void TestAdd()
        {
            var hDoc = new HDocument();

            // Can't add CData
            ArgumentException aex = Assert.Throws<ArgumentException>(() => hDoc.Add(new HCData("Content")));
            Assert.Equal("Can't add CData in a document", aex.Message);

            // Can't add document
            aex = Assert.Throws<ArgumentException>(() => hDoc.Add(new HDocument()));
            Assert.Equal("Can't add a document in a document", aex.Message);

            // Can't add non whitespace text
            hDoc.Add("  ");
            aex = Assert.Throws<ArgumentException>(() => hDoc.Add("Content"));
            Assert.Equal("Can't add non whitespace text in a document.", aex.Message);

            Assert.Null(hDoc.DocumentType);
            Assert.Null(hDoc.Root);

            var dt = new HDocumentType();
            hDoc.Add(dt);
            Assert.Same(dt, hDoc.DocumentType);
            Assert.Null(hDoc.Root);

            hDoc.Add("  ");
            Assert.Null(hDoc.Root);

            // Can't add a document type when it's already defined
            aex = Assert.Throws<ArgumentException>(() => hDoc.Add(new HDocumentType()));
            Assert.Equal("Document type is alreay defined.", aex.Message);

            var root = new HElement("root");
            hDoc.Add(root);
            Assert.Same(root, hDoc.Root);

            // Can't add element when Root is already defined
            aex = Assert.Throws<ArgumentException>(() => hDoc.Add(new HElement("other-root")));
            Assert.Equal("Root is already defined.", aex.Message);

            // Can't add a document type after the root
            hDoc = new HDocument(new HElement("root"));
            aex = Assert.Throws<ArgumentException>(() => hDoc.Add(new HDocumentType()));
            Assert.Equal("Can't add a document type after the root node.", aex.Message);

        }

    }
}
