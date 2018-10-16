using System;
using System.Collections.Generic;
using System.Text;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Data cache for the extended on-demand energy model data
    /// </summary>
    internal class EnergyModelCache : ModelDataCache<IEnergyCachePort>
    {
        /// <inheritdoc />
        public EnergyModelCache(IModelEventPort eventPort, IProjectServices projectServices)
            : base(eventPort, projectServices)
        {
        }

        /// <inheritdoc />
        public override IEnergyCachePort AsReadOnly()
        {
            return CachePort ?? (CachePort = new EnergyCacheManager(this, ProjectServices));
        }
    }
}
