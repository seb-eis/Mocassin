using System;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.Particles;
using ICon.Model.ProjectServices;

namespace ICon.Model.Structures.Handler
{
    /// <summary>
    /// Event handler that manages the processing of object reindexing events that the structure manager receives from the particle manager event port
    /// </summary>
    internal class ReindexedParticleObjectsHandler : NewObjectsEventHandler<IParticleEventPort, StructureModelData, StructureEventManager>
    {
        /// <summary>
        /// Create new handler using the provided project services, data access provider and event manager
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="dataAccessorProvider"></param>
        /// <param name="eventManager"></param>
        public ReindexedParticleObjectsHandler(IProjectServices projectServices, DataAccessProvider<StructureModelData> dataAccessorProvider, StructureEventManager eventManager)
            : base(projectServices, dataAccessorProvider, eventManager)
        {

        }

        /// <summary>
        /// Event reaction to a reindexed particle list in the particle manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleParticleListReindexing(IModelIndexingEventArgs<IParticle> eventArgs)
        {
            Console.WriteLine($"{eventArgs.ToString()} received on {ToString()}");
            return new ConflictReport();
        }

        /// <summary>
        /// Event reaction to a reindexed particle set list in the particle manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleParticleSetListReindexing(IModelIndexingEventArgs<IParticleSet> eventArgs)
        {
            Console.WriteLine($"{eventArgs.ToString()} received on {ToString()}");
            return new ConflictReport();
        }
    }
}
