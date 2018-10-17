using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;

namespace Mocassin.Model.DataManagement
{
    /// <summary>
    ///     Factory for new structure manager systems
    /// </summary>
    public class StructureManagerFactory : IModelManagerFactory
    {
        /// <inheritdoc />
        public IModelManager CreateNew(IModelProject modelProject, out object dataObject)
        {
            var data = StructureModelData.CreateNew();
            dataObject = data;
            return new StructureManager(modelProject, data);
        }
    }
}