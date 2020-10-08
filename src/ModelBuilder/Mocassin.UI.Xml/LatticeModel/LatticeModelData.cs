using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.UI.Data.Base;

namespace Mocassin.UI.Data.LatticeModel
{
    /// <summary>
    ///     Serializable data object to supply all data managed by the <see cref="Mocassin.Model.Lattices.ILatticeManager" />
    ///     system
    /// </summary>
    [XmlRoot]
    public class LatticeModelData : ModelManagerData
    {
        private ObservableCollection<BuildingBlockData> buildingBlocks;
        private ObservableCollection<DopingAbstractData> dopingCombination;
        private ObservableCollection<DopingData> dopings;

        /// <summary>
        ///     The list of defines building blocks
        /// </summary>
        [XmlArray]
        public ObservableCollection<BuildingBlockData> BuildingBlocks
        {
            get => buildingBlocks;
            set => SetProperty(ref buildingBlocks, value);
        }

        /// <summary>
        ///     The list of defines dopings
        /// </summary>
        [XmlArray]
        public ObservableCollection<DopingData> Dopings
        {
            get => dopings;
            set => SetProperty(ref dopings, value);
        }

        /// <summary>
        ///     The list of defines doping combinations
        /// </summary>
        [XmlArray]
        public ObservableCollection<DopingAbstractData> DopingCombination
        {
            get => dopingCombination;
            set => SetProperty(ref dopingCombination, value);
        }

        /// <summary>
        ///     Creates new <see cref="LatticeModelData" /> with empty component lists
        /// </summary>
        public LatticeModelData()
        {
            BuildingBlocks = new ObservableCollection<BuildingBlockData>();
            Dopings = new ObservableCollection<DopingData>();
            DopingCombination = new ObservableCollection<DopingAbstractData>();
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