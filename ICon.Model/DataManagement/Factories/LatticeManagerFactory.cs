using Mocassin.Model.Basic;
using Mocassin.Model.Lattices;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.DataManagement
{
    /// <summary>
    ///     Factory for new lattice manager systems
    /// </summary>
    public class LatticeManagerFactory : IModelManagerFactory
    {
        /// <inheritdoc />
        public IModelManager CreateNew(IModelProject modelProject, out object dataObject)
        {
            var data = LatticeModelData.CreateNew();
            dataObject = data;
            return new LatticeManager(modelProject, data);
        }
    }
}