using System;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Abstract base class for event handlers that handle parameter replacements provided by the specfified event port
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    internal abstract class ParameterChangedEventHandler<T1, T2, T3> : ModelEventHandler<T1, T2, T3>
        where T1 : IModelEventPort
        where T2 : ModelData
        where T3 : ModelEventManager
    {
        /// <inheritdoc />
        protected ParameterChangedEventHandler(IModelProject modelProject, DataAccessSource<T2> dataAccessorSource, T3 eventManager)
            : base(modelProject, dataAccessorSource, eventManager)
        {
        }

        /// <inheritdoc />
        public override IDisposable SubscribeToEvent(T1 eventPort)
        {
            return eventPort.WhenModelParameterChanged.Subscribe(ProcessEvent);
        }
    }
}