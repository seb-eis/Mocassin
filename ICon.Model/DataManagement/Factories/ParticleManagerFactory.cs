using ICon.Model.Basic;
using ICon.Model.ProjectServices;

using ICon.Model.Particles;

namespace ICon.Model.DataManagement
{
    /// <summary>
    /// Fatory for new particle manager systems
    /// </summary>
    public class ParticleManagerFactory : IModelManagerFactory
    {
        /// <summary>
        /// Tries to create a new particle manager with the provided project service. Returns the used data object as an out parameter
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public IModelManager CreateNew(IProjectServices projectServices, out object dataObject)
        {
            var data = ParticleModelData.CreateNew();
            dataObject = data;
            return new ParticleManager(projectServices, data);
        }
    }
}
