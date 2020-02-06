using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.Analysis;

namespace Mocassin.Model.Lattices
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
        /// <param name="modelProject"></param>
        public LatticeCacheManager(LatticeDataCache dataCache, IModelProject modelProject) : base(dataCache, modelProject)
        {
        }


        /// <inheritdoc />
        public IDopedByteLatticeSource GetDefaultByteLatticeSource()
	    {
		    return GetResultFromCache(CreateDefaultLatticeSource);
	    }

        /// <summary>
        ///     Creates the default thread safe <see cref="IDopedByteLatticeSource"/>
        /// </summary>
        /// <returns></returns>
		[CacheMethodResult]
	    protected IDopedByteLatticeSource CreateDefaultLatticeSource()
	    {
            var baseBlock = ModelProject.GetManager<ILatticeManager>().QueryPort.Query(x => x.GetBuildingBlocks().First());
            return new FastDopedByteLatticeSource(ModelProject, baseBlock);
	    }

    }
}
