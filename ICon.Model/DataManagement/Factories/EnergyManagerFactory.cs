using ICon.Model.Basic;
using ICon.Model.Energies;
using ICon.Model.ProjectServices;

namespace ICon.Model.DataManagement
{
    /// <summary>
    ///     Factory for new energy manager systems
    /// </summary>
    public class EnergyManagerFactory : IModelManagerFactory
    {
        /// <inheritdoc />
        public IModelManager CreateNew(IProjectServices projectServices, out object dataObject)
        {
            var data = EnergyModelData.CreateNew();
            dataObject = data;
            return new EnergyManager(projectServices, data);
        }
    }
}