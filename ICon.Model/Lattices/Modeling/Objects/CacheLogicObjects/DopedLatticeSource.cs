using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Mathematics.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.Analysis;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Basic implementation of a provider system for doped lattices using one base <see cref="IBuildingBlock" />
    /// </summary>
    public class DopedLatticeSource : IDopedLatticeSource
    {
        /// <summary>
        ///     Get the <see cref="IBuildingBlock" /> that supplies the default config
        /// </summary>
        private IBuildingBlock BaseBuildingBlock { get; }

        /// <summary>
        ///     Get the <see cref="IUnitCellProvider{T1}" /> of <see cref="ICellReferencePosition" /> that supplies the cell information
        /// </summary>
        private IUnitCellProvider<ICellReferencePosition> UnitCellProvider { get; }

        /// <summary>
        ///     Get the <see cref="IUnitCell{T1}" /> of <see cref="ICellReferencePosition" /> that supplies the cell information
        /// </summary>
        private IUnitCell<ICellReferencePosition> UnitCell { get; }

        /// <summary>
        ///     Get a list of integer tuples that contains all positions index and affiliated wyckoff index values for unit cell
        ///     entries that can be doped
        /// </summary>
        private List<(int PositionIndex, int WyckoffIndex)> DopingApplicationSequence { get; }

        /// <summary>
        ///     Get the parent <see cref="IModelProject" /> of the lattice source
        /// </summary>
        public IModelProject ModelProject { get; }

        /// <summary>
        ///     Get or set the charge balance tolerance for population calculation
        /// </summary>
        public double ChargeBalanceTolerance { get; set; }

        /// <summary>
        ///     Creates a new <see cref="DopedLatticeSource" /> by <see cref="IModelProject" /> and base
        ///     <see cref="IBuildingBlock" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="baseBuildingBlock"></param>
        public DopedLatticeSource(IModelProject modelProject, IBuildingBlock baseBuildingBlock)
        {
            ModelProject = modelProject ?? throw new ArgumentNullException(nameof(modelProject));
            BaseBuildingBlock = baseBuildingBlock ?? throw new ArgumentNullException(nameof(baseBuildingBlock));
            UnitCellProvider = modelProject.GetManager<IStructureManager>().QueryPort.Query(x => x.GetFullUnitCellProvider());
            UnitCell = UnitCellProvider.GetUnitCell(0, 0, 0);
            DopingApplicationSequence = MakeDopingApplicationSequence();
        }

        /// <inheritdoc />
        public byte[,,,] BuildByteLattice(DataIntVector3D sizeVector, IDictionary<IDoping, double> dopingDictionary, Random rng)
        {
            var (a, b, c) = (sizeVector.A, sizeVector.B, sizeVector.C);
            var result = CreateEmptyLattice(a, b, c);
            var populationTable = CreateDopedPopulationTable(dopingDictionary, a, b, c);
            ApplyPopulationTableToLattice(result, populationTable, rng);
            return result;
        }

        /// <summary>
        ///     Creates the doping application sequence that defines a set of position index wyckoff index tuples that define the
        ///     doping affected entries of the unit cell
        /// </summary>
        /// <returns></returns>
        private List<(int PositionIndex, int WyckoffIndex)> MakeDopingApplicationSequence()
        {
            var result = new List<(int,int)>();
            for (var i = 0; i < UnitCell.EntryCount; i++)
            {
                var wyckoff = UnitCell[i].Entry;
                if (!wyckoff.IsValidAndStable()) continue;
                result.Add((i, wyckoff.Index));
            }
            return result;
        }

        /// <summary>
        ///     Applies a population table to the passed 4D raw lattice
        /// </summary>
        /// <param name="lattice"></param>
        /// <param name="populationTable"></param>
        /// <param name="rng"></param>
        public void ApplyPopulationTableToLattice(byte[,,,] lattice, int[,] populationTable, Random rng)
        {
            var (sizeA, sizeB, sizeC, sizeD) = (lattice.GetLength(0), lattice.GetLength(1), lattice.GetLength(2), lattice.GetLength(3));
            var countTable = PopulationTableToPopulationCountSet(populationTable);

            for (var a = 0; a < sizeA; a++)
            {
                for (var b = 0; b < sizeB; b++)
                {
                    for (var c = 0; c < sizeC; c++)
                    {
                        foreach (var (positionIndex, wyckoffIndex) in DopingApplicationSequence)
                        {
                            var particle = SelectOccupationParticle(populationTable, countTable, wyckoffIndex, rng);
                            lattice[a, b, c, positionIndex] = particle;
                        }
                    }
                }
            }

            if (countTable.Any(x => x  !=  0)) throw new InvalidOperationException("Count table is not zeroed out. Doping is invalid.");
        }

        /// <summary>
        ///     Creates a population count set from a population table that contains the total number of particles per wyckoff
        ///     index
        /// </summary>
        /// <param name="populationTable"></param>
        /// <returns></returns>
        public int[] PopulationTableToPopulationCountSet(int[,] populationTable)
        {
            var result = new int[populationTable.GetLength(0)];
            for (var i = 1; i < populationTable.GetLength(1); i++)
            {
                for (var j = 0; j < result.Length; j++) result[j] += populationTable[j, i];
            }

            return result;
        }

        /// <summary>
        ///     Selects the occupation particle for a wyckoff index using the provided current state of population table and
        ///     population count set
        /// </summary>
        /// <param name="populationTable"></param>
        /// <param name="countTable"></param>
        /// <param name="wyckoffIndex"></param>
        /// <param name="rng"></param>
        /// <returns></returns>
        public byte SelectOccupationParticle(int[,] populationTable, int[] countTable, int wyckoffIndex, Random rng)
        {
            var random = rng.Next(countTable[wyckoffIndex]);
            for (var i = 1; i < byte.MaxValue; i++)
            {
                var localCount = populationTable[wyckoffIndex, i];
                if (random >= localCount)
                {
                    random -= localCount;
                    continue;
                }

                countTable[wyckoffIndex]--;
                populationTable[wyckoffIndex, i]--;
                return (byte) i;
            }

            throw new InvalidOperationException("Selection failure, the doping is not valid.");
        }

        /// <summary>
        ///     Creates a new empty 4D <see cref="byte" /> lattice that matches the passed size information and internally set
        ///     <see cref="IUnitCellProvider{T1}" />
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public byte[,,,] CreateEmptyLattice(int a, int b, int c)
        {
            return new byte[a, b, c, UnitCellProvider.VectorEncoder.PositionCount];
        }

        /// <summary>
        ///     Creates a population table for a supercell of specified size that assigns each [Wyckoff][Particle] index
        ///     combination its population count if only the set <see cref="IBuildingBlock" /> is used
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public int[,] CreateBasePopulationTable(int a, int b, int c)
        {
            var particleCount = ModelProject.DataTracker.ObjectCount<IParticle>();
            var wyckoffCount = ModelProject.DataTracker.ObjectCount<ICellReferencePosition>();
            var result = new int[wyckoffCount, particleCount];

            var cellCount = a * b * c;
            for (var i = 0; i < BaseBuildingBlock.CellEntries.Count; i++)
            {
                var wyckoff = UnitCell[i].Entry;
                if (wyckoff.Stability == PositionStability.Unstable) continue;
                result[wyckoff.Index, BaseBuildingBlock.CellEntries[i].Index] += cellCount;
            }

            return result;
        }

        /// <summary>
        ///     Creates a doped population table that assigns each [Wyckoff][Particle] index combination its population count if
        ///     the passed <see cref="IDoping" /> set is applied
        /// </summary>
        /// <param name="dopingDictionary"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public int[,] CreateDopedPopulationTable(IDictionary<IDoping, double> dopingDictionary, int a, int b, int c)
        {
            var result = CreateBasePopulationTable(a, b, c);
            foreach (var item in CreateOrderedDopingSequence(dopingDictionary))
            {
                var originalCount = result[item.Key.PrimaryDoping.CellReferencePosition.Index, item.Key.PrimaryDoping.Dopable.Index];
                var populationCounts = DopingToPopulationCount(item, originalCount);
                ApplyPopulationChangesToTable(result, item.Key, populationCounts);
            }

            return result;
        }

        /// <summary>
        ///     Applies a population change of a specific <see cref="IDoping" /> to a population table
        /// </summary>
        /// <param name="populationTable"></param>
        /// <param name="doping"></param>
        /// <param name="populations"></param>
        public void ApplyPopulationChangesToTable(int[,] populationTable, IDoping doping, in (int Primary, int Secondary) populations)
        {
            populationTable[doping.PrimaryDoping.CellReferencePosition.Index, doping.PrimaryDoping.Dopable.Index] -= populations.Primary;
            populationTable[doping.PrimaryDoping.CellReferencePosition.Index, doping.PrimaryDoping.Dopant.Index] += populations.Primary;

            if (populations.Secondary == 0) return;
            populationTable[doping.CounterDoping.CellReferencePosition.Index, doping.CounterDoping.Dopable.Index] -= populations.Secondary;
            populationTable[doping.CounterDoping.CellReferencePosition.Index, doping.CounterDoping.Dopant.Index] += populations.Secondary;
        }

        /// <summary>
        ///     Converts a <see cref="IDictionary{TKey,TValue}" /> of doping information into an ordered doping sequence
        /// </summary>
        /// <param name="dopingDictionary"></param>
        /// <returns></returns>
        private static IEnumerable<KeyValuePair<IDoping, double>> CreateOrderedDopingSequence(IDictionary<IDoping, double> dopingDictionary)
        {
            return dopingDictionary.OrderBy(x => x.Key.Priority);
        }

        /// <summary>
        ///     Calculates the resulting primary and secondary population counts if the passed doping
        ///     <see cref="KeyValuePair{TKey,TValue}" /> is applied to a count
        /// </summary>
        /// <param name="doping"></param>
        /// <param name="originalCount"></param>
        /// <returns></returns>
        /// <remarks> Ceil value of the primary count that does not violate the charge balance is created </remarks>
        public (int Primary, int Secondary) DopingToPopulationCount(in KeyValuePair<IDoping, double> doping, int originalCount)
        {
            var rawPrimary = (int) (originalCount * doping.Value);
            return !doping.Key.UseCounterDoping
                ? (rawPrimary, 0)
                : FloorDopingPopulations(rawPrimary, doping.Key.PrimaryDoping.GetChargeDelta(), doping.Key.CounterDoping.GetChargeDelta(),
                    ChargeBalanceTolerance);
        }

        /// <summary>
        ///     Takes a population number for the primary doping and get the floored result for primary and secondary doping based
        ///     on the charge values and tolerance
        /// </summary>
        /// <param name="primaryCount"></param>
        /// <param name="chargeDelta"></param>
        /// <param name="counterChargeDelta"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        private static (int PrimaryCount, int SecondaryCount) FloorDopingPopulations(int primaryCount, double chargeDelta,
            double counterChargeDelta, double tolerance)
        {
            var (primaryDelta, secondaryDelta) = (Math.Abs(chargeDelta), Math.Abs(counterChargeDelta));
            var chargeFraction = primaryDelta / secondaryDelta;
            var secondaryExact = primaryCount * chargeFraction;
            var secondaryCount = (int) Math.Round(secondaryExact);

            while (!(primaryCount * primaryDelta).AlmostEqualByRange(secondaryCount * secondaryDelta) && primaryCount > 0)
            {
                primaryCount -= 1;
                secondaryExact -= chargeFraction;
                secondaryCount = (int) Math.Round(secondaryExact);
            }

            return (primaryCount, secondaryCount);
        }
    }
}