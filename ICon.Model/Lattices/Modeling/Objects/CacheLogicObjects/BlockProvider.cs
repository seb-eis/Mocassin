using ICon.Framework.Collections;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Particles;
using ICon.Model.Structures;
using ICon.Symmetry.Analysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Lattices
{
    public class BlockProvider
    {
        void ConstructCustomLattice(ReadOnlyList<IBlockInfo> blockInfos, IReadOnlyDictionary<int, IUnitCellPosition> sublatticeIDs, Dictionary<IDoping, double> dopingConcentration)
        {

            DataIntVector3D latticeSize = new DataIntVector3D(0, 0, 0);
            foreach(var block in blockInfos)
            {
                if (block.Extent.A > latticeSize.A) latticeSize.A = block.Extent.A;
                if (block.Extent.B > latticeSize.B) latticeSize.B = block.Extent.B;
                if (block.Extent.C > latticeSize.C) latticeSize.C = block.Extent.C;
            }

        }
    }
}
