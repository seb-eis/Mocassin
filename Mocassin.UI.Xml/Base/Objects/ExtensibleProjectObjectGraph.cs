using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Base class for all <see cref="ProjectObjectGraph"/> instances that support a resource container
    /// </summary>
    public class ExtensibleProjectObjectGraph : ProjectObjectGraph
    {
        private ResourcesGraph resources;

        /// <summary>
        ///     Get or set the <see cref="ResourcesGraph" /> for attached properties
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        public ResourcesGraph Resources
        {
            get => resources;
            set => SetProperty(ref resources, value);
        }

        /// <summary>
        ///     Creates a new <see cref="ExtensibleProjectObjectGraph"/> with empty resources
        /// </summary>
        protected ExtensibleProjectObjectGraph()
        {
            Resources = new ResourcesGraph();
        }
    }
}