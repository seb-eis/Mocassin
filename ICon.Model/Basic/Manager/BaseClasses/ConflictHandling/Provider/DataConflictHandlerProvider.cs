using System;
using System.Linq;
using System.Reflection;
using Mocassin.Framework.Reflection;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Abstract base class for the implementation of specific data conflict resolvers
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public abstract class DataConflictHandlerProvider<T1> : IDataConflictHandlerProvider<T1>
        where T1 : ModelData
    {
        /// <summary>
        ///     Interface access to the project services
        /// </summary>
        protected IModelProject ModelProject { get; set; }

        /// <inheritdoc />
        [ConflictHandler(DataOperationType.NewObject)]
        public IDataConflictHandler<T1, ModelObject> NewModelObjectHandler { get; protected set; }

        /// <inheritdoc />
        [ConflictHandler(DataOperationType.ObjectChange)]
        public IDataConflictHandler<T1, ModelObject> ChangedModelObjectsHandler { get; protected set; }

        /// <inheritdoc />
        [ConflictHandler(DataOperationType.ObjectRemoval)]
        public IDataConflictHandler<T1, ModelObject> RemovedModelObjectsHandler { get; protected set; }

        /// <inheritdoc />
        [ConflictHandler(DataOperationType.ObjectCleaning)]
        public IDataConflictHandler<T1, ModelObject> ReindexedModelObjectsHandler { get; protected set; }

        /// <inheritdoc />
        [ConflictHandler(DataOperationType.ParameterChange)]
        public IDataConflictHandler<T1, ModelParameter> ChangedModelParameterHandler { get; protected set; }

        /// <summary>
        ///     Create new data conflict resolver provider with access to the provided project services
        /// </summary>
        /// <param name="modelProject"></param>
        protected DataConflictHandlerProvider(IModelProject modelProject)
        {
            ModelProject = modelProject ?? throw new ArgumentNullException(nameof(modelProject));
            InitializeConflictResolvers();
        }

        /// <summary>
        ///     Searches the class for all conflict resolver creation methods and assigns them to their affiliated property.
        ///     Assigns empty resolvers if no creation method is specified
        /// </summary>
        protected virtual void InitializeConflictResolvers()
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            foreach (var propertyInfo in GetType().GetProperties(flags)
                .Where(property => property.GetCustomAttribute(typeof(ConflictHandlerAttribute)) != null)) AutoAssignResolver(propertyInfo);
        }

        /// <summary>
        ///     Auto assigns a matching resolver (Or assigns a dummy if no resolver source is found in the source methods) to the
        ///     property described by the passed property info
        /// </summary>
        /// <param name="propertyInfo"></param>
        protected virtual void AutoAssignResolver(PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetValue(this) != null)
            {
                throw new ArgumentException("The passed resolver property is not null, possibly multiple resolver source definitions",
                    nameof(propertyInfo));
            }

            if (FindResolverSourceMethod(propertyInfo) is Func<object> resolverSource)
                propertyInfo.SetValue(this, resolverSource.Invoke());
            else
                propertyInfo.SetValue(this, MakeEmptyResolver(propertyInfo));
        }

        /// <summary>
        ///     Takes a property info that belongs to a data conflict resolver interface and creates a dummy handler (No resolving
        ///     required) matching the generic arguments
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        protected virtual object MakeEmptyResolver(PropertyInfo propertyInfo)
        {
            return Activator.CreateInstance(
                typeof(DataConflictHandlerDummy<,>).MakeGenericType(propertyInfo.PropertyType.GetGenericArguments()));
        }

        /// <summary>
        ///     Finds the method that is marked as a resolver source for the passed resolver property info and returns a delegate
        ///     for it. Returns null if none is found
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        protected virtual Func<object> FindResolverSourceMethod(PropertyInfo propertyInfo)
        {
            var operationType = ((ConflictHandlerAttribute) propertyInfo.GetCustomAttribute(typeof(ConflictHandlerAttribute)))
                .DataOperationType;

            bool SourceDelegateSearchPredicate(MethodInfo methodInfo)
            {
                if (methodInfo.GetCustomAttribute(typeof(HandlerFactoryMethodAttribute)) is HandlerFactoryMethodAttribute attribute)
                    return attribute.DataOperationType == operationType;

                return false;
            }

            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            if (!(new DelegateCreator().CreateWhere(this, SourceDelegateSearchPredicate, flags).SingleOrDefault() is Delegate @delegate)
            ) return null;

            if (@delegate.Method.ReturnParameter != null && @delegate.Method.ReturnParameter.ParameterType != typeof(object))
            {
                throw new InvalidOperationException(
                    "Marked provider delegate does not return an object, cannot be cast to Func<object> or is null");
            }

            if (@delegate.Method.GetParameters().Length != 0)
            {
                throw new InvalidOperationException(
                    "Marked provider delegate does not have an empty parameter list and cannot be cast to Func<object>");
            }

            if (@delegate is Func<object> creator) 
                return creator;

            throw new InvalidOperationException("Cast of provider delegate to Func<object> failed due to unknown reason");
        }
    }
}