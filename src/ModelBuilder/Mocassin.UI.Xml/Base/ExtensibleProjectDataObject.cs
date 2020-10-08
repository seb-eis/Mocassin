using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Mocassin.UI.Data.Base
{
    /// <summary>
    ///     Base class for all <see cref="ProjectDataObject" /> instances that support a resource container
    /// </summary>
    public class ExtensibleProjectDataObject : ProjectDataObject
    {
        private ResourcesData resources;

        /// <summary>
        ///     Get or set the <see cref="ResourcesData" /> for attached properties
        /// </summary>
        [XmlIgnore, NotMapped]
        public ResourcesData Resources
        {
            get => resources;
            set => SetProperty(ref resources, value);
        }

        /// <summary>
        ///     Creates a new <see cref="ExtensibleProjectDataObject" /> with empty resources
        /// </summary>
        protected ExtensibleProjectDataObject()
        {
            Resources = new ResourcesData();
        }
    }
}