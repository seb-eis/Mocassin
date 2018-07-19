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
        /// The binary header information. Property is for EF data storage only
        /// </summary>
        public byte[] HeaderBinary { get; set; }

        /// <summary>
        /// RThe binary data of the entity. Property is for EF data storage only
        /// </summary>
        public byte[] DataBinary { get; set; }

        /// <summary>
        /// Parses the blob entity object into the binary data and header properties
        /// </summary>
        public abstract void DataToBinary();

        /// <summary>
        /// Parses the binary data and header properties and populates the object
        /// </summary>
        public abstract void BinaryToData();

        /// <summary>
        /// Nulls the binary data and header information
        /// </summary>
        public void NullBinaries()
        {
            HeaderBinary = null;
            DataBinary = null;
        }
    }
}
