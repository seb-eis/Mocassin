using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Transitions;

namespace ICon.Model.DataManagement
{
    /// <summary>
    /// Fatory for new transition manager systems
    /// </summary>
    public class TransitionManagerFactory : IModelManagerFactory
    {
        /// <summary>
        /// Tries to create a new transition manager with the provided project service. Returns the used data object as an out parameter
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public IModelManager CreateNew(IProjectServices projectServices, out object dataObject)
        {
            var data = TransitionModelData.CreateNew();
            dataObject = data;
            return new TransitionManager(projectServices, data);
        }
    }
}
