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

        public SupercellWrapper<IParticle> GetLattice()
        {
            return AccessCacheableDataEntry(CreateLattice);
        }

        /// <summary>
        /// Generate Supercell
        /// </summary>
        /// <returns></returns>
        [CacheableMethod]
        public SupercellWrapper<IParticle> CreateLattice()
        {
            var latticeManager = ProjectServices.GetManager<ILatticeManager>();
            var structureManager = ProjectServices.GetManager<IStructureManager>();

            var buildingBlocks = latticeManager.QueryPort.Query((ILatticeDataPort port) => port.GetBuildingBlocks());
            var blockInfos = latticeManager.QueryPort.Query((ILatticeDataPort port) => port.GetBlockInfos());
            var sublatticeIDs = structureManager.QueryPort.Query((IStructureCachePort port) => port.GetExtendedIndexToPositionDictionary());
            var latticeSize = latticeManager.QueryPort.Query((ILatticeDataPort port) => port.GetLatticeInfo().Extent);
            var vectorEncoder = structureManager.QueryPort.Query((IStructureCachePort port) => port.GetVectorEncoder());

            WorkLattice workLattice = (new WorkLatticeFactory()).Fabricate(buildingBlocks, blockInfos, sublatticeIDs, latticeSize);

            var dopings = latticeManager.QueryPort.Query((ILatticeDataPort port) => port.GetDopings());

            (new DopingExecuter()).ExecuteMultible(workLattice, dopings);

            return (new SupercellTranslater()).Translate(workLattice, vectorEncoder);

        }

        /// <summary>
        /// Create WorkLattice (only for testing)
        /// </summary>
        /// <returns></returns>
        public WorkLattice CreateWorkLattice()
        {
            var latticeManager = ProjectServices.GetManager<ILatticeManager>();
            var structureManager = ProjectServices.GetManager<IStructureManager>();

            var buildingBlocks = latticeManager.QueryPort.Query((ILatticeDataPort port) => port.GetBuildingBlocks());
            var blockInfos = latticeManager.QueryPort.Query((ILatticeDataPort port) => port.GetBlockInfos());
            var sublatticeIDs = structureManager.QueryPort.Query((IStructureCachePort port) => port.GetExtendedIndexToPositionDictionary());
            var latticeSize = latticeManager.QueryPort.Query((ILatticeDataPort port) => port.GetLatticeInfo().Extent);
            var vectorEncoder = structureManager.QueryPort.Query((IStructureCachePort port) => port.GetVectorEncoder());

            WorkLattice workLattice = (new WorkLatticeFactory()).Fabricate(buildingBlocks, blockInfos, sublatticeIDs, latticeSize);

            var dopings = latticeManager.QueryPort.Query((ILatticeDataPort port) => port.GetDopings());
            (new DopingExecuter()).ExecuteMultible(workLattice, dopings);

            return workLattice;
        }
    }
}
