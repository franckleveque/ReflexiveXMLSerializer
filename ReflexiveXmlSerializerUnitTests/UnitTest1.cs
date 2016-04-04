using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReflexiveXMLSerializer;
using System.Xml.Serialization;
using System.Xml;

namespace ReflexiveXmlSerializerUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SerializeTest()
        {
            SerrializableKeyValuePair<string, int> obj = new SerrializableKeyValuePair<string, int>("Franck", 16);
            string expectedResult = GetExpectedXmlResult(obj);
            ReflexiveXmlSerializer xs = new ReflexiveXmlSerializer();
            string result = xs.Serialize(obj);
            Assert.AreEqual(expectedResult, result);
        }

        private static string GetExpectedXmlResult(object obj)
        {
            using (var stringWriter = new System.IO.StringWriter())
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add(string.Empty, string.Empty);

                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = true
                };
                using (var writer = XmlWriter.Create(stringWriter, settings))
                {
                    var xmlSerializer = new XmlSerializer(obj.GetType());
                    xmlSerializer.Serialize(writer, obj, ns);
                }

                return stringWriter.ToString();
            }
        }
    }

    public class SerrializableKeyValuePair<U, V>
    {
        public U Key { get; set; }
        public V Value { get; set; }

        public SerrializableKeyValuePair(U key, V value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
