using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Energies
{
    /// <summary>
    /// Data cache for the extended on-demand energy model data
    /// </summary>
    internal class EnergyModelCache : ModelDataCache<IEnergyCachePort>
    {
        /// <inheritdoc />
        public EnergyModelCache(IModelEventPort eventPort, IModelProject modelProject)
            : base(eventPort, modelProject)
        {
        }

        /// <inheritdoc />
        public override IEnergyCachePort AsReadOnly()
        {
            return CachePort ?? (CachePort = new EnergyCacheManager(this, ModelProject));
        }
    }
}
