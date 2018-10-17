using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    /// <summary>
    /// Abstract base class for objects that are stored as interop blob entities within their own table of the database.
    /// </summary>
    public abstract class BlobEntityBase : EntityBase
    {
        /// <summary>
        /// The blob type name for discrimination in the db blob table
        /// </summary>
        public virtual string BlobTypeName { get; protected set; }

        /// <summary>
        /// The total number of bytes for the blob containing the data and header bytes
        /// </summary>
        public virtual int BlobSize { get; protected set; }

        /// <summary>
        /// The number of bytes of the blobs that are header information and not actual data
        /// </summary>
        public virtual int HeaderSize { get; protected set; }

        /// <summary>
        /// The binary data of the entity. Property is for EF data storage only
        /// </summary>
        public byte[] BinaryState { get; set; }

        /// <summary>
        /// Parses the blob entity object into the binary data and header properties
        /// </summary>
        public abstract void ChangeStateToBinary(IMarshalProvider marshalProvider);

        /// <summary>
        /// Parses the binary data and header properties and populates the object
        /// </summary>
        public abstract void ChangeStateToObject(IMarshalProvider marshalProvider);
    }
}
