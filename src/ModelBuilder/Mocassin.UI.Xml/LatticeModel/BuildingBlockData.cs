using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Lattices;
using Mocassin.Model.Particles;
using Mocassin.UI.Data.Base;

namespace Mocassin.UI.Data.LatticeModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Lattices.IBuildingBlock" /> model object creation
    /// </summary>
    [XmlRoot]
    public class BuildingBlockData : ModelDataObject
    {
        private ObservableCollection<ModelObjectReference<Particle>> particleList = new ObservableCollection<ModelObjectReference<Particle>>();

        /// <summary>
        ///     List of particles which define the building block
        /// </summary>
        [XmlArray]
        public ObservableCollection<ModelObjectReference<Particle>> ParticleList
        {
            get => particleList;
            set => SetProperty(ref particleList, value);
        }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            var obj = new BuildingBlock
            {
                CellEntries = ParticleList.Select(x => new Particle {Key = x.Key}).Cast<IParticle>().ToList()
            };
            return obj;
        }
    }
}