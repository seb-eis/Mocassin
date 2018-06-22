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
        public WorkLattice Fabricate(ReadOnlyList<IBlockInfo> blockInfos, IReadOnlyDictionary<int, IUnitCellPosition> sublatticeIDs, DataIntVector3D latticeSize)
        {
            if (!blockInfos.Single(x => x.Index == 0).Origin.Equals(new DataIntVector3D(0,0,0)))
            {
                throw new ArgumentException("WorkLatticeFactory", "Extent of default building block does not originate at 0, 0, 0");
            }

            if (!blockInfos.Single(x => x.Index == 0).Extent.Equals(latticeSize))
            {
                throw new ArgumentException("WorkLatticeFactory", "Default block has different extent than lattice size");
            }

            WorkLattice workLattice = new WorkLattice() { WorkCells = new WorkCell[latticeSize.A, latticeSize.B, latticeSize.C] };

            foreach(var blockInfo in blockInfos)
            {
                for (int x = blockInfo.Origin.A; x < blockInfo.Extent.A; x++)
                {
                    for (int y = blockInfo.Origin.B; y < blockInfo.Extent.B; y++)
                    {
                        for (int z = blockInfo.Origin.B; z < blockInfo.Extent.C; z++)
                        {
                            DataIntVector3D blockPosition = new DataIntVector3D(x % blockInfo.Size.A, y % blockInfo.Size.B, z % blockInfo.Size.C);
                            IBuildingBlock block = (new LinearizedVectorSeeker<IBuildingBlock>()).Seek(blockPosition, blockInfo.Size, blockInfo.BlockAssembly);
                            workLattice.WorkCells[x, y, z] = (new WorkCellBuilder()).Fabricate(block, sublatticeIDs, block.Index);
                        }
                    }
                }
            }

            return workLattice;
        }
    }
}
