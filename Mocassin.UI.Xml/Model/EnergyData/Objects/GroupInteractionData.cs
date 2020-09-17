using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Energies;
using Mocassin.Model.Structures;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.EnergyModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Energies.IGroupInteraction" /> model object creation
    /// </summary>
    [XmlRoot]
    public class GroupInteractionData : ModelDataObject
    {
        private ModelObjectReference<CellSite> centerCellReferencePosition = new ModelObjectReference<CellSite>();
        private ObservableCollection<VectorData3D> groupGeometry = new ObservableCollection<VectorData3D>();

        /// <summary>
        ///     Get or set the key of the center unit cell position
        /// </summary>
        [XmlElement]
        public ModelObjectReference<CellSite> CenterCellReferencePosition
        {
            get => centerCellReferencePosition;
            set => SetProperty(ref centerCellReferencePosition, value);
        }

        /// <summary>
        ///     Get or seth the list of surrounding position geometry vectors
        /// </summary>
        [XmlArray]
        public ObservableCollection<VectorData3D> GroupGeometry
        {
            get => groupGeometry;
            set => SetProperty(ref groupGeometry, value);
        }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            var obj = new GroupInteraction
            {
                CenterCellSite = new CellSite {Key = CenterCellReferencePosition.Key},
                GeometryVectors = GroupGeometry.Select(x => x.AsFractional3D()).ToList()
            };
            return obj;
        }
    }
}