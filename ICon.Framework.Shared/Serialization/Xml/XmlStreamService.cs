using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Mocassin.Framework.Xml
{
    /// <summary>
    ///     Abstract base class for implementations of the XmlStreamService
    /// </summary>
    public abstract class XmlStreamService
    {
        /// <summary>
        ///     Creates a new default XmlReader from the provided stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public virtual XmlReader NewDefaultReader(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            return XmlReader.Create(stream, GetDefaultReaderSettings());
        }

        /// <summary>
        ///     Creates a new default XmlWrite from the provided file stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public virtual XmlWriter NewDefaultWriter(Stream stream)
        {
            if (stream == null) 
                throw new ArgumentNullException(nameof(stream));

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
        public virtual XmlWriterSettings GetDefaultWriterSettings()
        {
            return new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(),
                Indent = true,
                NewLineChars = Environment.NewLine,
                OmitXmlDeclaration = true
            };
        }

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
        public virtual void AttachHandlerPackageToSerializer(XmlSerializer serializer, XmlEventHandlers handlers)
        {
            if (serializer == null || handlers == null)
                return;

            handlers.AttributeHandlers?.ForEach(handler =>
            {
                serializer.UnknownAttribute += handler;
            });
            handlers.ElementHandlers?.ForEach(handler =>
            {
                serializer.UnknownElement += handler;
            });
            handlers.NodeHandlers?.ForEach(handler =>
            {
                serializer.UnknownNode += handler;
            });
            handlers.ObjectHandlers?.ForEach(handler =>
            {
                serializer.UnreferencedObject += handler;
            });
        }

        /// <summary>
        ///     Factory method to create new stream service for serializable object
        /// </summary>
        /// <typeparam name="TSerializable"></typeparam>
        /// <returns></returns>
        public static XmlStreamService<TSerializable> CreateFor<TSerializable>(TSerializable obj)
        {
            return new XmlStreamService<TSerializable>();
        }

        /// <summary>
        ///     Tries to deserialize and object from the passed file through the XML format
        /// </summary>
        /// <typeparam name="TSerializable"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="handlers"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool TryDeserialize<TSerializable>(string filePath, XmlEventHandlers handlers, out TSerializable obj)
        {
            return new XmlStreamService<TSerializable>().TryDeserialize(filePath, handlers, out obj);
        }
    }

    /// <summary>
    ///     Generic XML stream service that handles serialization and deserialization of data into/from streams containing data
    ///     in the XML format
    /// </summary>
    public class XmlStreamService<T1> : XmlStreamService
    {
        /// <summary>
        ///     Tries to serialize the object to the stream, returns false if not serializable
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public bool TrySerialize(Stream stream, T1 obj)
        {
            using (var writer = NewDefaultWriter(stream))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(T1));
                    serializer.Serialize(writer, obj, GetDefaultNamespaces());
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Tries to serialize the object to the console output
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool TrySerializeToConsole(T1 obj)
        {
            return TrySerialize(Console.OpenStandardOutput(), obj);
        }

        /// <summary>
        ///     Tries to serialize the object to the provided file, the file is overwritten
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool TrySerialize(string filepath, T1 obj)
        {
            using (var stream = new FileStream(filepath, FileMode.Create))
            {
                return TrySerialize(stream, obj);
            }
        }

        /// <summary>
        ///     Tries to deserialize a stream into the given out parameter, returns false on failed and sets the out parameter to
        ///     default
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="handlers"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool TryDeserialize(FileStream stream, XmlEventHandlers handlers, out T1 obj)
        {
            using (var reader = NewDefaultReader(stream))
            {
                try
                {
                    var serializer = GetSerializer(handlers);
                    var result = serializer.Deserialize(reader);
                    obj = (T1) result;
                    return true;
                }
                catch (Exception e)
                {
                    obj = default;
                    return false;
                }
            }
        }

        /// <summary>
        ///     Tries to deserialize an object directly from a file
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="handlers"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool TryDeserialize(string filepath, XmlEventHandlers handlers, out T1 obj)
        {
            if (File.Exists(filepath))
            {
                using (var stream = new FileStream(filepath, FileMode.Open))
                {
                    return TryDeserialize(stream, handlers, out obj);
                }
            }

            obj = default;
            return false;
        }

        /// <summary>
        ///     Creates a new XML serializer matching the passed object type and attaches all events handlers
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="handlers"></param>
        /// <returns></returns>
        public XmlSerializer GetSerializer(XmlEventHandlers handlers)
        {
            var serializer = new XmlSerializer(typeof(T1));
            AttachHandlerPackageToSerializer(serializer, handlers);
            return serializer;
        }
    }
}