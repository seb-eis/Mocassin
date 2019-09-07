using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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
        private ProjectJobTranslationGraph projectJobTranslationGraph;
        private ProjectCustomizationGraph projectCustomizationGraph;
        private ProjectModelGraph projectModelGraph;
        private string modelKey;
        private string customizationKey;
        private string jobTranslationKey;

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
        ///     Get or set the <see cref="Mocassin.UI.Xml.Model.ProjectModelGraph" />
        /// </summary>
        [XmlElement("ProjectModelGraph")]
        [JsonIgnore]
        public ProjectModelGraph ProjectModelGraph
        {
            get => projectModelGraph;
            set
            {
                SetProperty(ref projectModelGraph, value);
                ModelKey = value?.Key;
            }
        }

        /// <summary>
        ///     Get or set the key of the <see cref="ProjectModelGraph" />
        /// </summary>
        public string ModelKey
        {
            get => modelKey;
            set => SetProperty(ref modelKey, value);
        }

        /// <summary>
        ///     Get or set the <see cref="Mocassin.UI.Xml.Customization.ProjectCustomizationGraph" />
        /// </summary>
        [XmlElement("ProjectCustomizationGraph")]
        [JsonIgnore]
        public ProjectCustomizationGraph ProjectCustomizationGraph
        {
            get => projectCustomizationGraph;
            set
            {
                SetProperty(ref projectCustomizationGraph, value);
                CustomizationKey = value?.Key;
            }
        }

        /// <summary>
        ///     Get or set the key of the <see cref="ProjectCustomizationGraph" />
        /// </summary>
        public string CustomizationKey
        {
            get => customizationKey;
            set => SetProperty(ref customizationKey, value);
        }

        /// <summary>
        ///     Get or set the <see cref="Mocassin.UI.Xml.Jobs.ProjectJobTranslationGraph" />
        /// </summary>
        [XmlElement("ProjectJobTranslationGraph")]
        [JsonIgnore]
        public ProjectJobTranslationGraph ProjectJobTranslationGraph
        {
            get => projectJobTranslationGraph;
            set
            {
                SetProperty(ref projectJobTranslationGraph, value);
                JobTranslationKey = value?.Key;
            }
        }

        /// <summary>
        ///     Get or set the key of the <see cref="ProjectJobTranslationGraph" />
        /// </summary>
        public string JobTranslationKey
        {
            get => jobTranslationKey;
            set => SetProperty(ref jobTranslationKey, value);
        }

        /// <summary>
        ///     Restores the build object references not covered by the JSON serialization
        /// </summary>
        public void RestoreBuildReferences()
        {
            ProjectModelGraph = Parent?.ProjectModelGraph;
            ProjectCustomizationGraph = Parent?.ProjectCustomizationGraphs
                .SingleOrDefault(x => x.Key == CustomizationKey);
            ProjectJobTranslationGraph = Parent?.ProjectJobTranslationGraphs
                .SingleOrDefault(x => x.Key == JobTranslationKey);
        }
    }
}