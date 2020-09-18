using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Set of element indexes and sublattice index for doping information
    /// </summary>
    public interface IDopingCombination : IModelObject
    {
        /// <summary>
        ///     Dopand particle
        /// </summary>
        IParticle Dopant { get; }

        /// <summary>
        ///     Particle that is doped
        /// </summary>
        IParticle Dopable { get; }

        /// <summary>
        ///     unit cell position (contains information about the sublattice)
        /// </summary>
        ICellSite CellSite { get; }

        /// <summary>
        ///     Get the charge delta [Replacement] - [Original] occupation
        /// </summary>
        /// <returns></returns>
        double GetChargeDelta();
    }
}