﻿using NBi.Core.Hierarchical;
using NBi.Core.Hierarchical.Json;
using NBi.Core.Scalar.Resolver;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NBi.Testing.Core.Hierarchical.Json
{
    public class JsonPathFileEngineTest
    {
        private class JsonPathStreamEngine : JsonPathFileEngine
        {
            private StreamReader StreamReader { get; }

            protected internal JsonPathStreamEngine(StreamReader streamReader, string from, IEnumerable<ElementSelect> selects)
                : base(new LiteralScalarResolver<string>(string.Empty), string.Empty, from, selects)
                => StreamReader = streamReader;

            protected override TextReader GetTextReader(string filePath)
                => StreamReader;

            protected override string EnsureFileExist()
                => string.Empty;
        }

        protected StreamReader GetResourceReader(string filename)
        {
            // A Stream is needed to read the XML document.
            var stream = Assembly.GetExecutingAssembly()
                                           .GetManifestResourceStream($"{GetType().Namespace}.Resources.{filename}.json");
            var reader = new StreamReader(stream);
            return reader;
        }


        [Test]
        [TestCase("$.PurchaseOrders[*].Items[*]", 5)]
        [TestCase("$.PurchaseOrders[*]", 4)]
        public void Execute_Example_RowCount(string from, int rowCount)
        {
            var selects = new List<ElementSelect>()
            {
                new ElementSelect("$")
            };

            using (var reader = GetResourceReader("PurchaseOrders"))
            {
                var engine = new JsonPathStreamEngine(reader, from, selects);
                var result = engine.Execute();
                Assert.That(result.Count, Is.EqualTo(rowCount));
            }
        }

        [Test]
        public void Execute_Example_FirstColumnIsCorrect()
        {
            var from = "$.PurchaseOrders[*].Items[*]";
            var selects = new List<ElementSelect>()
            {
                new ElementSelect("PartNumber")
            };

            using (var reader = GetResourceReader("PurchaseOrders"))
            {
                var engine = new JsonPathStreamEngine(reader, from, selects);
                var result = engine.Execute();
                Assert.That(result.Count, Is.EqualTo(5));
                Assert.That(result.Select(x => ((x as IEnumerable<object>).ElementAt(0) as string).Length), Is.All.EqualTo(6)); //Format is 123-XY
            }
        }

        [Test]
        public void Execute_Example_AllColumnsAreCorrect()
        {
            var from = "$.PurchaseOrders[*].Items[*]";
            var selects = new List<ElementSelect>()
            {
                new ElementSelect("PartNumber"),
                new ElementSelect("Quantity")
            };

            using (var reader = GetResourceReader("PurchaseOrders"))
            {
                var engine = new JsonPathStreamEngine(reader, from, selects);
                var result = engine.Execute();
                Assert.That(result.Count, Is.EqualTo(5));
                Assert.That(result.Count, Is.EqualTo(5));
                Assert.That(result.Select(x => ((x as IEnumerable<object>).ElementAt(0) as string).Length), Is.All.EqualTo(6)); //Format is 123-XY
                Assert.That(result.Select(x => (x as IEnumerable<object>).ElementAt(1)), Is.All.EqualTo(1).Or.EqualTo(2)); //All quantity are between 1 and 2
            }
        }


        [Test]
        public void Execute_FromElement_ValueCorrect()
        {
            var from = "$.PurchaseOrders[*].Items[*].ProductName";
            var selects = new List<ElementSelect>()
            {
                new ElementSelect("$")
            };

            using (var reader = GetResourceReader("PurchaseOrders"))
            {
                var engine = new JsonPathStreamEngine(reader, from, selects);
                var result = engine.Execute();
                Assert.That((result.ElementAt(0) as IEnumerable<object>).ElementAt(0), Is.EqualTo("Lawnmower"));
            }
        }

        [Test]
        public void Execute_FromAttribute_ValueCorrect()
        {
            var from = "$.PurchaseOrders[*].Items[*]";
            var selects = new List<ElementSelect>()
            {
                new ElementSelect("$.PartNumber")
            };

            using (var reader = GetResourceReader("PurchaseOrders"))
            {
                var engine = new JsonPathStreamEngine(reader, from, selects);
                var result = engine.Execute();
                Assert.That((result.ElementAt(0) as IEnumerable<object>).ElementAt(0), Is.EqualTo("872-AA"));
            }
        }

        [Test]
        public void Execute_MissingElement_Null()
        {
            var from = "$.PurchaseOrders[*]";
            var selects = new List<ElementSelect>()
            {
                new ElementSelect("$.PurchaseOrderNumber")
            };

            using (var reader = GetResourceReader("PurchaseOrders"))
            {
                var engine = new JsonPathStreamEngine(reader, from, selects);
                var result = engine.Execute();
                Assert.That((result.ElementAt(3) as IEnumerable<object>).ElementAt(0), Is.EqualTo("(null)"));
            }
        }

        [Test]
        public void Execute_ParentElement_ValueCorrect()
        {
            var from = "$.PurchaseOrders[*].Items[*]";
            var selects = new List<ElementSelect>()
            {
                new ElementSelect("!!.PurchaseOrderNumber"),
                new ElementSelect("$.PartNumber")
            };

            using (var reader = GetResourceReader("PurchaseOrders"))
            {
                var engine = new JsonPathStreamEngine(reader, from, selects);
                var result = engine.Execute();
                Assert.That((result.ElementAt(0) as IEnumerable<object>).ElementAt(0), Does.Contain("99503"));
                Assert.That((result.ElementAt(0) as IEnumerable<object>).ElementAt(1), Does.Contain("872-AA"));

                Assert.That((result.ElementAt(1) as IEnumerable<object>).ElementAt(0), Does.Contain("99503"));
                Assert.That((result.ElementAt(1) as IEnumerable<object>).ElementAt(1), Does.Contain("926-AA"));

                Assert.That((result.ElementAt(2) as IEnumerable<object>).ElementAt(0), Does.Contain("99505"));
                Assert.That((result.ElementAt(2) as IEnumerable<object>).ElementAt(1), Does.Contain("456-NM"));
            }
        }

        [Test]
        public void Execute_ParentElementGoingAboveRoot_ValueCorrect()
        {
            var from = "$.PurchaseOrders[*].Items[*]";
            var selects = new List<ElementSelect>()
            {
                new ElementSelect("!!!!!!.PurchaseOrderNumber"),
                new ElementSelect("$.PartNumber")
            };

            using (var reader = GetResourceReader("PurchaseOrders"))
            {
                var engine = new JsonPathStreamEngine(reader, from, selects);
                var result = engine.Execute();
                Assert.That((result.ElementAt(0) as IEnumerable<object>).ElementAt(0), Does.Contain("(null)"));
                Assert.That((result.ElementAt(0) as IEnumerable<object>).ElementAt(1), Does.Contain("872-AA"));

                Assert.That((result.ElementAt(1) as IEnumerable<object>).ElementAt(0), Does.Contain("(null)"));
                Assert.That((result.ElementAt(1) as IEnumerable<object>).ElementAt(1), Does.Contain("926-AA"));

                Assert.That((result.ElementAt(2) as IEnumerable<object>).ElementAt(0), Does.Contain("(null)"));
                Assert.That((result.ElementAt(2) as IEnumerable<object>).ElementAt(1), Does.Contain("456-NM"));
            }
        }
    }
}
