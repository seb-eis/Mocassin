using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Attribute that marks a property as an owend blob property
    /// </summary>
    public class OwnedBlobPropertyAttribute : Attribute
    {
        /// <summary>
        /// The property name the blob binray representation is bound to
        /// </summary>
        public string BlobProperyName { get; }

        public OwnedBlobPropertyAttribute(string blobProperyName)
        {
            BlobProperyName = blobProperyName ?? throw new ArgumentNullException(nameof(blobProperyName));
        }
    }
}
