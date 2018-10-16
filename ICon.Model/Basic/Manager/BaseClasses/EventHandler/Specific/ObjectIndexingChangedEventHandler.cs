using System;

using ICon.Model.ProjectServices;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Abstract base class for event handlers that handle object list reindex operations provided by the specified event port
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    internal abstract class ObjectIndexingChangedEventHandler<T1, T2, T3> : ModelEventHandler<T1, T2, T3>
        where T1 : IModelEventPort
        where T2 : ModelData
        where T3 : ModelEventManager
    {
        /// <inheritdoc />
        protected ObjectIndexingChangedEventHandler(IProjectServices projectServices, DataAccessSource<T2> dataAccessorSource, T3 eventManager)
            : base(projectServices, dataAccessorSource, eventManager)
        {

        }

        /// <inheritdoc />
        public override IDisposable SubscribeToEvent(T1 eventPort)
        {
            return eventPort.WhenModelIndexingChanged.Subscribe(ProcessEvent);
        }
    }
}
