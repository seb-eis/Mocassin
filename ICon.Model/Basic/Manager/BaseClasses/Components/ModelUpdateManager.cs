using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ICon.Framework.Processing;
using ICon.Framework.Reflection;
using ICon.Model.ProjectServices;

namespace ICon.Model.Basic
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
            if (!(Connections.Keys.SingleOrDefault(key => key.IsInstanceOfType(eventPort)) is Type type))
                return false;

            if (Connections[type] != null)
                return false;

            if (!(ConnectionPipeline.Process(eventPort) is IDisposable unsubscriber))
                return false;

            Connections[type] = unsubscriber;
            return true;

        }

        /// <inheritdoc />
        public void Disconnect(IModelEventPort eventPort)
        {
            if (!(Connections.Keys.SingleOrDefault(key => key.IsInstanceOfType(eventPort)) is Type type))
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
                    creator.CreateDelegates(value, info => info.GetCustomAttribute(typeof(EventPortConnectorAttribute)) != null)
                        .SingleOrDefault());

            var bundleSubscriber = Activator.CreateInstance(typeof(EventPortSubscriber<>).MakeGenericType(eventSourceType), delegates);

            return processorCreator
                .CreateProcessors<IDisposable>(bundleSubscriber,
                    info => info.GetCustomAttribute(typeof(EventPortConnectorAttribute)) != null, flags)
                .SingleOrDefault();
        }

        /// <summary>
        ///     Get the empty processor handler that always returns a null disposables as reaction to not required connection requests
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
        protected IProjectServices ProjectServices { get; set; }

        /// <summary>
        ///     Structure event manager instances to notify dependent modules of critical changes due to the update process
        /// </summary>
        protected TEventManager EventManager { get; set; }

        /// <summary>
        ///     Provider for safe data accessors to the reference data object
        /// </summary>
        protected DataAccessSource<TData> DataAccessorSource { get; set; }

        /// <summary>
        ///     Creates new update manager for the provided base data, extended data, event manager and project services
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="eventManager"></param>
        /// <param name="projectServices"></param>
        protected ModelUpdateManager(TData baseData, TEventManager eventManager, IProjectServices projectServices)
        {
            if (baseData == null) 
                throw new ArgumentNullException(nameof(baseData));

            ProjectServices = projectServices ?? throw new ArgumentNullException(nameof(projectServices));
            EventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            DataAccessorSource = Basic.DataAccessorSource.Create(baseData, projectServices.AccessLockSource);
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
                var handler = Activator.CreateInstance(propertyInfo.PropertyType, ProjectServices, DataAccessorSource, EventManager);
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