using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Particles
{
    /// <summary>
    ///     Basic implementation of the particle update manager that handles event subscriptions and reactions to external
    ///     change events
    /// </summary>
    internal class ParticleUpdateManager : ModelUpdateManager<ParticleModelData, ParticleEventManager>
    {
        /// <inheritdoc />
        public ParticleUpdateManager(ParticleModelData baseData, ParticleEventManager eventManager, IModelProject modelProject)
            : base(baseData, eventManager, modelProject)
        {
        }
    }
}