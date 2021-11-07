using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.UI.Data.Base;
using Mocassin.UI.Data.Customization;
using Mocassin.UI.Data.Jobs;
using Newtonsoft.Json;

namespace Mocassin.UI.Data.Main
{
    /// <summary>
    ///     The main project data root for a Mocassin project building and database creation instructions
    /// </summary>
    [XmlRoot]
    public class SimulationDbBuildTemplate : ProjectChildEntity<MocassinProject>
    {
        private string customizationKey;
        private string jobTranslationKey;
        private string modelKey;
        private ProjectCustomizationTemplate projectCustomizationTemplate;
        private ProjectJobSetTemplate projectJobSetTemplate;
        private ProjectModelData projectModelData;

        /// <inheritdoc />
        [NotMapped, XmlIgnore, JsonIgnore]
        public override string Json
        {
            get => base.Json;
            set => base.Json = value;
        }

        /// <summary>
        ///     Get or set the <see cref="ProjectModelData" />
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public ProjectModelData ProjectModelData
        {
            get => projectModelData;
            set
            {
                SetProperty(ref projectModelData, value);
                ModelKey = value?.Key;
            }
        }

        /// <summary>
        ///     Get or set the key of the <see cref="ProjectModelData" />
        /// </summary>
        public string ModelKey
        {
            get => modelKey;
            set => SetProperty(ref modelKey, value);
        }

        /// <summary>
        ///     Get or set the <see cref="ProjectCustomizationTemplate" />
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public ProjectCustomizationTemplate ProjectCustomizationTemplate
        {
            get => projectCustomizationTemplate;
            set
            {
                SetProperty(ref projectCustomizationTemplate, value);
                CustomizationKey = value?.Key;
            }
        }

        /// <summary>
        ///     Get or set the key of the <see cref="ProjectCustomizationTemplate" />
        /// </summary>
        public string CustomizationKey
        {
            get => customizationKey;
            set => SetProperty(ref customizationKey, value);
        }

        /// <summary>
        ///     Get or set the <see cref="ProjectJobSetTemplate" />
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public ProjectJobSetTemplate ProjectJobSetTemplate
        {
            get => projectJobSetTemplate;
            set
            {
                SetProperty(ref projectJobSetTemplate, value);
                JobTranslationKey = value?.Key;
            }
        }

        /// <summary>
        ///     Get or set the key of the <see cref="ProjectJobSetTemplate" />
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
            ProjectModelData = Parent?.ProjectModelData;
            ProjectCustomizationTemplate = Parent?.CustomizationTemplates
                                                 .SingleOrDefault(x => x.Key == CustomizationKey);
            ProjectJobSetTemplate = Parent?.JobSetTemplates
                                          .SingleOrDefault(x => x.Key == JobTranslationKey);
        }
    }
}