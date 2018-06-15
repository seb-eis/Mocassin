using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.ValueTypes;
using ICon.Model.Particles;
using ICon.Model.Basic;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Represents a unit cell position that carries fractional position information and occupation as an index
    /// </summary>
    public interface IUnitCellPosition : IModelObject
    {
        /// <summary>
        /// The fractional position vector
        /// </summary>
        Fractional3D Vector { get; }

        /// <summary>
        /// The occupation set of this position
        /// </summary>
        IParticleSet OccupationSet { get; }

        /// <summary>
        /// The status flag of the unit cell position
        /// </summary>
        PositionStatus Status { get; }

        /// <summary>
        /// Creates a position struct from the unit cell position that supports fractional mass point and vector generic methods
        /// </summary>
        /// <returns></returns>
        FractionalPosition AsPosition();

        /// <summary>
        /// Checks if the position is stable and not deprecated
        /// </summary>
        /// <returns></returns>
        bool IsValidAndStable();

        /// <summary>
        /// Cehcks if the position is unstable and not deprecated
        /// </summary>
        /// <returns></returns>
        bool IsValidAndUnstable();
    }
}
