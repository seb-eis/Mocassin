using System;
using System.Text;
using System.Xml.Serialization;

using ICon.Framework.Xml;
using ICon.Framework.Extensions;
using ICon.Framework.Exceptions;

namespace ICon.Mathematics.ValueTypes
{
    /// <summary>
    /// Generic cartesian 3D mass point (Only supports primitive type masses that support System.Convert)
    /// </summary>
    /// <typeparam name="TMass"></typeparam>
    public readonly struct CartesianMassPoint3D<TMass> : ICartesianMassPoint3D<CartesianMassPoint3D<TMass>> where TMass : struct, IConvertible
    {
        /// <summary>
        /// The raw mass value of the mass point
        /// </summary>
        public TMass Mass { get; }

        /// <summary>
        /// The internal basic cartesian vector
        /// </summary>
        public Cartesian3D Vector { get; }

        /// <summary>
        /// The X coordinate of the internal vector
        /// </summary>
        public Double X => Vector.X;

        /// <summary>
        /// The Y coordinate of the internal vector
        /// </summary>
        public Double Y => Vector.Y;

        /// <summary>
        /// The Z coordinate of the internal vector
        /// </summary>
        public Double Z => Vector.Z;

        /// <summary>
        /// The coordinate tuple of the internal vector
        /// </summary>
        public Coordinates<Double, Double, Double> Coordinates => Vector.Coordinates;

        /// <summary>
        /// Static constructor, checks if the used mass is of primitive type
        /// </summary>
        static CartesianMassPoint3D()
        {
            if (typeof(TMass).IsPrimitive == false)
            {
                throw new InvalidGenericTypeException("Basic cartesian mass points only suppport mass values of primitive type", typeof(TMass));
            }
        }

        /// <summary>
        /// Creates new mass point from mass and cartesian vector
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="vector"></param>
        public CartesianMassPoint3D(TMass mass, in Cartesian3D vector) : this()
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
        public CartesianMassPoint3D(TMass mass, Double x, Double y, Double z) : this(mass, new Cartesian3D(x, y, z))
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
        public CartesianMassPoint3D<TMass> CreateNew(Double a, Double b, Double c, Double mass)
        {
            return new CartesianMassPoint3D<TMass>(ToMassType(mass), new Cartesian3D(a, b, c));
        }

        /// <summary>
        /// Mass point factory function to create a new mass point with new mass value
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        public CartesianMassPoint3D<TMass> CreateNew(in Coordinates<Double, Double, Double> coordinates, Double mass)
        {
            return new CartesianMassPoint3D<TMass>(ToMassType(mass), new Cartesian3D(coordinates));
        }

        /// <summary>
        /// Creates new by vector and mass value
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        public CartesianMassPoint3D<TMass> CreateNew(in Cartesian3D vector, Double mass)
        {
            return new CartesianMassPoint3D<TMass>(ToMassType(mass), vector);
        }

        /// <summary>
        /// Mass point vector factory to create a new mass point with new coordinates but unchanged mass value
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public CartesianMassPoint3D<TMass> CreateNew(Double a, Double b, Double c)
        {
            return new CartesianMassPoint3D<TMass>(Mass, new Cartesian3D(a, b, c));
        }

        /// <summary>
        /// Mass point vector factory to create a new mass point with new coordinates but unchanged mass value
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public CartesianMassPoint3D<TMass> CreateNew(Coordinates<Double, Double, Double> coordinates)
        {
            return new CartesianMassPoint3D<TMass>(Mass, new Cartesian3D(coordinates));
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
