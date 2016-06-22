using System;
using System.IO;
using System.Security.Permissions;
using System.Xml.Serialization;
using MapProxy.Models.WMS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MapProxyTests
{
    [TestClass]
    public class WMSModelsTest
    {
        private static readonly string ResourceBase = "../../Resources";
        [TestMethod]
        public void TestLayersClass()
        {
            var layer = DeserealizeResource<Layer>("Layer.xml");
            Assert.AreEqual("TestLayer", layer.Name);
            Assert.AreEqual("TestTitle", layer.Title);
            Assert.AreEqual(true, layer.Queryable);
            Assert.AreEqual(100d, layer.ScaleHint.Min);
            Assert.AreEqual(2000000d, layer.ScaleHint.Max);
        }

        [TestMethod]
        public void TestRangeClass()
        {
            var range = DeserealizeResource<Range>("Range.xml");
            Assert.AreEqual(100d, range.Min);
            Assert.AreEqual(2000000d, range.Max);
        }

        public static T DeserealizeResource<T>(string resourceName)
        {
            var serializer = new XmlSerializer(typeof(T));
            T result;
            var path = Path.Combine(ResourceBase, resourceName);
            using (StreamReader reader = new StreamReader(path))
            {
                result = (T)serializer.Deserialize(reader);
            }
            return result;
        }
    }
}
