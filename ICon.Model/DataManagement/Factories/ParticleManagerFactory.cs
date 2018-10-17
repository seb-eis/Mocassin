using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.DataManagement
{
    /// <summary>
    ///     Factory for new particle manager systems
    /// </summary>
    public class ParticleManagerFactory : IModelManagerFactory
    {
        /// <inheritdoc />
        public IModelManager CreateNew(IModelProject modelProject, out object dataObject)
        {
            var data = ParticleModelData.CreateNew();
            dataObject = data;
            return new ParticleManager(modelProject, data);
        }
    }
}