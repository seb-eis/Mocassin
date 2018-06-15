using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;
using ICon.Model.Particles;
using ICon.Model.Structures;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Set of element indexes and sublattice index for doping information
    /// </summary>
    public interface IDopingCode : IModelObject
    {

        /// <summary>
        /// Dopand particle
        /// </summary>
        IParticle Dopant { get; }

        /// <summary>
        /// Particle that is doped
        /// </summary>
        IParticle DopedParticle { get; }

        /// <summary>
        /// unit cell position (contains information about the sublattice)
        /// </summary>
        IUnitCellPosition UnitCellPosition { get; }
    }
}
