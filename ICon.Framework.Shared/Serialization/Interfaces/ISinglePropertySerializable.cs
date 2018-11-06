namespace Mocassin.Framework.Xml
{
    /// <summary>
    ///     Interface that specifies that the class or struct uses serialization/deserialization through a single splittable
    ///     attribute string
    /// </summary>
    public interface ISinglePropertySerializable<out T1>
    {
        /// <summary>
        ///     Get or set the class by attribute string
        /// </summary>
        string SerializationString { get; set; }

        /// <summary>
        ///     Serializes the values to the attribute string
        /// </summary>
        /// <returns></returns>
        string ToSerializationString(char separator = ',');

        /// <summary>
        ///     Deserializes the values to the attribute string
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        T1 FromSerializationString(string serial, char separator = ',');
    }
}