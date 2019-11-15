using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.LatticeModel
{
    /// <summary>
    ///     Serializable data object to supply all data managed by the <see cref="Mocassin.Model.Lattices.ILatticeManager" />
    ///     system
    /// </summary>
    [XmlRoot("LatticeModel")]
    public class LatticeModelGraph : ModelManagerGraph
    {
        private ObservableCollection<BuildingBlockGraph> buildingBlocks;
        private ObservableCollection<DopingGraph> dopings;
        private ObservableCollection<DopingCombinationGraph> dopingCombination;

        /// <summary>
        ///     The list of defines building blocks
        /// </summary>
        [XmlArray("BuildingBlocks")]
        [XmlArrayItem("BuildingBlock")]
        public ObservableCollection<BuildingBlockGraph> BuildingBlocks
        {
            get => buildingBlocks;
            set => SetProperty(ref buildingBlocks, value);
        }

        /// <summary>
        ///     The list of defines dopings
        /// </summary>
        [XmlArray("Dopings")]
        [XmlArrayItem("Doping")]
        public ObservableCollection<DopingGraph> Dopings
        {
            get => dopings;
            set => SetProperty(ref dopings, value);
        }

        /// <summary>
        ///     The list of defines doping combinations
        /// </summary>
        [XmlArray("DopingCombinations")]
        [XmlArrayItem("DopingCombination")]
        public ObservableCollection<DopingCombinationGraph> DopingCombination
        {
            get => dopingCombination;
            set => SetProperty(ref dopingCombination, value);
        }

        /// <summary>
        ///     Creates new <see cref="LatticeModelGraph" /> with empty component lists
        /// </summary>
        public LatticeModelGraph()
        {
            BuildingBlocks = new ObservableCollection<BuildingBlockGraph>();
            Dopings = new ObservableCollection<DopingGraph>();
            DopingCombination = new ObservableCollection<DopingCombinationGraph>();
        }

        /// <inheritdoc />
        public override IEnumerable<IModelParameter> GetInputParameters()
        {
            yield break;
        }

        /// <inheritdoc />
        public override IEnumerable<IModelObject> GetInputObjects()
        {
            return BuildingBlocks.Select(x => x.GetInputObject()).Cast<IModelObject>()
                .Concat(DopingCombination.Select(x => x.GetInputObject()))
                .Concat(Dopings.Select(x => x.GetInputObject()));
        }
    }
}