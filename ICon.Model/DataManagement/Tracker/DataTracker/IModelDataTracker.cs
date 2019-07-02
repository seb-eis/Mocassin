using System;
using System.Collections.Generic;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.DataManagement
{
    /// <summary>
    ///     Represents a model data tracker that handles the reference tracking of <see cref="ModelData" /> and indexed
    ///     <see cref="IModelObject" /> data
    /// </summary>
    public interface IModelDataTracker
    {
        /// <summary>
        ///     Performs  manager creation defined the passed factory, registers the manager with the model project and tracks
        ///     the generated model data object
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="managerFactory"></param>
        IModelManager CreateAndRegister(IModelProject modelProject, IModelManagerFactory managerFactory);

        /// <summary>
        ///     Replaces all marked foreign data references in the passed model data object by the ones known by the tracker
        /// </summary>
        /// <param name="obj"></param>
        void LinkModelObject(object obj);

        /// <summary>
        ///     Tries to replace all marked foreign data references in the passed model data object by the ones known by the
        ///     tracker
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool TryLinkModelObject(object obj);

        /// <summary>
        ///     Lookup the internal model object that can be assigned to the specified type that has the given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The found model object or null if none was found</returns>
        TObject FindObjectByIndex<TObject>(int index) where TObject : IModelObject;

        /// <summary>
        ///     Lookup the internal model object that can be assigned to the specified type that has the given key
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="key"></param>
        /// <returns>The found model object or null if none was found</returns>
        TObject FindObjectByKey<TObject>(string key) where TObject : IModelObject;

        /// <summary>
        ///     Enumerates the internal model objects of requested type
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        IEnumerable<TObject> EnumerateObjects<TObject>() where TObject : IModelObject;
    }
}