using System;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Translator.ModelContext;
using Mocassin.Tools.Evaluation.Extensions;

namespace Mocassin.Tools.Evaluation.Helper
{
    /// <summary>
    ///     Provides helper methods for handling translation of simulation mappings into higher level information
    /// </summary>
    public static class SimulationMappingHelper
    {
        /// <summary>
        ///     Get a mapper <see cref="Func{T, TResult}" /> that translates linear lattice position indices into
        ///     <see cref="CrystalVector4D" /> information
        /// </summary>
        /// <param name="latticeSize"></param>
        /// <returns></returns>
        public static Func<int, CrystalVector4D> GetPositionIndexToVector4DMapper(in CrystalVector4D latticeSize)
        {
            if (latticeSize.A < 1 || latticeSize.B < 1 || latticeSize.C < 1 || latticeSize.P < 1)
                throw new ArgumentException("Provided vector contains 0 or negative size information.");

            var blockSizeC = latticeSize.P;
            var blockSizeB = blockSizeC * latticeSize.C;
            var blockSizeA = blockSizeB * latticeSize.B;
            var totalSize = blockSizeA * latticeSize.A;

            CrystalVector4D GetValue(int index)
            {
                if (index >= totalSize || index < 0) throw new ArgumentException($"Index {index} is out of range of the lattice.");
                var a = Math.DivRem(index, blockSizeA, out index);
                var b = Math.DivRem(index, blockSizeB, out index);
                var c = Math.DivRem(index, blockSizeC, out index);
                return new CrystalVector4D(a, b, c, index);
            }

            return GetValue;
        }

        /// <summary>
        ///     Get a mapper <see cref="Func{T, TResult}" /> that translates a simulation unit cell index into
        ///     <see cref="CrystalVector4D" /> unit cell offset information
        /// </summary>
        /// <param name="latticeSize"></param>
        /// <returns></returns>
        public static Func<int, CrystalVector4D> GetCellIndexToCellOffset4DMapper(in CrystalVector4D latticeSize)
        {
            var mapper4D = GetPositionIndexToVector4DMapper(latticeSize);
            var cellSize = latticeSize.P;

            CrystalVector4D GetValue(int index)
            {
                return mapper4D.Invoke(index * cellSize);
            }

            return GetValue;
        }

        /// <summary>
        ///     Get a mapper <see cref="Func{T, TResult}" /> that translates linear lattice position indices into
        ///     <see cref="Fractional3D" /> information
        /// </summary>
        /// <param name="latticeSize"></param>
        /// <param name="vectorEncoder"></param>
        /// <returns></returns>
        public static Func<int, Fractional3D> GetPositionIndexToFractional3DMapper(in CrystalVector4D latticeSize, IUnitCellVectorEncoder vectorEncoder)
        {
            if (vectorEncoder.PositionCount != latticeSize.P) throw new ArgumentException("Size mismatch between encoder and lattice information.");
            var mapper4D = GetPositionIndexToVector4DMapper(latticeSize);

            Fractional3D GetValue(int index)
            {
                return vectorEncoder.TryDecode(mapper4D.Invoke(index), out Fractional3D decoded)
                    ? decoded
                    : throw new ArgumentException("Provided index cannot be mapped onto 3D information.");
            }

            return GetValue;
        }

        /// <summary>
        ///     Get a mapper <see cref="Func{T, TResult}" /> that translates linear lattice position indices into
        ///     <see cref="Cartesian3D" /> information
        /// </summary>
        /// <param name="latticeSize"></param>
        /// <param name="vectorEncoder"></param>
        /// <returns></returns>
        public static Func<int, Cartesian3D> GetPositionIndexToCartesian3DMapper(in CrystalVector4D latticeSize, IUnitCellVectorEncoder vectorEncoder)
        {
            if (vectorEncoder.PositionCount != latticeSize.P) throw new ArgumentException("Size mismatch between encoder and lattice information.");
            var mapper4D = GetPositionIndexToVector4DMapper(latticeSize);

            Cartesian3D GetValue(int index)
            {
                return vectorEncoder.TryDecode(mapper4D(index), out Cartesian3D decoded)
                    ? decoded
                    : throw new ArgumentException("Provided index cannot be mapped onto 3D information.");
            }

            return GetValue;
        }

        /// <summary>
        ///     Get a mapper <see cref="Func{T, TResult}" /> that translates linear lattice position indices into
        ///     <see cref="Cartesian3D" />, <see cref="Fractional3D" /> and <see cref="CrystalVector4D" /> information
        /// </summary>
        /// <param name="latticeSize"></param>
        /// <param name="vectorEncoder"></param>
        /// <returns></returns>
        public static Func<int, (CrystalVector4D, Fractional3D, Cartesian3D)> GetPositionIndexToCoordinateMapper(in CrystalVector4D latticeSize,
            IUnitCellVectorEncoder vectorEncoder)
        {
            if (vectorEncoder.PositionCount != latticeSize.P) throw new ArgumentException("Size mismatch between encoder and lattice information.");
            var mapper4D = GetPositionIndexToVector4DMapper(latticeSize);

            (CrystalVector4D, Fractional3D, Cartesian3D) GetValue(int index)
            {
                var vector4D = mapper4D(index);
                var fractional3D = vectorEncoder.TryDecode(vector4D, out Fractional3D decoded)
                    ? decoded
                    : throw new ArgumentException("Provided index cannot be mapped onto 3D information.");
                var cartesian3D = vectorEncoder.Transformer.ToCartesian(fractional3D);
                return (vector4D, fractional3D, cartesian3D);
            }

            return GetValue;
        }


        /// <summary>
        ///     Generates a mapper <see cref="Func{T, TResult}" /> that translates static tracker indices into
        ///     <see cref="CrystalVector4D" /> lattice position information
        /// </summary>
        /// <param name="modelContext"></param>
        /// <param name="simulationModel"></param>
        /// <param name="latticeSize"></param>
        /// <returns></returns>
        public static Func<int, CrystalVector4D> GetStaticTrackerIndexToVector4DMapper(IProjectModelContext modelContext, ISimulationModel simulationModel,
            in CrystalVector4D latticeSize)
        {
            var trackerModels = simulationModel.SimulationTrackingModel.StaticTrackerModels;
            var cellOffsetMapper = GetCellIndexToCellOffset4DMapper(latticeSize);

            CrystalVector4D GetValue(int trackerIndex)
            {
                var cellIndex = Math.DivRem(trackerIndex, trackerModels.Count, out var trackerModelIndex);
                var cellOffset = cellOffsetMapper.Invoke(cellIndex);
                return new CrystalVector4D(cellOffset.A, cellOffset.B, cellOffset.C, trackerModels[trackerModelIndex].TrackedPositionIndex);
            }

            return GetValue;
        }

        /// <summary>
        ///     Generates a mapper <see cref="Func{T, TResult}" /> that translates static tracker indices into
        ///     <see cref="Fractional3D" /> lattice position information
        /// </summary>
        /// <param name="modelContext"></param>
        /// <param name="simulationModel"></param>
        /// <param name="latticeSize"></param>
        /// <returns></returns>
        public static Func<int, Fractional3D> GetStaticTrackerIndexToFractional3DMapper(IProjectModelContext modelContext, ISimulationModel simulationModel,
            in CrystalVector4D latticeSize)
        {
            var mapper4D = GetStaticTrackerIndexToVector4DMapper(modelContext, simulationModel, latticeSize);
            var vectorEncoder = modelContext.GetUnitCellVectorEncoder();

            Fractional3D GetValue(int trackerIndex)
            {
                return vectorEncoder.TryDecode(mapper4D.Invoke(trackerIndex), out Fractional3D decoded)
                    ? decoded
                    : throw new ArgumentException("Provided index cannot be mapped onto a 3D position.");
            }

            return GetValue;
        }

        /// <summary>
        ///     Generates a mapper <see cref="Func{T, TResult}" /> that translates static tracker indices into
        ///     <see cref="Cartesian3D" /> lattice position information
        /// </summary>
        /// <param name="modelContext"></param>
        /// <param name="simulationModel"></param>
        /// <param name="latticeSize"></param>
        /// <returns></returns>
        public static Func<int, Cartesian3D> GetStaticTrackerIndexToCartesian3DMapper(IProjectModelContext modelContext, ISimulationModel simulationModel,
            in CrystalVector4D latticeSize)
        {
            var mapper4D = GetStaticTrackerIndexToVector4DMapper(modelContext, simulationModel, latticeSize);
            var vectorEncoder = modelContext.GetUnitCellVectorEncoder();

            Cartesian3D GetValue(int trackerIndex)
            {
                return vectorEncoder.TryDecode(mapper4D.Invoke(trackerIndex), out Cartesian3D decoded)
                    ? decoded
                    : throw new ArgumentException("Provided index cannot be mapped onto a 3D position.");
            }

            return GetValue;
        }
    }
}