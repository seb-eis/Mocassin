using System;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Particles
{
    /// <inheritdoc cref="IParticleManager" />
    internal class ParticleManager : ModelManager<ParticleModelData, ParticleModelCache, ParticleDataManager, ParticleCacheManager,
        ParticleInputManager, ParticleQueryManager, ParticleEventManager, ParticleUpdateManager>, IParticleManager
    {
        /// <summary>
        ///     Particle manager data input access port that handles user defined model data changes to the manager
        /// </summary>
        public new IParticleInputPort InputAccess => InputManager;

        /// <inheritdoc />
        public IParticleQueryPort DataAccess => QueryManager;

        /// <inheritdoc />
        public new IParticleEventPort EventAccess => EventManager;

        /// <inheritdoc />
        public ParticleManager(IModelProject modelProject, ParticleModelData data)
            : base(modelProject, data)
        {
        }

        /// <inheritdoc />
        public override IValidationService CreateValidationService(ProjectSettings settings)
        {
            if (!settings.TryGetModuleSettings(out MocassinParticleSettings moduleSettings))
                throw new InvalidOperationException("Settings object for the particle module is missing");

            return new ParticleValidationService(moduleSettings, ModelProject);
        }

        /// <inheritdoc />
        public override Type GetManagerInterfaceType() => typeof(IParticleManager);
    }
}