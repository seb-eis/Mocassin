using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Structures.Handler
{
    /// <summary>
    ///     Event handler that manages the processing of object removal events that the structure manager receives from the
    ///     particle manager event port
    /// </summary>
    internal class
        ParticleObjectRemovedEventHandler : ObjectRemovedEventHandler<IParticleEventPort, StructureModelData, StructureEventManager>
    {
        /// <inheritdoc />
        public ParticleObjectRemovedEventHandler(IModelProject modelProject, DataAccessorSource<StructureModelData> dataAccessorSource,
            StructureEventManager eventManager)
            : base(modelProject, dataAccessorSource, eventManager)
        {
        }

        /// <summary>
        ///     Event reaction to a removed particle in the particle manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleParticleRemoval(IModelObjectEventArgs<IParticle> eventArgs) => DummyHandleEvent(eventArgs);

        /// <summary>
        ///     Event reaction to a removed particle set in the particle manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleParticleSetRemoval(IModelObjectEventArgs<IParticleSet> eventArgs) => DummyHandleEvent(eventArgs);
    }
}