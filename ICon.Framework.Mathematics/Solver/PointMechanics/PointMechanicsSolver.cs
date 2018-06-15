using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Mathematics.ValueTypes;


namespace ICon.Mathematics.Solver
{
    /// <summary>
    /// Supplies solvers to handle group of mass points (e.g. mass center shifting) based upon read only mass point structs
    /// </summary>
    public class PointMechanicsSolver
    {
        /// <summary>
        /// Shifts the origin point of a set of mass points to the center of mass. Comparer used to compare for zero values
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="original"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public List<T1> ShiftOriginToMassCenter<T1>(IEnumerable<T1> original, IComparer<double> comparer) where T1 : struct, ICartesianMassPoint3D<T1>
        {
            var massCenter = GetMassCenter(original, comparer ?? Comparer<double>.Default);
            return original.Select((value) => value.CreateNew(value.Vector - massCenter, value.GetMass())).ToList();
        }

        /// <summary>
        /// Finds the center of mass for a list of fast mass points. Comparer is used to check for zero values
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="massPoints"></param>
        /// <returns></returns>
        public Cartesian3D GetMassCenter<T1>(IEnumerable<T1> massPoints, IComparer<double> comparer) where T1 : struct, ICartesianMassPoint3D<T1>
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            double totalMass = 0.0;
            var massCenter = new Cartesian3D(0, 0, 0);
            foreach (var point in massPoints)
            {
                totalMass += point.GetMass();
                massCenter += point.Vector * point.GetMass();
            }
            return massCenter / ((comparer.Compare(totalMass, 0.0) == 0) ? 1.0 : totalMass);
        }

        /// <summary>
        /// Calculates the inertia tensor of a set of mass points
        /// </summary>
        /// <param name="massPoints"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public Matrix2D GetInertiaTensor<T1>(IEnumerable<T1> massPoints) where T1 : struct, ICartesianMassPoint3D<T1>
        {
            var tensor = new Matrix2D(3, 3, null);
            foreach (var point in massPoints)
            {
                AddInertiaEntry(tensor, point);
            }
            MakeSymmetric(tensor);
            return tensor;
        }

        /// <summary>
        /// Calculates the sum of the product of absolute vector length times mass for a sequence of mass points
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="massPoints"></param>
        /// <returns></returns>
        public double GetSumOfMassTimesDistance<T1>(IEnumerable<T1> massPoints) where T1 : struct, ICartesianMassPoint3D<T1>
        {
            return massPoints.Sum(value => value.Vector.GetLength() * value.GetMass());
        }

        /// <summary>
        /// Creates the geometry information object for a sequence of mass points. Comparer is used to check for zero values
        /// </summary>
        /// <param name="massPoints"></param>
        /// <returns></returns>
        public MassPointGeomertyInfo CreateGeometryInfo<T1>(IEnumerable<T1> massPoints, IComparer<double> comparer) where T1 : struct, ICartesianMassPoint3D<T1>
        {
            var result = new MassPointGeomertyInfo(new Matrix2D(3, 3, null), 0, 0, 0);
            foreach (var point in ShiftOriginToMassCenter(massPoints, comparer))
            {
                AddInertiaEntry(result.MassCenterInertiaTensor, point);
                result.TotalMass += point.GetMass();
                result.PointCount++;
                result.SumOfMassTimesDistance += point.Vector.GetLength() * point.GetMass();
            }
            MakeSymmetric(result.MassCenterInertiaTensor);
            result.TotalMass = (comparer.Compare(0.0, result.TotalMass) == 0) ? 0.0 : result.TotalMass;
            result.SumOfMassTimesDistance = (comparer.Compare(0.0, result.SumOfMassTimesDistance) == 0) ? 0.0 : result.SumOfMassTimesDistance;
            return result;
        }

        /// <summary>
        /// Adds the inertia entry of the provided mass point to the tensor
        /// </summary>
        /// <param name="tensor"></param>
        /// <param name="point"></param>
        protected void AddInertiaEntry<T1>(Matrix2D tensor, T1 point) where T1 : struct, ICartesianMassPoint3D<T1>
        {
            tensor[0, 0] += (point.Vector.Y * point.Vector.Y + point.Vector.Z * point.Vector.Z) * point.GetMass();
            tensor[1, 1] += (point.Vector.X * point.Vector.X + point.Vector.Z * point.Vector.Z) * point.GetMass();
            tensor[2, 2] += (point.Vector.X * point.Vector.X + point.Vector.Y * point.Vector.Y) * point.GetMass();
            tensor[0, 1] -= point.Vector.Y * point.Vector.Z * point.GetMass();
            tensor[0, 2] -= point.Vector.X * point.Vector.Z * point.GetMass();
            tensor[1, 2] -= point.Vector.X * point.Vector.Y * point.GetMass();
        }

        /// <summary>
        /// Sets the missing symmetric entries of a tensor
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
