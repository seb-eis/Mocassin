using System;
using Mocassin.Framework.Operations;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Abstract base class for disposable conflict resolver logic implementation that handle a specific object change
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TDataObject"></typeparam>
    public abstract class ObjectConflictHandler<TObject, TDataObject> : IDisposable 
        where TDataObject : ModelData
    {
        /// <summary>
        ///     The data accessor providing safe access to the model data object
        /// </summary>
        protected IDataAccessor<TDataObject> DataAccess { get; set; }

        /// <summary>
        ///     The current project service instance to access all project functionality and data
        /// </summary>
        protected IModelProject ModelProject { get; set; }

        /// <summary>
        ///     Creates new object conflict handler that uses the provided data access and project services
        /// </summary>
        /// <param name="dataAccessor"></param>
        /// <param name="modelProject"></param>
        protected ObjectConflictHandler(IDataAccessor<TDataObject> dataAccessor, IModelProject modelProject)
        {
            DataAccess = dataAccessor ?? throw new ArgumentNullException(nameof(dataAccessor));
            ModelProject = modelProject ?? throw new ArgumentNullException(nameof(modelProject));
        }

        /// <summary>
        ///     Determine required changes due to provided object in the given context and update the internal management model
        ///     data system
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract ConflictReport HandleConflicts(TObject obj);

        /// <inheritdoc />
        public void Dispose()
        {
            DataAccess = null;
            ModelProject = null;
        }
    }
}