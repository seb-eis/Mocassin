using System;
using System.Collections.Generic;
using ICon.Framework.Messaging;
using ICon.Model.Basic;
using ICon.Model.DataManagement;
using ICon.Symmetry.Analysis;
using ICon.Symmetry.CrystalSystems;
using ICon.Symmetry.SpaceGroups;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    ///     Represents a project service collection that is shared across all modules of the current project
    /// </summary>
    public interface IProjectServices
    {
        /// <summary>
        ///     Flag that indicates if model data is currently added to the project
        /// </summary>
        bool IsInputInProgress { get; }

        /// <summary>
        ///     The default data locker that species the timespan and attempts before reader/writer creation operations throw
        ///     timeout exceptions
        /// </summary>
        AccessLockSource AccessLockSource { get; }

        /// <summary>
        ///     Get the project settings data object that contains all project settings
        /// </summary>
        ProjectSettingsData SettingsData { get; }

        /// <summary>
        ///     Data tracking system for all known model data objects
        /// </summary>
        IModelDataTracker DataTracker { get; }

        /// <summary>
        ///     Floating point service for numeric geometry operations
        /// </summary>
        INumericService GeometryNumeric { get; }

        /// <summary>
        ///     Floating point service for all common numeric operations
        /// </summary>
        INumericService CommonNumeric { get; }

        /// <summary>
        ///     Push notification messaging service to send 'fire-and-forget' messages to subscribers
        /// </summary>
        IPushMessageSystem MessageSystem { get; }

        /// <summary>
        ///     Space group service to load groups and create wyckoff position lists from reference information
        /// </summary>
        ISpaceGroupService SpaceGroupService { get; }

        /// <summary>
        ///     Crystal system service that handles the provision of crystal systems
        /// </summary>
        ICrystalSystemService CrystalSystemService { get; }

        /// <summary>
        ///     Validation services that handles validation requests for model objects and model parameters
        /// </summary>
        IValidationServiceProvider ValidationServices { get; }

        /// <summary>
        ///     Symmetry analysis service that supplies methods for symmetry comparisons and transformation sequence calculation
        /// </summary>
        ISymmetryAnalysisService SymmetryAnalysisService { get; }

        /// <summary>
        ///     Tries to lock the project for input and creates a disposable input lock on success, returns false if failed
        /// </summary>
        /// <param name="projectLocker"></param>
        /// <returns></returns>
        bool TryGetInputLock(out IDisposable projectLocker);

        /// <summary>
        ///     Creates and registers the manager that is created by the provided manager factory
        /// </summary>
        /// <param name="factory"></param>
        IModelManager CreateAndRegister(IModelManagerFactory factory);

        /// <summary>
        ///     Registers a new manager with the project services, overwrites existing one of same interface type if present
        /// </summary>
        /// <param name="manager"></param>
        void RegisterManager(IModelManager manager);

        /// <summary>
        ///     Get the registered manager of the specified type
        /// </summary>
        /// <typeparam name="TManager"></typeparam>
        /// <returns></returns>
        TManager GetManager<TManager>() where TManager : class, IModelManager;

        /// <summary>
        ///     Get the registered manager that implements the specified interface (Returns null im manager does not exist)
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        IModelManager GetManager(Type interfaceType);

        /// <summary>
        ///     Get all currently registered managers
        /// </summary>
        /// <returns></returns>
        IEnumerable<IModelManager> GetAllManagers();
    }
}