using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Transitions;

namespace ICon.Model.DataManagement
{
    /// <summary>
    ///     Factory for new transition manager systems
    /// </summary>
    public class TransitionManagerFactory : IModelManagerFactory
    {
        /// <inheritdoc />
        public IModelManager CreateNew(IProjectServices projectServices, out object dataObject)
        {
            var data = TransitionModelData.CreateNew();
            dataObject = data;
            return new TransitionManager(projectServices, data);
        }
    }
}