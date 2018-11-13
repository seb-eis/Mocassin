using System;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Attribute that marks a property as an owned blob property within a database and defines the sister property for
    ///     conversion from or to binary data
    /// </summary>
    public class OwnedBlobPropertyAttribute : Attribute
    {
        /// <summary>
        ///     The property name the blob binary representation is bound to
        /// </summary>
        public string BlobPropertyName { get; }

        /// <summary>
        ///     Crate new owned blob property attribute with the passed binary property name
        /// </summary>
        /// <param name="blobPropertyName"></param>
        public OwnedBlobPropertyAttribute(string blobPropertyName)
        {
            BlobPropertyName = blobPropertyName ?? throw new ArgumentNullException(nameof(blobPropertyName));
        }
    }
}