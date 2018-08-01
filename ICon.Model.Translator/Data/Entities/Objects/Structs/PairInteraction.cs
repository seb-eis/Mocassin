using System;
using System.Collections.Generic;
using System.Text;
using ICon.Mathematics.ValueTypes;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Describes a single pair interaction as a relative 4D vector and an index that indentifies the assigned pair energy table
    /// </summary>
    public readonly struct PairInteraction
    {
        /// <summary>
        /// The relative 4D vector to the target position
        /// </summary>
        public CrystalVector4D Vector { get; }

        /// <summary>
        /// The index of the pair table used for energy lookup
        /// </summary>
        public int PairTableIndex { get; }

        /// <summary>
        /// Create new pair interaction from crystal vector and pair table index
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="pairTableIndex"></param>
        public PairInteraction(CrystalVector4D vector, int pairTableIndex) : this()
        {
            Vector = vector;
            PairTableIndex = pairTableIndex;
        }
    }
}
