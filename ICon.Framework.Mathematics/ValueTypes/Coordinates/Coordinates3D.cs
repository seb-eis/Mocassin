namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Represents a context free, readonly 24 byte double precision coordinate information for 3D space
    /// </summary>
    public readonly struct Coordinates3D : ICoordinates
    {
        /// <summary>
        ///     First coordinate value
        /// </summary>
        public double A { get; }

        /// <summary>
        ///     Second coordinate value
        /// </summary>
        public double B { get; }

        /// <summary>
        ///     Third coordinate value
        /// </summary>
        public double C { get; }

        /// <inheritdoc />
        public int Dimension => 3;

        /// <summary>
        ///     Creates a new <see cref="Coordinates3D" /> struct from coordinate points
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public Coordinates3D(double a, double b, double c)
        {
            A = a;
            B = b;
            C = c;
        }
    }
}