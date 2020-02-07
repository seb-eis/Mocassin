using System;
using Mocassin.Mathematics.Extensions;
using Mocassin.Mathematics.Comparer;
using Mocassin.Mathematics.Constraints;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Mathematics.Coordinates
{
    /// <summary>
    ///     Defines double precision spherical coordinate system which follows the iso standard limitations (Theta: true, 0°,
    ///     180°, true, Phi: true, 0°, 360°, false)
    /// </summary>
    public class SphericalCoordinateSystem3D : AngularCoordinateSystem<Coordinates3D>
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
        public override Coordinates3D ToReferenceSystem(in Coordinates3D original)
        {
            return new Coordinates3D(GetCoordinateX(original), GetCoordinateY(original), GetCoordinateZ(original));
        }

        /// <summary>
        ///     Transforms a <see cref="Spherical3D"/> to a <see cref="Cartesian3D"/>
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public Cartesian3D ToReferenceSystem(in Spherical3D original)
        {
            return new Cartesian3D(ToReferenceSystem(original.Coordinates));
        }

        /// <inheritdoc />
        public override Coordinates3D ToSystem(in Coordinates3D original)
        {
            var radius = CalculateRadius(original);
            var theta = CalculatePolarAngle(original, radius);
            var phi = CalculateAzimuthalAngle(original, theta);
            return new Coordinates3D(radius, theta, phi);
        }

        /// <summary>
        ///     Transforms a <see cref="Cartesian3D"/> to a <see cref="Spherical3D"/>
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public Spherical3D ToSystem(in Cartesian3D original)
        {
            return new Spherical3D(ToSystem(original.Coordinates));
        }

        /// <summary>
        ///     Calculates the radius as the length of the cartesian coordinate tuple
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private static double CalculateRadius(in Coordinates3D original)
        {
            return Math.Sqrt(original.A * original.A + original.B * original.B + original.C * original.C);
        }

        /// <summary>
        ///     Calculates the polar angle under consideration of ISO standard (true, 0.0, PI, true)
        /// </summary>
        /// <param name="original"></param>
        /// <param name="radial"></param>
        /// <returns></returns>
        private double CalculatePolarAngle(in Coordinates3D original, double radial)
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
        private double CalculateAzimuthalAngle(in Coordinates3D original, double theta)
        {
            // The function has to return 0.0 if the theta angle is almost zero,
            // this is an inconsistent definition issue of z-aligned spherical coordinates

            if (theta.AlmostEqualByRange(PolarConstraint.MaxValue, AlmostEqualRange) || theta.AlmostEqualByRange(PolarConstraint.MinValue, AlmostEqualRange))
                return 0.0;

            return AzimuthalConstraint.ParseToPeriodicRange(Math.Atan2(original.B, original.A));
        }

        /// <summary>
        ///     Calculates coordinate X from polar coordinate tuple, corrects almost zero value
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private double GetCoordinateX(in Coordinates3D original)
        {
            var x = original.A * Math.Sin(original.B) * Math.Cos(original.C);
            return x.AlmostZero(AlmostEqualRange) ? 0.0 : x;
        }

        /// <summary>
        ///     Calculates coordinate Y from polar coordinate tuple, corrects almost zero value
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private double GetCoordinateY(in Coordinates3D original)
        {
            var y = original.A * Math.Sin(original.B) * Math.Sin(original.C);
            return y.AlmostZero(AlmostEqualRange) ? 0.0 : y;
        }

        /// <summary>
        ///     Calculates coordinate Z from polar coordinate tuple, corrects almost zero value
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private double GetCoordinateZ(in Coordinates3D original)
        {
            var z = original.A * Math.Cos(original.B);
            return z.AlmostZero(AlmostEqualRange) ? 0.0 : z;
        }
    }
}