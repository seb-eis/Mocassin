using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using ICon.Framework.Collections;
using ICon.Framework.Reflection;
using ICon.Framework.Extensions;
using ICon.Framework.Processing;
using ICon.Framework.Operations;
using ICon.Model.ProjectServices;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Abstract base class for all model input manager implementations
    /// </summary>
    internal abstract class ModelInputManager : IModelInputPort
    {
        /// <summary>
        /// Async object handler pipeline for parameter inputs that are always a single value and do not require indexing operations
        /// </summary>
        private AsyncBreakPipeline<IOperationReport> ParameterInputPipeline { get; }

        /// <summary>
        /// Async object handler pipeline for new model object input operations
        /// </summary>
        private AsyncBreakPipeline<IOperationReport> ObjectInputPipeline { get; }

        /// <summary>
        /// Async object handler pipeline for model object replacement operations
        /// </summary>
        private AsyncBreakPipeline<IOperationReport> ObjectReplacementPipeline { get; }

        /// <summary>
        /// Async object handler pipeline for model object removal operations
        /// </summary>
        private AsyncBreakPipeline<IOperationReport> ObjectRemovalPipeline { get; }

        /// <summary>
        /// General constructor that calls the initialization fucntions for the handler arrays
        /// </summary>
        protected ModelInputManager()
        {
            ParameterInputPipeline = new AsyncBreakPipeline<IOperationReport>(GetAsyncInvalidParameterHandler(), MakeAsyncParameterChangeProcessors());
            ObjectInputPipeline = new AsyncBreakPipeline<IOperationReport>(GetAsyncInvalidObjectHandler(), MakeAsyncObjectInputProcessors());
            ObjectRemovalPipeline = new AsyncBreakPipeline<IOperationReport>(GetAsyncInvalidObjectHandler(), MakeAsyncObjectRemovalProcessors());
            ObjectReplacementPipeline = new AsyncBreakPipeline<IOperationReport>(GetAsyncInvalidObjectHandler(), MakeAsyncObjectReplacementProcessors());
        }

        /// <summary>
        /// Generic model parameter set method to set unique model parameters of the manager if the manager supports this property
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <param name="modelParameter"></param>
        /// <returns></returns>
        public Task<IOperationReport> SetModelParameter<TParameter>(TParameter modelParameter) where TParameter : IModelParameter
        {
            if (modelParameter == null)
            {
                throw new ArgumentNullException(nameof(modelParameter));
            }
            return ParameterInputPipeline.Process(modelParameter);
        }

        /// <summary>
        /// Generic model object input method that starts the input process for a model object if the manager supports the model object type
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        public Task<IOperationReport> InputModelObject<TObject>(TObject modelObject) where TObject : IModelObject
        {
            if (modelObject == null)
            {
                throw new ArgumentNullException(nameof(modelObject)); ;
            }
            return ObjectInputPipeline.Process(modelObject);
        }

        /// <summary>
        /// Generic model object removal method that starts the removal process by index for a model object if the manager supports the model object type
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        public Task<IOperationReport> RemoveModelObject<TObject>(TObject modelObject) where TObject : IModelObject
        {
            if (modelObject == null)
            {
                throw new ArgumentNullException(nameof(modelObject));
            }
            return ObjectRemovalPipeline.Process(modelObject);
        }

        /// <summary>
        /// Generic model object replacement method that starts the replacement process for a model object by another if the manager supports the model object type
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="original"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public Task<IOperationReport> ReplaceModelObject<TObject>(TObject modelObject, TObject replacement) where TObject : IModelObject
        {
            if (modelObject == null)
            {
                throw new ArgumentNullException(nameof(modelObject));
            }
            if (replacement == null)
            {
                throw new ArgumentNullException(nameof(replacement));
            }
            return ObjectReplacementPipeline.Process(modelObject, replacement);
        }

        /// <summary>
        /// Schedules a new tasks that cleans the manager of deprecated data and returns an operation result informing about the success
        /// </summary>
        /// <returns></returns>
        public Task<IOperationReport> CleanupManager()
        {
            return Task.Run(() => TryCleanDeprecatedData());
        }

        /// <summary>
        /// Schedules a new tasks that resets the manager data to its default state and returns an operation result informing about the success
        /// </summary>
        /// <returns></returns>
        public Task<IOperationReport> ResetManager()
        {
            return Task.Run(() => TryResetManagerDataToDefault());
        }

        /// <summary>
        /// Tries to reset the manager data to its default state
        /// </summary>
        /// <returns></returns>
        protected abstract IOperationReport TryResetManagerDataToDefault();

        /// <summary>
        /// Tries to clean depreacted data from the manager
        /// </summary>
        /// <returns></returns>
        protected abstract IOperationReport TryCleanDeprecatedData();

        /// <summary>
        /// Get the async handler that is called if processing of model inputs in the pipelines fails because the manager does not support the type
        /// </summary>
        /// <param name="unexpectedType"></param>
        /// <returns></returns>
        protected IAsyncObjectProcessor<IOperationReport> GetAsyncInvalidObjectHandler()
        {
            IOperationReport HandlerFunction(object obj)
            {
                return OperationReport.MakeUnexpectedTypeResult(obj.GetType(), GetSupportedModelObjects());
            }
            return ObjectProcessorFactory.CreateAsync<object, IOperationReport>(HandlerFunction);
        }

        /// <summary>
        /// Get the async handler taht is called if processing of model parameters in the pipeline fails beacuse the manager does no support the type
        /// </summary>
        /// <returns></returns>
        protected IAsyncObjectProcessor<IOperationReport> GetAsyncInvalidParameterHandler()
        {
            IOperationReport HandlerFunction(object obj)
            {
                return OperationReport.MakeUnexpectedTypeResult(obj.GetType(), GetSupportedModelParameters());
            }
            return ObjectProcessorFactory.CreateAsync<object, IOperationReport>(HandlerFunction);
        }

        /// <summary>
        /// Identify all non public functions that are marked as data operation methods of new object inputs and create the processor list for pipelining
        /// </summary>
        protected virtual List<IAsyncObjectProcessor<IOperationReport>> MakeAsyncObjectInputProcessors()
        {
            var processors = MakeAsyncDataOperationProcessors(DataOperationType.NewObject).ToList();
            if (processors.Count != GetSupportedModelObjects().Length)
            {
                throw new InvalidOperationException("Object processor list mismatch to supported data objects. Input function is missing or could not be detected!");
            }
            return processors;
        }

        /// <summary>
        /// Identify all non public functions that are marked as data operation methods of model object replacements and create the processor list for pipelining
        /// </summary>
        protected virtual List<IAsyncObjectProcessor<IOperationReport>> MakeAsyncObjectReplacementProcessors()
        {
            var processors = MakeAsyncDataOperationProcessors(DataOperationType.ObjectChange).ToList();
            if (processors.Count != GetSupportedModelObjects().Length)
            {
                throw new InvalidOperationException("Object processor list mismatch to supported data objects. Input function is missing or could not be detected!");
            }
            return processors;
        }

        /// <summary>
        /// Identify all non public functions that are marked as data operation methods of new object inputs and create the processor list for pipelining
        /// </summary>
        protected virtual List<IAsyncObjectProcessor<IOperationReport>> MakeAsyncObjectRemovalProcessors()
        {
            var processors = MakeAsyncDataOperationProcessors(DataOperationType.ObjectRemoval).ToList();
            if (processors.Count != GetSupportedModelObjects().Length)
            {
                throw new InvalidOperationException("Object processor list mismatch to supported data objects. Input function is missing or could not be detected!");
            }
            return processors;
        }

        /// <summary>
        /// Identify all non public functions that are marked as data operation methods of new object inputs and create the processor list for pipelining
        /// </summary>
        /// <returns></returns>
        protected virtual List<IAsyncObjectProcessor<IOperationReport>> MakeAsyncParameterChangeProcessors()
        {
            var processors = MakeAsyncDataOperationProcessors(DataOperationType.ParameterChange).ToList();
            if (processors.Count != GetSupportedModelParameters().Length)
            {
                throw new InvalidOperationException("Object processor list mismatch to supported data objects. Input function is missing or could not be detected!");
            }
            return processors;
        }

        /// <summary>
        /// Get the types of all model objects that are supported by this manager
        /// </summary>
        /// <returns></returns>
        public abstract Type[] GetSupportedModelObjects();

        /// <summary>
        /// Get the types of all model parameters that are supported by this manager
        /// </summary>
        /// <returns></returns>
        public abstract Type[] GetSupportedModelParameters();

        /// <summary>
        /// Get an enumerable off all supported input model objects and parameters of this input manager
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> GetSupportedModelTypes()
        {
            return GetSupportedModelObjects().Concat(GetSupportedModelParameters());
        }

        /// <summary>
        /// Searches the input manager for all non public methods marked as data operations of the specififed type and creates a sequence of object processors
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        protected IEnumerable<IAsyncObjectProcessor<IOperationReport>> MakeAsyncDataOperationProcessors(DataOperationType operationType)
        {
            bool MethodSearch(MethodInfo methodInfo)
            {
                if (methodInfo.GetCustomAttribute(typeof(OperationMethodAttribute)) is OperationMethodAttribute attribute)
                {
                    return attribute.OperationType == operationType;
                }
                return false;
            }
            return new ObjectProcessorCreator().CreateAsyncProcessors<IOperationReport>(this, MethodSearch, BindingFlags.Instance | BindingFlags.NonPublic);
        }
    }

    /// <summary>
    /// Generic base class for all model data input managers that use specific data, read only data access port and event manager to distrube changes
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TDataPort"></typeparam>
    /// <typeparam name="TEventManager"></typeparam>
    internal abstract class ModelInputManager<TData, TDataPort, TEventManager> : ModelInputManager 
        where TData : ModelData<TDataPort> where TDataPort : class, IModelDataPort
        where TEventManager : ModelEventManager
    {
        /// <summary>
        /// Defines a generic delegate for data operations with specific return value
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="report"></param>
        internal delegate TResult DataOperation<out TResult>(DataAccessor<TData> dataAccess, OperationReport report);

        /// <summary>
        /// Defines a delegate for data operations with no return value
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="report"></param>
        internal delegate void DataOperation(DataAccessor<TData> dataAccess, OperationReport report);

        /// <summary>
        /// Provider for safe accessors to the data object
        /// </summary>
        protected DataAccessProvider<TData> DataAccessProvider { get; }

        /// <summary>
        /// Provider for read only accessors to the data object that use the read only specified data port
        /// </summary>
        protected DataReadProvider<TData, TDataPort> DataReaderProvider { get; }

        /// <summary>
        /// Event manager that handles the distribution of data changes to subcribers
        /// </summary>
        protected TEventManager EventManager { get; }

        /// <summary>
        /// Access to the project service that offers messaging service, numeric services and input validations
        /// </summary>
        protected IProjectServices ProjectServices { get; }

        /// <summary>
        /// Conflict resolver provider interface that provides conflict resolving solutions for model objects and parameters
        /// </summary>
        protected IDataConflictHandlerProvider<TData> ConflictHandlerProvider { get; }

        /// <summary>
        /// Creates a new model input manager from data object, event manager and project services
        /// </summary>
        /// <param name="data"></param>
        /// <param name="manager"></param>
        /// <param name="services"></param>
        public ModelInputManager(TData data, TEventManager manager, IProjectServices services) : base()
        {
            EventManager = manager ?? throw new ArgumentNullException(nameof(manager));
            ProjectServices = services ?? throw new ArgumentNullException(nameof(services));
            DataAccessProvider = Basic.DataAccessProvider.Create(data, services.DataAccessLocker);
            DataReaderProvider = Basic.DataReadProvider.Create(data, data.AsReadOnly(), services.DataAccessLocker);
            ConflictHandlerProvider = MakeConflictHandlerProvider();
        }

        /// <summary>
        /// Tries to reset the manager data to the default values and distributes the information to all subscribers
        /// </summary>
        /// <returns></returns>
        protected override IOperationReport TryResetManagerDataToDefault()
        {
            void Operation(DataAccessor<TData> dataAccess, OperationReport report)
            {
                dataAccess.Query(data => data.ResetToDefault());
            }
            void OnSuccess()
            {
                EventManager.OnManagerResets.DistributeAsync().Wait();
            }
            return InvokeDataOperation("Reset manager data to default state", Operation, OnSuccess);
        }

        /// <summary>
        /// Performs a data access query on the manager while using the required locking mechanisms and calling an on success action which handles the returned object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        protected IOperationReport InvokeDataOperation<TResult>(string name, DataOperation<TResult> accessQuery, Action<TResult> onSuccess)
        {
            var operationResult = new OperationReport(name);
            if (ProjectServices.TryGetInputLock(out IDisposable projectLock))
            {
                using (projectLock)
                {
                    try
                    {
                        TResult queryResult;
                        using (var dataAccess = DataAccessProvider.Create())
                        {
                            queryResult = accessQuery(dataAccess, operationResult);
                        }
                        if (operationResult.IsGood)
                        {
                            onSuccess(queryResult);
                        }
                    }
                    catch (Exception exception)
                    {
                        operationResult.AddException(exception);
                    }
                    return operationResult;
                }
            }
            return operationResult.AsBusySignal();
        }

        /// <summary>
        /// Performs a void data access query on the manager while using the required locking mechanisms and calling an on success action without arguments
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        protected IOperationReport InvokeDataOperation(string name, DataOperation accessQuery, Action onSuccess)
        {
            var operationResult = new OperationReport(name);
            if (ProjectServices.TryGetInputLock(out IDisposable projectLock))
            {
                using (projectLock)
                {
                    try
                    {
                        using (var dataAccess = DataAccessProvider.Create())
                        {
                            accessQuery(dataAccess, operationResult);
                        }
                        if (operationResult.IsGood)
                        {
                            onSuccess();
                        }
                    }
                    catch (Exception exception)
                    {
                        operationResult.AddException(exception);
                    }
                    return operationResult;
                }
            }
            return operationResult.AsBusySignal();
        }

        /// <summary>
        /// Takes a previously created, linked and validated model object and inputs it into the passed mode object list (Auto assigns an index)
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="newObj"></param>
        /// <param name="objData"></param>
        /// <returns></returns>
        protected void WriteToModelObjectList<TObject>(TObject newObj, IList<TObject> objData) where TObject : ModelObject, new()
        {
            newObj.Index = objData.ReplaceFirstOrAdd(item => item.IsDeprecated, newObj);
        }

        /// <summary>
        /// Deprecates model object in a data list, returns false if the object was already deprecated. With optional restricted index list
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="obj"></param>
        /// <param name="objData"></param>
        /// <param name="restricted"></param>
        /// <returns></returns>
        protected bool DeprecateModelObject<TObject>(IModelObject obj, IList<TObject> objData, params int[] restricted) where TObject : ModelObject
        {
            if (restricted.Contains(obj.Index))
            {
                throw new ArgumentException($"The index {obj.Index} for {obj.GetModelObjectName()} is protected from deprecation");
            }
            if (obj.Index >= objData.Count)
            {
                throw new ArgumentOutOfRangeException($"{obj.GetModelObjectName()} index is out of range", nameof(obj.Index));
            }
            bool changed = !objData[obj.Index].IsDeprecated;
            objData[obj.Index].Deprecate();
            return changed;
        }

        /// <summary>
        /// Popultaes the original object with new data or restores the non-deprecated status
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="orgObject"></param>
        /// <param name="replaceObject"></param>
        /// <param name="replace"></param>
        protected void RepopulateOrRestoreOriginal<TObject>(TObject orgObject, TObject replaceObject, bool replace) where TObject : ModelObject, new()
        {
            if (replace)
            {
                orgObject.PopulateObject(replaceObject);
            }
            orgObject.Restore();
        }

        /// <summary>
        /// Takes a list of model objects, cleans all deprecated entries, reindexes all and returns the reindexing information (Returns empty reindexing if nothing was removed)
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="objData"></param>
        /// <returns></returns>
        protected ReindexingList CleanAndReindexModelObjects<TObject>(IList<TObject> objData) where TObject : ModelObject
        {
            if (objData.All(obj => !obj.IsDeprecated))
            {
                return new ReindexingList();
            }

            var reindexing = new ReindexingList(objData.Count);
            int newIndex = -1;
            foreach (TObject obj in objData)
            {
                reindexing.Add((obj.Index, (obj.IsDeprecated) ? -1 : ++newIndex));
                obj.Index = newIndex;
            }
            objData.RemoveAll(obj => obj.IsDeprecated);
            reindexing.TrimExcess();
            return reindexing;
        }

        /// <summary>
        /// Sets a new model parameter value by populating the original one with the new information
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="orgParam"></param>
        /// <param name="newParam"></param>
        /// <returns></returns>
        protected void ReplaceModelParameter<T1, T2>(T2 orgParam, T1 newParam) where T2 : ModelParameter, T1 where T1 : IModelParameter
        {
            orgParam.PopulateObject(newParam);
        }

        /// <summary>
        /// Performs the default model parameter change operation with or without extended data invalidation (Validation, conflict resolving and event distribution)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="newParam"></param>
        /// <param name="paramAccessQuery"></param>
        /// <param name="invalidatesCache"></param>
        /// <returns></returns>
        protected IOperationReport DefaultSetModelParameter<T1, T2>(T1 newParam, Func<DataAccessor<TData>, T2> paramAccessQuery, bool invalidatesCache) where T2 : ModelParameter, T1, new() where T1 : IModelParameter
        {
            // Build and link the overrite information into a new internal object
            T2 tmpObject = ModelParameter.BuildInternalObject<T2>(newParam) ?? throw new ArgumentException("Conversion to internal type failed");

            T2 Operation(DataAccessor<TData> dataAccess, OperationReport report)
            {
                T2 orgObject = paramAccessQuery(dataAccess);
                report.SetValidationReport(ProjectServices.ValidationServices.ValidateParameter(tmpObject, dataAccess.AsReader(DataReaderProvider)));
                if (report.IsGood)
                {
                    report.CacheExpired = invalidatesCache;
                    ReplaceModelParameter(orgObject, tmpObject);

                    // Send the newly populated original into the handling and not the temporary
                    var conflictReport = ConflictHandlerProvider.ChangedModelParameterHandler.ResolveConflicts(orgObject, dataAccess);
                    report.SetConflictReport(conflictReport);
                }
                return orgObject;
            }

            void OnSuccess(T2 orgObject)
            {
                if (invalidatesCache)
                {
                    EventManager.OnExtendedDataExpiration.Distribute();
                }
                EventManager.OnChangedModelParameters.Distribute(ModelParameterEventArgs.Create(orgObject));
            }

            return InvokeDataOperation($"Set model parameter {tmpObject.GetParameterName()} in the manager", Operation, OnSuccess);
        }

        /// <summary>
        /// Executes the default register operation for a new model object through a data access query (Validation, slot assignment and event distribution)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dataAccessQuery"></param>
        /// <returns></returns>
        protected IOperationReport DefaultRegisterModelObject<T1, T2>(T1 obj, Func<DataAccessor<TData>, IList<T2>> dataAccessQuery) where T1 : IModelObject where T2 : ModelObject, T1, new()
        {
            // Build and link a new internal object of the replacemed type
            T2 newInternal = ModelObject.BuildInternalObject<T2>(obj) ?? throw new ArgumentException("Could not build internal object from interface");
            ProjectServices.DataTracker.LinkModelObject(newInternal);

            bool Operation(DataAccessor<TData> accessor, OperationReport report)
            {
                report.SetValidationReport(ProjectServices.ValidationServices.ValidateObject(newInternal, accessor.AsReader(DataReaderProvider)));
                if (report.IsGood)
                {
                    WriteToModelObjectList(newInternal, dataAccessQuery(accessor));

                    var conflictReport = ConflictHandlerProvider.NewModelObjectHandler.ResolveConflicts(newInternal, accessor);
                    report.SetConflictReport(conflictReport);
                    return true;
                }
                return false;
            }
            void OnSuccess(bool isGood)
            {
                EventManager.OnNewModelObjects.Distribute(ModelObjectEventArgs.Create((T1)newInternal));
            }
            return InvokeDataOperation($"Add new {newInternal.GetModelObjectName()} to the manager", Operation, OnSuccess);
        }

        /// <summary>
        /// Executes the default removal operation of a model object (Deprecate object, distribute event)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dataAccessQuery"></param>
        /// <returns></returns>
        protected IOperationReport DefaultRemoveModelObject<T1, T2>(T1 obj, Func<DataAccessor<TData>, IList<T2>> dataAccessQuery, params int[] restrictedIndices) where T1 : IModelObject where T2 : ModelObject, T1, new()
        {
            // Lookup the actual internal object interface and cast it to the internal data type
            T2 internalObj = (T2)ProjectServices.DataTracker.FindObjectInterfaceByIndex<T1>(obj.Index);

            bool Operation(DataAccessor<TData> accessor, OperationReport report)
            {
                var result = DeprecateModelObject(internalObj, dataAccessQuery(accessor), restrictedIndices);

                var conflictReport = ConflictHandlerProvider.RemovedModelObjectsHandler.ResolveConflicts(internalObj, accessor);
                report.SetConflictReport(conflictReport);

                return result;
            }

            void OnSuccess(bool deprecationSuccess)
            {
                if (deprecationSuccess)
                {
                    EventManager.OnExtendedDataExpiration.Distribute();
                    EventManager.OnRemovedModelObjects.Distribute(ModelObjectEventArgs.Create((T1)internalObj));
                }
            }
            return InvokeDataOperation($"Remove {internalObj.GetModelObjectName()} ({ internalObj.Index}) from manager", Operation, OnSuccess);
        }

        /// <summary>
        /// Executes the default model object replacement operation (Deprecate original, validate new, restore original or set new and distribute event)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dataAccessQuery"></param>
        /// <param name="restrictedIndices"></param>
        /// <returns></returns>
        protected IOperationReport DefaultReplaceModelObject<T1, T2>(T1 orgObj, T1 newObj, Func<DataAccessor<TData>, IList<T2>> dataAccessQuery) where T1 : IModelObject where T2 : ModelObject, T1, new()
        {
            // Build and link a temporary internal object with the replacement information
            T2 tmpObject = ModelObject.BuildInternalObject<T2>(newObj) ?? throw new ArgumentException("Could not build internal object from interface");
            ProjectServices.DataTracker.LinkModelObject(tmpObject);
            tmpObject.Index = orgObj.Index;

            T2 Operation(DataAccessor<TData> accessor, OperationReport report)
            {
                T2 orgInternal = dataAccessQuery(accessor)[orgObj.Index];
                orgInternal.Deprecate();

                report.SetValidationReport(ProjectServices.ValidationServices.ValidateObject(tmpObject, accessor.AsReader(DataReaderProvider)));
                RepopulateOrRestoreOriginal(orgInternal, tmpObject, report.IsGood);

                if (report.IsGood)
                {
                    var conflictReport = ConflictHandlerProvider.ChangedModelObjectsHandler.ResolveConflicts(orgInternal, accessor);
                    report.SetConflictReport(conflictReport);
                    return orgInternal;
                }

                return null;
            }
            void OnSuccess(T2 changedObject)
            {
                if (changedObject != null)
                {
                    EventManager.OnChangedModelObjects.Distribute(ModelObjectEventArgs.Create((T1)changedObject));
                }
            }
            return InvokeDataOperation($"Replace {tmpObject.GetModelObjectName()} ({ orgObj.Index}) in the manager", Operation, OnSuccess);
        }

        /// <summary>
        /// Exectutes the default reflection based clean deprecated data operation for indexed model data (Lookup data, remove deprecated and distribute reindexing lists)
        /// </summary>
        /// <returns></returns>
        protected IOperationReport DefaultCleanDeprecatedData()
        {
            (PropertyInfo, ReindexingList)[] Operation(DataAccessor<TData> accessor, OperationReport report)
            {
                var propertyInfo = GetIndexedDataProperties();
                var dataLists = GetIndexedDataLists(propertyInfo, accessor);

                // This cannot be returned as a zip iterator currently (Causes unexpected error which invalidates the zip iterator)
                return propertyInfo.Zip(dataLists, (info, list) => (info, CleanAndReindexModelObjects(list))).ToArray();
            }
            void OnSuccess(IEnumerable<(PropertyInfo, ReindexingList)> reindexingData)
            {
                foreach (var (Info, Reindexing) in reindexingData)
                {
                    EventManager.OnChangedModelIndexing.Distribute(MakeModelIndexingEventArgs(Info, Reindexing));
                }
            }
            return InvokeDataOperation("Clean deprecated data and reindex model objects", Operation, OnSuccess);
        }

        /// <summary>
        /// Virtual method to get the conflict resolver for the implementing manager (Default is an empty pipeline resolver that does not contain any handlers)
        /// </summary>
        /// <returns></returns>
        protected abstract IDataConflictHandlerProvider<TData> MakeConflictHandlerProvider();

        /// <summary>
        /// Searches the data type for all properties that are marked as indexed object data lists
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<PropertyInfo> GetIndexedDataProperties()
        {
            return typeof(TData)
                .GetProperties(BindingFlags.Instance| BindingFlags.Public | BindingFlags.NonPublic)
                .Where(property => property.GetCustomAttribute(typeof(IndexedModelDataAttribute)) != null);
        }

        /// <summary>
        /// Takes a sequence of property info for indexable data lists and create the sequence of list interfaces to the model object lists
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        protected IEnumerable<IList<ModelObject>> GetIndexedDataLists(IEnumerable<PropertyInfo> propertyInfo, DataAccessor<TData> accessor)
        {
            return propertyInfo.Select(info => new ListInterfaceAdapter<ModelObject>((IList)info.GetValue(accessor.Query(data => data))));
        }

        /// <summary>
        /// Creates a reindexin event argument from a property info and reindexing list
        /// </summary>
        /// <param name="info"></param>
        /// <param name="reindexing"></param>
        /// <returns></returns>
        protected ModelIndexingEventArgs MakeModelIndexingEventArgs(PropertyInfo info, ReindexingList reindexing)
        {
            if (info.GetCustomAttribute(typeof(IndexedModelDataAttribute)) is IndexedModelDataAttribute attribute)
            {
                return (ModelIndexingEventArgs)Activator.CreateInstance(typeof(ModelIndexingEventArgs<>).MakeGenericType(attribute.InterfaceType), reindexing);
            }
            throw new ArgumentException("The proprety info is not marked as an indexed data set", nameof(info));
        }

        /// <summary>
        /// Get all supported model object types (Default method uses reflection to serach data object)
        /// </summary>
        /// <returns></returns>
        public override Type[] GetSupportedModelObjects()
        {
            var properties = typeof(TData).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return properties
                .Select(property => property.GetCustomAttribute(typeof(IndexedModelDataAttribute)) as IndexedModelDataAttribute)
                .Where(attribute => attribute != null)
                .Where(attribute => !attribute.IsAutoManaged)
                .Select(attribute => attribute.InterfaceType).ToArray();
        }

        /// <summary>
        /// Get all supported model parameter types (Default method uses reflection to serach data object)
        /// </summary>
        /// <returns></returns>
        public override Type[] GetSupportedModelParameters()
        {
            var properties = typeof(TData).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return properties
                .Select(property => property.GetCustomAttribute(typeof(ModelParameterAttribute)) as ModelParameterAttribute)
                .Where(attribute => attribute != null)
                .Select(attribute => attribute.InterfaceType)
                .ToArray();
        }


    }
}
