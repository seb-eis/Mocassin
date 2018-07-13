using System;
using System.Collections.Generic;
using System.Linq;
using ICon.Framework.Collections;
using ICon.Mathematics.Coordinates;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;
using ICon.Model.Particles;
using ICon.Model.ProjectServices;
using ICon.Model.Structures;
using ICon.Symmetry.Analysis;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Basic implementation of the lattice cache manager that provides read only access to the extended 'on demand' lattice data
    /// </summary>
    internal class LatticeCacheManager : ModelCacheManager<LatticeDataCache, ILatticeCachePort>, ILatticeCachePort
    {
        /// <summary>
        /// Create new lattice cache manager for the provided data cache and project services
        /// </summary>
        /// <param name="dataCache"></param>
        /// <param name="projectServices"></param>
        public LatticeCacheManager(LatticeDataCache dataCache, IProjectServices projectServices) : base(dataCache, projectServices)
        {
        }

        public LatticeCreationProvider GetLatticeCreationProvider()
        {

            return new LatticeCreationProvider(ProjectServices.GetManager<LatticeManager>().QueryPort, 
                ProjectServices.GetManager<StructureManager>().QueryPort, ProjectServices.SettingsData);

        }
    }
}
