using System;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.Particles;
using ICon.Model.ProjectServices;

namespace ICon.Model.Structures.Handler
{
    /// <summary>
    /// Event handler that manages the processing of object change events that the structure manager receives from the particle manager event port
    /// </summary>
    internal class ChangedParticleObjectsHandler : ChangedObjectsEventHandler<IParticleEventPort, StructureModelData, StructureEventManager>
    {
        /// <summary>
        /// Create new handler using the provided project services, data access provider and event manager
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="dataAccessorProvider"></param>
        /// <param name="eventManager"></param>
        public ChangedParticleObjectsHandler(IProjectServices projectServices, DataAccessProvider<StructureModelData> dataAccessorProvider, StructureEventManager eventManager)
            : base(projectServices, dataAccessorProvider, eventManager)
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
