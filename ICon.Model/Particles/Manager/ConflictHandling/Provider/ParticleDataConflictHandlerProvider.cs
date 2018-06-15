using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Particles.ConflictHandling
{
    /// <summary>
    /// Resolver provider for all particle conflict resolvers that handle internal data conflicts of the particle manager
    /// </summary>
    public class ParticleDataConflictHandlerProvider : DataConflictHandlerProvider<ParticleModelData>
    {
        /// <summary>
        /// Creates new particle data conflict resolver provider with access to the provided project services
        /// </summary>
        /// <param name="projectServices"></param>
        public ParticleDataConflictHandlerProvider(IProjectServices projectServices) : base(projectServices)
        {

        }
    }
}
