using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Structures;

namespace ICon.Model.DataManagement
{
    /// <summary>
    ///     Factory for new structure manager systems
    /// </summary>
    public class StructureManagerFactory : IModelManagerFactory
    {
        /// <inheritdoc />
        public IModelManager CreateNew(IProjectServices projectServices, out object dataObject)
        {
            var data = StructureModelData.CreateNew();
            dataObject = data;
            return new StructureManager(projectServices, data);
        }
    }
}