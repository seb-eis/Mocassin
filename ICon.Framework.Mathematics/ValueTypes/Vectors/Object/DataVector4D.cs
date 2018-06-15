using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace ICon.Mathematics.ValueTypes
{
    /// <summary>
    /// Serializable version of the 4D 128bit Int-Vector that does not specify its coordinate system type. Intended for data storage and serialization
    /// </summary>
    [DataContract]
    public class DataVector4D
    {
        /// <summary>
        /// Coordinate value in A direction
        /// </summary>
        [DataMember]
        public int A { get; set; }

        /// <summary>
        /// Coordinate value in B direction
        /// </summary>
        [DataMember]
        public int B { get; set; }

        /// <summary>
        /// Coordinate value in C direction
        /// </summary>
        [DataMember]
        public int C { get; set; }

        /// <summary>
        /// Coordinate value in D direction
        /// </summary>
        [DataMember]
        public int D { get; set; }



        /// <summary>
        /// Construct from any linear 4D vector interface
        /// </summary>
        public DataVector4D(ILinearVector4D vector) : this(vector.Coordinates.A, vector.Coordinates.B, vector.Coordinates.C, vector.Coordinates.D)
        {
        }

        /// <summary>
        /// Construct from 4 integer coordinate values
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        public DataVector4D(int a, int b, int c, int d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        /// <summary>
        /// Returns JSON representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Returns the coordinate values a crystal vector struct
        /// </summary>
        /// <returns></returns>
        public CrystalVector4D AsCrystalVector()
        {
            return new CrystalVector4D(A, B, C, D);
        }
    }
}
