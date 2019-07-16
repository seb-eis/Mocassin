using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Provider to create superlattice from data in structure and lattice manager
    /// </summary>
    /// <remarks>
    ///     The lattice is created by first creating an internal WorkLattice consisting of LatticeEntries. These contain
    ///     information about
    ///     the occupation, symmetry position and building block. The structure of the WorkLattice corresponds to unit cells
    ///     ordered in a supercell.
    ///     This WorkLattice is then doped with the DopingExecuter and finally translated to a SuperCellWrapper.
    /// </remarks>
    public class LatticeCreationProvider : IDopedLatticeSource
    {
        /// <summary>
        ///     The default block which is used to fill spaces not defined by custom blocks (default block is the one with Index =
        ///     0)
        /// </summary>
        private IBuildingBlock DefaultBlock { get; }

        /// <summary>
        ///     List that maps the corresponding <see cref="IUnitCellPosition"/> to the linear position index
        /// </summary>
        private IReadOnlyList<IUnitCellPosition> PositionIndexToCellPositionList { get; }

        /// <summary>
        ///     Vector encoder for supercellWrapper
        /// </summary>
        private IUnitCellVectorEncoder VectorEncoder { get; }

        /// <summary>
        ///     List of dopings
        /// </summary>
        private ReadOnlyListAdapter<IDoping> Dopings { get; }

        /// <summary>
        ///     Doping tolerance for automated calculation of counter dopant
        /// </summary>
        private double DopingTolerance { get; }

        /// <summary>
        ///     General double compare tolerance
        /// </summary>
        private double DoubleCompareTolerance { get; }

        /// <summary>
        ///     Get or set the current work lattice
        /// </summary>
        private LatticeEntry[,,][] WorkLattice { get; set; }

        /// <summary>
        ///     Builds a new <see cref="LatticeCreationProvider" /> for the passed <see cref="IModelProject" />
        /// </summary>
        /// <param name="modelProject"></param>
        public LatticeCreationProvider(IModelProject modelProject)
        {
            DefaultBlock = modelProject.GetManager<LatticeManager>().QueryPort
                .Query(port => port.GetBuildingBlocks())
                .Single(x => x.Index == 0);

            PositionIndexToCellPositionList = modelProject.GetManager<StructureManager>().QueryPort
                .Query(port => port.GetExtendedIndexToPositionList());

            VectorEncoder = modelProject.GetManager<StructureManager>().QueryPort
                .Query(port => port.GetVectorEncoder());

            Dopings = modelProject.GetManager<LatticeManager>().QueryPort
                .Query(port => port.GetDopings());

            DopingTolerance = modelProject.Settings.DopingToleranceSetting;
            DoubleCompareTolerance = modelProject.Settings.CommonNumericSettings.RangeValue;
        }


        /// <summary>
        ///     Generates a default work lattice
        /// </summary>
        /// <param name="buildingBlock"></param>
        /// <param name="sublatticeIDs"></param>
        /// <param name="latticeSize"></param>
        /// <returns></returns>
        private LatticeEntry[,,][] GenerateDefaultLattice(IBuildingBlock buildingBlock,
            IReadOnlyList<IUnitCellPosition> sublatticeIDs, DataIntVector3D latticeSize)
        {
            var workLattice = new LatticeEntry[latticeSize.A, latticeSize.B, latticeSize.C][];

            workLattice.Populate(() => CreateWorkCell(buildingBlock, sublatticeIDs));

            return workLattice;
        }

        /// <summary>
        ///     Create a unit cell which consists of LatticeEntries for later manipulation from BuildingBlock
        /// </summary>
        /// <param name="buildingBlock"></param>
        /// <param name="sublatticeIDs"></param>
        /// <returns></returns>
        private LatticeEntry[] CreateWorkCell(IBuildingBlock buildingBlock, IReadOnlyList<IUnitCellPosition> sublatticeIDs)
        {
            if (buildingBlock.CellEntries.Count != sublatticeIDs.Count)
                throw new ArgumentException("Building block does not match sub lattice indexing", nameof(sublatticeIDs));

            var workCell = new LatticeEntry[buildingBlock.CellEntries.Count];

            for (var i = 0; i < buildingBlock.CellEntries.Count; i++)
                workCell[i] = new LatticeEntry
                    {Particle = buildingBlock.CellEntries[i], CellPosition = sublatticeIDs[i], Block = buildingBlock};

            return workCell;
        }

        /// <summary>
        ///     Translates a work lattice to a 4D rectangular array of particle index bytes
        /// </summary>
        /// <param name="workLattice"></param>
        /// <returns></returns>
        public byte[,,,] Translate(LatticeEntry[,,][] workLattice)
        {
            var (sizeA, sizeB, sizeC, sizeD) = (workLattice.GetLength(0),
                workLattice.GetLength(1),
                workLattice.GetLength(2),
                workLattice[0, 0, 0].GetLength(0));

            var byteLattice = new byte[sizeA, sizeB, sizeC, sizeD];
            for (var x = 0; x < sizeA; x++)
            {
                for (var y = 0; y < sizeB; y++)
                {
                    for (var z = 0; z < sizeC; z++)
                    {
                        for (var p = 0; p < sizeD; p++) byteLattice[x, y, z, p] = (byte) workLattice[x, y, z][p].Particle.Index;
                    }
                }
            }

            return byteLattice;
        }

        /// <inheritdoc />
        public byte[,,,] BuildByteLattice(DataIntVector3D sizeVector, IDictionary<IDoping, double> dopingDictionary, Random rng)
        {
            var workLattice = GenerateDefaultLattice(DefaultBlock, PositionIndexToCellPositionList, sizeVector);

            var dopingExecuter = new DopingExecuter(DoubleCompareTolerance, DopingTolerance, rng);

            dopingExecuter.DopeLattice(workLattice, Dopings, dopingDictionary);

            return Translate(workLattice);
        }
    }
}