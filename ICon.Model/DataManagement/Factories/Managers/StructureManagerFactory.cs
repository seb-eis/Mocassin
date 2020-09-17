using System;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;

namespace Mocassin.Model.DataManagement
{
    /// <summary>
    ///     Factory for new structure manager systems
    /// </summary>
    public class StructureManagerFactory : ModelManagerFactoryBase
    {
        /// <inheritdoc />
        public StructureManagerFactory()
            : base(typeof(StructureManager))
        {
        }

        /// <inheritdoc />
        protected StructureManagerFactory(Type managerType)
            : base(managerType)
        {
        }

        /// <inheritdoc />
        public override IModelManager CreateNew(IModelProject modelProject, out object dataObject)
        {
            var data = StructureModelData.CreateNew();
            dataObject = data;
            return new StructureManager(modelProject, data);
        }
    }
}