using System;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.DataManagement
{
    /// <summary>
    ///     Factory for new transition manager systems
    /// </summary>
    public class TransitionManagerFactory : ModelManagerFactoryBase
    {
        /// <inheritdoc />
        public TransitionManagerFactory()
            : base(typeof(TransitionManager))
        {
        }

        /// <inheritdoc />
        protected TransitionManagerFactory(Type managerType)
            : base(managerType)
        {
        }

        /// <inheritdoc />
        public override IModelManager CreateNew(IModelProject modelProject, out object dataObject)
        {
            var data = TransitionModelData.CreateNew();
            dataObject = data;
            return new TransitionManager(modelProject, data);
        }
    }
}