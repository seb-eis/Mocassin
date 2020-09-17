using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Mocassin.Framework.Xml
{
    /// <summary>
    ///     Abstract base class for implementations Xml stream helper
    /// </summary>
    public abstract class XmlSerializationHelper
    {
        /// <summary>
        ///     Creates a new default <see cref="XmlReader" /> from the provided stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public virtual XmlReader CreateDefaultReader(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            return XmlReader.Create(stream, GetDefaultReaderSettings());
        }

        /// <summary>
        ///     Creates a new default <see cref="XmlWriter" /> from the provided file stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public virtual XmlWriter CreateDefaultWriter(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            return XmlWriter.Create(stream, GetDefaultWriterSettings());
        }

        /// <summary>
        ///     Gets the default reader settings object and attach the event handlers
        /// </summary>
        /// <returns></returns>
        public virtual XmlReaderSettings GetDefaultReaderSettings()
        {
            var settings = new XmlReaderSettings
            {
                IgnoreComments = true
            };
            return settings;
        }

        /// <summary>
        ///     Get the default writer settings
        /// </summary>
        /// <returns></returns>
        public virtual XmlWriterSettings GetDefaultWriterSettings() =>
            new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(),
                Indent = true,
                NewLineChars = Environment.NewLine,
                OmitXmlDeclaration = true
            };

        /// <summary>
        ///     Get the default (empty) xml serializer namespaces
        /// </summary>
        /// <returns></returns>
        public virtual XmlSerializerNamespaces GetDefaultNamespaces()
        {
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");
            return namespaces;
        }

        /// <summary>
        ///     Attaches all non null event handlers of a handler package to a serializer
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="handlers"></param>
        public static void AttachHandlerPackageToSerializer(XmlSerializer serializer, XmlEventHandlers handlers)
        {
            if (serializer == null || handlers == null)
                return;

            handlers.AttributeHandlers?.ForEach(handler => { serializer.UnknownAttribute += handler; });
            handlers.ElementHandlers?.ForEach(handler => { serializer.UnknownElement += handler; });
            handlers.NodeHandlers?.ForEach(handler => { serializer.UnknownNode += handler; });
            handlers.ObjectHandlers?.ForEach(handler => { serializer.UnreferencedObject += handler; });
        }

        /// <summary>
        ///     Creates a new XML serializer matching the passed object type and attaches all events handlers
        /// </summary>
        /// <param name="type"></param>
        /// <param name="handlers"></param>
        /// <returns></returns>
        public static XmlSerializer GetSerializer(Type type, XmlEventHandlers handlers)
        {
            var serializer = new XmlSerializer(type);
            AttachHandlerPackageToSerializer(serializer, handlers);
            return serializer;
        }

        /// <summary>
        ///     Factory method to create new stream service for serializable object
        /// </summary>
        /// <typeparam name="TSerializable"></typeparam>
        /// <returns></returns>
        public static XmlSerializationHelper<TSerializable> Create<TSerializable>(TSerializable obj) => new XmlSerializationHelper<TSerializable>();

        /// <summary>
        ///     Tries to deserialize and object from the passed file through the XML format
        /// </summary>
        /// <typeparam name="TSerializable"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="handlers"></param>
        /// <param name="obj"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static bool TryDeserialize<TSerializable>(string filePath, XmlEventHandlers handlers, out TSerializable obj,
            out Exception exception) =>
            new XmlSerializationHelper<TSerializable>().TryDeserialize(filePath, handlers, out obj, out exception);

        /// <summary>
        ///     Tries to serialize the passed object to the passed file path
        /// </summary>
        /// <typeparam name="TSerializable"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="obj"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static bool TrySerialize<TSerializable>(string filePath, TSerializable obj, out Exception exception) =>
            new XmlSerializationHelper<TSerializable>().TrySerialize(filePath, obj, out exception);

        /// <summary>
        ///     Tries to serialize the passed object to the passed stream
        /// </summary>
        /// <typeparam name="TSerializable"></typeparam>
        /// <param name="stream"></param>
        /// <param name="obj"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static bool TrySerialize<TSerializable>(Stream stream, TSerializable obj, out Exception exception) =>
            new XmlSerializationHelper<TSerializable>().TrySerialize(stream, obj, out exception);

        /// <summary>
        ///     Serializes the given object into its xml representation without further formatting options
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="encoding"></param>
        /// <param name="eventHandlers"></param>
        /// <returns></returns>
        public static string Serialize(object obj, Encoding encoding, XmlEventHandlers eventHandlers = null)
        {
            encoding ??= Encoding.UTF8;
            var stream = new MemoryStream(1000000);
            try
            {
                var serializer = GetSerializer(obj.GetType(), eventHandlers);
                serializer.Serialize(stream, obj);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }

            return encoding.GetString(stream.ToArray());
        }

        /// <summary>
        ///     Deserializes an xml representation into an object of the specified type without further formatting options
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="type"></param>
        /// <param name="eventHandlers"></param>
        /// <returns></returns>
        public static object Deserialize(string xml, Type type, XmlEventHandlers eventHandlers = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var serializer = GetSerializer(type, eventHandlers);
            var reader = new StringReader(xml);
            try
            {
                return serializer.Deserialize(reader);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }
    }

    /// <summary>
    ///     Generic XML stream service that handles serialization and deserialization of data into/from streams containing data
    ///     in the XML format
    /// </summary>
    public class XmlSerializationHelper<T1> : XmlSerializationHelper
    {
        /// <summary>
        ///     Tries to serialize the object to the stream, returns false if not serializable
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public bool TrySerialize(Stream stream, T1 obj, out Exception exception)
        {
            using (var writer = CreateDefaultWriter(stream))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(T1));
                    serializer.Serialize(writer, obj, GetDefaultNamespaces());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    exception = e;
                    return false;
                }
            }

            exception = default;
            return true;
        }

        /// <summary>
        ///     Tries to serialize the object to the console output
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public bool TrySerializeToConsole(T1 obj, out Exception exception) => TrySerialize(Console.OpenStandardOutput(), obj, out exception);

        /// <summary>
        ///     Tries to serialize the object to the provided file, the file is overwritten
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="obj"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public bool TrySerialize(string filepath, T1 obj, out Exception exception)
        {
            using var stream = new FileStream(filepath, FileMode.Create);
            return TrySerialize(stream, obj, out exception);
        }

        /// <summary>
        ///     Tries to deserialize a stream into the given out parameter, returns false on failed and sets the out parameter to
        ///     default
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="handlers"></param>
        /// <param name="obj"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public bool TryDeserialize(FileStream stream, XmlEventHandlers handlers, out T1 obj, out Exception exception)
        {
            exception = default;
            using var reader = CreateDefaultReader(stream);
            try
            {
                var serializer = GetSerializer(typeof(T1), handlers);
                var result = serializer.Deserialize(reader);
                obj = (T1) result;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                obj = default;
                exception = e;
                return false;
            }
        }

        /// <summary>
        ///     Tries to deserialize an object directly from a file
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="handlers"></param>
        /// <param name="obj"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public bool TryDeserialize(string filepath, XmlEventHandlers handlers, out T1 obj, out Exception exception)
        {
            try
            {
                using var stream = new FileStream(filepath, FileMode.Open);
                return TryDeserialize(stream, handlers, out obj, out exception);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                exception = e;
                obj = default;
                return false;
            }
        }
    }
}