using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Lattices;
using Mocassin.Model.Particles;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.LatticeModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Lattices.IBuildingBlock" /> model object creation
    /// </summary>
    [XmlRoot("BuildingBlock")]
    public class BuildingBlockGraph : ModelObjectGraph
    {
        private ObservableCollection<ModelObjectReferenceGraph<Particle>> particleList;

        /// <summary>
        ///     List of particles which define the building block
        /// </summary>
        [XmlArray("ParticleList")]
        [XmlArrayItem("Particle")]
        public ObservableCollection<ModelObjectReferenceGraph<Particle>> ParticleList
        {
            get => particleList;
            set => SetProperty(ref particleList, value);
        }

        public BuildingBlockGraph()
        {
            ParticleList = new ObservableCollection<ModelObjectReferenceGraph<Particle>>();
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