using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Contains the information about the particles and unit cell positions which are involved in the doping process
    /// </summary>
    public class DopingCombination : ModelObject, IDopingCombination
    {
        /// <summary>
        ///     Dopand particle
        /// </summary>
        [UseTrackedData]
        public IParticle Dopant { set; get; }

        /// <summary>
        ///     Particle that is doped
        /// </summary>
        [UseTrackedData]
        public IParticle Dopable { set; get; }

        /// <summary>
        ///     Unit cell position that should be doped
        /// </summary>
        [UseTrackedData]
        public ICellSite CellSite { get; set; }

        /// <summary>
        ///     Get the type name string
        /// </summary>
        /// <returns></returns>
        public override string ObjectName => "DopingCombination";

        /// <inheritdoc />
        public double GetChargeDelta() => Dopant.Charge - Dopable.Charge;

        /// <summary>
        ///     Copies the information from the provided parameter interface and returns the object (Returns null if type mismatch)
        /// </summary>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject modelObject)
        {
            if (!(CastIfNotDeprecated<IDopingCombination>(modelObject) is {} dopingCombination)) return null;

            Dopant = dopingCombination.Dopant;
            Dopable = dopingCombination.Dopable;
            CellSite = dopingCombination.CellSite;
            return this;
        }
    }
}