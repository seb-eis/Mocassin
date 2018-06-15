using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;

using ICon.Framework.Provider;
using ICon.Framework.Messaging;
using ICon.Symmetry.SpaceGroups;
using ICon.Symmetry.CrystalSystems;

using ICon.Model.Basic;
using ICon.Symmetry.Analysis;
using ICon.Model.DataManagement;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    /// Project service class that provides numeric comparators, validations and port provision etc. for the model managers
    /// </summary>
    internal class ProjectServicesManager : IProjectServices
    {
        /// <summary>
        /// The project lock object to safely set/unset the input in progress flag
        /// </summary>
        private Object ProjectLock { get; } = new Object();

        /// <summary>
        /// Flag that indicates if a model data input operation is currently in progress
        /// </summary>
        public Boolean InputInProgress { get; protected set; }

        /// <summary>
        /// The basic data access locker that handles how long a data reader/writer is allowed to wait for a valid lock before throwing a timeout exception
        /// </summary>
        public DataAccessLocker DataAccessLocker { get; protected set; }

        /// <summary>
        /// The project settings data object
        /// </summary>
        private ProjectSettingsData ProjectSettings { get; set; }

        /// <summary>
        /// List of active registered model managers
        /// </summary>
        private List<IModelManager> ActiveManagers { get; set; }

        /// <summary>
        /// Service provider for validations
        /// </summary>
        private ValidationServiceProvider ValidationServiceProvider { get; set; }

        /// <summary>
        /// Push notification based messaging system
        /// </summary>
        public IPushMessageSystem MessageSystem { get; protected set; }

        /// <summary>
        /// Service to redirect validation requests to the correct validation service
        /// </summary>
        public IValidationServiceProvider ValidationServices => ValidationServiceProvider;

        /// <summary>
        /// The geometry numeric service for uniform floating point comparisons in geometry calculations
        /// </summary>
        public INumericService GeometryNumerics { get; protected set; }

        /// <summary>
        /// The common numeric service for uniform floating point comparisons during basic model tasks
        /// </summary>
        public INumericService CommonNumerics { get; protected set; }

        /// <summary>
        /// The space group service for loading space groups and creating wyckoff position lists
        /// </summary>
        public ISpaceGroupService SpaceGroupService { get; protected set; }

        /// <summary>
        /// The crystal system provider service that handles the provision of crystal systems
        /// </summary>
        public ICrystalSystemService CrystalSystemService { get; protected set; }

        /// <summary>
        /// The symmetry analysis service that supplies methods for symmetry comparisons and transformation sequence calculations
        /// </summary>
        public ISymmetryAnalysisService SymmetryAnalysisService { get; protected set; }

        /// <summary>
        /// The model data tracking system that handles the tracking of all indexed model objects
        /// </summary>
        public IModelDataTracker DataTracker { get; protected set; }

        /// <summary>
        /// Creates new project services manager with empty manager list
        /// </summary>
        public ProjectServicesManager()
        {
            ActiveManagers = new List<IModelManager>();
        }

        /// <summary>
        /// Tries to create a disposbale input lock that protectes the project from further data input until the current input is completed
        /// </summary>
        /// <param name="projectLocker"></param>
        /// <returns></returns>
        public bool TryGetInputLock(out IDisposable projectLocker)
        {
            lock (ProjectLock)
            {
                projectLocker = null;
                if (InputInProgress)
                {
                    return false;
                }
                InputInProgress = true;
                projectLocker = Disposable.Create(() => { lock (ProjectLock) { InputInProgress = false; }; });
            }
            return true;
        }

        /// <summary>
        /// Uses the provided model manager factory to create and register a new manager in the service and data tracker. Returns the newly linked manager
        /// </summary>
        /// <param name="factory"></param>
        public IModelManager CreateAndRegister(IModelManagerFactory factory)
        {
            return DataTracker.CreateAndRegister(this, factory);
        }

        /// <summary>
        /// Registers a model manager with the project services, overwrites manager of same interface type if already present
        /// </summary>
        /// <param name="manager"></param>
        public void RegisterManager(IModelManager manager)
        {
            for (int i = 0; i < ActiveManagers.Count; i++)
            {
                if (ActiveManagers[i].GetManagerInterfaceType() == manager.GetManagerInterfaceType())
                {
                    ActiveManagers[i].DisconnectManager();
                    ActiveManagers[i] = manager;
                    ShareAndConnectEventPorts(manager);
                    ValidationServiceProvider.RegisterService(manager.MakeValidationService(ProjectSettings));
                    return;
                }
            }
            ActiveManagers.Add(manager);
            ShareAndConnectEventPorts(manager);
            ValidationServiceProvider.RegisterService(manager.MakeValidationService(ProjectSettings));
        }

        /// <summary>
        /// Distributes all existing event ports to a new manager and vise versa
        /// </summary>
        /// <param name="manager"></param>
        public void ShareAndConnectEventPorts(IModelManager manager)
        {
            foreach (var otherManager in ActiveManagers)
            {
                if (otherManager != manager && manager.EventPort != null)
                {
                    otherManager.TryConnectManager(manager.EventPort);
                }
                if (otherManager.EventPort != null)
                {
                    manager.TryConnectManager(otherManager.EventPort);
                }
            }
        }

        /// <summary>
        /// Get the manager of the specififed type, throws exception if requested type is not registered
        /// </summary>
        /// <typeparam name="TManager"></typeparam>
        /// <returns></returns>
        public TManager GetManager<TManager>() where TManager : class, IModelManager
        {
            foreach (IModelManager manager in ActiveManagers)
            {
                if (manager is TManager casted)
                {
                    return casted;
                }
            }
            throw new InvalidOperationException("Requested manager is not registered with the project services");
        }

        /// <summary>
        /// Get the registered manager that implements the specifified interface
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public IModelManager GetManager(Type interfaceType)
        {
            return ActiveManagers.SingleOrDefault(value => interfaceType.IsAssignableFrom(value.GetType()));
        }

        /// <summary>
        /// Get all currently registerd managers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IModelManager> GetAllManagers()
        {
            return ActiveManagers;
        }

        /// <summary>
        /// Factory to create new project services interface from data object
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ProjectServicesManager Create(ProjectSettingsData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            var projectService = new ProjectServicesManager();
            var geometryNumerics = new NumericService(data.GeometryNumericSettings);
            var commonNumerics = new NumericService(data.CommonNumericSettings);
            var spaceGroupService = new SpaceGroupService(data.SymmetrySettings.DatabaseFilepath, data.SymmetrySettings.VectorTolerance);
            var crystalSystemProvider = CrystalSystemProvider.CreateSoft(data.StructureSettings.MaxBaseParameterLength, data.SymmetrySettings.ParameterTolerance);
            var crystalSystemService = new CrystalSystemService(crystalSystemProvider, data.CommonNumericSettings.CompRange);
            var validationServices = new ValidationServiceProvider();
            var dataAccessLocker = new DataAccessLocker(data.ConcurrencySettings.MaxAttempts, data.ConcurrencySettings.AttemptInterval);
            var messageSystem = new AsyncMessageSystem();
            var symmetryService = new SymmetryAnalysisService(SymmetryIndicator.MakeComparer(geometryNumerics.RelativeComparer), ObjectProvider.Create(() => crystalSystemService.VectorTransformer));
            var dataTracker = new ModelDataTracker();

            projectService.SymmetryAnalysisService = symmetryService;
            projectService.ProjectSettings = data;
            projectService.GeometryNumerics = geometryNumerics;
            projectService.CommonNumerics = commonNumerics;
            projectService.ValidationServiceProvider = validationServices;
            projectService.DataAccessLocker = dataAccessLocker;
            projectService.MessageSystem = messageSystem;
            projectService.SpaceGroupService = spaceGroupService;
            projectService.CrystalSystemService = crystalSystemService;
            projectService.DataTracker = dataTracker;
            return projectService;
        }
    }
}
