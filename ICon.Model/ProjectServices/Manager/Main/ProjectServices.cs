using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using ICon.Framework.Messaging;
using ICon.Framework.Provider;
using ICon.Model.Basic;
using ICon.Model.DataManagement;
using ICon.Symmetry.Analysis;
using ICon.Symmetry.CrystalSystems;
using ICon.Symmetry.SpaceGroups;

namespace ICon.Model.ProjectServices
{
    /// <inheritdoc />
    internal class ProjectServices : IProjectServices
    {
        /// <summary>
        ///     List of active registered model managers
        /// </summary>
        private List<IModelManager> ActiveManagers { get; }

        /// <summary>
        ///     Service provider for validations
        /// </summary>
        private ValidationServiceProvider ValidationServiceProvider { get; set; }

        /// <summary>
        ///     The project lock object to safely set/unset the input in progress flag
        /// </summary>
        private object ProjectLock { get; } = new object();

        /// <inheritdoc />
        public bool IsInputInProgress { get; protected set; }

        /// <inheritdoc />
        public AccessLockSource AccessLockSource { get; protected set; }

        /// <inheritdoc />
        public ProjectSettingsData SettingsData { get; set; }

        /// <inheritdoc />
        public IPushMessageSystem MessageSystem { get; protected set; }

        /// <inheritdoc />
        public IValidationServiceProvider ValidationServices => ValidationServiceProvider;

        /// <inheritdoc />
        public INumericService GeometryNumeric { get; protected set; }
        
        /// <inheritdoc />
        public INumericService CommonNumeric { get; protected set; }
        
        /// <inheritdoc />
        public ISpaceGroupService SpaceGroupService { get; protected set; }
        
        /// <inheritdoc />
        public ICrystalSystemService CrystalSystemService { get; protected set; }
        
        /// <inheritdoc />
        public ISymmetryAnalysisService SymmetryAnalysisService { get; protected set; }
        
        /// <inheritdoc />
        public IModelDataTracker DataTracker { get; protected set; }

        /// <summary>
        ///     Creates new project services manager with empty manager list
        /// </summary>
        public ProjectServices()
        {
            ActiveManagers = new List<IModelManager>();
        }
      
        /// <inheritdoc />
        public bool TryGetInputLock(out IDisposable projectLocker)
        {
            lock (ProjectLock)
            {
                projectLocker = null;
                if (IsInputInProgress) 
                    return false;

                IsInputInProgress = true;
                projectLocker = Disposable.Create(() =>
                {
                    lock (ProjectLock)
                    {
                        IsInputInProgress = false;
                    }
                });
            }

            return true;
        }
        
        /// <inheritdoc />
        public IModelManager CreateAndRegister(IModelManagerFactory factory)
        {
            return DataTracker.CreateAndRegister(this, factory);
        }
        
        /// <inheritdoc />
        public void RegisterManager(IModelManager manager)
        {
            for (var i = 0; i < ActiveManagers.Count; i++)
            {
                if (ActiveManagers[i].GetManagerInterfaceType() != manager.GetManagerInterfaceType())
                    continue;

                ActiveManagers[i].DisconnectManager();
                ActiveManagers[i] = manager;
                ShareAndConnectEventPorts(manager);
                ValidationServiceProvider.RegisterService(manager.CreateValidationService(SettingsData));
                return;
            }

            ActiveManagers.Add(manager);
            ShareAndConnectEventPorts(manager);
            ValidationServiceProvider.RegisterService(manager.CreateValidationService(SettingsData));
        }

        /// <summary>
        ///     Distributes all existing event ports to a new manager and vise versa
        /// </summary>
        /// <param name="manager"></param>
        public void ShareAndConnectEventPorts(IModelManager manager)
        {
            foreach (var otherManager in ActiveManagers)
            {
                if (otherManager != manager && manager.EventPort != null)
                    otherManager.TryConnectManager(manager.EventPort);

                if (otherManager.EventPort != null) 
                    manager.TryConnectManager(otherManager.EventPort);
            }
        }
      
        /// <inheritdoc />
        public TManager GetManager<TManager>() 
            where TManager : class, IModelManager
        {
            foreach (var manager in ActiveManagers)
            {
                if (manager is TManager castManager)
                    return castManager;
            }

            throw new InvalidOperationException("Requested manager is not registered with the project services");
        }

        
        /// <inheritdoc />
        public IModelManager GetManager(Type interfaceType)
        {
            return ActiveManagers.SingleOrDefault(interfaceType.IsInstanceOfType);
        }

        
        /// <inheritdoc />
        public IEnumerable<IModelManager> GetAllManagers()
        {
            return ActiveManagers;
        }

        /// <summary>
        ///     Factory to create new project services interface from data object
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ProjectServices Create(ProjectSettingsData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            var projectService = new ProjectServices();
            var geometryNumeric = new NumericService(data.GeometryNumericSettings);
            var commonNumeric = new NumericService(data.CommonNumericSettings);
            var spaceGroupService = new SpaceGroupService(data.SymmetrySettings.SpaceGroupDbPath, data.SymmetrySettings.VectorTolerance);
            var crystalSystemProvider = CrystalSystemProvider.CreateSoft(data.StructureSettings.CellParameter.MaxValue,
                data.SymmetrySettings.ParameterTolerance);
            var crystalSystemService = new CrystalSystemService(crystalSystemProvider, data.CommonNumericSettings.RangeValue);
            var validationServices = new ValidationServiceProvider();
            var dataAccessLocker = new AccessLockSource(data.ConcurrencySettings.MaxAttempts, data.ConcurrencySettings.AttemptInterval);
            var messageSystem = new AsyncMessageSystem();
            var symmetryService = new SymmetryAnalysisService(SymmetryIndicator.MakeComparer(geometryNumeric.RelativeComparer),
                ObjectProvider.Create(() => crystalSystemService.VectorTransformer));
            var dataTracker = new ModelDataTracker();

            projectService.SymmetryAnalysisService = symmetryService;
            projectService.SettingsData = data;
            projectService.GeometryNumeric = geometryNumeric;
            projectService.CommonNumeric = commonNumeric;
            projectService.ValidationServiceProvider = validationServices;
            projectService.AccessLockSource = dataAccessLocker;
            projectService.MessageSystem = messageSystem;
            projectService.SpaceGroupService = spaceGroupService;
            projectService.CrystalSystemService = crystalSystemService;
            projectService.DataTracker = dataTracker;
            return projectService;
        }
    }
}