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

<<<<<<< HEAD
=======
        public SupercellAdapter<IParticle> GetLattice()
        {
            return GetResultFromCache(CreateLattice);
        }

>>>>>>> origin/s.eisele@dev
        /// <summary>
        /// Get provider for generating a lattice with the model data
        /// </summary>
        /// <returns></returns>
<<<<<<< HEAD
        public ILatticeCreationProvider GetLatticeCreationProvider()
        {
=======
        [CacheMethodResult]
        public SupercellAdapter<IParticle> CreateLattice()
        {
            var latticeManager = ModelProject.GetManager<ILatticeManager>();
            var structureManager = ModelProject.GetManager<IStructureManager>();

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
            var latticeManager = ModelProject.GetManager<ILatticeManager>();
            var structureManager = ModelProject.GetManager<IStructureManager>();

            var buildingBlocks = latticeManager.QueryPort.Query((ILatticeDataPort port) => port.GetBuildingBlocks());
            var blockInfos = latticeManager.QueryPort.Query((ILatticeDataPort port) => port.GetBlockInfos());
            var sublatticeIDs = structureManager.QueryPort.Query((IStructureCachePort port) => port.GetExtendedIndexToPositionDictionary());
            var latticeSize = latticeManager.QueryPort.Query((ILatticeDataPort port) => port.GetLatticeInfo().Extent);
            var vectorEncoder = structureManager.QueryPort.Query((IStructureCachePort port) => port.GetVectorEncoder());

            WorkLattice workLattice = (new WorkLatticeFactory()).Fabricate(buildingBlocks, blockInfos, sublatticeIDs, latticeSize);
>>>>>>> origin/s.eisele@dev

            return new LatticeCreationProvider(ProjectServices.GetManager<LatticeManager>().QueryPort, 
                ProjectServices.GetManager<StructureManager>().QueryPort, ProjectServices.SettingsData);

        }
    }
}
