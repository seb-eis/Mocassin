﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mocassin.Framework.Collections;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Operations;
using Mocassin.Framework.Processing;
using Mocassin.Framework.Reflection;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Abstract base class for all model input manager implementations
    /// </summary>
    internal abstract class ModelInputManager : IModelInputPort
    {
        /// <summary>
        ///     Async object handler pipeline for parameter inputs that are always a single value and do not require indexing
        ///     operations
        /// </summary>
        private AsyncBreakPipeline<IOperationReport> ParameterInputPipeline { get; }

        /// <summary>
        ///     Async object handler pipeline for new model object input operations
        /// </summary>
        private AsyncBreakPipeline<IOperationReport> ObjectInputPipeline { get; }

        /// <summary>
        ///     Async object handler pipeline for model object replacement operations
        /// </summary>
        private AsyncBreakPipeline<IOperationReport> ObjectReplacementPipeline { get; }

        /// <summary>
        ///     Async object handler pipeline for model object removal operations
        /// </summary>
        private AsyncBreakPipeline<IOperationReport> ObjectRemovalPipeline { get; }

        /// <summary>
        ///     General constructor that calls the initialization functions for the handler arrays
        /// </summary>
        protected ModelInputManager()
        {
            ParameterInputPipeline =
                new AsyncBreakPipeline<IOperationReport>(GetAsyncInvalidParameterHandler(), MakeAsyncParameterChangeProcessors());
            ObjectInputPipeline =
                new AsyncBreakPipeline<IOperationReport>(GetAsyncInvalidObjectHandler(), MakeAsyncObjectInputProcessors());
            ObjectRemovalPipeline =
                new AsyncBreakPipeline<IOperationReport>(GetAsyncInvalidObjectHandler(), MakeAsyncObjectRemovalProcessors());
            ObjectReplacementPipeline =
                new AsyncBreakPipeline<IOperationReport>(GetAsyncInvalidObjectHandler(), MakeAsyncObjectReplacementProcessors());
        }

        /// <inheritdoc />
        public Task<IOperationReport> SetModelParameter<TParameter>(TParameter modelParameter)
            where TParameter : IModelParameter
        {
            if (modelParameter == null)
                throw new ArgumentNullException(nameof(modelParameter));

            return ParameterInputPipeline.Process(modelParameter);
        }

        /// <inheritdoc />
        public Task<IOperationReport> InputModelObject<TObject>(TObject modelObject)
            where TObject : IModelObject =>
            modelObject == null
                ? throw new ArgumentNullException(nameof(modelObject))
                : ObjectInputPipeline.Process(modelObject);

        /// <inheritdoc />
        public Task<IOperationReport> RemoveModelObject<TObject>(TObject modelObject)
            where TObject : IModelObject
        {
            if (modelObject == null)
                throw new ArgumentNullException(nameof(modelObject));

            return ObjectRemovalPipeline.Process(modelObject);
        }

        /// <inheritdoc />
        public Task<IOperationReport> ReplaceModelObject<TObject>(TObject modelObject, TObject replacement)
            where TObject : IModelObject
        {
            if (modelObject == null)
                throw new ArgumentNullException(nameof(modelObject));

            if (replacement == null)
                throw new ArgumentNullException(nameof(replacement));

            return ObjectReplacementPipeline.Process(modelObject, replacement);
        }

        /// <inheritdoc />
        public Task<IOperationReport> CleanupManager() => Task.Run(TryCleanDeprecatedData);

        /// <inheritdoc />
        public Task<IOperationReport> ResetManager() => Task.Run(TryResetManagerDataToDefault);

        /// <inheritdoc />
        public IEnumerable<Type> GetSupportedModelTypes() => GetSupportedModelObjects().Concat(GetSupportedModelParameters());

        /// <inheritdoc />
        public abstract IDataReader<IModelDataPort> GetDataReader();

        /// <summary>
        ///     Tries to reset the manager data to its default state
        /// </summary>
        /// <returns></returns>
        protected abstract IOperationReport TryResetManagerDataToDefault();

        /// <summary>
        ///     Tries to clean deprecate data from the manager
        /// </summary>
        /// <returns></returns>
        protected abstract IOperationReport TryCleanDeprecatedData();

        /// <summary>
        ///     Get the async handler that is called if processing of model inputs in the pipelines fails because the manager does
        ///     not support the type
        /// </summary>
        /// <returns></returns>
        protected IAsyncObjectProcessor<IOperationReport> GetAsyncInvalidObjectHandler()
        {
            IOperationReport HandlerFunction(object obj) => OperationReport.MakeUnexpectedTypeResult(obj.GetType(), GetSupportedModelObjects());

            return ObjectProcessorFactory.CreateAsync<object, IOperationReport>(HandlerFunction);
        }

        /// <summary>
        ///     Get the async handler that is called if processing of model parameters in the pipeline fails because the manager
        ///     does no support the type
        /// </summary>
        /// <returns></returns>
        protected IAsyncObjectProcessor<IOperationReport> GetAsyncInvalidParameterHandler()
        {
            IOperationReport HandlerFunction(object obj) => OperationReport.MakeUnexpectedTypeResult(obj.GetType(), GetSupportedModelParameters());

            return ObjectProcessorFactory.CreateAsync<object, IOperationReport>(HandlerFunction);
        }

        /// <summary>
        ///     Identify all non public functions that are marked as data operation methods of new object inputs and create the
        ///     processor list for the pipeline
        /// </summary>
        protected List<IAsyncObjectProcessor<IOperationReport>> MakeAsyncObjectInputProcessors()
        {
            var processors = MakeAsyncDataOperationProcessors(DataOperationType.NewObject).ToList();
            if (processors.Count != GetSupportedModelObjects().Length)
            {
                throw new InvalidOperationException(
                    "Object processor list mismatch to supported data objects. Input function is missing or could not be detected!");
            }

            return processors;
        }

        /// <summary>
        ///     Identify all non public functions that are marked as data operation methods of model object replacements and create
        ///     the processor list for the pipeline
        /// </summary>
        protected List<IAsyncObjectProcessor<IOperationReport>> MakeAsyncObjectReplacementProcessors()
        {
            var processors = MakeAsyncDataOperationProcessors(DataOperationType.ObjectChange).ToList();
            if (processors.Count != GetSupportedModelObjects().Length)
            {
                throw new InvalidOperationException(
                    "Object processor list mismatch to supported data objects. Input function is missing or could not be detected!");
            }

            return processors;
        }

        /// <summary>
        ///     Identify all non public functions that are marked as data operation methods of new object inputs and create the
        ///     processor list for pipeline
        /// </summary>
        protected List<IAsyncObjectProcessor<IOperationReport>> MakeAsyncObjectRemovalProcessors()
        {
            var processors = MakeAsyncDataOperationProcessors(DataOperationType.ObjectRemoval).ToList();
            if (processors.Count != GetSupportedModelObjects().Length)
            {
                throw new InvalidOperationException(
                    "Object processor list mismatch to supported data objects. Input function is missing or could not be detected!");
            }

            return processors;
        }

        /// <summary>
        ///     Identify all non public functions that are marked as data operation methods of new object inputs and create the
        ///     processor list for pipeline
        /// </summary>
        /// <returns></returns>
        protected List<IAsyncObjectProcessor<IOperationReport>> MakeAsyncParameterChangeProcessors()
        {
            var processors = MakeAsyncDataOperationProcessors(DataOperationType.ParameterChange).ToList();
            if (processors.Count != GetSupportedModelParameters().Length)
            {
                throw new InvalidOperationException(
                    "Object processor list mismatch to supported data objects. Input function is missing or could not be detected!");
            }

            return processors;
        }

        /// <summary>
        ///     Get the types of all model objects that are supported by this manager
        /// </summary>
        /// <returns></returns>
        public abstract Type[] GetSupportedModelObjects();

        /// <summary>
        ///     Get the types of all model parameters that are supported by this manager
        /// </summary>
        /// <returns></returns>
        public abstract Type[] GetSupportedModelParameters();

        /// <summary>
        ///     Searches the input manager for all non public methods marked as data operations of the specified type and creates
        ///     a sequence of object processors
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        protected IEnumerable<IAsyncObjectProcessor<IOperationReport>> MakeAsyncDataOperationProcessors(DataOperationType operationType)
        {
            bool MethodSearch(MethodInfo methodInfo)
            {
                if (methodInfo.GetCustomAttribute(typeof(DataOperationAttribute)) is DataOperationAttribute attribute)
                    return attribute.OperationType == operationType;

                return false;
            }

            return new ObjectProcessorBuilder().CreateAsyncProcessors<IOperationReport>(this, MethodSearch,
                BindingFlags.Instance | BindingFlags.NonPublic);
        }
    }

    /// <summary>
    ///     Generic base class for all model data input managers that use specific data, read only data access port and event
    ///     manager to distribute changes
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TPort"></typeparam>
    /// <typeparam name="TEventManager"></typeparam>
    internal abstract class ModelInputManager<TData, TPort, TEventManager> : ModelInputManager
        where TData : ModelData<TPort>
        where TPort : class, IModelDataPort
        where TEventManager : ModelEventManager
    {
        /// <summary>
        ///     Provider for safe accessors to the data object
        /// </summary>
        protected DataAccessorSource<TData> DataAccessorSource { get; }

        /// <summary>
        ///     Provider for read only accessors to the data object that use the read only specified data port
        /// </summary>
        protected DataReaderSource<TData, TPort> DataReaderSource { get; }

        /// <summary>
        ///     Event manager that handles the distribution of data changes to subscribers
        /// </summary>
        protected TEventManager EventManager { get; }

        /// <summary>
        ///     Access to the project service that offers messaging service, numeric services and input validations
        /// </summary>
        protected IModelProject ModelProject { get; }

        /// <summary>
        ///     Conflict resolver provider interface that provides conflict resolving solutions for model objects and parameters
        /// </summary>
        protected IDataConflictHandlerProvider<TData> ConflictHandlerProvider { get; }

        /// <summary>
        ///     Creates a new model input manager from data object, event manager and project services
        /// </summary>
        /// <param name="modelData"></param>
        /// <param name="eventManager"></param>
        /// <param name="project"></param>
        protected ModelInputManager(TData modelData, TEventManager eventManager, IModelProject project)
        {
            EventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            ModelProject = project ?? throw new ArgumentNullException(nameof(project));
            DataAccessorSource = Basic.DataAccessorSource.Create(modelData, project.AccessLockSource);
            DataReaderSource = Basic.DataReaderSource.Create(modelData, modelData.AsReadOnly(), project.AccessLockSource);
            ConflictHandlerProvider = CreateDataConflictHandlerProvider();
        }

        /// <inheritdoc />
        public override IDataReader<IModelDataPort> GetDataReader() => DataReaderSource.Create();

        /// <inheritdoc />
        protected override IOperationReport TryResetManagerDataToDefault()
        {
            void Operation(DataAccessor<TData> dataAccess, OperationReport report)
            {
                dataAccess.Query(data => data.ResetToDefault());
            }

            void OnSuccess()
            {
                EventManager.OnManagerResets.OnNext();
            }

            return InvokeDataOperation("Reset manager data to default state", Operation, OnSuccess);
        }

        /// <summary>
        ///     Performs a data access query on the manager while using the required locking mechanisms and calling an on success
        ///     action which handles the returned object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="accessQuery"></param>
        /// <param name="onSuccess"></param>
        /// <returns></returns>
        protected IOperationReport InvokeDataOperation<TResult>(string name, DataOperation<TResult> accessQuery, Action<TResult> onSuccess)
        {
            var operationResult = new OperationReport(name);
            if (!ModelProject.TryGetInputLock(out var projectLock))
                return operationResult.AsBusySignal();

            using (projectLock)
            {
                try
                {
                    TResult queryResult;
                    using (var dataAccess = DataAccessorSource.Create())
                    {
                        queryResult = accessQuery(dataAccess, operationResult);
                    }

                    if (operationResult.IsGood) onSuccess(queryResult);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    operationResult.AddException(exception);
                }

                return operationResult;
            }
        }

        /// <summary>
        ///     Performs a void data access query on the manager while using the required locking mechanisms and calling an on
        ///     success action without arguments
        /// </summary>
        /// <param name="name"></param>
        /// <param name="accessQuery"></param>
        /// <param name="onSuccess"></param>
        /// <returns></returns>
        protected IOperationReport InvokeDataOperation(string name, DataOperation accessQuery, Action onSuccess)
        {
            var operationResult = new OperationReport(name);
            if (!ModelProject.TryGetInputLock(out var projectLock))
                return operationResult.AsBusySignal();

            using (projectLock)
            {
                try
                {
                    using (var dataAccess = DataAccessorSource.Create())
                    {
                        accessQuery(dataAccess, operationResult);
                    }

                    if (operationResult.IsGood) onSuccess();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    operationResult.AddException(exception);
                }

                return operationResult;
            }
        }

        /// <summary>
        ///     Takes a previously created, linked and validated model object and inputs it into the passed mode object list (Auto
        ///     assigns an index)
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="newObj"></param>
        /// <param name="objData"></param>
        /// <returns></returns>
        protected void WriteToModelObjectList<TObject>(TObject newObj, IList<TObject> objData)
            where TObject : ModelObject, new()
        {
            newObj.Index = objData.ReplaceFirstOrAdd(item => item.IsDeprecated, newObj);
        }

        /// <summary>
        ///     Deprecates model object in a data list, returns false if the object was already deprecated. With optional
        ///     restricted index list
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="obj"></param>
        /// <param name="objData"></param>
        /// <param name="restricted"></param>
        /// <returns></returns>
        protected bool DeprecateModelObject<TObject>(IModelObject obj, IList<TObject> objData, params int[] restricted)
            where TObject : ModelObject
        {
            if (restricted.Contains(obj.Index))
                throw new ArgumentException($"The index {obj.Index} for {obj} is protected from deprecation");

            if (obj.Index >= objData.Count)
                throw new ArgumentOutOfRangeException($"{obj} index is out of range", nameof(obj.Index));

            var changed = !objData[obj.Index].IsDeprecated;
            objData[obj.Index].Deprecate();
            return changed;
        }

        /// <summary>
        ///     Populates the original object with new data or restores the non-deprecated status
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="orgObject"></param>
        /// <param name="replaceObject"></param>
        /// <param name="replace"></param>
        protected void RepopulateOrRestoreOriginal<TObject>(TObject orgObject, TObject replaceObject, bool replace)
            where TObject : ModelObject, new()
        {
            if (replace) orgObject.PopulateFrom(replaceObject);

            orgObject.Restore();
        }

        /// <summary>
        ///     Takes a list of model objects, cleans all deprecated entries, re-indexes all and returns the reindexing information
        ///     (Returns empty reindexing if nothing was removed)
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="objData"></param>
        /// <returns></returns>
        protected ReindexingList CleanAndReindexModelObjects<TObject>(IList<TObject> objData)
            where TObject : ModelObject
        {
            if (objData.All(obj => !obj.IsDeprecated))
                return new ReindexingList();

            var reindexing = new ReindexingList(objData.Count);
            var newIndex = -1;
            foreach (var obj in objData)
            {
                reindexing.Add((obj.Index, obj.IsDeprecated ? -1 : ++newIndex));
                obj.Index = newIndex;
            }

            objData.RemoveAll(obj => obj.IsDeprecated);
            reindexing.TrimExcess();
            return reindexing;
        }

        /// <summary>
        ///     Sets a new model parameter value by populating the original one with the new information
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
        ///     Performs the default model parameter change operation with or without extended data invalidation (Validation,
        ///     conflict resolving and event distribution)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="newParam"></param>
        /// <param name="paramAccessQuery"></param>
        /// <param name="invalidatesCache"></param>
        /// <returns></returns>
        protected IOperationReport DefaultSetModelParameter<T1, T2>(T1 newParam, Func<DataAccessor<TData>, T2> paramAccessQuery,
            bool invalidatesCache)
            where T2 : ModelParameter, T1, new()
            where T1 : IModelParameter
        {
            if (!TryBuildAndLinkInternalModelParameter<T2>(newParam, out var newInternal, out var message))
                return OperationReport.MakeObjectBuildErrorReport($"Set [{newParam?.GetParameterName() ?? "??"}]", message);

            T2 Operation(DataAccessor<TData> dataAccess, OperationReport report)
            {
                var orgObject = paramAccessQuery(dataAccess);
                report.SetValidationReport(
                    ModelProject.ValidationServices.ValidateParameter(newInternal, dataAccess.AsReader(DataReaderSource)));

                if (!report.IsGood)
                    return orgObject;

                report.IsCacheExpired = invalidatesCache;
                ReplaceModelParameter(orgObject, newInternal);

                // Send the newly populated original into the handling and not the temporary
                var conflictReport = ConflictHandlerProvider.ChangedModelParameterHandler.ResolveConflicts(orgObject, dataAccess);
                report.SetConflictReport(conflictReport);

                return orgObject;
            }

            void OnSuccess(T2 orgObject)
            {
                if (invalidatesCache)
                    EventManager.OnExtendedDataExpiration.OnNext();

                EventManager.OnChangedModelParameters.OnNext(ModelParameterEventArgs.Create(orgObject));
            }

            return InvokeDataOperation($"Register [{newInternal.GetParameterName()}]", Operation, OnSuccess);
        }

        /// <summary>
        ///     Executes the default register operation for a new model object through a data access query (Validation, slot
        ///     assignment and event distribution)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dataAccessQuery"></param>
        /// <returns></returns>
        protected IOperationReport DefaultRegisterModelObject<T1, T2>(T1 obj, Func<DataAccessor<TData>, IList<T2>> dataAccessQuery)
            where T1 : IModelObject
            where T2 : ModelObject, T1, new()
        {
            // Build and link a new internal object of the replacement type
            if (!TryBuildAndLinkInternalModelObject<T2>(obj, out var newInternal, out var message))
                return OperationReport.MakeObjectBuildErrorReport($"Register [{obj}]", message);


            bool Operation(DataAccessor<TData> accessor, OperationReport report)
            {
                report.SetValidationReport(
                    ModelProject.ValidationServices.ValidateObject(newInternal, accessor.AsReader(DataReaderSource)));

                if (!report.IsGood)
                    return false;

                WriteToModelObjectList(newInternal, dataAccessQuery(accessor));

                var conflictReport = ConflictHandlerProvider.NewModelObjectHandler.ResolveConflicts(newInternal, accessor);
                report.SetConflictReport(conflictReport);
                return true;
            }

            void OnSuccess(bool isGood)
            {
                EventManager.OnNewModelObjects.OnNext(ModelObjectEventArgs.Create((T1) newInternal));
            }

            return InvokeDataOperation($"Register [{newInternal}]", Operation, OnSuccess);
        }

        /// <summary>
        ///     Executes the default removal operation of a model object (Deprecate object, distribute event)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dataAccessQuery"></param>
        /// <param name="restrictedIndices"></param>
        /// <returns></returns>
        protected IOperationReport DefaultRemoveModelObject<T1, T2>(T1 obj, Func<DataAccessor<TData>, IList<T2>> dataAccessQuery,
            params int[] restrictedIndices)
            where T1 : IModelObject
            where T2 : ModelObject, T1, new()
        {
            // Lookup the actual internal object interface and cast it to the internal data type
            var internalObj = (T2) ModelProject.DataTracker.FindObject<T1>(obj.Index);

            bool Operation(DataAccessor<TData> accessor, OperationReport report)
            {
                var result = DeprecateModelObject(internalObj, dataAccessQuery(accessor), restrictedIndices);

                var conflictReport = ConflictHandlerProvider.RemovedModelObjectsHandler.ResolveConflicts(internalObj, accessor);
                report.SetConflictReport(conflictReport);

                return result;
            }

            void OnSuccess(bool deprecationSuccess)
            {
                if (!deprecationSuccess)
                    return;

                EventManager.OnExtendedDataExpiration.OnNext();
                EventManager.OnRemovedModelObjects.OnNext(ModelObjectEventArgs.Create((T1) internalObj));
            }

            return InvokeDataOperation($"Remove [{internalObj}] ({internalObj.Index})", Operation, OnSuccess);
        }

        /// <summary>
        ///     Executes the default model object replacement operation (Deprecate original, validate new, restore original or set
        ///     new and distribute event)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="newObj"></param>
        /// <param name="dataAccessQuery"></param>
        /// <param name="orgObj"></param>
        /// <returns></returns>
        protected IOperationReport DefaultReplaceModelObject<T1, T2>(T1 orgObj, T1 newObj,
            Func<DataAccessor<TData>, IList<T2>> dataAccessQuery)
            where T1 : IModelObject
            where T2 : ModelObject, T1, new()
        {
            // Build and link a temporary internal object with the replacement information
            if (!TryBuildAndLinkInternalModelObject<T2>(newObj, out var tmpObject, out var message))
                return OperationReport.MakeObjectBuildErrorReport($"Replace [{orgObj}]", message);

            tmpObject.Index = orgObj.Index;

            T2 Operation(DataAccessor<TData> accessor, OperationReport report)
            {
                var orgInternal = dataAccessQuery(accessor)[orgObj.Index];
                orgInternal.Deprecate();

                report.SetValidationReport(ModelProject.ValidationServices.ValidateObject(tmpObject, accessor.AsReader(DataReaderSource)));
                RepopulateOrRestoreOriginal(orgInternal, tmpObject, report.IsGood);

                if (!report.IsGood) return null;

                var conflictReport = ConflictHandlerProvider.ChangedModelObjectsHandler.ResolveConflicts(orgInternal, accessor);
                report.SetConflictReport(conflictReport);
                return orgInternal;
            }

            void OnSuccess(T2 changedObject)
            {
                if (changedObject != null) EventManager.OnChangedModelObjects.OnNext(ModelObjectEventArgs.Create((T1) changedObject));
            }

            return InvokeDataOperation($"Replace [{tmpObject}] ({orgObj.Index})", Operation, OnSuccess);
        }

        /// <summary>
        ///     Executes the default reflection based clean deprecated data operation for indexed model data (Lookup data, remove
        ///     deprecated and distribute reindexing lists)
        /// </summary>
        /// <returns></returns>
        protected IOperationReport DefaultCleanDeprecatedData()
        {
            (PropertyInfo, ReindexingList)[] Operation(DataAccessor<TData> accessor, OperationReport report)
            {
                var propertyInfo = GetIndexedDataProperties().ToList();
                var dataLists = GetIndexedDataLists(propertyInfo, accessor);

                // This cannot be returned as a zip iterator currently (Causes unexpected error which invalidates the zip iterator)
                return propertyInfo.Zip(dataLists, (info, list) => (info, CleanAndReindexModelObjects(list))).ToArray();
            }

            void OnSuccess(IEnumerable<(PropertyInfo, ReindexingList)> reindexingData)
            {
                foreach (var (info, reindexing) in reindexingData)
                    EventManager.OnChangedModelIndexing.OnNext(MakeModelIndexingEventArgs(info, reindexing));
            }

            return InvokeDataOperation("Clean deprecated data and reindex model objects", Operation, OnSuccess);
        }

        /// <summary>
        ///     Virtual method to get the conflict resolver for the implementing manager (Default is an empty pipeline resolver
        ///     that does not contain any handlers)
        /// </summary>
        /// <returns></returns>
        protected abstract IDataConflictHandlerProvider<TData> CreateDataConflictHandlerProvider();

        /// <summary>
        ///     Searches the data type for all properties that are marked as indexed object data lists
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<PropertyInfo> GetIndexedDataProperties()
        {
            return typeof(TData)
                   .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                   .Where(property => property.GetCustomAttribute(typeof(IndexedModelDataAttribute)) != null);
        }

        /// <summary>
        ///     Takes a sequence of property info for index-able data lists and create the sequence of list interfaces to the model
        ///     object lists
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="accessor"></param>
        /// <returns></returns>
        protected IEnumerable<IList<ModelObject>> GetIndexedDataLists(IEnumerable<PropertyInfo> propertyInfo, DataAccessor<TData> accessor)
        {
            return propertyInfo.Select(info => new GenericListEmulator<ModelObject>((IList) info.GetValue(accessor.Query(data => data))));
        }

        /// <summary>
        ///     Creates a reindexing event argument from a property info and reindexing list
        /// </summary>
        /// <param name="info"></param>
        /// <param name="reindexing"></param>
        /// <returns></returns>
        protected ModelIndexingEventArgs MakeModelIndexingEventArgs(PropertyInfo info, ReindexingList reindexing)
        {
            if (info.GetCustomAttribute(typeof(IndexedModelDataAttribute)) is IndexedModelDataAttribute attribute)
            {
                return (ModelIndexingEventArgs) Activator.CreateInstance(
                    typeof(ModelIndexingEventArgs<>).MakeGenericType(attribute.InterfaceType), reindexing);
            }

            throw new ArgumentException("The property info is not marked as an indexed data set", nameof(info));
        }

        /// <inheritdoc />
        public override Type[] GetSupportedModelObjects()
        {
            var properties = typeof(TData).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return properties
                   .Select(property => property.GetCustomAttribute(typeof(IndexedModelDataAttribute)) as IndexedModelDataAttribute)
                   .Where(attribute => attribute != null)
                   .Where(attribute => !attribute.IsAutoManaged)
                   .Select(attribute => attribute.InterfaceType).ToArray();
        }

        /// <inheritdoc />
        public override Type[] GetSupportedModelParameters()
        {
            var properties = typeof(TData).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return properties
                   .Select(property => property.GetCustomAttribute(typeof(ModelParameterAttribute)) as ModelParameterAttribute)
                   .Where(attribute => attribute != null)
                   .Select(attribute => attribute.InterfaceType)
                   .ToArray();
        }

        /// <summary>
        ///     Tries to build and link an internal <see cref="ModelObject" /> from the passed <see cref="IModelObject" />
        ///     interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="modelObject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool TryBuildAndLinkInternalModelObject<T>(IModelObject obj, out T modelObject, out string message)
            where T : ModelObject, new()
        {
            message = null;

            if (!(ModelObject.BuildInternalObject<T>(obj) is { } tmpObj))
            {
                message = $"Could not convert [{obj}] to internal model object!";
                modelObject = null;
                return false;
            }

            try
            {
                ModelProject.DataTracker.LinkObject(tmpObj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                message = e.Message;
                modelObject = null;
                return false;
            }

            modelObject = tmpObj;
            return true;
        }

        /// <summary>
        ///     Tries to build and link an internal <see cref="ModelParameter" /> from the passed <see cref="IModelParameter" />
        ///     interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="modelObject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool TryBuildAndLinkInternalModelParameter<T>(IModelParameter obj, out T modelObject, out string message)
            where T : ModelParameter, new()
        {
            message = null;

            if (!(ModelParameter.BuildInternalObject<T>(obj) is { } tmpObj))
            {
                message = $"Could not convert interface [{obj?.GetParameterName()}] to internal parameter!";
                modelObject = null;
                return false;
            }

            try
            {
                ModelProject.DataTracker.LinkObject(tmpObj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                message = e.Message;
                modelObject = null;
                return false;
            }

            modelObject = tmpObj;
            return true;
        }

        /// <summary>
        ///     Defines a generic delegate for data operations with specific return value
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="report"></param>
        internal delegate TResult DataOperation<out TResult>(DataAccessor<TData> dataAccess, OperationReport report);

        /// <summary>
        ///     Defines a delegate for data operations with no return value
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="report"></param>
        internal delegate void DataOperation(DataAccessor<TData> dataAccess, OperationReport report);
    }
}