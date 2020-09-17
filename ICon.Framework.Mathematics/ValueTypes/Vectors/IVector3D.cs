namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Interface for all double precision three dimensional vectors of unspecified type
    /// </summary>
    public interface IVector3D
    {
        /// <summary>
        ///     Get the coordinate information as a <see cref="Coordinates3D" /> struct
        /// </summary>
        Coordinates3D Coordinates { get; }
    }

    /// <summary>
    ///     General interface of all cartesian double precision vectors
    /// </summary>
    public interface ICartesian3D : IVector3D
    {
        /// <summary>
        ///     The X coordinate value
        /// </summary>
        double X { get; }

        /// <summary>
        ///     The Y coordinate value
        /// </summary>
        double Y { get; }

        /// <summary>
        ///     The Z coordinate value
        /// </summary>
        double Z { get; }
    }


    /// <summary>
    ///     General interface of all fractional double precision vectors with three coordinates
    /// </summary>
    public interface IFractional3D : IVector3D
    {
        /// <summary>
        ///     The A coordinate value
        /// </summary>
        double A { get; }

        /// <summary>
        ///     The B coordinate value
        /// </summary>
        double B { get; }

        /// <summary>
        ///     The C coordinate value
        /// </summary>
        double C { get; }
    }


    /// <summary>
    ///     General interface of all spherical double precision vectors
    /// </summary>
    public interface ISpherical3D : IVector3D
    {
        /// <summary>
        ///     The radius coordinate value
        /// </summary>
        double Radius { get; }

        /// <summary>
        ///     The theta (polar) coordinate value
        /// </summary>
        double Theta { get; }

        /// <summary>
        ///     The phi (azimuthal) coordinate value
        /// </summary>
        double Phi { get; }
    }
}