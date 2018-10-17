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
        ///     Create a new manager with the provided project service and provides the used data object as an out parameter.
        ///     This function should never link the manager to the service
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        IModelManager CreateNew(IModelProject modelProject, out object dataObject);
    }
}