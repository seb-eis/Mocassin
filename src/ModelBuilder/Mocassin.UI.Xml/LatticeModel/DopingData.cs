using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Lattices;
using Mocassin.UI.Data.Base;

namespace Mocassin.UI.Data.LatticeModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Lattices.IDoping" /> model object creation
    /// </summary>
    [XmlRoot]
    public class DopingData : ModelDataObject
    {
        private ModelObjectReference<BuildingBlock> buildingBlock = new ModelObjectReference<BuildingBlock>();
        private ModelObjectReference<DopingCombination> counterDoping = new ModelObjectReference<DopingCombination>();
        private ModelObjectReference<DopingCombination> primaryDoping = new ModelObjectReference<DopingCombination>();
        private int priority;
        private bool useCounterDoping;

        /// <summary>
        ///     The doping which is applied
        /// </summary>
        [XmlElement]
        public ModelObjectReference<DopingCombination> PrimaryDoping
        {
            get => primaryDoping;
            set => SetProperty(ref primaryDoping, value);
        }

        /// <summary>
        ///     The doping to compensate the primary doping
        /// </summary>
        [XmlElement]
        public ModelObjectReference<DopingCombination> CounterDoping
        {
            get => counterDoping;
            set => SetProperty(ref counterDoping, value);
        }

        /// <summary>
        ///     The building block in which the doping is used
        /// </summary>
        [XmlElement]
        public ModelObjectReference<BuildingBlock> BuildingBlock
        {
            get => buildingBlock;
            set => SetProperty(ref buildingBlock, value);
        }

        /// <summary>
        ///     Flag to indicate if a counter doping should be used
        /// </summary>
        [XmlAttribute]
        public bool UseCounterDoping
        {
            get => useCounterDoping;
            set => SetProperty(ref useCounterDoping, value);
        }

        /// <summary>
        ///     The priority order in which the doping is applied
        /// </summary>
        [XmlAttribute]
        public int Priority
        {
            get => priority;
            set => SetProperty(ref priority, value);
        }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            var obj = new Doping
            {
                CounterDoping = new DopingCombination {Key = CounterDoping?.Key ?? PrimaryDoping.Key},
                PrimaryDoping = new DopingCombination {Key = PrimaryDoping.Key},
                BuildingBlock = new BuildingBlock {Key = BuildingBlock.Key},
                UseCounterDoping = UseCounterDoping,
                Priority = Priority
            };
            return obj;
        }
    }
}