using System;
using System.Text;
using System.Xml.Serialization;

using ICon.Framework.Xml;
using ICon.Framework.Extensions;
using ICon.Framework.Exceptions;

namespace ICon.Mathematics.ValueTypes
{
    /// <summary>
    /// Generic basic spherical mass point that specifies a 3D spherical vector with additional information
    /// </summary>
    /// <typeparam name="TMass"></typeparam>
    public readonly struct SphericalMassPoint3D<TMass> : ISphericalMassPoint3D<SphericalMassPoint3D<TMass>> where TMass : struct, IConvertible
    {
        /// <summary>
        /// The raw mass value of the mass point
        /// </summary>
        public TMass Mass { get; }

        /// <summary>
        /// The internal basic spherical vector
        /// </summary>
        public Spherical3D Vector { get; }

        /// <summary>
        /// The radius coordinate of the internal vector
        /// </summary>
        public Double Radius => Vector.Radius;

        /// <summary>
        /// The theta coordinate of the internal vector
        /// </summary>
        public Double Theta => Vector.Theta;

        /// <summary>
        /// The phi coordinate of the internal vector
        /// </summary>
        public Double Phi => Vector.Phi;

        /// <summary>
        /// The coordinate tuple of the internal vector
        /// </summary>
        public Coordinates<Double, Double, Double> Coordinates => Vector.Coordinates;

        /// <summary>
        /// Static constructor, checks if the used mass is of primitive type
        /// </summary>
        static SphericalMassPoint3D()
        {
            if (typeof(TMass).IsPrimitive == false)
            {
                throw new InvalidGenericTypeException("Basic sphercial mass points only suppport mass values of primitive type", typeof(TMass));
            }
        }

        /// <summary>
        /// Creates new mass point from mass and spherical vector
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="vector"></param>
        public SphericalMassPoint3D(TMass mass, in Spherical3D vector) : this()
        {
            Mass = mass;
            Vector = vector;
        }

        /// <summary>
        /// Creates new mass point from mass and coordinate infos
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public SphericalMassPoint3D(TMass mass, Double x, Double y, Double z) : this(mass, new Spherical3D(x, y, z))
        {

        }

        /// <summary>
        /// Mass point factory function to create a new mass point with new mass value
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        public SphericalMassPoint3D<TMass> CreateNew(Double a, Double b, Double c, Double mass)
        {
            return new SphericalMassPoint3D<TMass>(ToMassType(mass), new Spherical3D(a, b, c));
        }

        /// <summary>
        /// Mass point factory function to create a new mass point with new mass value
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        public SphericalMassPoint3D<TMass> CreateNew(in Coordinates<Double, Double, Double> coordinates, Double mass)
        {
            return new SphericalMassPoint3D<TMass>(ToMassType(mass), new Spherical3D(coordinates));
        }

        /// <summary>
        /// Creates new by vector and mass
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        public SphericalMassPoint3D<TMass> CreateNew(in Spherical3D vector, Double mass)
        {
            return new SphericalMassPoint3D<TMass>(ToMassType(mass), vector);
        }


        /// <summary>
        /// Mass point vector factory to create a new mass point with new coordinates but unchanged mass value
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public SphericalMassPoint3D<TMass> CreateNew(Double a, Double b, Double c)
        {
            return new SphericalMassPoint3D<TMass>(Mass, new Spherical3D(a, b, c));
        }

        /// <summary>
        /// Mass point vector factory to create a new mass point with new coordinates but unchanged mass value
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public SphericalMassPoint3D<TMass> CreateNew(Coordinates<Double, Double, Double> coordinates)
        {
            return new SphericalMassPoint3D<TMass>(Mass, new Spherical3D(coordinates));
        }

        /// <summary>
        /// Get the internal mass value converted to a double value
        /// </summary>
        /// <returns></returns>
        public Double GetMass()
        {
            return Mass.ToPrimitive<TMass, Double>();
        }

        /// <summary>
        /// Culture invariant conversion of the double value to the internal mass type
        /// </summary>
        /// <param name="mass"></param>
        /// <returns></returns>
        public TMass ToMassType(Double mass)
        {
            return mass.ToPrimitive<Double, TMass>();
        }
    }
}
