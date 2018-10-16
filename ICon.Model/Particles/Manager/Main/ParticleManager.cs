using System;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Particles
{
    /// <inheritdoc cref="ICon.Model.Particles.IParticleManager"/>
    internal class ParticleManager : ModelManager<ParticleModelData, ParticleModelCache, ParticleDataManager, ParticleCacheManager,
        ParticleInputManager, ParticleQueryManager, ParticleEventManager, ParticleUpdateManager>, IParticleManager
    {
        /// <summary>
        ///     Particle manager data input access port that handles user defined model data changes to the manager
        /// </summary>
        public new IParticleInputPort InputPort => InputManager;

        /// <inheritdoc />
        public IParticleQueryPort QueryPort => QueryManager;

        /// <inheritdoc />
        public new IParticleEventPort EventPort => EventManager;

        /// <inheritdoc />
        public ParticleManager(IProjectServices projectServices, ParticleModelData data)
            : base(projectServices, data)
        {
        }

        /// <inheritdoc />
        public override IValidationService CreateValidationService(ProjectSettingsData settingsData)
        {
            return new ParticleValidationService(settingsData.ParticleSettings, ProjectServices);
        }

        /// <inheritdoc />
        public override Type GetManagerInterfaceType()
        {
            return typeof(IParticleManager);
        }
    }
}