using System;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Structures.Handler
{
    /// <summary>
    /// Event handler that manages the processing of object reindexing events that the structure manager receives from the particle manager event port
    /// </summary>
    internal class ReindexedParticleObjectsHandler : ObjectAddedEventHandler<IParticleEventPort, StructureModelData, StructureEventManager>
    {
        /// <summary>
        /// Create new handler using the provided project services, data access provider and event manager
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="dataAccessorSource"></param>
        /// <param name="eventManager"></param>
        public ReindexedParticleObjectsHandler(IModelProject modelProject, DataAccessSource<StructureModelData> dataAccessorSource, StructureEventManager eventManager)
            : base(modelProject, dataAccessorSource, eventManager)
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
