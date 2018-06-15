using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using ICon.Framework.Operations;
using ICon.Framework.Processing;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Particles
{
    /// <summary>
    /// Basic particle input manager that handles the controlled access to the particle manager
    /// </summary>
    internal class ParticleInputManager : ModelInputManager<ParticleModelData, IParticleDataPort, ParticleEventManager>, IParticleInputPort
    {
        /// <summary>
        /// Creates a new particle input manager from data object, notification port and project services
        /// </summary>
        /// <param name="data"></param>
        /// <param name="manager"></param>
        /// <param name="services"></param>
        public ParticleInputManager(ParticleModelData data, ParticleEventManager manager, IProjectServices services) : base(data, manager, services)
        {

        }

        /// <summary>
        /// Registers a new particle to the manager if it passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="particle"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewParticle(IParticle particle)
        {
            return DefaultRegisterModelObject(particle, accessor => accessor.Query(data => data.Particles));
        }

        /// <summary>
        /// Registers a new particle set to the manager if it passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="particleSet"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.NewObject)]
        protected IOperationReport TryRegisterNewParticleSet(IParticleSet particleSet)
        {
            return DefaultRegisterModelObject(particleSet, accessor => accessor.Query(data => data.ParticleSets));
        }

        /// <summary>
        /// Removes a particle from the manager by deprecation if possible (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="particle"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveParticle(IParticle particle)
        {
            return DefaultRemoveModelObject(particle, accessor => accessor.Query(data => data.Particles), 0);
        }

        /// <summary>
        /// Removes a particle set from the manager by deprecation if possible (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="particleSet"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectRemoval)]
        protected IOperationReport TryRemoveParticleSet(IParticleSet particleSet)
        {
            return DefaultRemoveModelObject(particleSet, accessor => accessor.Query(data => data.ParticleSets), 0);
        }

        /// <summary>
        /// Replaces a particle in the manager by another if the new one passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="orgParticle"></param>
        /// <param name="newParticle"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceParticle(IParticle orgParticle, IParticle newParticle)
        {
            return DefaultReplaceModelObject(orgParticle, newParticle, accessor => accessor.Query(data => data.Particles));
        }

        /// <summary>
        /// Replaces a particle set in the manager by another if the new one passes validation (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <param name="orgSet"></param>
        /// <param name="newSet"></param>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectChange)]
        protected IOperationReport TryReplaceParticleSet(IParticleSet orgSet, IParticleSet newSet)
        {
            return DefaultReplaceModelObject(orgSet, newSet, accessor => accessor.Query(data => data.ParticleSets));
        }

        /// <summary>
        /// Tries to clean deprecated data an creates new model object indexings (Awaits distribution of affiliated events in case of operation success)
        /// </summary>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectCleaning)]
        protected override IOperationReport TryCleanDeprecatedData()
        {
            return DefaultCleanDeprecatedData();
        }

        /// <summary>
        /// Get the particle specific data conflict resolver provider
        /// </summary>
        /// <returns></returns>
        protected override IDataConflictHandlerProvider<ParticleModelData> MakeConflictHandlerProvider()
        {
            return new ConflictHandling.ParticleDataConflictHandlerProvider(ProjectServices);
        }
    }
}
