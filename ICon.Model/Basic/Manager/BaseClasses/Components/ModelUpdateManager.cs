using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mocassin.Framework.Processing;
using Mocassin.Framework.Reflection;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Abstract base class for all update managers that handle updates through event port subscriptions
    /// </summary>
    internal abstract class ModelUpdateManager : IModelUpdatePort
    {
        /// <summary>
        ///     Connection dictionary that stores event port subscription disposables by event port
        /// </summary>
        protected Dictionary<Type, IDisposable> Connections { get; set; }

        /// <summary>
        ///     Pipeline that handles the processing of new port connections and returns unsubscribe disposables
        /// </summary>
        protected BreakPipeline<IDisposable> ConnectionPipeline { get; set; }

        /// <inheritdoc />
        public bool Connect(IModelEventPort eventPort)
        {
            if (!(Connections.Keys.SingleOrDefault(key => key.IsInstanceOfType(eventPort)) is { } type))
                return false;

            if (Connections[type] != null)
                return false;

            if (!(ConnectionPipeline.Process(eventPort) is { } subscription))
                return false;

            Connections[type] = subscription;
            return true;
        }

        /// <inheritdoc />
        public void Disconnect(IModelEventPort eventPort)
        {
            if (!(Connections.Keys.SingleOrDefault(key => key.IsInstanceOfType(eventPort)) is { } type))
                return;

            Connections[type]?.Dispose();
            Connections[type] = null;
        }

        /// <inheritdoc />
        public void DisconnectAll()
        {
            foreach (var item in Connections)
                item.Value?.Dispose();
        }

        /// <summary>
        ///     Searches the update manager for marked connection functions and builds the objects processors
        /// </summary>
        /// <returns></returns>
        protected virtual List<IObjectProcessor<IDisposable>> MakeConnectionProcessors()
        {
            var result = new List<IObjectProcessor<IDisposable>>();
            foreach (var item in GetUpdateHandlingPropertiesAndCreateConnectionDictionary())
                result.Add(MakeBundleSubscriber(item.Value, item.Key));

            return result;
        }

        /// <summary>
        ///     Searches the class for all properties that are marked as update handlers and returns them sorted by the type of
        ///     marked event port. Adds all entries to the connection dictionary
        /// </summary>
        /// <returns></returns>
        protected virtual Dictionary<Type, List<PropertyInfo>> GetUpdateHandlingPropertiesAndCreateConnectionDictionary()
        {
            var result = new Dictionary<Type, List<PropertyInfo>>();
            foreach (var propertyInfo in GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (!(propertyInfo.GetCustomAttribute(typeof(UpdateHandlerAttribute)) is UpdateHandlerAttribute attribute))
                    continue;

                if (!result.Keys.Contains(attribute.EventSourceType))
                    result[attribute.EventSourceType] = new List<PropertyInfo>();

                result[attribute.EventSourceType].Add(propertyInfo);
            }

            Connections = new Dictionary<Type, IDisposable>();
            foreach (var key in result.Keys) Connections[key] = null;

            return result;
        }

        /// <summary>
        ///     Takes the property infos that handle one type of event port subscription and create the multi-subscription object
        ///     processor through a subscription package creator
        /// </summary>
        /// <param name="propertyInfos"></param>
        /// <param name="eventSourceType"></param>
        /// <returns></returns>
        protected virtual IObjectProcessor<IDisposable> MakeBundleSubscriber(IEnumerable<PropertyInfo> propertyInfos, Type eventSourceType)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            var creator = new DelegateCreator();
            var processorCreator = new ObjectProcessorCreator();
            var delegates = propertyInfos
                .Select(property => property.GetValue(this))
                .Select(value =>
                    creator.CreateWhere(value, info => info.GetCustomAttribute(typeof(EventPortConnectorAttribute)) != null)
                        .SingleOrDefault());

            var bundleSubscriber = Activator.CreateInstance(typeof(EventPortSubscriber<>).MakeGenericType(eventSourceType), delegates);

            return processorCreator
                .CreateProcessors<IDisposable>(bundleSubscriber,
                    info => info.GetCustomAttribute(typeof(EventPortConnectorAttribute)) != null, flags)
                .SingleOrDefault();
        }

        /// <summary>
        ///     Get the empty processor handler that always returns a null disposables as reaction to not required connection
        ///     requests
        /// </summary>
        /// <returns></returns>
        protected virtual IObjectProcessor<IDisposable> GetOnCannotProcessConnection()
        {
            return new ObjectProcessor<object, IDisposable>(obj => null);
        }
    }

    /// <summary>
    ///     Generic abstract base class for implementations of model update managers that handle specific data objects and
    ///     distribute update followups through a specific event manager
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TEventManager"></typeparam>
    internal abstract class ModelUpdateManager<TData, TEventManager> : ModelUpdateManager
        where TData : ModelData
        where TEventManager : ModelEventManager
    {
        /// <summary>
        ///     Shared project services interface
        /// </summary>
        protected IModelProject ModelProject { get; set; }

        /// <summary>
        ///     Structure event manager instances to notify dependent modules of critical changes due to the update process
        /// </summary>
        protected TEventManager EventManager { get; set; }

        /// <summary>
        ///     Provider for safe data accessors to the reference data object
        /// </summary>
        protected DataAccessorSource<TData> DataAccessorSource { get; set; }

        /// <summary>
        ///     Creates new update manager for the provided base data, event manager and project services
        /// </summary>
        /// <param name="modelData"></param>
        /// <param name="eventManager"></param>
        /// <param name="modelProject"></param>
        protected ModelUpdateManager(TData modelData, TEventManager eventManager, IModelProject modelProject)
        {
            if (modelData == null)
                throw new ArgumentNullException(nameof(modelData));

            ModelProject = modelProject ?? throw new ArgumentNullException(nameof(modelProject));
            EventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            DataAccessorSource = Basic.DataAccessorSource.Create(modelData, modelProject.AccessLockSource);
            InitializeEventHandlingSystem();
        }

        /// <summary>
        ///     Searches and initializes all marked update handler properties
        /// </summary>
        protected void InitializeUpdateHandlers()
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
            foreach (var propertyInfo in GetType().GetProperties(flags)
                .Where(info => info.GetCustomAttribute(typeof(UpdateHandlerAttribute)) != null))
            {
                var handler = Activator.CreateInstance(propertyInfo.PropertyType, ModelProject, DataAccessorSource, EventManager);
                propertyInfo.SetValue(this, handler);
            }
        }

        /// <summary>
        ///     Initializes the event handling system (Event handlers, connection pipelines, ...)
        /// </summary>
        protected void InitializeEventHandlingSystem()
        {
            InitializeUpdateHandlers();
            ConnectionPipeline = new BreakPipeline<IDisposable>(GetOnCannotProcessConnection(), MakeConnectionProcessors());
        }
    }
}