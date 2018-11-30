using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.DataManagement
{
    /// <summary>
    ///     Abstract base class for model manager factories that use the default model manager base class system
    /// </summary>
    public abstract class ModelManagerFactoryBase : IModelManagerFactory
    {
        /// <inheritdoc />
        public Type ManagerType { get; }

        /// <summary>
        ///     Get the type of the model data object that the manager uses
        /// </summary>
        public Type DataObjectType { get; }

        /// <summary>
        ///     Creates new model manager factory that supports the provided manager type
        /// </summary>
        /// <param name="managerType"></param>
        protected ModelManagerFactoryBase(Type managerType)
        {
            DataObjectType = GetDataObjectType(managerType);
            ManagerType = managerType;
        }

        /// <inheritdoc />
        public abstract IModelManager CreateNew(IModelProject modelProject, out object dataObject);

        /// <inheritdoc />
        public IList<Type> GetInputParameterTypes()
        {
            var properties = DataObjectType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return properties
                .Select(property => property.GetCustomAttribute(typeof(ModelParameterAttribute)) as ModelParameterAttribute)
                .Where(attribute => attribute != null)
                .Select(attribute => attribute.InterfaceType)
                .ToList();
        }

        /// <inheritdoc />
        public IList<Type> GetInputObjectTypes()
        {
            var properties = DataObjectType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return properties
                .Select(property => property.GetCustomAttribute(typeof(IndexedModelDataAttribute)) as IndexedModelDataAttribute)
                .Where(attribute => attribute != null)
                .Where(attribute => !attribute.IsAutoManaged)
                .Select(attribute => attribute.InterfaceType)
                .ToList();
        }

        /// <summary>
        ///     Determines the type of the data object that the manager uses
        /// </summary>
        /// <param name="managerType"></param>
        /// <exception cref="InvalidOperationException">If the type hierarchy does not implement the generic model manager basis</exception>
        protected Type GetDataObjectType(Type managerType)
        {
            var dataType = GetBaseTypes(managerType)
                .First(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ModelManager<,,,,,,,>))
                .GetGenericArguments()
                .Where(type => GetBaseTypes(type).Any(x => x.IsSubclassOf(typeof(ModelData))))
                .SingleOrDefault(x =>
                    GetBaseTypes(x).All(b => !b.IsGenericType || b.GetGenericTypeDefinition() != typeof(ModelDataCache<>)));

            return dataType ?? throw new InvalidOperationException("Manager type does not inherit from generic model manager base class");
        }

        /// <summary>
        ///     Returns the sequence of base types of the passed type in inheritance order
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected IEnumerable<Type> GetBaseTypes(Type type)
        {
            var currentType = type;
            while (currentType.BaseType != null)
            {
                yield return currentType;
                currentType = currentType.BaseType;
            }
        }
    }
}