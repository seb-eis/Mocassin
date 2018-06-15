using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.DataManagement
{
    /// <summary>
    /// Represents a model manager factory to create a new model manager and its affiliated basic data object
    /// </summary>
    public interface IModelManagerFactory
    {
        /// <summary>
        /// Create a new manager with the provided project service and provides the used data object as an out parameter.
        /// This function should never link the manager to the service
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        IModelManager CreateNew(IProjectServices projectServices, out object dataObject);
    }
}
