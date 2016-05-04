using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReflexiveXmlSerializerUnitTests
{
    /// <summary>
    /// Summary description for Deserialization
    /// </summary>
    [TestClass]
    public class Deserialization
    {
        public Deserialization()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void SimpleObjectTest()
        {
            string xml = @"<SimpleTest><Id>1</Id><Name>Franck</Name><BirthDate>1978-9-16</BirthDate></SimpleTest>";
            ReflexiveXMLSerializer.ReflexiveXmlDeserializer des = new ReflexiveXMLSerializer.ReflexiveXmlDeserializer(typeof(SimpleSample));
            var t = des.Deserialize(xml);
            Assert.IsInstanceOfType(t, typeof(SimpleSample));
            var test = (SimpleSample)t;
            Assert.AreEqual(1, test.Id);
            Assert.AreEqual("Franck", test.Name);
            Assert.AreEqual(new DateTime(1978,9,16), test.BirthDate);
        }

        [TestMethod]
        public void ObjectArrayTest()
        {
            string xml = @"<ObjectArrayTest><Id>1</Id><Friends><SimpleTest><Id>1</Id><Name>Franck</Name><BirthDate>1978-9-16</BirthDate></SimpleTest></Friends></ObjectArrayTest>";
            ReflexiveXMLSerializer.ReflexiveXmlDeserializer des = new ReflexiveXMLSerializer.ReflexiveXmlDeserializer(typeof(ObjectArraySample));
            var t = des.Deserialize(xml);
            Assert.IsInstanceOfType(t, typeof(ObjectArraySample));
            var test = (ObjectArraySample)t;
            Assert.AreEqual(1, test.Id);
            //CollectionAssert.AreEqual(new string[] { "varaxor@free.fr", "leveque.franck@gmail.com", "franck.leveque2@free.fr" }, test.Mails);
        }

        [TestMethod]
        public void SimpleArrayTest()
        {
            string xml = @"<SimpleTest><Id>1</Id><Mails><String>varaxor@free.fr</String><String>leveque.franck@gmail.com</String><String>franck.leveque2@free.fr</String></Mails></SimpleTest>";
            ReflexiveXMLSerializer.ReflexiveXmlDeserializer des = new ReflexiveXMLSerializer.ReflexiveXmlDeserializer(typeof(SimpleArraySample));
            var t = des.Deserialize(xml);
            Assert.IsInstanceOfType(t, typeof(SimpleArraySample));
            var test = (SimpleArraySample)t;
            Assert.AreEqual(1, test.Id);
            CollectionAssert.AreEqual(new string[] { "varaxor@free.fr", "leveque.franck@gmail.com", "franck.leveque2@free.fr" }, test.Mails);
        }
    }
}
