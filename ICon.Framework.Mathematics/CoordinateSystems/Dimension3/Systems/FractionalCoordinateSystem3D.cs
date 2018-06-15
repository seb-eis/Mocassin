using System;

using ICon.Framework.Exceptions;
using ICon.Mathematics.Extensions;
using ICon.Mathematics.Comparers;
using ICon.Mathematics.Solver;
using ICon.Mathematics.ValueTypes;

using ACoorTuple = ICon.Mathematics.ValueTypes.Coordinates<double, double, double>;

namespace ICon.Mathematics.Coordinates
{
    /// <summary>
    /// Basic fractional coordinate system in 3D that supports the 7 crystal systems and fractional coordinates
    /// </summary>
    public class FractionalCoordinateSystem3D : AffineCoordinateSystem3D<ACoorTuple>
    {
        /// <summary>
        /// The internal tolerance double comparer of the coordinate system
        /// </summary>
        public DoubleComparer Comparer { get; protected set; }

        /// <summary>
        /// Returns the base vectors of the system as cartesian information
        /// </summary>
        public override (ACoorTuple A, ACoorTuple B, ACoorTuple C) BaseVectors { get; protected set; }

        /// <summary>
        /// The transformation matrix to transform cartesian into fractional coordinates
        /// </summary>
        public TransformMatrix2D ToFractionalMatrix { get; protected set; }

        /// <summary>
        /// The transformation matrix to transform fractional to cartesian coordinates
        /// </summary>
        public TransformMatrix2D ToCartesianMatrix { get; protected set; }

        /// <summary>
        /// Returns the cartesian base coordinates
        /// </summary>
        public override (ACoorTuple A, ACoorTuple B, ACoorTuple C) ReferenceBaseVectors
        {
            get { return CartesianCoordinateSystem.BaseCoordinates; }
            protected set { throw new InvalidObjectChangeException("Cartesian base or refernce vectors are constant and cannot be set"); }
        }

        /// <summary>
        /// Returns the cartesian type
        /// </summary>
        public override Type ReferenceCoorSystemType => typeof(CartesianCoordinateSystem);

        /// <summary>
        /// Construct a new fractional coor system from the base vectors and range comparer
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <param name="vectorC"></param>
        /// <param name="comparer"></param>
        public FractionalCoordinateSystem3D(ACoorTuple vectorA, ACoorTuple vectorB, ACoorTuple vectorC, DoubleComparer comparer)
        {
            Double[,] baseVectors = new Double[3, 3] { { vectorA.A, vectorB.A, vectorC.A }, { vectorA.B, vectorB.B, vectorC.B }, { vectorA.C, vectorB.C, vectorC.C } };
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            if (vectorA.IsLinearIndependentFrom(vectorB, vectorC, comparer) == false)
            {
                throw new ArgumentException("Vectors are not linear independent, cannot construct fractional coordinate system", $"{nameof(vectorA)}, {nameof(vectorB)}, {nameof(vectorC)}");
            }
            ToCartesianMatrix = GetToCartesianMatrix(baseVectors, comparer);
            ToFractionalMatrix = GetToFractionalMatrix(baseVectors, comparer);
            BaseVectors = (vectorA, vectorB, vectorC);
        }

        /// <summary>
        /// Transforms a coordinate tuple using the fractional to cartesian transformation matrix
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public override ACoorTuple TransformToReference(ACoorTuple original)
        {
            return ToCartesianMatrix * original;
        }

        /// <summary>
        /// Transforms a basic cartesian vector to a basic fractional vector
        /// </summary>
        /// <param name="cartesian"></param>
        /// <returns></returns>
        public Fractional3D TransfromToFractional(Cartesian3D cartesian)
        {
            return new Fractional3D(ToFractionalMatrix * cartesian.Coordinates);
        }

        /// <summary>
        /// Transforms the cartesian source to fractional target and creates a new target fractional vector from the original with the new coordinates
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="fractional"></param>
        /// <param name="cartesian"></param>
        /// <returns></returns>
        public TTarget TransformToFractional<TSource, TTarget>(TSource cartesian, TTarget fractional) where TSource : struct, ICartesian3D<TSource> where TTarget : struct, IFractional3D<TTarget>
        {
            return fractional.CreateNew(ToFractionalMatrix.Transform(cartesian.Coordinates));
        }

        /// <summary>
        /// Transforms a cartesian source to a basic fractional 3D vector (Additional information of the source is lost)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="cartesian"></param>
        /// <returns></returns>
        public Fractional3D TranformToFractional<TSource>(TSource cartesian) where TSource : struct, ICartesian3D<TSource>
        {
            return new Fractional3D(TransformToSystem(cartesian.Coordinates));
        }

        /// <summary>
        /// Transforms a coordinate tuple using the cartesian to fractional transformation matrix
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public override ACoorTuple TransformToSystem(ACoorTuple original)
        {
            return ToFractionalMatrix * original;
        }

        /// <summary>
        /// Transforms a basic fractional vector to a basic cartesian vector
        /// </summary>
        /// <param name="fractional"></param>
        /// <returns></returns>
        public Cartesian3D TransfromToCartesian(Fractional3D fractional)
        {
            return new Cartesian3D(ToCartesianMatrix * fractional.Coordinates);
        }

        /// <summary>
        /// Transforms the fractional source to cartesian target and creates a new target cartesian vector from the original with the new coordinates
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="fractional"></param>
        /// <param name="cartesian"></param>
        /// <returns></returns>
        public TTarget TransfromToCartesian<TSource, TTarget>(TSource fractional, TTarget cartesian) where TSource : struct, IFractional3D<TSource> where TTarget : struct, ICartesian3D<TTarget>
        {
            return cartesian.CreateNew(ToFractionalMatrix.Transform(fractional.Coordinates));
        }

        /// <summary>
        /// Transforms a fractional source to a basic cartesian 3D vector (Additional information of the source is lost)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="cartesian"></param>
        /// <returns></returns>
        public Cartesian3D TransformToCartesian<TSource>(TSource fractional) where TSource : struct, IFractional3D<TSource>
        {
            return new Cartesian3D(TransformToReference(fractional.Coordinates));
        }

        /// <summary>
        /// Calculates the transform matrix from fractional to cartesian vectors
        /// </summary>
        /// <param name="baseVectors"></param>
        /// <returns></returns>
        protected TransformMatrix2D GetToCartesianMatrix(Double[,] baseVectors, DoubleComparer comparer)
        {
            return new TransformMatrix2D(baseVectors, comparer);
        }

        /// <summary>
        /// Calculates the transform matrix from cartesian to fractional vectors using the Gauss-Jordan algorithm
        /// </summary>
        /// <param name="baseVectors"></param>
        /// <returns></returns>
        protected TransformMatrix2D GetToFractionalMatrix(Double[,] baseVectors, DoubleComparer comparer)
        {
            var rightMatrix = new Double[3, 3] { { 1.0, 0.0, 0.0 }, { 0.0, 1.0, 0.0 }, { 0.0, 0.0, 1.0 } };
            var solver = new GaussJordanSolver();
            if (solver.TrySolve((Double[,])baseVectors.Clone(), rightMatrix, comparer) == false)
            {
                throw new ArgumentException("Transform matrix could not be calculated for the base vectors, solver reported fail", nameof(baseVectors));
            }
            return new TransformMatrix2D(rightMatrix, comparer);
        }

        /// <summary>
        /// Get the base vectors of the system as cartesian vectors
        /// </summary>
        /// <returns></returns>
        public (Cartesian3D A, Cartesian3D B, Cartesian3D C) GetBaseVectors()
        {
            return (new Cartesian3D(BaseVectors.A), new Cartesian3D(BaseVectors.B), new Cartesian3D(BaseVectors.C));
        }
    }
}
