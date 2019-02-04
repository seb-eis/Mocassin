using ICon.Model.Particles;
using ICon.Model.Structures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Contains information about a CellEntry within the WorkLattice
    /// </summary>
    public class LatticeEntry : IEquatable<LatticeEntry>
    {
        /// <summary>
        /// Occupation on cell entry
        /// </summary>
        public IParticle Particle { get; set; }

        /// <summary>
        /// Sublattice information on cell entry position
        /// </summary>
        public IUnitCellPosition CellPosition { get; set; }

        /// <summary>
        /// BuildingBlock information on cell entry position
        /// </summary>
        public IBuildingBlock Block { get; set; }

        /// <summary>
        /// Return the HashCode resulting from MemberVariables (LatticeEntries with the same Members return the same HashCode)
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = -1261372869;
                result *= -1521134295;
                if (Particle != null) result += Particle.Index;
                result *= -1521134295;
                if (CellPosition != null) result += CellPosition.Index;
                result *= -1521134295;
                if (Block != null) result += Block.Index;
                return result;
            }
        }

        /// <summary>
        /// Equality comparison
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as LatticeEntry);
        }

        /// <summary>
        /// Make a shallow copy of this object
        /// </summary>
        /// <returns></returns>
        public LatticeEntry ShallowCopy()
        {
            return (LatticeEntry) MemberwiseClone();
        }

        /// <summary>
        /// Equality comparison
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(LatticeEntry other)
        {
            return other != null && Particle == other.Particle &&
                CellPosition == other.CellPosition && Block == other.Block;
        }
    }
}
