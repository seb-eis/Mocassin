using System;
using Mocassin.Mathematics.Comparer;
using Mocassin.Mathematics.Extensions;
using Mocassin.Mathematics.Solver;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Mathematics.Coordinates
{
    /// <summary>
    ///     Basic fractional coordinate system in 3D that supports the 7 crystal systems and fractional coordinates
    /// </summary>
    public sealed class FractionalCoordinateSystem3D : AffineCoordinateSystem3D<Coordinates3D>
    {
        /// <summary>
        ///     The internal tolerance double comparer of the coordinate system
        /// </summary>
        public NumericComparer Comparer { get; }

        /// <inheritdoc />
        public override (Coordinates3D A, Coordinates3D B, Coordinates3D C) BaseVectors { get; }

        /// <summary>
        ///     The transformation matrix to transform cartesian into fractional coordinates
        /// </summary>
        public TransformMatrix2D ToFractionalMatrix { get; }

        /// <summary>
        ///     The transformation matrix to transform fractional to cartesian coordinates
        /// </summary>
        public TransformMatrix2D ToCartesianMatrix { get; }

        /// <inheritdoc />
        public override (Coordinates3D A, Coordinates3D B, Coordinates3D C) ReferenceBaseVectors =>
            CartesianCoordinateSystem.BaseCoordinates;

        /// <inheritdoc />
        public override Type ReferenceSystemType => typeof(CartesianCoordinateSystem);

        /// <summary>
        ///     Construct a new fractional coordinate system from the base vectors and range comparer
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <param name="vectorC"></param>
        /// <param name="comparer"></param>
        public FractionalCoordinateSystem3D(in Coordinates3D vectorA, in Coordinates3D vectorB, in Coordinates3D vectorC,
            NumericComparer comparer)
        {
            var baseVectors = new[,]
            {
                {vectorA.A, vectorB.A, vectorC.A},
                {vectorA.B, vectorB.B, vectorC.B},
                {vectorA.C, vectorB.C, vectorC.C}
            };

            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            if (!vectorA.IsLinearIndependentFrom(vectorB, vectorC, comparer))
                throw new ArgumentException("Vectors are not linear independent.");

            ToCartesianMatrix = GetToCartesianMatrix(baseVectors, comparer);
            ToFractionalMatrix = GetToFractionalMatrix(baseVectors, comparer);
            BaseVectors = (vectorA, vectorB, vectorC);
        }

        /// <inheritdoc />
        public override Coordinates3D ToReferenceSystem(in Coordinates3D original) => ToCartesianMatrix * original;

        /// <summary>
        ///     Transforms a basic cartesian vector to a basic fractional vector
        /// </summary>
        /// <param name="cartesian"></param>
        /// <returns></returns>
        public Fractional3D ToSystem(in Cartesian3D cartesian) => new Fractional3D(ToFractionalMatrix * cartesian.Coordinates);

        /// <inheritdoc />
        public override Coordinates3D ToSystem(in Coordinates3D original) => ToFractionalMatrix * original;

        /// <summary>
        ///     Transforms a basic fractional vector to a basic cartesian vector
        /// </summary>
        /// <param name="fractional"></param>
        /// <returns></returns>
        public Cartesian3D ToReferenceSystem(in Fractional3D fractional) => new Cartesian3D(ToCartesianMatrix * fractional.Coordinates);

        /// <summary>
        ///     Calculates the transform matrix from fractional to cartesian vectors
        /// </summary>
        /// <param name="baseVectors"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        private static TransformMatrix2D GetToCartesianMatrix(double[,] baseVectors, NumericComparer comparer) => new TransformMatrix2D(baseVectors, comparer);

        /// <summary>
        ///     Calculates the transform matrix from cartesian to fractional vectors using the Gauss-Jordan algorithm
        /// </summary>
        /// <param name="baseVectors"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        private TransformMatrix2D GetToFractionalMatrix(double[,] baseVectors, NumericComparer comparer)
        {
            var rightMatrix = new[,]
            {
                {1.0, 0.0, 0.0},
                {0.0, 1.0, 0.0},
                {0.0, 0.0, 1.0}
            };

            var solver = new GaussJordanSolver();
            if (!solver.TrySolve((double[,]) baseVectors.Clone(), rightMatrix, comparer))
                throw new ArgumentException("Transform matrix could not be calculated for the base vectors.", nameof(baseVectors));
            return new TransformMatrix2D(rightMatrix, comparer);
        }

        /// <summary>
        ///     Get the base vectors of the system as cartesian vectors
        /// </summary>
        /// <returns></returns>
        public (Cartesian3D A, Cartesian3D B, Cartesian3D C) GetBaseVectors() =>
            (new Cartesian3D(BaseVectors.A), new Cartesian3D(BaseVectors.B), new Cartesian3D(BaseVectors.C));
    }
}