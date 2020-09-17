namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Basic fractional vector that carries fractional affine coordinate system information (A,B,C)
    /// </summary>
    public readonly struct Spherical3D : ISpherical3D
    {
        /// <summary>
        ///     The readonly <see cref="Coordinates3D" /> backing field
        /// </summary>
        public readonly Coordinates3D Coordinates;

        /// <inheritdoc />
        Coordinates3D IVector3D.Coordinates => Coordinates;

        /// <inheritdoc />
        public double Radius => Coordinates.A;

        /// <inheritdoc />
        public double Theta => Coordinates.B;

        /// <inheritdoc />
        public double Phi => Coordinates.C;

        /// <summary>
        ///     Construct from radius, theta and phi information
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="theta"></param>
        /// <param name="phi"></param>
        public Spherical3D(double radius, double theta, double phi)
            : this()
        {
            Coordinates = new Coordinates3D(radius, theta, phi);
        }

        /// <summary>
        ///     Creates new spherical vector from 3D coordinate tuple
        /// </summary>
        /// <param name="coordinates"></param>
        public Spherical3D(in Coordinates3D coordinates)
            : this()
        {
            Coordinates = coordinates;
        }
    }
}