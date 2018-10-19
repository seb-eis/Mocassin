using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles.ConflictHandling;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Particles
{
    /// <summary>
    ///     Basic particle input manager that handles the controlled access to the particle manager
    /// </summary>
    internal class ParticleInputManager : ModelInputManager<ParticleModelData, IParticleDataPort, ParticleEventManager>, IParticleInputPort
    {
        /// <inheritdoc />
        public ParticleInputManager(ParticleModelData modelData, ParticleEventManager eventManager, IModelProject project)
            : base(modelData, eventManager, project)
        {
        }

        /// <summary>
        ///     Registers a new particle to the manager if it passes validation (Awaits distribution of affiliated events in case
        ///     of operation success)
        /// </summary>
        /// <param name="particle"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewParticle(IParticle particle)
        {
            return DefaultRegisterModelObject(particle, accessor => accessor.Query(data => data.Particles));
        }

        /// <summary>
        ///     Registers a new particle set to the manager if it passes validation (Awaits distribution of affiliated events in
        ///     case of operation success)
        /// </summary>
        /// <param name="particleSet"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewParticleSet(IParticleSet particleSet)
        {
            return DefaultRegisterModelObject(particleSet, accessor => accessor.Query(data => data.ParticleSets));
        }

        /// <summary>
        ///     Removes a particle from the manager by deprecation if possible (Awaits distribution of affiliated events in case of
        ///     operation success)
        /// </summary>
        /// <param name="particle"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveParticle(IParticle particle)
        {
            return DefaultRemoveModelObject(particle, accessor => accessor.Query(data => data.Particles), 0);
        }

        /// <summary>
        ///     Removes a particle set from the manager by deprecation if possible (Awaits distribution of affiliated events in
        ///     case of operation success)
        /// </summary>
        /// <param name="particleSet"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveParticleSet(IParticleSet particleSet)
        {
            return DefaultRemoveModelObject(particleSet, accessor => accessor.Query(data => data.ParticleSets), 0);
        }

        /// <summary>
        ///     Replaces a particle in the manager by another if the new one passes validation (Awaits distribution of affiliated
        ///     events in case of operation success)
        /// </summary>
        /// <param name="orgParticle"></param>
        /// <param name="newParticle"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceParticle(IParticle orgParticle, IParticle newParticle)
        {
            return DefaultReplaceModelObject(orgParticle, newParticle, accessor => accessor.Query(data => data.Particles));
        }

        /// <summary>
        ///     Replaces a particle set in the manager by another if the new one passes validation (Awaits distribution of
        ///     affiliated events in case of operation success)
        /// </summary>
        /// <param name="orgSet"></param>
        /// <param name="newSet"></param>
        /// <returns></returns>
        [DataOperation(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceParticleSet(IParticleSet orgSet, IParticleSet newSet)
        {
            return DefaultReplaceModelObject(orgSet, newSet, accessor => accessor.Query(data => data.ParticleSets));
        }

        /// <inheritdoc />
        [DataOperation(DataOperationType.ObjectCleaning)]
        protected override IOperationReport TryCleanDeprecatedData()
        {
            return DefaultCleanDeprecatedData();
        }

        /// <inheritdoc />
        protected override IDataConflictHandlerProvider<ParticleModelData> CreateDataConflictHandlerProvider()
        {
            return new ParticleDataConflictHandlerProvider(ModelProject);
        }
    }
}