using System;

using ICon.Model.ProjectServices;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Abstract base class for event handlers that handle object removals provided by the specfified event port
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    internal abstract class RemovedObjectsEventHandler<T1, T2, T3> : ModelEventHandler<T1, T2, T3>
        where T1 : IModelEventPort
        where T2 : ModelData
        where T3 : ModelEventManager
    {
        /// <summary>
        /// Create new object added event handler
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="dataAccessorProvider"></param>
        /// <param name="eventManager"></param>
        public RemovedObjectsEventHandler(IProjectServices projectServices, DataAccessProvider<T2> dataAccessorProvider, T3 eventManager)
            : base(projectServices, dataAccessorProvider, eventManager)
        {

        }

        /// <summary>
        /// Connects the pipeline processing function to the object added event of the port and redirects report to the affiliated property
        /// </summary>
        /// <param name="eventPort"></param>
        /// <returns></returns>
        public override IDisposable SubscribeToEvent(T1 eventPort)
        {
            return eventPort.WhenModelObjectRemoved.Subscribe(ProcessEvent);
        }
    }
}
