using System;

namespace ICon.Mathematics.ValueTypes
{
    /// <summary>
    /// General interface for all 3D mass points that support a double convertible mass object
    /// </summary>
    public interface IMassPoint3D : IVector3D
    {
        /// <summary>
        /// The mass value converted to double
        /// </summary>
        double GetMass();
    }

    /// <summary>
    /// Generic mass point interface that specializes the mass point type and affiliated factory methods
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public interface IMassPoint3D<T1> : IMassPoint3D where T1 : struct, IMassPoint3D<T1>
    {
        /// <summary>
        /// Factory function to create a new 3D mass point of type T1 from double coordinates and mass information
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        T1 CreateNew(double a, double b, double c, double mass);

        /// <summary>
        /// Factory function to create a new 3D mass point of this type from mass and coordinate tuple
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        T1 CreateNew(in Coordinates<double, double, double> coordinates, double mass);
    }

    /// <summary>
    /// Combined vector and mass point interface for cartesian case
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public interface ICartesianMassPoint3D<T1> : IMassPoint3D<T1>, ICartesian3D<T1> where T1 : struct, ICartesianMassPoint3D<T1>
    {
        /// <summary>
        /// Get the cartesian vector
        /// </summary>
        Cartesian3D Vector { get; }

        /// <summary>
        /// Creates new by vetor and mass
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        T1 CreateNew(in Cartesian3D vector, double mass);
    }

    /// <summary>
    /// Combined vector and mass point interface for fractional case
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public interface IFractionalMassPoint3D<T1> : IMassPoint3D<T1>, IFractional3D<T1> where T1 : struct, IFractionalMassPoint3D<T1>
    {
        /// <summary>
        /// Get the fractional vector
        /// </summary>
        Fractional3D Vector { get; }

        /// <summary>
        /// Creates new by vetor and mass
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        T1 CreateNew(in Fractional3D vector, double mass);
    }

    /// <summary>
    /// Combined vector and mass point interface for spherical case
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public interface ISphericalMassPoint3D<T1> : IMassPoint3D<T1>, ISpherical3D<T1> where T1 : struct, ISphericalMassPoint3D<T1>
    {
        /// <summary>
        /// Get the spherical vector
        /// </summary>
        Spherical3D Vector { get; }

        /// <summary>
        /// Creates new by vetor and mass
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        T1 CreateNew(in Spherical3D vector, double mass);
    }
}
