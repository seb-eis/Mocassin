using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Abstract base class for all database entities that have at least a primary context key
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary>
        ///     The database entity context key
        /// </summary>
        [Key, XmlIgnore]
        public virtual int Id { get; set; }
    }
}