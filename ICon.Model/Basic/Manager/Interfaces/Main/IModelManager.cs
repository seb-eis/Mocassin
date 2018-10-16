using System;
using ICon.Model.ProjectServices;

namespace ICon.Model.Basic
{
    /// <summary>
    ///     Represents a mocassin model manager that handles a specific part of the model process
    /// </summary>
    public interface IModelManager
    {
        /// <summary>
        ///     Get the event port interface for this manager (General interface)
        /// </summary>
        IModelEventPort EventPort { get; }

        /// <summary>
        ///     Get the input port for this manager (General interface)
        /// </summary>
        IModelInputPort InputPort { get; }

        /// <summary>
        ///     Makes a new validation service for this manager from the project settings data
        /// </summary>
        /// <returns></returns>
        IValidationService CreateValidationService(ProjectSettingsData settingsData);

        /// <summary>
        ///     Disconnects the manager from all linked manager ports to prepare for deletion or move operation
        /// </summary>
        /// <returns></returns>
        void DisconnectManager();

        /// <summary>
        ///     Tries to connect manager to an event port. Returns false if failed
        /// </summary>
        /// <param name="eventPort"></param>
        bool TryConnectManager(IModelEventPort eventPort);

        /// <summary>
        ///     Get the interface type of the manager
        /// </summary>
        /// <returns></returns>
        Type GetManagerInterfaceType();
    }

    /// <summary>
    ///     Generic model manager interface that supplies input, event and query port for requests to the manager
    /// </summary>
    /// <typeparam name="TInputPort"></typeparam>
    /// <typeparam name="TEventPort"></typeparam>
    /// <typeparam name="TQueryPort"></typeparam>
    public interface IModelManager<out TInputPort, out TEventPort, out TQueryPort> : IModelManager
        where TInputPort : IModelInputPort
        where TEventPort : IModelEventPort
        where TQueryPort : IModelQueryPort
    {
        /// <summary>
        ///     The manager input port to add, remove or replace model data
        /// </summary>
        new TInputPort InputPort { get; }

        /// <summary>
        ///     The manager data query port for read only access to model or extended data in the manager
        /// </summary>
        TQueryPort QueryPort { get; }

        /// <summary>
        ///     The manager event port that provides push notifications about internal changes to the model data or the manager
        /// </summary>
        new TEventPort EventPort { get; }
    }
}