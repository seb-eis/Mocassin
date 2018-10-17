using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.DataManagement
{
    /// <summary>
    ///     Represents a model data tracker that handles the inter linking between model data and indexed model objects
    /// </summary>
    public interface IModelDataTracker
    {
        /// <summary>
        ///     Performs  manager creation defined the passed factory, registers the manager with the project service and tracks
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
        ///     Tries to replace all marked foreign data references in the passed model data object by the ones known by the tracker
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool TryLinkModelObject(object obj);

        /// <summary>
        ///     Lookup the internal data object of the specified interface type that belongs to the given index, returns null if
        ///     non exists
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        TInterface FindObjectInterfaceByIndex<TInterface>(int index);
    }
}