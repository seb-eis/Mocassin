using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.DataManagement
{
    /// <summary>
    ///     Factory for new transition manager systems
    /// </summary>
    public class TransitionManagerFactory : IModelManagerFactory
    {
        /// <inheritdoc />
        public IModelManager CreateNew(IModelProject modelProject, out object dataObject)
        {
            var data = TransitionModelData.CreateNew();
            dataObject = data;
            return new TransitionManager(modelProject, data);
        }
    }
}