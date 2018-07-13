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

        LatticeCreationProvider GetLatticeCreationProvider();

    }
}
