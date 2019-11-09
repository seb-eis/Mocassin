﻿using Newtonsoft.Json;
using System.Runtime.Serialization;
using Mocassin.Mathematics.ValueTypes;
using Moccasin.Mathematics.ValueTypes;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    /// Serializable version of the 3D 192bit Flp-Vector that does not specify its coordinate system type. Intended for data storage and serialization
    /// </summary>
    public class DataIntVector3D
    {
        /// <summary>
        /// Value in first direction
        /// </summary>
        public int A { get; set; }

        /// <summary>
        /// Value in second direction
        /// </summary>
        public int B { get; set; }

        /// <summary>
        /// Value in third direction
        /// </summary>
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
        public DataIntVector3D(in VectorI3 coord)
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

		public VectorI3 AsReadOnly() => new VectorI3(A, B, C);
	}
}
