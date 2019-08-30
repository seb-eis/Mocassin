using System;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     General interface for describing defect energies of position and particle combinations
    /// </summary>
    public interface IDefectEnergy : IComparable<IDefectEnergy>, IEquatable<IDefectEnergy>
    {
        /// <summary>
        ///     Get the <see cref="IUnitCellPosition" /> the defect belongs to
        /// </summary>
        IUnitCellPosition UnitCellPosition { get; }

        /// <summary>
        ///     Get the <see cref="IParticle" /> that describes the defect
        /// </summary>
        IParticle Particle { get; }

        /// <summary>
        ///     Get the energy value affiliated with the defect in [eV]
        /// </summary>
        double Energy { get; }
    }
}