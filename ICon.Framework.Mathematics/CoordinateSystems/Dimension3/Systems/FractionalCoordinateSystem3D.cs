using System;
using ICon.Framework.Exceptions;
using ICon.Mathematics.Comparers;
using ICon.Mathematics.Extensions;
using ICon.Mathematics.Solver;
using ICon.Mathematics.ValueTypes;
using ACoordinates = ICon.Mathematics.ValueTypes.Coordinates<double, double, double>;

namespace ICon.Mathematics.Coordinates
{
    /// <summary>
    ///     Basic fractional coordinate system in 3D that supports the 7 crystal systems and fractional coordinates
    /// </summary>
    public sealed class FractionalCoordinateSystem3D : AffineCoordinateSystem3D<ACoordinates>
    {
        /// <summary>
        ///     The internal tolerance double comparer of the coordinate system
        /// </summary>
        public NumericComparer Comparer { get; }

        /// <inheritdoc />
        public override (ACoordinates A, ACoordinates B, ACoordinates C) BaseVectors { get; protected set; }

        /// <summary>
        ///     The transformation matrix to transform cartesian into fractional coordinates
        /// </summary>
        public TransformMatrix2D ToFractionalMatrix { get; }

        /// <summary>
        ///     The transformation matrix to transform fractional to cartesian coordinates
        /// </summary>
        public TransformMatrix2D ToCartesianMatrix { get; }

        /// <inheritdoc />
        public override (ACoordinates A, ACoordinates B, ACoordinates C) ReferenceBaseVectors
        {
            get => CartesianCoordinateSystem.BaseCoordinates;
            protected set =>
                throw new InvalidStateChangeException("Cartesian base or reference vectors are constant and cannot be set");
        }

        /// <inheritdoc />
        public override Type ReferenceSystemType => typeof(CartesianCoordinateSystem);

        /// <summary>
        ///     Construct a new fractional coordinate system from the base vectors and range comparer
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <param name="vectorC"></param>
        /// <param name="comparer"></param>
        public FractionalCoordinateSystem3D(ACoordinates vectorA, ACoordinates vectorB, ACoordinates vectorC, NumericComparer comparer)
        {
            var baseVectors = new [,]
            {
                {vectorA.A, vectorB.A, vectorC.A},
                {vectorA.B, vectorB.B, vectorC.B},
                {vectorA.C, vectorB.C, vectorC.C}
            };

            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            if (!vectorA.IsLinearIndependentFrom(vectorB, vectorC, comparer))
                throw new ArgumentException("Vectors are not linear independent, cannot construct fractional coordinate system",
                    $"{nameof(vectorA)}, {nameof(vectorB)}, {nameof(vectorC)}");

            ToCartesianMatrix = GetToCartesianMatrix(baseVectors, comparer);
            ToFractionalMatrix = GetToFractionalMatrix(baseVectors, comparer);
            BaseVectors = (vectorA, vectorB, vectorC);
        }

        /// <inheritdoc />
        public override ACoordinates ToReferenceSystem(in ACoordinates original)
        {
            return ToCartesianMatrix * original;
        }

        /// <summary>
        ///     Transforms a basic cartesian vector to a basic fractional vector
        /// </summary>
        /// <param name="cartesian"></param>
        /// <returns></returns>
        public Fractional3D ToSystem(in Cartesian3D cartesian)
        {
            return new Fractional3D(ToFractionalMatrix * cartesian.Coordinates);
        }

        /// <summary>
        ///     Transforms the cartesian source to fractional target and creates a new target fractional vector from the original
        ///     with the new coordinates
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="fractional"></param>
        /// <param name="cartesian"></param>
        /// <returns></returns>
        public TTarget ToSystem<TSource, TTarget>(in TSource cartesian, in TTarget fractional)
            where TSource : struct, ICartesian3D<TSource>
            where TTarget : struct, IFractional3D<TTarget>
        {
            return fractional.CreateNew(ToFractionalMatrix.Transform(cartesian.Coordinates));
        }

        /// <inheritdoc />
        public override ACoordinates ToSystem(in ACoordinates original)
        {
            return ToFractionalMatrix * original;
        }

        /// <summary>
        ///     Transforms a cartesian source to a basic fractional 3D vector (Additional information of the source is lost)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="cartesian"></param>
        /// <returns></returns>
        public Fractional3D VectorToSystem<TSource>(in TSource cartesian)
            where TSource : struct, ICartesian3D<TSource>
        {
            return new Fractional3D(ToSystem(cartesian.Coordinates));
        }

        /// <summary>
        ///     Transforms a basic fractional vector to a basic cartesian vector
        /// </summary>
        /// <param name="fractional"></param>
        /// <returns></returns>
        public Cartesian3D ToReferenceSystem(in Fractional3D fractional)
        {
            return new Cartesian3D(ToCartesianMatrix * fractional.Coordinates);
        }

        /// <summary>
        ///     Transforms the fractional source to cartesian target and creates a new target cartesian vector from the original
        ///     with the new coordinates
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="fractional"></param>
        /// <param name="cartesian"></param>
        /// <returns></returns>
        public TTarget ToReferenceSystem<TSource, TTarget>(TSource fractional, TTarget cartesian)
            where TSource : struct, IFractional3D<TSource> 
            where TTarget : struct, ICartesian3D<TTarget>
        {
            return cartesian.CreateNew(ToFractionalMatrix.Transform(fractional.Coordinates));
        }

        /// <summary>
        ///     Transforms a fractional source to a basic cartesian 3D vector (Additional information of the source is lost)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="fractional"></param>
        /// <returns></returns>
        public Cartesian3D TransformToCartesian<TSource>(in TSource fractional) 
            where TSource : struct, IFractional3D<TSource>
        {
            return new Cartesian3D(ToReferenceSystem(fractional.Coordinates));
        }

        /// <summary>
        ///     Calculates the transform matrix from fractional to cartesian vectors
        /// </summary>
        /// <param name="baseVectors"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        private static TransformMatrix2D GetToCartesianMatrix(double[,] baseVectors, NumericComparer comparer)
        {
            return new TransformMatrix2D(baseVectors, comparer);
        }

        /// <summary>
        ///     Calculates the transform matrix from cartesian to fractional vectors using the Gauss-Jordan algorithm
        /// </summary>
        /// <param name="baseVectors"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        private TransformMatrix2D GetToFractionalMatrix(double[,] baseVectors, NumericComparer comparer)
        {
            var rightMatrix = new [,]
            {
                {1.0, 0.0, 0.0},
                {0.0, 1.0, 0.0},
                {0.0, 0.0, 1.0}
            };

            var solver = new GaussJordanSolver();
            if (!solver.TrySolve((double[,]) baseVectors.Clone(), rightMatrix, comparer))
                throw new ArgumentException("Transform matrix could not be calculated for the base vectors, solver reported fail",
                    nameof(baseVectors));
            return new TransformMatrix2D(rightMatrix, comparer);
        }

        /// <summary>
        ///     Get the base vectors of the system as cartesian vectors
        /// </summary>
        /// <returns></returns>
        public (Cartesian3D A, Cartesian3D B, Cartesian3D C) GetBaseVectors()
        {
            return (new Cartesian3D(BaseVectors.A), new Cartesian3D(BaseVectors.B), new Cartesian3D(BaseVectors.C));
        }
    }
}