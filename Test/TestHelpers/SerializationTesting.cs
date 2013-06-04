using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml;
using System.Xml.Serialization;
using Test.Diesel.Generated;

namespace Test.Diesel.TestHelpers
{
    public class SerializationTesting
    {
        public static T SerializeDeserializeWithSoapFormatter<T>(T graph)
        {
            return SerializeDeserialize(graph, new SoapFormatter());
        }

        public static T SerializeDeserializeWithBinaryFormatter<T>(T graph)
        {
            return SerializeDeserialize(graph, new BinaryFormatter());
        }

        public static T SerializeDeserializeWithXmlSerializer<T>(T data)
        {
            var serializer = new XmlSerializer(typeof(T));
            TextWriter textWriter = new StringWriter();
            serializer.Serialize(textWriter, data);
            var serialized = textWriter.ToString();
            var deserialized = (T) serializer.Deserialize(new StringReader(serialized));
            return deserialized;
        }

        private static T SerializeDeserialize<T>(T graph, IFormatter formatter)
        {
            var ms = new MemoryStream();
            formatter.Serialize(ms, graph);
            ms.Position = 0;
            var deserialized = (T)formatter.Deserialize(ms);
            return deserialized;
        }

        public static T SerializeDeserializeWithDataContractSerializer<T>(T data)
        {
            var serializer = new DataContractSerializer(typeof (T));
            var stringWriter = new StringWriter();
            serializer.WriteObject(new XmlTextWriter(stringWriter), data);
            var serialized = stringWriter.ToString();
            var deserialized = (T)serializer.ReadObject(new XmlTextReader(new StringReader(serialized)));
            return deserialized;
        }
    }
}