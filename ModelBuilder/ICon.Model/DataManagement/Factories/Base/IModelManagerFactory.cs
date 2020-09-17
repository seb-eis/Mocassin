using System;
using System.Collections.Generic;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.DataManagement
{
    /// <summary>
    ///     Represents a model manager factory to create a new model manager and its affiliated basic data object
    /// </summary>
    public interface IModelManagerFactory
    {
        /// <summary>
        ///     Get the type of the manager that will be created
        /// </summary>
        Type ManagerType { get; }

        /// <summary>
        ///     Create a new manager with the provided project service and provides the used data object as an out parameter
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="dataObject"></param>
        /// <remarks> Implementations should never call the register function on the model project manually </remarks>
        /// <returns></returns>
        IModelManager CreateNew(IModelProject modelProject, out object dataObject);

        /// <summary>
        ///     Get the list of model parameters types that the created manager will support
        /// </summary>
        /// <returns></returns>
        IList<Type> GetInputParameterTypes();

        /// <summary>
        ///     Get the list of model object types that the created manager will support
        /// </summary>
        /// <returns></returns>
        IList<Type> GetInputObjectTypes();
    }
}