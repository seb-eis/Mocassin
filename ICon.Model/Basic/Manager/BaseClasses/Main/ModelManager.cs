using System;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Basic
{
    /// <inheritdoc />
    /// <remarks> Abstract base class for all model manager implementations </remarks>
    internal abstract class ModelManager : IModelManager
    {
        /// <inheritdoc />
        public IModelEventPort EventPort => EventManagerBase;

        /// <inheritdoc />
        public IModelInputPort InputPort => InputManagerBase;

        /// <summary>
        ///     Project services instance that is shared between all model managers of a simulation project and offers various
        ///     services e.g. validations, numeric comparer etc.
        /// </summary>
        internal IModelProject ModelProject { get; set; }

        /// <summary>
        ///     Internal access to update manager base class (Cast to actual manager only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal abstract ModelUpdateManager UpdateManagerBase { get; }

        /// <summary>
        ///     Internal access to event manager base class (Cast to actual manager only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal abstract ModelEventManager EventManagerBase { get; }

        /// <summary>
        ///     Internal access to data manager base class (Cast to actual manager only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal abstract ModelDataManager DataManagerBase { get; }

        /// <summary>
        ///     Internal access to cached data manager base class (Cast to actual manager only in generic/polymorphic usage
        ///     scenarios!)
        /// </summary>
        internal abstract ModelCacheManager CacheManagerBase { get; }

        /// <summary>
        ///     Internal access to query manager base class (Cast to actual manager only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal abstract ModelQueryManager QueryManagerBase { get; }

        /// <summary>
        ///     Internal access to input manager base class (Cast to actual manager only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal abstract ModelInputManager InputManagerBase { get; }

        /// <summary>
        ///     Internal access to data object base class (Cast to actual object only in generic/polymorphic usage scenarios!)
        /// </summary>
        internal abstract ModelData ModelDataBase { get; }

        /// <summary>
        ///     Internal access to extended data object base class (Cast to actual object only in generic/polymorphic usage
        ///     scenarios!)
        /// </summary>
        internal abstract ModelData DataCacheBase { get; }

        /// <summary>
        ///     Creates new model manager that uses the provided project services and automatically registers to it
        /// </summary>
        /// <param name="modelProject"></param>
        protected ModelManager(IModelProject modelProject)
        {
            ModelProject = modelProject ?? throw new ArgumentNullException(nameof(modelProject));
        }

        /// <inheritdoc />
        public virtual void DisconnectManager()
        {
            EventManagerBase?.OnManagerDisconnectRequests.OnNextAsync().Wait();
            UpdateManagerBase?.DisconnectAll();
        }

        /// <inheritdoc />
        public abstract IValidationService CreateValidationService(ProjectSettings settings);

        /// <inheritdoc />
        public abstract Type GetManagerInterfaceType();

        /// <inheritdoc />
        public bool TryConnectManager(IModelEventPort eventPort)
        {
            if (UpdateManagerBase == null || EventPort == eventPort)
                return false;

            return UpdateManagerBase.Connect(eventPort);
        }
    }

    /// <summary>
    ///     Full generic abstract base class for model managers that support the default property structure with the default
    ///     relations of relevant components
    ///     (Inherit to directly create a fully functional component manager with default construction and component linking)
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
        ///     The manager reference data object. Contains all data required for full model description
        /// </summary>
        protected TData ModelData { get; set; }

        /// <summary>
        ///     The manager cache data object. Contains extended data that is calculated on demand from the data object
        /// </summary>
        protected TCache ModelCache { get; set; }

        /// <summary>
        ///     The manager data manager that provides save access to the reference data
        /// </summary>
        protected TDataMan DataManager { get; set; }

        /// <summary>
        ///     The manager cache manager that provides access to on demand data
        /// </summary>
        protected TCacheMan CacheManager { get; set; }

        /// <summary>
        ///     The manager input manager that handles all modeling requests from external sources
        /// </summary>
        protected TInputMan InputManager { get; set; }

        /// <summary>
        ///     The manager query manager that handles all data queries to the manager
        /// </summary>
        protected TQueryMan QueryManager { get; set; }

        /// <summary>
        ///     The manager event manager that provides all subscriptions to internal changes
        /// </summary>
        protected TEventMan EventManager { get; set; }

        /// <summary>
        ///     The update manager that handles all reactions to incoming events from other managers or external sources
        /// </summary>
        protected TUpdateMan UpdateManager { get; set; }

        /// <inheritdoc />
        internal override ModelUpdateManager UpdateManagerBase => UpdateManager;

        /// <inheritdoc />
        internal override ModelEventManager EventManagerBase => EventManager;

        /// <inheritdoc />
        internal override ModelDataManager DataManagerBase => DataManager;

        /// <inheritdoc />
        internal override ModelCacheManager CacheManagerBase => CacheManager;

        /// <inheritdoc />
        internal override ModelQueryManager QueryManagerBase => QueryManager;

        /// <inheritdoc />
        internal override ModelInputManager InputManagerBase => InputManager;

        /// <inheritdoc />
        internal override ModelData ModelDataBase => ModelData;

        /// <inheritdoc />
        internal override ModelData DataCacheBase => ModelCache;

        /// <inheritdoc />
        protected ModelManager(IModelProject modelProject, TData data)
            : base(modelProject)
        {
            Initialize(data);
        }

        /// <summary>
        ///     Performs the initialization of the manager with the provided data object and project service (Default reflective
        ///     initialization)
        /// </summary>
        /// <param name="baseData"></param>
        public virtual void Initialize(TData baseData)
        {
            if (baseData == null)
                throw new ArgumentNullException(nameof(baseData));

            var eventManager = (TEventMan) Activator.CreateInstance(typeof(TEventMan));
            var cacheData = (TCache) Activator.CreateInstance(typeof(TCache), eventManager, ModelProject);
            var queryManager =
                (TQueryMan) Activator.CreateInstance(typeof(TQueryMan), baseData, cacheData, ModelProject.AccessLockSource);
            var inputManager = (TInputMan) Activator.CreateInstance(typeof(TInputMan), baseData, eventManager, ModelProject);
            var dataManager = (TDataMan) Activator.CreateInstance(typeof(TDataMan), baseData);
            var cacheManager = (TCacheMan) Activator.CreateInstance(typeof(TCacheMan), cacheData, ModelProject);
            var updateManager = (TUpdateMan) Activator.CreateInstance(typeof(TUpdateMan), baseData, eventManager, ModelProject);

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