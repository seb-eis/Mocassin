﻿using System;

using ICon.Model.ProjectServices;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Abstract base class for event handlers that handle parameter replacements provided by the specfified event port
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    internal abstract class ChangedParameterEventHandler<T1, T2, T3> : ModelEventHandler<T1, T2, T3>
        where T1 : IModelEventPort
        where T2 : ModelData
        where T3 : ModelEventManager
    {
        /// <summary>
        /// Create new handler for parameter change events
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="dataAccessorProvider"></param>
        /// <param name="eventManager"></param>
        protected ChangedParameterEventHandler(IProjectServices projectServices, DataAccessProvider<T2> dataAccessorProvider, T3 eventManager)
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
            return eventPort.WhenModelParameterChanged.Subscribe(ProcessEvent);
        }
    }
}
