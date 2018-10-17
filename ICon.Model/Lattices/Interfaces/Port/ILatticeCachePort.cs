using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Symmetry.Analysis;

namespace Mocassin.Model.Lattices
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
        SupercellAdapter<IParticle> CreateLattice();

        /// <summary>
        /// Create WorkLattice (only for testing)
        /// </summary>
        /// <returns></returns>
        WorkLattice CreateWorkLattice();
    }
}
