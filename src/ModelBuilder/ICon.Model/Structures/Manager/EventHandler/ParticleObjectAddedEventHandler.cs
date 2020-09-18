using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Structures.Handler
{
    /// <summary>
    ///     Event handler that manages the processing of object added events that the structure manager receives from the
    ///     particle manager event port
    /// </summary>
    internal class ParticleObjectAddedEventHandler : ObjectAddedEventHandler<IParticleEventPort, StructureModelData, StructureEventManager>
    {
        /// <inheritdoc />
        public ParticleObjectAddedEventHandler(IModelProject modelProject, DataAccessorSource<StructureModelData> dataAccessorSource,
            StructureEventManager eventManager)
            : base(modelProject, dataAccessorSource, eventManager)
        {
        }

        /// <summary>
        ///     Event reaction to a new particle in the particle manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleNewParticle(IModelObjectEventArgs<IParticle> eventArgs) => new ConflictReport();

        /// <summary>
        ///     Event reaction to a new particle set in the particle manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleNewParticleSet(IModelObjectEventArgs<IParticleSet> eventArgs) => new ConflictReport();
    }
}