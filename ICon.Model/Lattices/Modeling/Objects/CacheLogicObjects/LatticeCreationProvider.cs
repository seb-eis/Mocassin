﻿using ICon.Framework.Collections;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Particles;
using ICon.Model.Structures;
using ICon.Symmetry.Analysis;
using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Extensions;
using ICon.Mathematics;
using ICon.Mathematics.Coordinates;
using ICon.Model.Basic;
using ICon.Model.Simulations;
using System.Linq;
using ICon.Model.ProjectServices;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Provider to create superlattice from data in structure and lattice manager
    /// </summary>
    /// <remarks>
    /// The lattice is created by first creating an internal WorkLattice consisting of LatticeEntries. These contain information about
    /// the occupation, symmetry position and building block. The structure of the WorkLattice corresponds to unit cells ordered in a supercell.
    /// This WorkLattice is then doped with the DopingExecuter and finally translated to a SuperCellWrapper.
    /// </remarks>
    public class LatticeCreationProvider
    {
        /// <summary>
        /// Lattice query port
        /// </summary>
        private ILatticeQueryPort LatticePort { get; set; }

        /// <summary>
        /// Structure query port
        /// </summary>
        private IStructureQueryPort StructurePort { get; set; }

        /// <summary>
        /// Settings data
        /// </summary>
        private ProjectSettingsData SettingsData { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="latticePort"></param>
        /// <param name="structurePort"></param>
        /// <param name="settingsData"></param>
        public LatticeCreationProvider(ILatticeQueryPort latticePort, IStructureQueryPort structurePort, ProjectSettingsData settingsData)
        {
            LatticePort = latticePort;
            StructurePort = structurePort;
            SettingsData = settingsData;
        }

        /// <summary>
        /// Construct the lattice with informations provided in blueprint and managers
        /// </summary>
        /// <param name="bluePrint"></param>
        /// <returns></returns>
        public SupercellWrapper<IParticle> ConstructLattice(ILatticeBlueprint bluePrint)
        {
            var defaultBlock = LatticePort.Query(port => port.GetBuildingBlocks()).Single(x => x.Index == 0);
            var blockInfos = LatticePort.Query(port => port.GetBlockInfos());
            var sublatticeIDs = StructurePort.Query(port => port.GetExtendedIndexToPositionDictionary());
            var vectorEncoder = StructurePort.Query(port => port.GetVectorEncoder());
            var dopings = LatticePort.Query(port => port.GetDopings());
            var dopingTolerance = SettingsData.LatticeSettings.DopingCompensationTolerance;
            var doubleCompareTolerance = SettingsData.CommonNumericSettings.RangeValue;

            LatticeEntry[,,][] workLattice = GenerateDefaultLattice(defaultBlock, sublatticeIDs, bluePrint.SizeVector);

            FillLatticeWithCustomBlocks(workLattice, blockInfos, sublatticeIDs);

            var dopingExecuter = new DopingExecuter(doubleCompareTolerance, dopingTolerance, bluePrint.CustomRng);

            dopingExecuter.DopeLattice(workLattice, dopings, bluePrint.DopingConcentrations);

            return Translate(workLattice, vectorEncoder);
        }

        /// <summary>
        /// Generate a default lattice with user defined size and the BuildingBlocks with index 0 
        /// </summary>
        /// <param name="blockInfo"></param>
        /// <param name="sublatticeIDs"></param>
        /// <param name="latticeSize"></param>
        /// <returns></returns>
        LatticeEntry[,,][] GenerateDefaultLattice(IBuildingBlock buildingBlock, IReadOnlyDictionary<int, IUnitCellPosition> sublatticeIDs, VectorInt3D latticeSize)
        {
            LatticeEntry[,,][] workLattice = new LatticeEntry[latticeSize.A, latticeSize.B, latticeSize.C][];

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
        void FillLatticeWithCustomBlocks(LatticeEntry[,,][] workLattice, ReadOnlyList<IBlockInfo> blockInfos, IReadOnlyDictionary<int, IUnitCellPosition> sublatticeIDs)
        {
            foreach (var blockInfo in blockInfos)
            {
                if (blockInfo.Index == 0) continue;

                for (int x = blockInfo.Origin.A; x < blockInfo.Extent.A; x++)
                {
                    for (int y = blockInfo.Origin.B; y < blockInfo.Extent.B; y++)
                    {
                        for (int z = blockInfo.Origin.C; z < blockInfo.Extent.C; z++)
                        {
                            // If block lies outside of lattice: continue
                            if (x > workLattice.GetLength(0) || y > workLattice.GetLength(1) || z > workLattice.GetLength(2)) continue;

                            // Get building block coordinates within superblock
                            VectorInt3D blockPosition = new VectorInt3D(x, y, z) % blockInfo.Size;

                            // Find building block from linearised list in BlockInfo
                            int index = blockPosition.A + blockPosition.B * blockInfo.Size.A + blockPosition.C * blockInfo.Size.B * blockInfo.Size.A;
                            IBuildingBlock block = blockInfo.BlockGrouping[index];

                            // Update workcell
                            workLattice[x, y, z] = CreateWorkCell(block, sublatticeIDs);
                        }
                    }
                }
            }
        }

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
        public SupercellWrapper<IParticle> Translate(LatticeEntry[,,][] workLattice, UnitCellVectorEncoder encoder)
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

            return new SupercellWrapper<IParticle>(particlesMultiDim, encoder);
        }
    }
}
