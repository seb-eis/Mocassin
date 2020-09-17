using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Mocassin.Framework.Xml
{
    /// <summary>
    ///     Static class that contains methods for using <see cref="DataContractSerializer" /> in a convenient way
    /// </summary>
    public static class DataContractConvert
    {
        /// <summary>
        ///     Serializes the passed <see cref="object" /> using the <see cref="DataContractSerializer" /> and provided
        ///     <see cref="Encoding" /> into a string, null defaults to UTF8 and default serializer
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="encoding"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static string Serialize(object obj, Encoding encoding, DataContractSerializer serializer)
        {
            encoding ??= Encoding.UTF8;
            using var memoryStream = new MemoryStream();
            serializer ??= new DataContractSerializer(obj.GetType());
            serializer.WriteObject(memoryStream, obj);
            return encoding.GetString(memoryStream.ToArray());
        }

        /// <summary>
        ///     Serializes the passed <see cref="object" /> using the <see cref="DataContractSerializer" /> and provided
        ///     <see cref="Encoding" /> into a new file at the provided location, null defaults to UTF8 and default serializer
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filePath"></param>
        /// <param name="encoding"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static void Serialize(object obj, string filePath, Encoding encoding, DataContractSerializer serializer)
        {
            var xml = Serialize(obj, encoding, serializer);
            File.WriteAllText(filePath, xml, encoding ?? Encoding.UTF8);
        }

        /// <summary>
        ///     Deserializes an object using the <see cref="DataContractSerializer" /> and provided
        ///     <see cref="Encoding" /> from a string, null defaults to UTF8 and default serializer
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="encoding"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string xml, Encoding encoding, DataContractSerializer serializer)
        {
            encoding ??= Encoding.UTF8;
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var reader = XmlDictionaryReader.CreateTextReader(memoryStream, encoding, new XmlDictionaryReaderQuotas(), null);
            serializer ??= new DataContractSerializer(typeof(T));
            return (T) serializer.ReadObject(reader);
        }

        /// <summary>
        ///     Deserializes an object using the <see cref="DataContractSerializer" /> and provided
        ///     <see cref="Encoding" /> from a file path, null defaults to UTF8 and default serializer
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="encoding"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static T DeserializeFromFile<T>(string filePath, Encoding encoding, DataContractSerializer serializer)
        {
            if (!File.Exists(filePath)) throw new ArgumentException("Provided file path does not exist");
            return Deserialize<T>(File.ReadAllText(filePath), encoding, serializer);
        }
    }
}