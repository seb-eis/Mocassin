﻿using System.Collections.Generic;
using Mocassin.Framework.Constraints;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Represents a group energy setter that enables save energy entry manipulation for a user defined position group
    /// </summary>
    public interface IGroupEnergySetter : IValueSetter
    {
        /// <summary>
        ///     Get a read only collection of all local energy entries of the setter
        /// </summary>
        IReadOnlyCollection<GroupEnergyEntry> EnergyEntries { get; }

        /// <summary>
        ///     The group interaction the setter can manipulate
        /// </summary>
        IGroupInteraction GroupInteraction { get; }

        /// <summary>
        ///     Constraint for the energy values
        /// </summary>
        IValueConstraint<double, double> EnergyConstraint { get; }

        /// <summary>
        ///     Get the <see cref="IPositionGroupInfo" /> that describes the symmetry of the <see cref="IGroupInteraction" />
        /// </summary>
        IPositionGroupInfo PositionGroupInfo { get; }

        /// <summary>
        ///     Set an energy value in the local energy entry set
        /// </summary>
        /// <param name="energyEntry"></param>
        void SetEnergyEntry(in GroupEnergyEntry energyEntry);

        /// <summary>
        ///     Sets multiple energy entries in the local energy entry set
        /// </summary>
        /// <param name="energyEntries"></param>
        void SetEnergyEntries(IEnumerable<GroupEnergyEntry> energyEntries);
    }
}