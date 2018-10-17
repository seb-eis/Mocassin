using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Particles.ConflictHandling
{
    /// <summary>
    ///     Resolver provider for all particle conflict resolvers that handle internal data conflicts of the particle manager
    /// </summary>
    public class ParticleDataConflictHandlerProvider : DataConflictHandlerProvider<ParticleModelData>
    {
        /// <inheritdoc />
        public ParticleDataConflictHandlerProvider(IModelProject modelProject)
            : base(modelProject)
        {
        }
    }
}