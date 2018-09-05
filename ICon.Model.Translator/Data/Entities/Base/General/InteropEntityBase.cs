using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Represents an interop entity that has properties that are representat as local blobs in the database but is not itself a blob
    /// </summary>
    public abstract class InteropEntityBase : EntityBase
    {
        /// <summary>
        /// Parses the blob entity object into the binary data and header properties
        /// </summary>
        public virtual void ChangePropertyStatesToBinaries()
        {

        }

        /// <summary>
        /// Parses the binary data and header properties and populates the object
        /// </summary>
        public virtual void ChangePropertyStatesToObjects()
        {

        }
    }
}
