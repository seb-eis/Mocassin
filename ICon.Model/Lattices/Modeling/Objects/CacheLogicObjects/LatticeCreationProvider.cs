using Mocassin.Framework.Collections;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.Analysis;
using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics;
using Mocassin.Mathematics.Coordinates;
using Moccasin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Simulations;
using System.Linq;
using Mocassin.Framework.Random;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    /// Provider to create superlattice from data in structure and lattice manager
    /// </summary>
    /// <remarks>
    /// The lattice is created by first creating an internal WorkLattice consisting of LatticeEntries. These contain information about
    /// the occupation, symmetry position and building block. The structure of the WorkLattice corresponds to unit cells ordered in a supercell.
    /// This WorkLattice is then doped with the DopingExecuter and finally translated to a SuperCellWrapper.
    /// </remarks>
    public class LatticeCreationProvider : ILatticeCreationProvider
    {
        /// <summary>
        /// The default block which is used to fill spaces not defined by custom blocks (default block is the one with Index = 0)
        /// </summary>
        private IBuildingBlock DefaultBlock { get; set; }

        /// <summary>
        /// Dictionary of sublattice ID and corresponding unit cell position
        /// </summary>
        private IReadOnlyDictionary<int, IUnitCellPosition> SublatticeIDs { get; set; }

        /// <summary>
        /// Vector encoder for supercellWrapper
        /// </summary>
        private IUnitCellVectorEncoder VectorEncoder { get; set; }

        /// <summary>
        /// List of dopings
        /// </summary>
        private ReadOnlyListAdapter<IDoping> Dopings { get; set; }

        /// <summary>
        /// Doping tolerance for automated calculation of counter dopant
        /// </summary>
        private double DopingTolerance { get; set; }

        /// <summary>
        /// General double compare tolerance
        /// </summary>
        private double DoubleCompareTolerance { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="latticePort"></param>
        /// <param name="structurePort"></param>
        /// <param name="settingsData"></param>
        public LatticeCreationProvider(ILatticeQueryPort latticePort, IStructureQueryPort structurePort, ProjectSettings settingsData)
            {
                DefaultBlock = latticePort.Query(port => port.GetBuildingBlocks()).Single(x => x.Index == 0);
                SublatticeIDs = structurePort.Query(port => port.GetExtendedIndexToPositionDictionary());
                VectorEncoder = structurePort.Query(port => port.GetVectorEncoder());
                Dopings = latticePort.Query(port => port.GetDopings());
	            DopingTolerance = settingsData.DopingToleranceSetting;
                DoubleCompareTolerance = settingsData.CommonNumericSettings.RangeValue;
            }

        /// <summary>
        /// Construct the lattice with informations provided in blueprint and managers
        /// </summary>
        /// <param name="bluePrint"></param>
        /// <returns></returns>
        public List<SupercellAdapter<IParticle>> ConstructLattices(int numberOfLattices, int randomSeed, DataVector3D size, IDictionary<IDoping, double> dopingConcentrations)
        {
	        PcgRandom32 randomGenerator = new PcgRandom32(randomSeed);

            List<SupercellAdapter<IParticle>> lattices = new List<SupercellAdapter<IParticle>>();
		
            for (int i = 0; i < numberOfLattices; i++)
            {
                LatticeEntry[,,][] workLattice = GenerateDefaultLattice(DefaultBlock, SublatticeIDs, size);
	
                var dopingExecuter = new DopingExecuter(DoubleCompareTolerance, DopingTolerance, randomGenerator);
	
                dopingExecuter.DopeLattice(workLattice, Dopings, dopingConcentrations);
	
	            lattices.Add(Translate(workLattice, VectorEncoder));
            }
		
            return lattices;
        }

        /// <summary>
        /// Generate a default lattice with user defined size and the BuildingBlocks with index 0 
        /// </summary>
        /// <param name="blockInfo"></param>
        /// <param name="sublatticeIDs"></param>
        /// <param name="latticeSize"></param>
        /// <returns></returns>
        LatticeEntry[,,][] GenerateDefaultLattice(IBuildingBlock buildingBlock, IReadOnlyDictionary<int, IUnitCellPosition> sublatticeIDs, DataVector3D latticeSize)
        {
            LatticeEntry[,,][] workLattice = new LatticeEntry[(int) latticeSize.A, (int) latticeSize.B, (int) latticeSize.C][];

            Func<LatticeEntry[]> func = delegate () { return CreateWorkCell(buildingBlock, sublatticeIDs); };

            workLattice.Populate(func);

            return workLattice;
        }

        /// <summary>
        /// Replace blocks in default superlattice with user defined custom BuildingBlocks
        /// </summary>
        /// <param name="workLattice"></param>
        /// <param name="blockInfos"></param>
        /// <param name="sublatticeIDs"></param>
        //TODO: add functionality later on
        //void FillLatticeWithCustomBlocks(LatticeEntry[,,][] workLattice, ReadOnlyListAdapter<IBlockInfo> blockInfos, IReadOnlyDictionary<int, IUnitCellPosition> sublatticeIDs)
        //{
        //    foreach (var blockInfo in blockInfos)
        //    {
        //        if (blockInfo.Index == 0) continue;
		//
        //        for (int x = blockInfo.Origin.A; x < blockInfo.Extent.A; x++)
        //        {
        //            for (int y = blockInfo.Origin.B; y < blockInfo.Extent.B; y++)
        //            {
        //                for (int z = blockInfo.Origin.C; z < blockInfo.Extent.C; z++)
        //                {
        //                    // If block lies outside of lattice: continue
        //                    if (x > workLattice.GetLength(0) || y > workLattice.GetLength(1) || z > workLattice.GetLength(2)) continue;
		//
        //                    // Get building block coordinates within superblock
        //                    VectorInt3D blockPosition = new VectorInt3D(x, y, z) % blockInfo.Size;
		//
        //                    // Find building block from linearised list in BlockInfo
        //                    int index = blockPosition.A + blockPosition.B * blockInfo.Size.A + blockPosition.C * blockInfo.Size.B * blockInfo.Size.A;
        //                    IBuildingBlock block = blockInfo.BlockGrouping[index];
		//
        //                    // Update workcell
        //                    workLattice[x, y, z] = CreateWorkCell(block, sublatticeIDs);
        //                }
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Create a unit cell which consists of LatticeEntries for later manipulation from BuildingBlock
        /// </summary>
        /// <param name="buildingBlock"></param>
        /// <param name="sublatticeIDs"></param>
        /// <returns></returns>
        LatticeEntry[] CreateWorkCell(IBuildingBlock buildingBlock, IReadOnlyDictionary<int, IUnitCellPosition> sublatticeIDs)
        {
            if (buildingBlock.CellEntries.Count != sublatticeIDs.Count)
            {
                throw new ArgumentException("WorkCellFactory", "Different number of unitCellWrapper entries and sublatticeID entries!");
            }

            LatticeEntry[] workCell = new LatticeEntry[buildingBlock.CellEntries.Count];

            for (int i = 0; i < buildingBlock.CellEntries.Count; i++)
            {
                workCell[i] = new LatticeEntry { Particle = buildingBlock.CellEntries[i], CellPosition = sublatticeIDs[i], Block = buildingBlock };
            }

            return workCell;
        }

        /// <summary>
        /// Translates the WorkLattice to a SupercellWrapper
        /// </summary>
        /// <param name="workLattice"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public SupercellAdapter<IParticle> Translate(LatticeEntry[,,][] workLattice, IUnitCellVectorEncoder encoder)
        {

            IParticle[][] particlesLineratized = new IParticle[workLattice.GetLength(0) * workLattice.GetLength(1) * workLattice.GetLength(2)][];

            int counter = 0;
            foreach (var item in workLattice)
            {
                IParticle[] entries = new IParticle[item.Length];
                for (int p = 0; p < item.Length; p++)
                {
                    entries[p] = item[p].Particle;
                }
                particlesLineratized[counter] = entries;
                counter++;
            }

            var particlesMultiDim = new IParticle[workLattice.GetLength(0), workLattice.GetLength(1), workLattice.GetLength(2)][].Populate(particlesLineratized);

            return new SupercellAdapter<IParticle>(particlesMultiDim, encoder);
        }
    }
}
