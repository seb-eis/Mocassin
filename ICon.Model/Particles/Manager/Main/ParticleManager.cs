using System;

using ICon.Model.ProjectServices;
using ICon.Model.Basic;

namespace ICon.Model.Particles
{
    /// <summary>
    /// Basic particle manager for the simulation model process that handles and distributes particles and particle sets
    /// </summary>
    internal class ParticleManager : ModelManager<ParticleModelData, ParticleDataCache, ParticleDataManager, ParticleCacheManager, ParticleInputManager, ParticleQueryManager, ParticleEventManager, ParticleUpdateManager>, IParticleManager
    {
        /// <summary>
        /// Particle manager data input access port that handles user defined model data changes to the manager
        /// </summary>
        public new IParticleInputPort InputPort => InputManager;

        /// <summary>
        /// Particle manager query access port that handles safe read-only data access through reader provision and hanling of custom data queries
        /// </summary>
        public IParticleQueryPort QueryPort => QueryManager;

        /// <summary>
        /// Particle manager event access port that provides subscriptions to 'hot' manager events (i.e. changes in the base data object)
        /// </summary>
        public new IParticleEventPort EventPort => EventManager;

        /// <summary>
        /// Creates new particle manager using the provided project service and data object
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="data"></param>
        public ParticleManager(IProjectServices projectServices, ParticleModelData data) : base(projectServices, data)
        {

        }

        /// <summary>
        /// Get the particle validation service
        /// </summary>
        /// <returns></returns>
        public override IValidationService MakeValidationService(ProjectSettingsData settingsData)
        {
            return new ParticleValidationService(settingsData.ParticleSettings, ProjectServices);
        }

        /// <summary>
        /// Get the manager interface type (IParticleManager)
        /// </summary>
        /// <returns></returns>
        public override Type GetManagerInterfaceType()
        {
            return typeof(IParticleManager);
        }
    }
}
