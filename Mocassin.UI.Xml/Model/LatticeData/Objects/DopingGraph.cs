using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Lattices;
using Mocassin.UI.Xml.Base;
using Newtonsoft.Json;

namespace Mocassin.UI.Xml.LatticeModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Lattices.IDoping" /> model object creation
    /// </summary>
    [XmlRoot("Doping")]
    public class DopingGraph : ModelObjectGraph
    {
        private ModelObjectReferenceGraph<DopingCombination> primaryDoping;
        private ModelObjectReferenceGraph<DopingCombination> counterDoping;
        private ModelObjectReferenceGraph<BuildingBlock> buildingBlock;
        private bool useCounterDoping;
        private int priority;

        /// <summary>
        ///     The doping which is applied
        /// </summary>
        [XmlElement("PrimaryDoping")]
        [JsonProperty("PrimaryDoping")]
        public ModelObjectReferenceGraph<DopingCombination> PrimaryDoping
        {
            get => primaryDoping;
            set => SetProperty(ref primaryDoping, value);
        }

        /// <summary>
        ///     The doping to compensate the primary doping
        /// </summary>
        [XmlElement("CounterDoping")]
        [JsonProperty("CounterDoping")]
        public ModelObjectReferenceGraph<DopingCombination> CounterDoping
        {
            get => counterDoping;
            set => SetProperty(ref counterDoping, value);
        }

        /// <summary>
        ///     The building block in which the doping is used
        /// </summary>
        [XmlElement("BuildingBlock")]
        [JsonProperty("BuildingBlock")]
        public ModelObjectReferenceGraph<BuildingBlock> BuildingBlock
        {
            get => buildingBlock;
            set => SetProperty(ref buildingBlock, value);
        }

        /// <summary>
        ///     Flag to indicate if a counter doping should be used
        /// </summary>
        [XmlAttribute("UseCounterDoping")]
        [JsonProperty("UseCounterDoping")]
        public bool UseCounterDoping
        {
            get => useCounterDoping;
            set => SetProperty(ref useCounterDoping, value);
        }

        /// <summary>
        ///     The priority order in which the doping is applied
        /// </summary>
        [XmlAttribute("Priority")]
        [JsonProperty("Priority")]
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
                CounterDoping = new DopingCombination {Key = CounterDoping.Key},
                PrimaryDoping = new DopingCombination {Key = PrimaryDoping.Key},
                BuildingBlock = new BuildingBlock {Key = BuildingBlock.Key},
                UseCounterDoping = UseCounterDoping,
                Priority = Priority
            };
            return obj;
        }
    }
}