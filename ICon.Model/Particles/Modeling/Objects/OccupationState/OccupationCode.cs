using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ICon.Framework.Extensions;
using ICon.Framework.Collections;

namespace ICon.Model.Particles
{
    /// <summary>
    /// Represents a particle occupation code that describes the actual occupation of a grouping through a set of particle indices
    /// </summary>
    [DataContract]
    public class OccupationCode : ArrayCode<int>, IComparable<OccupationCode>, IEquatable<OccupationCode>
    {
        /// <summary>
        /// Creates new occupation code with the specfified size and default values
        /// </summary>
        /// <param name="size"></param>
        public OccupationCode(int size)
        {
            CodeValues = new int[size];
        }

        /// <summary>
        /// Create new occupation code with null array
        /// </summary>
        public OccupationCode()
        {
        }

        /// <summary>
        /// Compares to other grouping particle code (Lexicographic compare)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(OccupationCode other)
        {
            return CodeValues.LexicographicCompare(other.CodeValues);
        }

        /// <summary>
        /// Checks the particle arrays for sequence equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(OccupationCode other)
        {
            return CodeValues.SequenceEqual(other.CodeValues);
        }

        /// <summary>
        /// Get a string representing the name of this modelling object
        /// </summary>
        /// <returns></returns>
        public override string GetTypeName()
        {
            return "'Occupation Code'";
        }
    }
}
