using System.Runtime.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Contains the information about the particles and unit cell positions which are involved in the doping process
    /// </summary>
    [DataContract(Name = "Doping")]
    public class DopingCombination : ModelObject, IDopingCombination
    {
        /// <summary>
        ///     Dopand particle
        /// </summary>
        [DataMember]
        [UseTrackedReferences]
        public IParticle Dopant { set; get; }

        /// <summary>
        ///     Particle that is doped
        /// </summary>
        [DataMember]
        [UseTrackedReferences]
        public IParticle Dopable { set; get; }

        /// <summary>
        ///     Unit cell position that should be doped
        /// </summary>
        [DataMember]
        [UseTrackedReferences]
        public ICellReferencePosition CellReferencePosition { get; set; }

        /// <inheritdoc />
        public double GetChargeDelta()
        {
            return Dopant.Charge - Dopable.Charge;
        }

        /// <summary>
        ///     Get the type name string
        /// </summary>
        /// <returns></returns>
        public override string ObjectName => "DopingCombination";

        /// <summary>
        ///     Copies the information from the provided parameter interface and returns the object (Returns null if type mismatch)
        /// </summary>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject modelObject)
        {
            if (!(CastIfNotDeprecated<IDopingCombination>(modelObject) is var casted)) return null;

            Dopant = casted.Dopant;
            Dopable = casted.Dopable;
            CellReferencePosition = casted.CellReferencePosition;
            return this;
        }
    }
}