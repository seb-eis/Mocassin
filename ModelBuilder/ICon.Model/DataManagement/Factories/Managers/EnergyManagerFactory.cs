using System;
using Mocassin.Model.Basic;
using Mocassin.Model.Energies;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.DataManagement
{
    /// <summary>
    ///     Factory for new energy manager systems
    /// </summary>
    public class EnergyManagerFactory : ModelManagerFactoryBase
    {
        /// <inheritdoc />
        public EnergyManagerFactory()
            : this(typeof(EnergyManager))
        {
        }

        /// <inheritdoc />
        protected EnergyManagerFactory(Type managerType)
            : base(managerType)
        {
        }

        /// <inheritdoc />
        public override IModelManager CreateNew(IModelProject modelProject, out object dataObject)
        {
            var data = EnergyModelData.CreateNew();
            dataObject = data;
            return new EnergyManager(modelProject, data);
        }
    }
}