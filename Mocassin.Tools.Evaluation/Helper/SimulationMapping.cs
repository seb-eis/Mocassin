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
    public static class SimulationMapping
    {
        /// <summary>
        ///     Get a mapper <see cref="Func{T, TResult}" /> that translates linear lattice position indices into
        ///     <see cref="Vector4I" /> information
        /// </summary>
        /// <param name="latticeSize"></param>
        /// <returns></returns>
        public static Func<int, Vector4I> GetPositionIndexToVector4Mapper(in Vector4I latticeSize)
        {
            if (latticeSize.A < 1 || latticeSize.B < 1 || latticeSize.C < 1 || latticeSize.P < 1)
                throw new ArgumentException("Provided vector contains 0 or negative size information.");

            var blockSizeC = latticeSize.P;
            var blockSizeB = blockSizeC * latticeSize.C;
            var blockSizeA = blockSizeB * latticeSize.B;
            var totalSize = blockSizeA * latticeSize.A;

            Vector4I GetValue(int index)
            {
                if (index >= totalSize || index < 0) throw new ArgumentException($"Index {index} is out of range of the lattice.");
                var a = Math.DivRem(index, blockSizeA, out index);
                var b = Math.DivRem(index, blockSizeB, out index);
                var c = Math.DivRem(index, blockSizeC, out index);
                return new Vector4I(a, b, c, index);
            }

            return GetValue;
        }

        /// <summary>
        ///     Get a mapper <see cref="Func{T, TResult}" /> that translates a simulation unit cell index into
        ///     <see cref="Vector4I" /> unit cell offset information
        /// </summary>
        /// <param name="latticeSize"></param>
        /// <returns></returns>
        public static Func<int, Vector4I> GetCellIndexToCellOffset4DMapper(in Vector4I latticeSize)
        {
            var mapper4D = GetPositionIndexToVector4Mapper(latticeSize);
            var cellSize = latticeSize.P;

            Vector4I GetValue(int index) => mapper4D.Invoke(index * cellSize);

            return GetValue;
        }

        /// <summary>
        ///     Get a mapper <see cref="Func{T, TResult}" /> that translates linear lattice position indices into
        ///     <see cref="Fractional3D" /> information
        /// </summary>
        /// <param name="latticeSize"></param>
        /// <param name="vectorEncoder"></param>
        /// <returns></returns>
        public static Func<int, Fractional3D> GetPositionIndexToFractionalMapper(in Vector4I latticeSize, IUnitCellVectorEncoder vectorEncoder)
        {
            if (vectorEncoder.PositionCount != latticeSize.P) throw new ArgumentException("Size mismatch between encoder and lattice information.");
            var mapper4D = GetPositionIndexToVector4Mapper(latticeSize);

            Fractional3D GetValue(int index) =>
                vectorEncoder.TryDecode(mapper4D.Invoke(index), out Fractional3D decoded)
                    ? decoded
                    : throw new ArgumentException("Provided index cannot be mapped onto 3D information.");

            return GetValue;
        }

        /// <summary>
        ///     Get a mapper <see cref="Func{T, TResult}" /> that translates linear lattice position indices into
        ///     <see cref="Cartesian3D" /> information
        /// </summary>
        /// <param name="latticeSize"></param>
        /// <param name="vectorEncoder"></param>
        /// <returns></returns>
        public static Func<int, Cartesian3D> GetPositionIndexToCartesianMapper(in Vector4I latticeSize, IUnitCellVectorEncoder vectorEncoder)
        {
            if (vectorEncoder.PositionCount != latticeSize.P) throw new ArgumentException("Size mismatch between encoder and lattice information.");
            var mapper4D = GetPositionIndexToVector4Mapper(latticeSize);

            Cartesian3D GetValue(int index) =>
                vectorEncoder.TryDecode(mapper4D(index), out Cartesian3D decoded)
                    ? decoded
                    : throw new ArgumentException("Provided index cannot be mapped onto 3D information.");

            return GetValue;
        }

        /// <summary>
        ///     Get a mapper <see cref="Func{T, TResult}" /> that translates linear lattice position indices into
        ///     <see cref="Cartesian3D" />, <see cref="Fractional3D" /> and <see cref="Vector4I" /> information
        /// </summary>
        /// <param name="latticeSize"></param>
        /// <param name="vectorEncoder"></param>
        /// <returns></returns>
        public static Func<int, (Vector4I, Fractional3D, Cartesian3D)> GetPositionIndexToCoordinateMapper(in Vector4I latticeSize,
            IUnitCellVectorEncoder vectorEncoder)
        {
            if (vectorEncoder.PositionCount != latticeSize.P) throw new ArgumentException("Size mismatch between encoder and lattice information.");
            var mapper4D = GetPositionIndexToVector4Mapper(latticeSize);

            (Vector4I, Fractional3D, Cartesian3D) GetValue(int index)
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
        ///     <see cref="Vector4I" /> lattice position information
        /// </summary>
        /// <param name="modelContext"></param>
        /// <param name="simulationModel"></param>
        /// <param name="latticeSize"></param>
        /// <returns></returns>
        public static Func<int, Vector4I> GetStaticTrackerIndexToVector4Mapper(IProjectModelContext modelContext, ISimulationModel simulationModel,
            in Vector4I latticeSize)
        {
            var trackerModels = simulationModel.SimulationTrackingModel.StaticTrackerModels;
            var cellOffsetMapper = GetCellIndexToCellOffset4DMapper(latticeSize);

            Vector4I GetValue(int trackerIndex)
            {
                var cellIndex = Math.DivRem(trackerIndex, trackerModels.Count, out var trackerModelIndex);
                var cellOffset = cellOffsetMapper.Invoke(cellIndex);
                return new Vector4I(cellOffset.A, cellOffset.B, cellOffset.C, trackerModels[trackerModelIndex].TrackedPositionIndex);
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
        public static Func<int, Fractional3D> GetStaticTrackerIndexToFractionalMapper(IProjectModelContext modelContext, ISimulationModel simulationModel,
            in Vector4I latticeSize)
        {
            var mapper4D = GetStaticTrackerIndexToVector4Mapper(modelContext, simulationModel, latticeSize);
            var vectorEncoder = modelContext.GetUnitCellVectorEncoder();

            Fractional3D GetValue(int trackerIndex) =>
                vectorEncoder.TryDecode(mapper4D.Invoke(trackerIndex), out Fractional3D decoded)
                    ? decoded
                    : throw new ArgumentException("Provided index cannot be mapped onto a 3D position.");

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
        public static Func<int, Cartesian3D> GetStaticTrackerIndexToCartesianMapper(IProjectModelContext modelContext, ISimulationModel simulationModel,
            in Vector4I latticeSize)
        {
            var mapper4D = GetStaticTrackerIndexToVector4Mapper(modelContext, simulationModel, latticeSize);
            var vectorEncoder = modelContext.GetUnitCellVectorEncoder();

            Cartesian3D GetValue(int trackerIndex) =>
                vectorEncoder.TryDecode(mapper4D.Invoke(trackerIndex), out Cartesian3D decoded)
                    ? decoded
                    : throw new ArgumentException("Provided index cannot be mapped onto a 3D position.");

            return GetValue;
        }
    }
}