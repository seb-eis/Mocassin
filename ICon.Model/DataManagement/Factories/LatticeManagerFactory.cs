using ICon.Model.Basic;
using ICon.Model.Lattices;
using ICon.Model.ProjectServices;

namespace ICon.Model.DataManagement
{
    /// <summary>
    ///     Factory for new lattice manager systems
    /// </summary>
    public class LatticeManagerFactory : IModelManagerFactory
    {
        /// <inheritdoc />
        public IModelManager CreateNew(IProjectServices projectServices, out object dataObject)
        {
            var data = LatticeModelData.CreateNew();
            dataObject = data;
            return new LatticeManager(projectServices, data);
        }
    }
}