using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Structures;

namespace ICon.Model.DataManagement
{
    /// <summary>
    /// Fatory for new structure manager systems
    /// </summary>
    public class StructureManagerFactory : IModelManagerFactory
    {
        /// <summary>
        /// Tries to create a new structure manager with the provided project service. Returns the used data object as an out parameter
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public IModelManager CreateNew(IProjectServices projectServices, out object dataObject)
        {
            var data = StructureModelData.CreateNew();
            dataObject = data;
            return new StructureManager(projectServices, data);
        }
    }
}
