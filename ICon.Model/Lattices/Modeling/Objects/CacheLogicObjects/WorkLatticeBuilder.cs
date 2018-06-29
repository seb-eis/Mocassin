using ICon.Framework.Collections;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Particles;
using ICon.Model.Structures;
using ICon.Symmetry.Analysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using ICon.Mathematics;
using ICon.Mathematics.Permutation;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Produces WorkLattices for supercell creation
    /// </summary>
    public class WorkLatticeBuilder
    {
        /// <summary>
        /// Fabricates the WorkLattice
        /// </summary>
        /// <param name="blockInfos"></param>
        /// <param name="sublatticeIDs"></param>
        /// <param name="latticeSize"></param>
        /// <returns></returns>
        public WorkLattice Fabricate(ReadOnlyList<IBlockInfo> blockInfos, IReadOnlyDictionary<int, IUnitCellPosition> sublatticeIDs, CartesianInt3D latticeSize)
        {
            WorkLattice workLattice = new WorkLattice() { WorkCells = new WorkCell[latticeSize.A, latticeSize.B, latticeSize.C] };

            foreach (var blockInfo in blockInfos)
            {
                for (int x = blockInfo.Origin.A; x < blockInfo.Extent.A; x++)
                {
                    for (int y = blockInfo.Origin.B; y < blockInfo.Extent.B; y++)
                    {
                        for (int z = blockInfo.Origin.C; z < blockInfo.Extent.C; z++)
                        {
                            // Get building block coordinates within superblock
                            CartesianInt3D blockPosition = new CartesianInt3D(x,y,z) % blockInfo.Size;

                            // Find building block from linearised list in BlockInfo
                            IBuildingBlock block = (new LinearizedVectorSeeker<IBuildingBlock>()).Seek(blockPosition, blockInfo.Size, blockInfo.BlockAssembly);

                            // Build workcell
                            workLattice.WorkCells[x, y, z] = (new WorkCellBuilder()).Build(block, sublatticeIDs, block.Index);
                        }
                    }
                }
            }

            return workLattice;
        }
    }
}
