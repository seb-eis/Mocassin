using System;
using System.Reflection;

using ICon.Model.ProjectServices;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Abstract base class for model manager implementations that defines serialization methods and base component access properties
    /// </summary>
    internal abstract class ModelManager : IModelManager
    {
        /// <summary>
        /// Public explicit access to the event port interface
        /// </summary>
        public IModelEventPort EventPort => EventManagerBase;

        /// <summary>
        /// Get the input port for this manager (General interface)
        /// </summary>
        public IModelInputPort InputPort => InputManagerBase;

        /// <summary>
        /// Project services instance that is shared between all model managers of a simulation project and offers various services e.g. validations, numeric comparers etc.
        /// </summary>
        internal IProjectServices ProjectServices { get; set; }

        /// <summary>
        /// Internal access to update manager base class (Cast to actual manager only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal abstract ModelUpdateManager UpdateManagerBase { get; }

        /// <summary>
        /// Internal access to event manager base class (Cast to actual manager only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal abstract ModelEventManager EventManagerBase { get; }

        /// <summary>
        /// Internal access to data manager base class (Cast to actual manager only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal abstract ModelDataManager DataManagerBase { get; }

        /// <summary>
        /// Internal access to cached data manager base class (Cast to actual manager only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal abstract ModelCacheManager CacheManagerBase { get; }

        /// <summary>
        /// Internal access to query manager base class (Cast to actual manager only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal abstract ModelQueryManager QueryManagerBase { get; }

        /// <summary>
        /// Internal access to input manager base class (Cast to actual manager only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal abstract ModelInputManager InputManagerBase { get; }

        /// <summary>
        /// Internal access to data object base class (Cast to actual object only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal abstract ModelData DataObjectBase { get; }

        /// <summary>
        /// Internal access to extended data object base class (Cast to actual object only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal abstract ModelData ExtendedDataObjectBase { get; }

        /// <summary>
        /// Creates new model manager that uses the provided project services and automatically registeres to it
        /// </summary>
        /// <param name="projectServices"></param>
        protected ModelManager(IProjectServices projectServices)
        {
            ProjectServices = projectServices ?? throw new ArgumentNullException(nameof(projectServices));
        }

        /// <summary>
        /// Informs all subscribed modules that the manager is about to disconnect and disposes all own subscriptions to other managers
        /// </summary>
        public virtual void DisconnectManager()
        {
            EventManagerBase?.OnManagerDisconnectRequests.DistributeAsync().Wait();
            UpdateManagerBase?.DisconnectAll();
        }

        /// <summary>
        /// Makes a new validation service for this manager using the project settings data
        /// </summary>
        /// <returns></returns>
        public abstract IValidationService MakeValidationService(ProjectSettingsData settingsData);

        /// <summary>
        /// Get the type of the actual manager interface
        /// </summary>
        /// <returns></returns>
        public abstract Type GetManagerInterfaceType();

        /// <summary>
        /// Tries to connect the manager update port to the provided model event port
        /// </summary>
        /// <param name="eventPort"></param>
        /// <returns></returns>
        public bool TryConnectManager(IModelEventPort eventPort)
        {
            if (UpdateManagerBase == null || EventPort == eventPort)
            {
                return false;
            }
            return UpdateManagerBase.Connect(eventPort);
        }
    }

    /// <summary>
    /// Full generic abstract base class for model managers that support the default property structure with the default relations of relevant components
    /// (Inherit to directly create a fully functional component manager with default construction and component linking)
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TCache"></typeparam>
    /// <typeparam name="TDataMan"></typeparam>
    /// <typeparam name="TCacheMan"></typeparam>
    /// <typeparam name="TInputMan"></typeparam>
    /// <typeparam name="TQueryMan"></typeparam>
    /// <typeparam name="TEventMan"></typeparam>
    /// <typeparam name="TUpdateMan"></typeparam>
    internal abstract class ModelManager<TData, TCache, TDataMan, TCacheMan, TInputMan, TQueryMan, TEventMan, TUpdateMan> : ModelManager
        where TData : ModelData
        where TCache : ModelData
        where TDataMan : ModelDataManager<TData>
        where TCacheMan : ModelCacheManager
        where TInputMan : ModelInputManager
        where TQueryMan : ModelQueryManager
        where TEventMan : ModelEventManager
        where TUpdateMan : ModelUpdateManager
    {
        /// <summary>
        /// The manager refernce data object. Contains all data required for full model description
        /// </summary>
        protected TData ModelData { get; set; }

        /// <summary>
        /// The manager cache data object. Contains extended data that is calculated on demand from the data object
        /// </summary>
        protected TCache ModelCache { get; set; }

        /// <summary>
        /// The manager data manager that provides save access to the reference data
        /// </summary>
        protected TDataMan DataManager { get; set; }

        /// <summary>
        /// The manager cache manager that provides access to on demand data
        /// </summary>
        protected TCacheMan CacheManager { get; set; }

        /// <summary>
        /// The manager input maanger that handles all modelling requests from external sources
        /// </summary>
        protected TInputMan InputManager { get; set; }

        /// <summary>
        /// The manager query manager that handles all data queries to the manager
        /// </summary>
        protected TQueryMan QueryManager { get; set; }

        /// <summary>
        /// The manager event manager that provides all subscriptions to internal cahnges
        /// </summary>
        protected TEventMan EventManager { get; set; }

        /// <summary>
        /// The update manager that handles all reactions to incoming events from other managers or external sources
        /// </summary>
        protected TUpdateMan UpdateManager { get; set; }

        /// <summary>
        /// Internal access to update manager base class (Cast to actual manager only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal override ModelUpdateManager UpdateManagerBase => UpdateManager;

        /// <summary>
        /// Internal access to event manager base class (Cast to actual manager only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal override ModelEventManager EventManagerBase => EventManager;

        /// <summary>
        /// Internal access to data manager base class (Cast to actual manager only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal override ModelDataManager DataManagerBase => DataManager;

        /// <summary>
        /// Internal access to cache manager base class (Cast to actual manager only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal override ModelCacheManager CacheManagerBase => CacheManager;

        /// <summary>
        /// Internal access to query manager base class (Cast to actual manager only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal override ModelQueryManager QueryManagerBase => QueryManager;

        /// <summary>
        /// Internal access to input manager base class (Cast to actual manager only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal override ModelInputManager InputManagerBase => InputManager;

        /// <summary>
        /// Internal access to model data object base class (Cast to actual manager only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal override ModelData DataObjectBase => ModelData;

        /// <summary>
        /// Internal access to cache data object base class (Cast to actual manager only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal override ModelData ExtendedDataObjectBase => ModelCache;

        /// <summary>
        /// Constructs new manager with the provided project services and data object
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="data"></param>
        protected ModelManager(IProjectServices projectServices, TData data) : base(projectServices)
        {
            Initialize(data);
        }

        /// <summary>
        /// Performs the intialization of the manager with the provided data object and project service (Default reflective initialization)
        /// </summary>
        /// <param name="baseData"></param>
        public virtual void Initialize(TData baseData)
        {
            if (baseData == null)
            {
                throw new ArgumentNullException(nameof(baseData));
            }

            var eventManager = (TEventMan)Activator.CreateInstance(typeof(TEventMan));
            var cacheData = (TCache)Activator.CreateInstance(typeof(TCache),eventManager, ProjectServices);
            var queryManager = (TQueryMan)Activator.CreateInstance(typeof(TQueryMan), baseData, cacheData, ProjectServices.DataAccessLocker);
            var inputManager = (TInputMan)Activator.CreateInstance(typeof(TInputMan), baseData, eventManager, ProjectServices);
            var dataManager = (TDataMan)Activator.CreateInstance(typeof(TDataMan), baseData);
            var cacheManager = (TCacheMan)Activator.CreateInstance(typeof(TCacheMan), cacheData, ProjectServices);
            var updateManager = (TUpdateMan)Activator.CreateInstance(typeof(TUpdateMan), baseData, eventManager, ProjectServices);

            ModelData = baseData;
            ModelCache = cacheData;
            QueryManager = queryManager;
            InputManager = inputManager;
            DataManager = dataManager;
            CacheManager = cacheManager;
            UpdateManager = updateManager;
            EventManager = eventManager;
        }
    }

}
