using ICon.Model.Particles;
using ICon.Model.Structures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Temporary Object for lattice creation
    /// </summary>
    public class CellEntry
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
        /// Sublattice information on cell entry position
        /// </summary>
        public IBuildingBlock Block { get; set; }

        /// <summary>
        /// Original occupation before doping (important for simutaneous doping process)
        /// </summary>
        public bool IsDoped { get; set; }

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
                result *= 397;
                result += IsDoped.GetHashCode();
                return result;
            }
        }

        public override bool Equals(object obj)
        {
            //if (obj == null || GetType() != obj.GetType()) return false;
            //return GetHashCode() == obj.GetHashCode();

            if (obj is CellEntry cast)
            {
                return (Particle == cast.Particle && CellPosition == cast.CellPosition && Block == cast.Block && IsDoped == cast.IsDoped);
            }

            return false;
        }
    }
}
