using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Attribute that marks a property as an owend blob property
    /// </summary>
    public class OwendBlobPropertyAttribute : Attribute
    {
        /// <summary>
        /// The property name the blob binray representation is bound to
        /// </summary>
        public string BlobProperyName { get; }

        /// <summary>
        /// The property name where the size information should be stored
        /// </summary>
        public string BlobSizePropertyName { get; }

        public OwendBlobPropertyAttribute(string blobProperyName, string blobSizePropertyName)
        {
            BlobProperyName = blobProperyName ?? throw new ArgumentNullException(nameof(blobProperyName));
            BlobSizePropertyName = blobSizePropertyName;
        }
    }
}
