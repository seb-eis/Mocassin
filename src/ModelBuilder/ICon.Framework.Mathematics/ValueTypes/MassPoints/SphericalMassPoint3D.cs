namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Spherical mass point
    /// </summary>
    public readonly struct SphericalMassPoint3D : ISphericalMassPoint3D
    {
        /// <summary>
        ///     The raw mass value of the mass point
        /// </summary>
        public double Mass { get; }

        /// <inheritdoc />
        public Spherical3D Vector { get; }

        /// <inheritdoc />
        public double Radius => Vector.Radius;

        /// <inheritdoc />
        public double Theta => Vector.Theta;

        /// <inheritdoc />
        public double Phi => Vector.Phi;

        /// <inheritdoc />
        public Coordinates3D Coordinates => Vector.Coordinates;

        /// <summary>
        ///     Creates new mass point from mass and spherical vector
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="vector"></param>
        public SphericalMassPoint3D(double mass, in Spherical3D vector)
            : this()
        {
            Mass = mass;
            Vector = vector;
        }

        /// <summary>
        ///     Creates new mass point from mass and coordinate infos
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public SphericalMassPoint3D(double mass, double x, double y, double z)
            : this(mass, new Spherical3D(x, y, z))
        {
        }
    }
}