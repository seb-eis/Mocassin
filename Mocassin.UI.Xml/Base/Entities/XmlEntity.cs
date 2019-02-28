using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    /// Base class for all objects that support both xml serialization and storage in a database
    /// </summary>
    [XmlRoot]
    public abstract class XmlEntity
    {
        /// <summary>
        ///     Get or set the primary context key
        /// </summary>
        [Column("Id"), XmlIgnore, Key]
        public int ContextId { get; set; }
    }
}