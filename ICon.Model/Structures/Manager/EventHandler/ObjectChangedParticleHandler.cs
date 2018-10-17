using System;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Structures.Handler
{
    /// <summary>
    /// Event handler that manages the processing of object change events that the structure manager receives from the particle manager event port
    /// </summary>
    internal class ObjectChangedParticleHandler : ObjectChangedEventHandler<IParticleEventPort, StructureModelData, StructureEventManager>
    {
        /// <summary>
        /// Create new handler using the provided project services, data access provider and event manager
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="dataAccessorSource"></param>
        /// <param name="eventManager"></param>
        public ObjectChangedParticleHandler(IModelProject modelProject, DataAccessSource<StructureModelData> dataAccessorSource, StructureEventManager eventManager)
            : base(modelProject, dataAccessorSource, eventManager)
        {

        }

        /// <summary>
        /// Event reaction to a particle change in the particle manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleParticleChange(IModelObjectEventArgs<IParticle> eventArgs)
        {
            Console.WriteLine($"{eventArgs.ToString()} received on {ToString()}");
            return new ConflictReport();
        }

        /// <summary>
        /// Event reaction to a particle set change in the particle manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleParticleSetChange(IModelObjectEventArgs<IParticleSet> eventArgs)
        {
            Console.WriteLine($"{eventArgs.ToString()} received on {ToString()}");
            return new ConflictReport();
        }
    }
}
