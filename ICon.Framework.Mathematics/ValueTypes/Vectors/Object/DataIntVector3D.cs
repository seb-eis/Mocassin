using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    /// Serializable version of the 3D 192bit Flp-Vector that does not specify its coordinate system type. Intended for data storage and serialization
    /// </summary>
    [DataContract]
    public class DataIntVector3D
    {
        /// <summary>
        /// Value in first direction
        /// </summary>
        [DataMember]
        public int A { get; set; }

        /// <summary>
        /// Value in second direction
        /// </summary>
        [DataMember]
        public int B { get; set; }

        /// <summary>
        /// Value in third direction
        /// </summary>
        [DataMember]
        public int C { get; set; }

        /// <summary>
        /// Construct new data vector from 3 int coordinate values
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public DataIntVector3D(int a, int b, int c)
        {
            A = a;
            B = b;
            C = c;
        }

        /// <summary>
        /// Construct new data vector from coordinates
        /// </summary>
        /// <param name="coord"></param>
        public DataIntVector3D(Coordinates<int,int,int> coord)
        {
            A = coord.A;
            B = coord.B;
            C = coord.C;
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
        /// Compares the components of two vectors for equality
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public bool Equals(DataIntVector3D vector)
        {
            return A == vector.A && B == vector.B && C == vector.C;
        }

        public VectorInt3D AsReadOnly()
        {
            return new VectorInt3D(A, B, C);
        }
    }
}
