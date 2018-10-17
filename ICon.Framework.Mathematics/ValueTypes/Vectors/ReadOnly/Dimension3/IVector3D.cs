using System;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    /// Interface for all double precision three dimensional vectors of unspecified type
    /// </summary>
    public interface IVector3D
    {
        /// <summary>
        /// The internal double coordinate 3D tuple object
        /// </summary>
        Coordinates<double, double, double> Coordinates { get; }
    }

    /// <summary>
    /// Interface for all double precision three dimensional vectors of specific type
    /// </summary>
    /// <typeparam name="TVector"></typeparam>
    public interface IVector3D<TVector> : IVector3D where TVector : struct, IVector3D<TVector>
    {
       /// <summary>
       /// Factory function to create a new 3D vector of type T1 from double values
       /// </summary>
       /// <param name="a"></param>
       /// <param name="b"></param>
       /// <param name="c"></param>
       /// <returns></returns>
        TVector CreateNew(Double a, Double b, Double c);

        /// <summary>
        /// Factory function to create a new 3D vector of type T1 from coordinate tuple
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        TVector CreateNew(Coordinates<Double, Double, Double> coordinates);
    }

    /// <summary>
    /// General interface of all cartesian double precision vectors
    /// </summary>
    public interface ICartesian3D : IVector3D
    {
        /// <summary>
        /// The X coordinate value
        /// </summary>
        Double X { get; }

        /// <summary>
        /// The Y coordinate value
        /// </summary>
        Double Y { get; }

        /// <summary>
        /// The Z coordinate value
        /// </summary>
        Double Z { get; }
    }

    /// <summary>
    /// Generic interface that specializes the cartesian vector interface
    /// </summary>
    /// <typeparam name="TVector"></typeparam>
    public interface ICartesian3D<TVector> : ICartesian3D, IVector3D<TVector> where TVector : struct, ICartesian3D<TVector>
    {

    }

    /// <summary>
    /// General interface of all fractional double precision vectors with three coordinates
    /// </summary>
    public interface IFractional3D : IVector3D
    {
        /// <summary>
        /// The A coordinate value
        /// </summary>
        Double A { get; }

        /// <summary>
        /// The B coordinate value
        /// </summary>
        Double B { get; }

        /// <summary>
        /// The C coordinate value
        /// </summary>
        Double C { get; }
    }

    /// <summary>
    /// Generic interface that specializes the fractional vector interface
    /// </summary>
    /// <typeparam name="TVector"></typeparam>
    public interface IFractional3D<TVector> : IFractional3D, IVector3D<TVector> where TVector : struct, IFractional3D<TVector>
    {

    }

    /// <summary>
    /// General interface of all spherical double precision vectors
    /// </summary>
    public interface ISpherical3D : IVector3D
    {
        /// <summary>
        /// The radius coordinate value
        /// </summary>
        Double Radius { get; }

        /// <summary>
        /// The theta (polar) coordinate value
        /// </summary>
        Double Theta { get; }

        /// <summary>
        /// The phi (azimuthal) coordinate value
        /// </summary>
        Double Phi { get; }
    }

    /// <summary>
    /// Generic interface that specializes the spherical vector interface
    /// </summary>
    /// <typeparam name="TVector"></typeparam>
    public interface ISpherical3D<TVector> : ISpherical3D, IVector3D<TVector> where TVector : struct, ISpherical3D<TVector>
    {

    }
}
