using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Mocassin.Framework.Messaging;
using Mocassin.Framework.Provider;
using Mocassin.Model.Basic;
using Mocassin.Model.DataManagement;
using Mocassin.Symmetry.Analysis;
using Mocassin.Symmetry.CrystalSystems;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Model.ModelProject
{
    /// <inheritdoc />
    public class ModelProject : IModelProject
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
        public ProjectSettings Settings { get; set; }

        /// <inheritdoc />
        public IPushMessageSystem MessageSystem { get; protected set; }

        /// <inheritdoc />
        public IValidationServiceProvider ValidationServices => ValidationServiceProvider;

        /// <inheritdoc />
        public IProjectInputPipeline InputPipeline { get; protected set; }

        /// <inheritdoc />
        public INumericService GeometryNumeric { get; protected set; }

        /// <inheritdoc />
        public INumericService CommonNumeric { get; protected set; }

        /// <inheritdoc />
        public ISpaceGroupService SpaceGroupService { get; protected set; }

        /// <inheritdoc />
        public ICrystalSystemService CrystalSystemService { get; protected set; }

        /// <inheritdoc />
        public IModelDataTracker DataTracker { get; protected set; }

        /// <summary>
        ///     Creates new project services manager with empty manager list
        /// </summary>
        public ModelProject()
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
        public IModelManager CreateAndRegister(IModelManagerFactory factory) => DataTracker.CreateAndRegister(this, factory);

        /// <inheritdoc />
        public void CreateAndRegister(IEnumerable<IModelManagerFactory> factories)
        {
            if (factories == null) throw new ArgumentNullException(nameof(factories));
            foreach (var item in factories) CreateAndRegister(item);
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
                ValidationServiceProvider.RegisterService(manager.CreateValidationService(Settings));
                return;
            }

            ActiveManagers.Add(manager);
            ShareAndConnectEventPorts(manager);
            ValidationServiceProvider.RegisterService(manager.CreateValidationService(Settings));
        }

        /// <inheritdoc />
        public TManager Manager<TManager>()
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
        public IModelManager Manager(Type interfaceType) => ActiveManagers.SingleOrDefault(interfaceType.IsInstanceOfType);


        /// <inheritdoc />
        public IEnumerable<IModelManager> Managers() => ActiveManagers;

        /// <inheritdoc />
        public void ResetProject()
        {
            foreach (var modelManager in Managers())
                modelManager.InputAccess.ResetManager().Wait();
        }

        /// <summary>
        ///     Distributes all existing event ports to a new manager and vise versa
        /// </summary>
        /// <param name="manager"></param>
        public void ShareAndConnectEventPorts(IModelManager manager)
        {
            foreach (var otherManager in ActiveManagers)
            {
                if (otherManager != manager && manager.EventAccess != null)
                    otherManager.TryConnectManager(manager.EventAccess);

                if (otherManager.EventAccess != null)
                    manager.TryConnectManager(otherManager.EventAccess);
            }
        }

        /// <summary>
        ///     Factory to create new project services interface from data object
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static ModelProject Create(ProjectSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            var structureSettings = settings.GetModuleSettings<MocassinStructureSettings>();

            var modelProject = new ModelProject
            {
                DataTracker = new ModelDataTracker()
            };

            modelProject.InputPipeline = new ProjectInputPipeline(modelProject);

            var geometryNumeric = new NumericService(settings.GeometryNumericSettings);
            var commonNumeric = new NumericService(settings.CommonNumericSettings);
            var spaceGroupService = new SpaceGroupService(settings.SymmetrySettings.SpaceGroupDbPath, settings.SymmetrySettings.VectorTolerance);
            var crystalSystemProvider = CrystalSystemSource.CreateSoft(structureSettings.CellParameter.MaxValue,
                settings.SymmetrySettings.ParameterTolerance);
            var crystalSystemService = new CrystalSystemService(crystalSystemProvider, settings.CommonNumericSettings.RangeValue);
            var validationServices = new ValidationServiceProvider(modelProject);
            var dataAccessLocker = new AccessLockSource(settings.ConcurrencySettings.MaxAttempts, settings.ConcurrencySettings.AttemptInterval);
            var messageSystem = new AsyncMessageSystem();

            modelProject.Settings = settings;
            modelProject.GeometryNumeric = geometryNumeric;
            modelProject.CommonNumeric = commonNumeric;
            modelProject.ValidationServiceProvider = validationServices;
            modelProject.AccessLockSource = dataAccessLocker;
            modelProject.MessageSystem = messageSystem;
            modelProject.SpaceGroupService = spaceGroupService;
            modelProject.CrystalSystemService = crystalSystemService;
            return modelProject;
        }
    }
}