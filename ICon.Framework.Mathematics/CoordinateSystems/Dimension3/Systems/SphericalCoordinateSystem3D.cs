using System;
using Mocassin.Mathematics.Extensions;
using Mocassin.Mathematics.Comparers;
using Mocassin.Mathematics.Constraints;
using Mocassin.Mathematics.ValueTypes;
using ACoordinates = Mocassin.Mathematics.ValueTypes.Coordinates<double, double, double>;

namespace Mocassin.Mathematics.Coordinates
{
    /// <summary>
    ///     Defines double precision spherical coordinate system which follows the iso standard limitations (Theta: true, 0°,
    ///     180°, true, Phi: true, 0°, 360°, false)
    /// </summary>
    public class SphericalCoordinateSystem3D : AngularCoordinateSystem<ACoordinates>
    {
        /// <summary>
        ///     The azimuthal constraint, ISO standard (true, 0.0, 2*PI, false) with construction specified boundary tolerance
        /// </summary>
        public NumericPeriodicConstraint AzimuthalConstraint { get; }

        /// <summary>
        ///     The polar constraint, ISO standard (true, 0.0, PI, true) with construction specified boundary tolerance
        /// </summary>
        public NumericPeriodicConstraint PolarConstraint { get; }

        /// <summary>
        ///     The internal tolerance range for almost equal comparisons of double values
        /// </summary>
        public double AlmostEqualRange { get; }


        /// <inheritdoc />
        public override Type ReferenceSystemType => typeof(CartesianCoordinateSystem);

        /// <summary>
        ///     Create new spherical coordinate system with the provided constraints
        /// </summary>
        /// <param name="azimuthalConstraint"></param>
        /// <param name="polarConstraint"></param>
        /// <param name="almostEqualRange"></param>
        protected SphericalCoordinateSystem3D(NumericPeriodicConstraint azimuthalConstraint, NumericPeriodicConstraint polarConstraint,
            double almostEqualRange)
        {
            AzimuthalConstraint = azimuthalConstraint;
            PolarConstraint = polarConstraint;
            AlmostEqualRange = almostEqualRange;
        }

        /// <summary>
        ///     Creates new spherical coordinate system following the ISO standard with the given tolerance range
        /// </summary>
        /// <param name="almostEqualRange"></param>
        /// <returns></returns>
        public static SphericalCoordinateSystem3D CreateIsoSystem(double almostEqualRange)
        {
            var comparer = NumericComparer.CreateRanged(almostEqualRange);
            var azimuthalConstraint = new NumericPeriodicConstraint(true, 0.0, Math.PI * 2.0, false, comparer);
            var polarConstraint = new NumericPeriodicConstraint(true, 0.0, Math.PI, true, comparer);
            return new SphericalCoordinateSystem3D(azimuthalConstraint, polarConstraint, Math.Abs(almostEqualRange));
        }

        /// <summary>
        ///     Creates new spherical coordinate system following the ISO standard with the provided range comparer
        /// </summary>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static SphericalCoordinateSystem3D CreateIsoSystem(NumericRangeComparer comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            var azimuthalConstraint = new NumericPeriodicConstraint(true, 0.0, Math.PI * 2.0, false, comparer);
            var polarConstraint = new NumericPeriodicConstraint(true, 0.0, Math.PI, true, comparer);
            return new SphericalCoordinateSystem3D(azimuthalConstraint, polarConstraint, comparer.Range);
        }

        /// <inheritdoc />
        public override ACoordinates ToReferenceSystem(in ACoordinates original)
        {
            return new ACoordinates(CalculateCoordinateX(original), CalculateCoordinateY(original), CalculateCoordinateZ(original));
        }

        /// <inheritdoc />
        public override ACoordinates ToSystem(in ACoordinates original)
        {
            var radius = CalculateRadius(original);
            var theta = CalculatePolarAngle(original, radius);
            var phi = CalculateAzimuthalAngle(original, theta);
            return new ACoordinates(radius, theta, phi);
        }

        /// <summary>
        ///     Calculates the radius as the length of the cartesian coordinate tuple
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private static double CalculateRadius(in ACoordinates original)
        {
            return Math.Sqrt(original.A * original.A + original.B * original.B + original.C * original.C);
        }

        /// <summary>
        ///     Calculates the polar angle under consideration of ISO standard (true, 0.0, PI, true)
        /// </summary>
        /// <param name="original"></param>
        /// <param name="radial"></param>
        /// <returns></returns>
        private double CalculatePolarAngle(in ACoordinates original, double radial)
        {
            return PolarConstraint.ParseToPeriodicRange(Math.Acos(original.C / radial));
        }

        /// <summary>
        ///     Calculates azimuthal angle under consideration of ISO standard (true, 0.0, 2*PI, false) and corrects inconsistency
        ///     issue for z-Alignment if polar angle is 0.0
        /// </summary>
        /// <param name="original"></param>
        /// <param name="theta"></param>
        /// <returns></returns>
        private double CalculateAzimuthalAngle(in ACoordinates original, double theta)
        {
            // The function has to return 0.0 if the theta angle is almost zero,
            // this is an inconsistent definition issue of z-aligned spherical coordinates

            if (theta.IsAlmostEqualByRange(PolarConstraint.MaxValue, AlmostEqualRange) || theta.IsAlmostEqualByRange(PolarConstraint.MinValue, AlmostEqualRange))
                return 0.0;

            return AzimuthalConstraint.ParseToPeriodicRange(Math.Atan2(original.B, original.A));
        }

        /// <summary>
        ///     Calculates coordinate X from polar coordinate tuple, corrects almost zero value
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private double CalculateCoordinateX(in ACoordinates original)
        {
            var x = original.A * Math.Sin(original.B) * Math.Cos(original.C);
            return x.IsAlmostZero(AlmostEqualRange) ? 0.0 : x;
        }

        /// <summary>
        ///     Calculates coordinate Y from polar coordinate tuple, corrects almost zero value
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private double CalculateCoordinateY(in ACoordinates original)
        {
            var y = original.A * Math.Sin(original.B) * Math.Sin(original.C);
            return y.IsAlmostZero(AlmostEqualRange) ? 0.0 : y;
        }

        /// <summary>
        ///     Calculates coordinate Z from polar coordinate tuple, corrects almost zero value
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private double CalculateCoordinateZ(in ACoordinates original)
        {
            var z = original.A * Math.Cos(original.B);
            return z.IsAlmostZero(AlmostEqualRange) ? 0.0 : z;
        }
    }
}