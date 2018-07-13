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
    public class LatticeEntry
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
                int result = 37;
                result *= 397;
                if (Particle != null) result += Particle.GetHashCode();
                result *= 397;
                if (CellPosition != null) result += CellPosition.GetHashCode();
                result *= 397;
                if (Block != null) result += Block.GetHashCode();
                return result;
            }
        }

        /// <summary>
        /// Equal comparison based on HashCode
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return GetHashCode() == obj.GetHashCode();
        }

        /// <summary>
        /// Make a shallow copy of this object (members variables are not compied)
        /// </summary>
        /// <returns></returns>
        public LatticeEntry ShallowCopy()
        {
            return (LatticeEntry) MemberwiseClone();
        }

    }
}
