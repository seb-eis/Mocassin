using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Structures.Handler
{
    /// <summary>
    ///     Event handler that manages the processing of object change events that the structure manager receives from the
    ///     particle manager event port
    /// </summary>
    internal class ParticleObjectChangedEventHandler : ObjectChangedEventHandler<IParticleEventPort, StructureModelData, StructureEventManager>
    {
        /// <inheritdoc />
        public ParticleObjectChangedEventHandler(IModelProject modelProject, DataAccessorSource<StructureModelData> dataAccessorSource,
            StructureEventManager eventManager)
            : base(modelProject, dataAccessorSource, eventManager)
        {
        }

        /// <summary>
        ///     Event reaction to a particle change in the particle manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleParticleChange(IModelObjectEventArgs<IParticle> eventArgs)
        {
            return new ConflictReport();
        }

        /// <summary>
        ///     Event reaction to a particle set change in the particle manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleParticleSetChange(IModelObjectEventArgs<IParticleSet> eventArgs)
        {
            return new ConflictReport();
        }
    }
}