using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.DataManagement
{
    /// <summary>
    ///     Factory for new simulation manager systems
    /// </summary>
    public class SimulationManagerFactory : IModelManagerFactory
    {
        /// <inheritdoc />
        public IModelManager CreateNew(IModelProject modelProject, out object dataObject)
        {
            var data = SimulationModelData.CreateNew();
            dataObject = data;
            return new SimulationManager(modelProject, data);
        }
    }
}