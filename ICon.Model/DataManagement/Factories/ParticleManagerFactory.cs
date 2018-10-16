using ICon.Model.Basic;
using ICon.Model.Particles;
using ICon.Model.ProjectServices;

namespace ICon.Model.DataManagement
{
    /// <summary>
    ///     Factory for new particle manager systems
    /// </summary>
    public class ParticleManagerFactory : IModelManagerFactory
    {
        /// <inheritdoc />
        public IModelManager CreateNew(IProjectServices projectServices, out object dataObject)
        {
            var data = ParticleModelData.CreateNew();
            dataObject = data;
            return new ParticleManager(projectServices, data);
        }
    }
}