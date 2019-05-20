using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Resolver provider for all lattice conflict resolvers
    /// </summary>
    public class LatticeDataConflictResolverProvider : DataConflictHandlerProvider<LatticeModelData>
    {
        /// <summary>
        ///     Creates new Lattice data conflict resolver provider with access to the provided project services
        /// </summary>
        /// <param name="modelProject"></param>
        public LatticeDataConflictResolverProvider(IModelProject modelProject)
            : base(modelProject)
        {
        }
    }
}