using Mocassin.Model.Basic;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Represents an access port for on-demand extended lattice data that is automatically cached
    /// </summary>
    public interface ILatticeCachePort : IModelCachePort
    {
        /// <summary>
        ///     Get the default <see cref="IDopedByteLatticeSource" /> for doped lattice provision
        /// </summary>
        /// <returns></returns>
        IDopedByteLatticeSource GetDefaultByteLatticeSource();
    }
}