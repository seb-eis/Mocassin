using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Lattices;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.UI.Xml.Base;
using Newtonsoft.Json;

namespace Mocassin.UI.Xml.LatticeModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Lattices.IDopingCombination" /> model object creation
    /// </summary>
    [XmlRoot("DopingCombination")]
    public class DopingCombinationGraph : ModelObjectGraph
    {
        /// <summary>
        ///     Particle that is replaced by dopant
        /// </summary>
        [XmlElement("Dopable")]
        [JsonProperty("Dopable")]
        public ModelObjectReferenceGraph<Particle> Dopable { get; set; }

        /// <summary>
        ///     Particle that is used as dopant
        /// </summary>
        [XmlElement("Dopant")]
        [JsonProperty("Dopant")]
        public ModelObjectReferenceGraph<Particle> Dopant { get; set; }

        /// <summary>
        ///     Unit cell position on which the doping is applied
        /// </summary>
        [XmlElement("UnitCellPosition")]
        [JsonProperty("UnitCellPosition")]
        public ModelObjectReferenceGraph<UnitCellPosition> UnitCellPosition { get; set; }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            var obj = new DopingCombination
            {
                Dopable = new Particle {Key = Dopable.Key},
                Dopant = new Particle {Key = Dopant.Key},
                UnitCellPosition = new UnitCellPosition {Key = UnitCellPosition.Key}
            };
            return obj;
        }
    }
}