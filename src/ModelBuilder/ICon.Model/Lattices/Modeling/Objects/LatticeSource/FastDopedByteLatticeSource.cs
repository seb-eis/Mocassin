using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Mathematics.Extensions;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.Analysis;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Provides a fast <see cref="IDopedByteLatticeSource" /> that supports doping based on only one reference
    ///     <see cref="IBuildingBlock" />
    /// </summary>
    public class FastDopedByteLatticeSource : IDopedByteLatticeSource
    {
        /// <summary>
        ///     Get the <see cref="ArrayPool{T}" /> for linear <see cref="byte" /> buffers
        /// </summary>
        private ArrayPool<byte> BufferPool { get; }

        /// <summary>
        ///     Get the <see cref="IBuildingBlock" /> that supplies the default config
        /// </summary>
        private IBuildingBlock DefaultBlock { get; }

        /// <summary>
        ///     Get the <see cref="IUnitCellProvider{T1}" /> of <see cref="ICellSite" /> that supplies the cell
        ///     information
        /// </summary>
        private IUnitCellProvider<ICellSite> UnitCellProvider { get; }

        /// <summary>
        ///     Get the <see cref="IUnitCell{T1}" /> of <see cref="ICellSite" /> that supplies the cell information
        /// </summary>
        private IUnitCell<ICellSite> UnitCell { get; }

        /// <summary>
        ///     Get an array of integer tuples that contains all positions index and affiliated reference index values for unit
        ///     cell
        ///     entries that can be doped
        /// </summary>
        private (int PositionIndex, int ReferenceIndex)[] DopingTargets { get; }

        /// <summary>
        ///     Get the parent <see cref="IModelProject" /> of the lattice source
        /// </summary>
        private IModelProject ModelProject { get; }

        /// <summary>
        ///     Get the charge balance tolerance for population calculation
        /// </summary>
        public double ChargeBalanceTolerance { get; }

        /// <summary>
        ///     Creates a new <see cref="FastDopedByteLatticeSource" /> by <see cref="IModelProject" /> and base
        ///     <see cref="IBuildingBlock" /> with optional charge balance tolerance
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="baseBuildingBlock"></param>
        /// <param name="chargeBalanceTolerance"></param>
        public FastDopedByteLatticeSource(IModelProject modelProject, IBuildingBlock baseBuildingBlock, double chargeBalanceTolerance = 1.0e-6)
        {
            ChargeBalanceTolerance = chargeBalanceTolerance;
            ModelProject = modelProject ?? throw new ArgumentNullException(nameof(modelProject));
            DefaultBlock = baseBuildingBlock ?? throw new ArgumentNullException(nameof(baseBuildingBlock));
            UnitCellProvider = modelProject.Manager<IStructureManager>().DataAccess.Query(x => x.GetFullUnitCellProvider());
            UnitCell = UnitCellProvider.GetUnitCell(0, 0, 0);
            DopingTargets = CreateDopingTargetList().ToArray();
            BufferPool = ArrayPool<byte>.Create();
        }

        /// <inheritdoc />
        public byte[,,,] CreateLattice(VectorI3 sizeVector, IDictionary<IDoping, double> dopingDictionary, Random rng)
        {
            var (a, b, c) = (sizeVector.A, sizeVector.B, sizeVector.C);
            var result = CreateEmptyLattice(a, b, c);
            var populationTable = CreateDopedPopulationTable(dopingDictionary, a, b, c);
            ApplyPopulationTableToLattice(result, populationTable, rng);
            return result;
        }

        /// <inheritdoc />
        public void PopulateLattice(IDictionary<IDoping, double> dopingDictionary, Random rng, byte[,,,] target)
        {
            if (target.GetLength(3) != UnitCell.EntryCount)
                throw new InvalidOperationException("Dimension 3 of provided array does not match the unit cell size.");

            Array.Clear(target, 0, target.Length);
            var (a, b, c) = (target.GetLength(0), target.GetLength(1), target.GetLength(2));
            var populationTable = CreateDopedPopulationTable(dopingDictionary, a, b, c);
            ApplyPopulationTableToLattice(target, populationTable, rng);
        }

        /// <summary>
        ///     Creates the doping application sequence that defines a set of position index and reference index pairs that define
        ///     the
        ///     doping affected entries of the unit cell
        /// </summary>
        /// <returns></returns>
        private List<(int PositionIndex, int ReferenceIndex)> CreateDopingTargetList()
        {
            var result = new List<(int, int)>();
            for (var i = 0; i < UnitCell.EntryCount; i++)
            {
                var wyckoff = UnitCell[i].Content;
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

            var bufferSize = sizeA * sizeB * sizeC * sizeD;
            var buffer = BufferPool.Rent(bufferSize);
            for (var offset = 0; offset < bufferSize; offset += sizeD)
            {
                foreach (var (positionIndex, referenceIndex) in DopingTargets)
                {
                    var particle = RollNextOccupationByte(populationTable, countTable, referenceIndex, rng);
                    buffer[offset + positionIndex] = particle;
                }
            }

            Buffer.BlockCopy(buffer, 0, lattice, 0, bufferSize);
            BufferPool.Return(buffer);
            if (countTable.Any(x => x != 0)) throw new InvalidOperationException("Count table is not zeroed out. Doping is invalid.");
        }

        /// <summary>
        ///     Creates a population count set from a population table that contains the total number of particles per wyckoff
        ///     index
        /// </summary>
        /// <param name="populationTable"></param>
        /// <returns></returns>
        public int[] PopulationTableToPopulationCountSet(int[,] populationTable)
        {
            var (length0, length1) = (populationTable.GetLength(0), populationTable.GetLength(1));
            var result = new int[length0];
            for (var i = 1; i < length1; i++)
            {
                for (var j = 0; j < length0; j++) result[j] += populationTable[j, i];
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
        public byte RollNextOccupationByte(int[,] populationTable, int[] countTable, int wyckoffIndex, Random rng)
        {
            var random = rng.Next(countTable[wyckoffIndex]);
            for (byte i = 1; i < byte.MaxValue; i++)
            {
                var localPopulation = populationTable[wyckoffIndex, i];
                if (random < localPopulation)
                {
                    countTable[wyckoffIndex]--;
                    populationTable[wyckoffIndex, i]--;
                    return i;
                }

                random -= localPopulation;
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
        public byte[,,,] CreateEmptyLattice(int a, int b, int c) => new byte[a, b, c, UnitCellProvider.VectorEncoder.PositionCount];

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
            var wyckoffCount = ModelProject.DataTracker.ObjectCount<ICellSite>();
            var result = new int[wyckoffCount, particleCount];

            var cellCount = a * b * c;
            for (var i = 0; i < DefaultBlock.CellEntries.Count; i++)
            {
                var wyckoff = UnitCell[i].Content;
                if (wyckoff.Stability == PositionStability.Unstable) continue;
                result[wyckoff.Index, DefaultBlock.CellEntries[i].Index] += cellCount;
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
                var originalCount = result[item.Key.PrimaryDoping.CellSite.Index, item.Key.PrimaryDoping.Dopable.Index];
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
            populationTable[doping.PrimaryDoping.CellSite.Index, doping.PrimaryDoping.Dopable.Index] -= populations.Primary;
            populationTable[doping.PrimaryDoping.CellSite.Index, doping.PrimaryDoping.Dopant.Index] += populations.Primary;

            if (populations.Secondary == 0) return;
            populationTable[doping.CounterDoping.CellSite.Index, doping.CounterDoping.Dopable.Index] -= populations.Secondary;
            populationTable[doping.CounterDoping.CellSite.Index, doping.CounterDoping.Dopant.Index] += populations.Secondary;
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

            while (!(primaryCount * primaryDelta).AlmostEqualByRange(secondaryCount * secondaryDelta, tolerance) && primaryCount > 0)
            {
                primaryCount -= 1;
                secondaryExact -= chargeFraction;
                secondaryCount = (int) Math.Round(secondaryExact);
            }

            return (primaryCount, secondaryCount);
        }
    }
}