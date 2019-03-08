using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Model;
using Newtonsoft.Json;

namespace Mocassin.UI.Xml.Main
{
    /// <summary>
    ///     The main project data root for a Mocassin project building and database creation instructions
    /// </summary>
    [XmlRoot("MocassinProjectBuild")]
    public class MocassinProjectBuildGraph : MocassinProjectChildEntity<MocassinProjectGraph>
    {
        /// <inheritdoc />
        [NotMapped]
        [XmlIgnore]
        [JsonIgnore]
        public override string Json
        {
            get => base.Json;
            set => base.Json = value;
        }

        /// <summary>
        ///     Get or set a name for the <see cref="MocassinProjectBuildGraph" />
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        ///     Get or set the <see cref="Mocassin.UI.Xml.Model.ProjectModelGraph" />
        /// </summary>
        [XmlElement("ProjectModelGraph")]
        public ProjectModelGraph ProjectModelGraph { get; set; }

        /// <summary>
        ///     Get or set the context id of the <see cref="ProjectModelGraph" />
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        [ForeignKey(nameof(ProjectModelGraph))]
        public int ProjectModelGraphId { get; set; }

        /// <summary>
        ///     Get or set the <see cref="Mocassin.UI.Xml.Customization.ProjectCustomizationGraph" />
        /// </summary>
        [XmlElement("ProjectCustomizationGraph")]
        public ProjectCustomizationGraph ProjectCustomizationGraph { get; set; }

        /// <summary>
        ///     Get or set the context id of the <see cref="ProjectCustomizationGraph" />
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        [ForeignKey(nameof(ProjectCustomizationGraph))]
        public int ProjectCustomizationGraphId { get; set; }

        /// <summary>
        ///     Get or set the <see cref="Mocassin.UI.Xml.Jobs.ProjectJobTranslationGraph" />
        /// </summary>
        [XmlElement("ProjectJobTranslationGraph")]
        public ProjectJobTranslationGraph ProjectJobTranslationGraph { get; set; }

        /// <summary>
        ///     Get or set the context id of the <see cref="ProjectJobTranslationGraph" />
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        [ForeignKey(nameof(ProjectJobTranslationGraph))]
        public int ProjectJobTranslationGraphId { get; set; }
    }
}