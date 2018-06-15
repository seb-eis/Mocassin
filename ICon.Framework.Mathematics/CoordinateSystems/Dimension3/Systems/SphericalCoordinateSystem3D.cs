using System;

using ICon.Mathematics.Constraints;
using ICon.Mathematics.Extensions;
using ICon.Mathematics.Comparers;

using ACoorTuple = ICon.Mathematics.ValueTypes.Coordinates<System.Double, System.Double, System.Double>;

namespace ICon.Mathematics.Coordinates
{
    /// <summary>
    /// Defines double precision spherical coordinate system which follows the iso standard limitations (Theta: true, 0°, 180°, true, Phi: true, 0°, 360°, false)
    /// </summary>
    public class SphericalCoordinateSystem3D : AngularCoordinateSystem<ACoorTuple>
    {
        /// <summary>
        /// The azimuthal constraint, ISO standard (true, 0.0, 2*PI, false) with construction specified boundary tolerance
        /// </summary>
        public DoublePeriodicConstraint AzimuthalConstraint { get; }

        /// <summary>
        /// The polar constraint, ISO standard (true, 0.0, PI, true) with construction specified boundary tolerance
        /// </summary>
        public DoublePeriodicConstraint PolarConstraint { get; }

        /// <summary>
        /// The internal tolerance range for almost equal comparisons of double values
        /// </summary>
        public Double AlmostEqualRange { get; }


        /// <summary>
        /// The reference coordinate system, for spehrical coordinates this is the cartesian system
        /// </summary>
        public override Type ReferenceCoorSystemType => typeof(CartesianCoordinateSystem);

        /// <summary>
        /// Create new spherical coordinate system with the provided constraints
        /// </summary>
        /// <param name="azimuthalConstraint"></param>
        /// <param name="polarConstraint"></param>
        /// <param name="almostEqualRange"></param>
        protected SphericalCoordinateSystem3D(DoublePeriodicConstraint azimuthalConstraint, DoublePeriodicConstraint polarConstraint, Double almostEqualRange)
        {
            AzimuthalConstraint = azimuthalConstraint;
            PolarConstraint = polarConstraint;
            AlmostEqualRange = almostEqualRange;
        }

        /// <summary>
        /// Creates new spherical coordinate system following the ISO standard with the given tolerance range
        /// </summary>
        /// <param name="almostEqualRange"></param>
        /// <returns></returns>
        public static SphericalCoordinateSystem3D CreateISO(Double almostEqualRange)
        {
            var comparer = DoubleComparer.CreateRanged(almostEqualRange);
            var azimuthalConstraint = new DoublePeriodicConstraint(true, 0.0, Math.PI * 2.0, false, comparer);
            var polarConstraint = new DoublePeriodicConstraint(true, 0.0, Math.PI, true, comparer);
            return new SphericalCoordinateSystem3D(azimuthalConstraint, polarConstraint, Math.Abs(almostEqualRange));
        }

        /// <summary>
        /// Creates new spherical coordinate system following the ISO standard with the provided range comparer
        /// </summary>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static SphericalCoordinateSystem3D CreateISO(DoubleRangeComparer comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            var azimuthalConstraint = new DoublePeriodicConstraint(true, 0.0, Math.PI * 2.0, false, comparer);
            var polarConstraint = new DoublePeriodicConstraint(true, 0.0, Math.PI, true, comparer);
            return new SphericalCoordinateSystem3D(azimuthalConstraint, polarConstraint, comparer.Range);
        }


        /// <summary>
        /// Interprets a 3D coordinate tuple to be of spherical type and returns the resulting cartesian tuple
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public override ACoorTuple TransformToReference(ACoorTuple original)
        {
            return new ACoorTuple(CalculateCoordinateX(ref original), CalculateCoordinateY(ref original), CalculateCoordinateZ(ref original));
        }

        /// <summary>
        /// Interprets a 3D coordinate tuple to be of cartesain type and returns the rsulting ISO standard spherical tuple
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public override ACoorTuple TransformToSystem(ACoorTuple original)
        {
            Double radius = CalculateRadius(ref original);
            Double theta = CalculatePolarAngle(ref original, radius);
            Double phi = CalculateAzimuthalAngle(ref original, theta);
            return new ACoorTuple(radius, theta, phi);
        }

        /// <summary>
        /// Calculates the radius as the length of the cartesian coordinate tuple
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private Double CalculateRadius(ref ACoorTuple original)
        {
            return Math.Sqrt(original.A * original.A + original.B * original.B + original.C * original.C);
        }

        /// <summary>
        /// Calcualtes the polar angle under consideration of ISO standard (true, 0.0, PI, true)
        /// </summary>
        /// <param name="original"></param>
        /// <param name="radial"></param>
        /// <returns></returns>
        private Double CalculatePolarAngle(ref ACoorTuple original, Double radial)
        {
            return PolarConstraint.ParseToPeriodicRange(Math.Acos(original.C / radial));
        }

        /// <summary>
        /// Calculates azimuthal angle under condieration of ISO standard (true, 0.0, 2*PI, false) and corrects inconsistency issue for z-Alignment if polar angle is 0.0
        /// </summary>
        /// <param name="original"></param>
        /// <param name="theta"></param>
        /// <returns></returns>
        private Double CalculateAzimuthalAngle(ref ACoorTuple original, Double theta)
        {
            // The function has to return 0.0 if the theta angle is almost zero, this is an inconsitent definition issue of z-aligned spherical coordinates
            if (theta == PolarConstraint.MaxValue || theta == PolarConstraint.MinValue)
            {
                return 0.0;
            }
            return AzimuthalConstraint.ParseToPeriodicRange(Math.Atan2(original.B, original.A));
        }

        /// <summary>
        /// Calculates coordinate X from polar coordiante tuple, coorects almost zero value
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private Double CalculateCoordinateX(ref ACoorTuple original)
        {
            Double coorX = original.A * Math.Sin(original.B) * Math.Cos(original.C);
            return (coorX.AlmostZero(AlmostEqualRange) == true) ? 0.0 : coorX;
        }

        /// <summary>
        /// Calculates coordinate Y from polar coordiante tuple, coorects almost zero value
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private Double CalculateCoordinateY(ref ACoorTuple original)
        {
            Double coorY = original.A * Math.Sin(original.B) * Math.Sin(original.C);
            return (coorY.AlmostZero(AlmostEqualRange) == true) ? 0.0 : coorY;
        }

        /// <summary>
        /// Calculates coordinate Z from polar coordiante tuple, coorects almost zero value
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private Double CalculateCoordinateZ(ref ACoorTuple original)
        {
            Double coorZ = original.A * Math.Cos(original.B);
            return (coorZ.AlmostZero(AlmostEqualRange) == true) ? 0.0 : coorZ;
        }
    }
}
