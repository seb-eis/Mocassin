using System;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Attribute to mark a property as an interop object that supports binary conversion from or to a binary
    ///     sister property
    /// </summary>
    public class InteropPropertyAttribute : Attribute
    {
        /// <summary>
        ///     The name of the property used for binary storage
        /// </summary>
        public string BinaryPropertyName { get; }

        /// <summary>
        ///     Create new interop property attribute with the passed binary sister property name
        /// </summary>
        /// <param name="binaryPropertyName"></param>
        public InteropPropertyAttribute(string binaryPropertyName)
        {
            BinaryPropertyName = binaryPropertyName ?? throw new ArgumentNullException(nameof(binaryPropertyName));
        }
    }
}