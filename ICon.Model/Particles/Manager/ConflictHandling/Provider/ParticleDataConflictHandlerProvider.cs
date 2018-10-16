using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Particles.ConflictHandling
{
    /// <summary>
    ///     Resolver provider for all particle conflict resolvers that handle internal data conflicts of the particle manager
    /// </summary>
    public class ParticleDataConflictHandlerProvider : DataConflictHandlerProvider<ParticleModelData>
    {
        /// <inheritdoc />
        public ParticleDataConflictHandlerProvider(IProjectServices projectServices)
            : base(projectServices)
        {
        }
    }
}