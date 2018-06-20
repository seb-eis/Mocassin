using ICon.Model.Basic;
using ICon.Model.Particles;
using ICon.Symmetry.Analysis;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Represents an access port for on-demand extended lattice data that is automatically cached
    /// </summary>
    public interface ILatticeCachePort : IModelCachePort
    {
        /// <summary>
        /// Create Supercellwrapper
        /// </summary>
        /// <returns></returns>
        SupercellWrapper<IParticle> CreateLattice();

        /// <summary>
        /// Create WorkLattice (only for testing)
        /// </summary>
        /// <returns></returns>
        WorkLattice CreateWorkLattice();
    }
}
