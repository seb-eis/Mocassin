using System;
using Mocassin.Model.Basic;
using Mocassin.Model.Lattices;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.DataManagement
{
    /// <summary>
    ///     Factory for new lattice manager systems
    /// </summary>
    public class LatticeManagerFactory : ModelManagerFactoryBase
    {
        /// <inheritdoc />
        public LatticeManagerFactory()
            : base(typeof(LatticeManager))
        {
        }

        /// <inheritdoc />
        protected LatticeManagerFactory(Type managerType)
            : base(managerType)
        {
        }

        /// <inheritdoc />
        public override IModelManager CreateNew(IModelProject modelProject, out object dataObject)
        {
            var data = LatticeModelData.CreateNew();
            dataObject = data;
            return new LatticeManager(modelProject, data);
        }
    }
}