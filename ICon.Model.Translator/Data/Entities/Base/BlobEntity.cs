using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Abstract base class for entities that are stored as blobs in the database. The blobs support an also binary header information
    /// </summary>
    public abstract class BlobEntity : EntityBase
    {
        /// <summary>
        /// The number of bytes of the blobs that are header information and not actual data
        /// </summary>
        public int HeaderSize { get; protected set; }

        /// <summary>
        /// RThe binary data of the entity. Property is for EF data storage only
        /// </summary>
        public byte[] Binary { get; set; }

        /// <summary>
        /// Parses the blob entity object into the binary data and header properties
        /// </summary>
        public abstract void ChangeToBlobState();

        /// <summary>
        /// Parses the binary data and header properties and populates the object
        /// </summary>
        public abstract void ChangeToDataState();
    }
}
