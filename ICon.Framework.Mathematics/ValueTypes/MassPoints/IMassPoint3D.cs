namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     General interface for all 3D mass points that support a double convertible mass object
    /// </summary>
    public interface IMassPoint3D : IVector3D
    {
        /// <summary>
        ///     The mass value
        /// </summary>
        double Mass { get; }
    }

    /// <summary>
    ///     Combined vector and mass point interface for cartesian case
    /// </summary>
    public interface ICartesianMassPoint3D : IMassPoint3D, ICartesian3D
    {
        /// <summary>
        ///     Get the cartesian vector
        /// </summary>
        Cartesian3D Vector { get; }
    }

    /// <summary>
    ///     Combined vector and mass point interface for fractional case
    /// </summary>
    public interface IFractionalMassPoint3D : IMassPoint3D, IFractional3D
    {
        /// <summary>
        ///     Get the fractional vector
        /// </summary>
        Fractional3D Vector { get; }
    }

    /// <summary>
    ///     Combined vector and mass point interface for spherical case
    /// </summary>
    public interface ISphericalMassPoint3D : IMassPoint3D, ISpherical3D
    {
        /// <summary>
        ///     Get the spherical vector
        /// </summary>
        Spherical3D Vector { get; }
    }
}