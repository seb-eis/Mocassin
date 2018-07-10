using ICon.Framework.Collections;
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

namespace ICon.Model.Lattices
{
    public class LatticeCreationProvider
    {
        SupercellWrapper<IParticle> ConstructCustomLattice(ReadOnlyList<IBlockInfo> blockInfos, 
            IReadOnlyDictionary<int, IUnitCellPosition> sublatticeIDs, IDictionary<IDoping, double> dopingConcentration, 
            CartesianInt3D latticeSize, UnitCellVectorEncoder encoder)
        {
            CellEntry[,,][] workLattice = GenerateDefaultLattice(blockInfos[0], sublatticeIDs, latticeSize);

            FillLatticeWithCustomBlocks(workLattice, blockInfos, sublatticeIDs);

            new DopingExecuter(0.001, 0.01).ExecuteMultiple(workLattice, dopingConcentration);

            return Translate(workLattice, encoder);
        }

        CellEntry[,,][] GenerateDefaultLattice(IBlockInfo blockInfo, IReadOnlyDictionary<int, IUnitCellPosition> sublatticeIDs, CartesianInt3D latticeSize)
        {
            CellEntry[,,][] workLattice = new CellEntry[latticeSize.A, latticeSize.B, latticeSize.C][];

            Func<CellEntry[]> func = delegate () { return CreateWorkCell(blockInfo.BlockGrouping[0], sublatticeIDs); };

            workLattice.Populate(func);

            return workLattice;
        }

        void FillLatticeWithCustomBlocks(CellEntry[,,][] workLattice, ReadOnlyList<IBlockInfo> blockInfos, IReadOnlyDictionary<int, IUnitCellPosition> sublatticeIDs)
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
                            CartesianInt3D blockPosition = new CartesianInt3D(x, y, z) % blockInfo.Size;

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

        CellEntry[] CreateWorkCell(IBuildingBlock buildingBlock, IReadOnlyDictionary<int, IUnitCellPosition> sublatticeIDs)
        {
            if (buildingBlock.CellEntries.Count != sublatticeIDs.Count)
            {
                throw new ArgumentException("WorkCellFactory", "Different number of unitCellWrapper entries and sublatticeID entries!");
            }

            CellEntry[] workCell = new CellEntry[buildingBlock.CellEntries.Count];

            for (int i = 0; i < buildingBlock.CellEntries.Count; i++)
            {
                workCell[i] = new CellEntry { Particle = buildingBlock.CellEntries[i], CellPosition = sublatticeIDs[i], OriginalOccupation = buildingBlock.CellEntries[i], Block = buildingBlock };
            }

            return workCell;
        }

        /// <summary>
        /// Translates the WorkLattice to a SupercellWrapper
        /// </summary>
        /// <param name="workLattice"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public SupercellWrapper<IParticle> Translate(CellEntry[,,][] workLattice, UnitCellVectorEncoder encoder)
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
