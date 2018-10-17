using Mocassin.Model.Basic;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    /// Represents a query port for query based access to the reference and extended lattices data
    /// </summary>
    public interface ILatticeQueryPort : IModelQueryPort<ILatticeDataPort>, IModelQueryPort<ILatticeCachePort>
    {

    }
}
