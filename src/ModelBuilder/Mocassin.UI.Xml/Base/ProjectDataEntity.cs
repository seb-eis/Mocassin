using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Mocassin.UI.Data.ProjectLibrary;
using Newtonsoft.Json;

namespace Mocassin.UI.Data.Base
{
    /// <summary>
    ///     Base class for all objects that support both xml, json serialization and storage in a
    ///     <see cref="IMocassinProjectLibrary" />
    /// </summary>
    [XmlRoot]
    public abstract class ProjectDataEntity : ProjectDataObject
    {
        /// <summary>
        ///     Get or set the primary context key
        /// </summary>
        [Column("Id"), JsonIgnore, XmlIgnore, Key]
        public int Id { get; set; }
    }
}