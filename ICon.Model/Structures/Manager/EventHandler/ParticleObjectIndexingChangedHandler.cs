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
    internal class ParticleObjectIndexingChangedHandler : ObjectIndexingChangedEventHandler<IParticleEventPort, StructureModelData, StructureEventManager>
    {
        /// <inheritdoc />
        public ParticleObjectIndexingChangedHandler(IModelProject modelProject, DataAccessorSource<StructureModelData> dataAccessorSource, StructureEventManager eventManager)
            : base(modelProject, dataAccessorSource, eventManager)
        {

        }

        /// <summary>
        /// Event reaction to a changed particle list indexing in the particle manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleParticleListReindexing(IModelIndexingEventArgs<IParticle> eventArgs)
        {
            return new ConflictReport();
        }

        /// <summary>
        /// Event reaction to a changed particle set list indexing in the particle manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleParticleSetListReindexing(IModelIndexingEventArgs<IParticleSet> eventArgs)
        {
            return new ConflictReport();
        }
    }
}
