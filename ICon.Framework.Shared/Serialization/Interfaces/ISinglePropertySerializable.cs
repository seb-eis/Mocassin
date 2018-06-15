using System;

namespace ICon.Framework.Xml
{
    /// <summary>
    /// Interface that specifies that the class or struct uses serialization/deserialization through a single splittable attribute string
    /// </summary>
    public interface ISinglePropertySerializable<T1>
    {
        /// <summary>
        /// Get or set the class by attribute string
        /// </summary>
        String SerializationString { get; set; }

        /// <summary>
        /// Serializes the values to the attribute string
        /// </summary>
        /// <returns></returns>
        String ToSerializationString(Char separator = ',');

        /// <summary>
        /// Deserializes the values to the attribute string
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        T1 FromSerializationString(String serial, Char separator = ',');
    }
}
