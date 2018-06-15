using System;
using System.Xml.Serialization;
using System.Runtime.Serialization;

using ICon.Framework.Xml;

namespace ICon.Mathematics.ValueTypes
{
    /// <summary>
    /// Basic fractional vector that carries fractional affine coordinate system information (A,B,C)
    /// </summary>
    public readonly struct Spherical3D : ISpherical3D<Spherical3D>
    {
        /// <summary>
        /// The 3D coordinate tuple
        /// </summary>
        public Coordinates<Double, Double, Double> Coordinates { get; }

        /// <summary>
        /// Get the X coordinate value
        /// </summary>
        public Double Radius => Coordinates.A;

        /// <summary>
        /// Get the Y coordinate value
        /// </summary>
        public Double Theta => Coordinates.B;

        /// <summary>
        /// Get the Z coordinate value
        /// </summary>
        public Double Phi => Coordinates.C;

        /// <summary>
        /// Construct from radius, theta and phi information
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="theta"></param>
        /// <param name="phi"></param>
        public Spherical3D(Double radius, Double theta, Double phi) : this()
        {
            Coordinates = new Coordinates<Double, Double, Double>(radius, theta, phi);
        }
        
        /// <summary>
        /// Creates new sphercial vector from 3D coordinate tuple
        /// </summary>
        /// <param name="coordinates"></param>
        public Spherical3D(Coordinates<Double, Double, Double> coordinates) : this()
        {
            Coordinates = coordinates;
        }

        /// <summary>
        /// Factory function to create new basic spherical vector
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public Spherical3D CreateNew(Double a, Double b, Double c)
        {
            return new Spherical3D(a, b, c);
        }

        /// <summary>
        /// Factory function to create new basic spherical vector
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public Spherical3D CreateNew(Coordinates<Double, Double, Double> coordinates)
        {
            return new Spherical3D(coordinates);
        }
    }
}
