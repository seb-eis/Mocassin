namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Fractional mass point
    /// </summary>
    public readonly struct FractionalMassPoint3D : IFractionalMassPoint3D
    {
        /// <summary>
        ///     The raw mass value of the mass point
        /// </summary>
        public double Mass { get; }

        /// <inheritdoc />
        public Fractional3D Vector { get; }

        /// <inheritdoc />
        public double A => Vector.A;

        /// <inheritdoc />
        public double B => Vector.B;

        /// <inheritdoc />
        public double C => Vector.C;

        /// <inheritdoc />
        public Coordinates3D Coordinates => Vector.Coordinates;

        /// <summary>
        ///     Creates new mass point from mass and fractional vector
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="vector"></param>
        public FractionalMassPoint3D(double mass, in Fractional3D vector)
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
        public FractionalMassPoint3D(double mass, double x, double y, double z)
            : this(mass, new Fractional3D(x, y, z))
        {
        }
    }
}