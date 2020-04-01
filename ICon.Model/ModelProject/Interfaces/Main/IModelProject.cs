using System;
using System.Collections.Generic;
using Mocassin.Framework.Messaging;
using Mocassin.Model.Basic;
using Mocassin.Model.DataManagement;
using Mocassin.Symmetry.Analysis;
using Mocassin.Symmetry.CrystalSystems;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Represents a project service and module container that serves as the main data and service exchange
    ///     point for a single model project
    /// </summary>
    public interface IModelProject
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
        ProjectSettings Settings { get; }

        /// <summary>
        ///     Data tracking system for all known model data objects
        /// </summary>
        IModelDataTracker DataTracker { get; }

        /// <summary>
        ///     The <see cref="IProjectInputPipeline" /> that handles automated input redirection to the correct
        ///     <see cref="IModelInputPort" />
        /// </summary>
        IProjectInputPipeline InputPipeline { get; }

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
        ///     Creates and registers multiple managers that are created by the provided factories
        /// </summary>
        /// <param name="factories"></param>
        void CreateAndRegister(IEnumerable<IModelManagerFactory> factories);

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
        TManager Manager<TManager>() where TManager : class, IModelManager;

        /// <summary>
        ///     Get the registered manager that implements the specified interface (Returns null im manager does not exist)
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        IModelManager Manager(Type interfaceType);

        /// <summary>
        ///     Get all currently registered managers
        /// </summary>
        /// <returns></returns>
        IEnumerable<IModelManager> Managers();

        /// <summary>
        ///     Resets the <see cref="IModelProject" /> to mint factory status with default data status
        /// </summary>
        void ResetProject();
    }
}