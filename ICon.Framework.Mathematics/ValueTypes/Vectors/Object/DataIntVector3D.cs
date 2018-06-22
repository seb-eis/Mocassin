using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace ICon.Mathematics.ValueTypes
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
        /// Get a JSON representation of this object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is DataIntVector3D casted)
            {
                return A == casted.A && B == casted.B && C == casted.C;
            }
            else
            {
                return false;
            }
        }

    }
}
