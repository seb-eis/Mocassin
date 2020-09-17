using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Mathematics.Solver
{
    /// <summary>
    ///     Supplies solvers to handle group of mass points (e.g. mass center shifting) based upon read only mass point structs
    /// </summary>
    public class PointMechanicsSolver
    {
        /// <summary>
        ///     Shifts the origin point of a set of mass points to the center of mass. Comparer used to compare for zero values
        /// </summary>
        /// <param name="original"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public IList<CartesianMassPoint3D> ShiftOriginToMassCenter(IEnumerable<CartesianMassPoint3D> original, IComparer<double> comparer)
        {
            if (!(original is ICollection<CartesianMassPoint3D> orgCollection))
                orgCollection = original.ToList();

            var massCenter = GetMassCenter(orgCollection, comparer ?? Comparer<double>.Default);
            return orgCollection
                   .Select(value => new CartesianMassPoint3D(value.Mass, value.Vector - massCenter))
                   .ToList();
        }

        /// <summary>
        ///     Finds the center of mass for a list of fast mass points. Comparer is used to check for zero values
        /// </summary>
        /// <param name="massPoints"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public Cartesian3D GetMassCenter(IEnumerable<CartesianMassPoint3D> massPoints, IComparer<double> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            var totalMass = 0.0;
            var massCenter = new Cartesian3D(0, 0, 0);
            foreach (var point in massPoints)
            {
                totalMass += point.Mass;
                massCenter += point.Vector * point.Mass;
            }

            return massCenter / (comparer.Compare(totalMass, 0.0) == 0 ? 1.0 : totalMass);
        }

        /// <summary>
        ///     Calculates the inertia tensor of a set of mass points
        /// </summary>
        /// <param name="massPoints"></param>
        /// <returns></returns>
        public Matrix2D GetInertiaTensor(IEnumerable<CartesianMassPoint3D> massPoints)
        {
            var tensor = new Matrix2D(3, 3, null);
            foreach (var point in massPoints)
                AddInertiaEntry(tensor, point);

            MakeSymmetric(tensor);
            return tensor;
        }

        /// <summary>
        ///     Calculates the sum of the product of absolute vector length times mass for a sequence of mass points
        /// </summary>
        /// <param name="massPoints"></param>
        /// <returns></returns>
        public double GetSumOfMassTimesDistance(IEnumerable<CartesianMassPoint3D> massPoints)
        {
            return massPoints.Sum(value => value.Vector.GetLength() * value.Mass);
        }

        /// <summary>
        ///     Creates the geometry information object for a sequence of mass points. Comparer is used to check for zero values
        /// </summary>
        /// <param name="massPoints"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public MassPointGeometryInfo CreateGeometryInfo(IEnumerable<CartesianMassPoint3D> massPoints, IComparer<double> comparer)
        {
            var result = new MassPointGeometryInfo(new Matrix2D(3, 3, null), 0, 0, 0);
            foreach (var point in ShiftOriginToMassCenter(massPoints, comparer))
            {
                AddInertiaEntry(result.MassCenterInertiaTensor, point);
                result.TotalMass += point.Mass;
                result.PointCount++;
                result.SumOfMassTimesDistance += point.Vector.GetLength() * point.Mass;
            }

            MakeSymmetric(result.MassCenterInertiaTensor);
            result.TotalMass = comparer.Compare(0.0, result.TotalMass) == 0
                ? 0.0
                : result.TotalMass;

            result.SumOfMassTimesDistance = comparer.Compare(0.0, result.SumOfMassTimesDistance) == 0
                ? 0.0
                : result.SumOfMassTimesDistance;

            return result;
        }

        /// <summary>
        ///     Adds the inertia entry of the provided mass point to the tensor
        /// </summary>
        /// <param name="tensor"></param>
        /// <param name="point"></param>
        protected void AddInertiaEntry(Matrix2D tensor, CartesianMassPoint3D point)
        {
            tensor[0, 0] += (point.Vector.Y * point.Vector.Y + point.Vector.Z * point.Vector.Z) * point.Mass;
            tensor[1, 1] += (point.Vector.X * point.Vector.X + point.Vector.Z * point.Vector.Z) * point.Mass;
            tensor[2, 2] += (point.Vector.X * point.Vector.X + point.Vector.Y * point.Vector.Y) * point.Mass;
            tensor[0, 1] -= point.Vector.Y * point.Vector.Z * point.Mass;
            tensor[0, 2] -= point.Vector.X * point.Vector.Z * point.Mass;
            tensor[1, 2] -= point.Vector.X * point.Vector.Y * point.Mass;
        }

        /// <summary>
        ///     Sets the missing symmetric entries of a tensor
        /// </summary>
        /// <param name="tensor"></param>
        protected void MakeSymmetric(Matrix2D tensor)
        {
            tensor[1, 0] = tensor[0, 1];
            tensor[2, 0] = tensor[0, 2];
            tensor[2, 1] = tensor[1, 2];
        }
    }
}