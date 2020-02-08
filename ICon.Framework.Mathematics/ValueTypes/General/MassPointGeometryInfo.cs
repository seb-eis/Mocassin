using System;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Geometric information for a mass point set. Contains multiple values to characterize the mechanic properties of the
    ///     mass points
    /// </summary>
    public class MassPointGeometryInfo
    {
        /// <summary>
        ///     The inertia tensor calculated for the mass center as a matrix
        /// </summary>
        public Matrix2D MassCenterInertiaTensor { get; set; }

        /// <summary>
        ///     Sum of mass times distance from the center of mass (Pseudo torsional moment around the center)
        /// </summary>
        public double SumOfMassTimesDistance { get; set; }

        /// <summary>
        ///     The sum of all mass points
        /// </summary>
        public double TotalMass { get; set; }

        /// <summary>
        ///     The number of contained mass points
        /// </summary>
        public int PointCount { get; set; }

        /// <summary>
        ///     Create new mass point geometry info
        /// </summary>
        /// <param name="massCenterInertiaTensor"></param>
        /// <param name="sumOfMassTimesDistance"></param>
        /// <param name="totalMass"></param>
        /// <param name="totalMassPoints"></param>
        public MassPointGeometryInfo(Matrix2D massCenterInertiaTensor, double sumOfMassTimesDistance, double totalMass, int totalMassPoints)
        {
            if (massCenterInertiaTensor == null)
                throw new ArgumentNullException(nameof(massCenterInertiaTensor));

            if (massCenterInertiaTensor.Rows != 3 || massCenterInertiaTensor.Cols != 3)
                throw new ArgumentException("Tensor is not a 3x3 matrix", nameof(massCenterInertiaTensor));

            MassCenterInertiaTensor = massCenterInertiaTensor;
            SumOfMassTimesDistance = sumOfMassTimesDistance;
            TotalMass = totalMass;
            PointCount = totalMassPoints;
        }
    }
}