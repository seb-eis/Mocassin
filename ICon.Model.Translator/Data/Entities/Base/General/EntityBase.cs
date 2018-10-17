using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Mocassin.Model.Translator
{
    /// <summary>
    /// Abstract base class for all database entities thta have at least a key
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary>
        /// The database entity context key
        /// </summary>
        [Key]
        [XmlIgnore]
        public int Id { get; set; }
    }
}
