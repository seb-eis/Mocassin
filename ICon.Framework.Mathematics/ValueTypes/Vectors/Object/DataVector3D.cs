using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    /// Serializable version of the 3D 192bit Flp-Vector that does not specify its coordinate system type. Intended for data storage and serialization
    /// </summary>
    [DataContract]
    public class DataVector3D
    {
        /// <summary>
        /// Value in first direction
        /// </summary>
        [DataMember]
        public double A { get; set; }

        /// <summary>
        /// Value in second direction
        /// </summary>
        [DataMember]
        public double B { get; set; }

        /// <summary>
        /// Value in third direction
        /// </summary>
        [DataMember]
        public double C { get; set; }

        /// <summary>
        /// Deafult construct (0,0,0) vector
        /// </summary>
        public DataVector3D()
        {
        }

        /// <summary>
        /// Construct new data vector from 3 double coordinate values
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public DataVector3D(double a, double b, double c)
        {
            A = a;
            B = b;
            C = c;
        }

        /// <summary>
        /// Construct from 3D vector interface
        /// </summary>
        /// <param name="vector"></param>
        public DataVector3D(IVector3D vector) : this (vector.Coordinates.A, vector.Coordinates.B, vector.Coordinates.C)
        {
        }

        /// <summary>
        /// Get a JSON representation of this object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Return the vector as a fractional vector struct
        /// </summary>
        /// <returns></returns>
        public Fractional3D AsFractional()
        {
            return new Fractional3D(A, B, C);
        }

        /// <summary>
        /// Return the vector as a cartesian vector struct
        /// </summary>
        /// <returns></returns>
        public Cartesian3D AsCartesian()
        {
            return new Cartesian3D(A, B, C);
        }

        /// <summary>
        /// Return the vector as a spherical vector struct
        /// </summary>
        /// <returns></returns>
        public Spherical3D AsSpherical()
        {
            return new Spherical3D(A, B, C);
        }
    }
}
