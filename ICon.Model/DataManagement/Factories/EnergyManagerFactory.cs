using Mocassin.Model.Basic;
using Mocassin.Model.Energies;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.DataManagement
{
    /// <summary>
    ///     Factory for new energy manager systems
    /// </summary>
    public class EnergyManagerFactory : IModelManagerFactory
    {
        /// <inheritdoc />
        public IModelManager CreateNew(IModelProject modelProject, out object dataObject)
        {
            var data = EnergyModelData.CreateNew();
            dataObject = data;
            return new EnergyManager(modelProject, data);
        }
    }
}