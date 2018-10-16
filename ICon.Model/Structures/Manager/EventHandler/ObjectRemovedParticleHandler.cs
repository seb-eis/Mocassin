using System;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.Particles;
using ICon.Model.ProjectServices;

namespace ICon.Model.Structures.Handler
{
    /// <summary>
    /// Event handler that manages the processing of object removal events that the structure manager receives from the particle manager event port
    /// </summary>
    internal class ObjectRemovedParticleHandler : ObjectRemovedEventHandler<IParticleEventPort, StructureModelData, StructureEventManager>
    {
        /// <summary>
        /// Create new handler using the provided project services, data access provider and event manager
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="dataAccessorSource"></param>
        /// <param name="eventManager"></param>
        public ObjectRemovedParticleHandler(IProjectServices projectServices, DataAccessSource<StructureModelData> dataAccessorSource, StructureEventManager eventManager)
            : base(projectServices, dataAccessorSource, eventManager)
        {

        }

        /// <summary>
        /// Event reaction to a removed particle in the particle manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleParticleRemoval(IModelObjectEventArgs<IParticle> eventArgs)
        {
            Console.WriteLine($"{eventArgs.ToString()} received on {ToString()}");
            return new ConflictReport();
        }

        /// <summary>
        /// Event reaction to a removed particle set in the particle manager
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        [EventHandlingMethod]
        protected IConflictReport HandleParticleSetRemoval(IModelObjectEventArgs<IParticleSet> eventArgs)
        {
            Console.WriteLine($"{eventArgs.ToString()} received on {ToString()}");
            return new ConflictReport();
        }
    }
}
