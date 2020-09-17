namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Cartesian 3D mass point
    /// </summary>
    public readonly struct CartesianMassPoint3D : ICartesianMassPoint3D
    {
        /// <inheritdoc />
        public double Mass { get; }

        /// <inheritdoc />
        public Cartesian3D Vector { get; }

        /// <inheritdoc />
        public double X => Vector.X;

        /// <inheritdoc />
        public double Y => Vector.Y;

        /// <inheritdoc />
        public double Z => Vector.Z;

        /// <inheritdoc />
        public Coordinates3D Coordinates => Vector.Coordinates;

        /// <summary>
        ///     Creates new mass point from mass and cartesian vector
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="vector"></param>
        public CartesianMassPoint3D(double mass, in Cartesian3D vector)
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
        public CartesianMassPoint3D(double mass, double x, double y, double z)
            : this(mass, new Cartesian3D(x, y, z))
        {
        }
    }
}