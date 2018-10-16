using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Particles
{
    /// <summary>
    ///     Basic implementation of the particle update manager that handles event subscriptions and reactions to external
    ///     change events
    /// </summary>
    internal class ParticleUpdateManager : ModelUpdateManager<ParticleModelData, ParticleEventManager>
    {
        /// <inheritdoc />
        public ParticleUpdateManager(ParticleModelData baseData, ParticleEventManager eventManager, IProjectServices projectServices)
            : base(baseData, eventManager, projectServices)
        {
        }
    }
}