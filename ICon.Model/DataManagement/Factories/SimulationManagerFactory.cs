using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Simulations;

namespace ICon.Model.DataManagement
{
    /// <inheritdoc />
    public class SimulationManagerFactory : IModelManagerFactory
    {
        /// <inheritdoc />
        public IModelManager CreateNew(IProjectServices projectServices, out object dataObject)
        {
            var data = SimulationModelData.CreateNew();
            dataObject = data;
            return new SimulationManager(projectServices, data);
        }
    }
}