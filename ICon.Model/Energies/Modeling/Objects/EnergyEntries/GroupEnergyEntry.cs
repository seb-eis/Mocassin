﻿using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Particles;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Represents a group energy entry that is fully described by center particle, a surrounding occupation state and an energy value
    /// </summary>
    public readonly struct GroupEnergyEntry : IEquatable<GroupEnergyEntry>
    {
        /// <summary>
        /// The center particle of the group energy entry
        /// </summary>
        public IParticle CenterParticle { get; }

        /// <summary>
        /// The occupation state of the surrounding positions
        /// </summary>
        public IOccupationState GroupOccupation { get; }

        /// <summary>
        /// The enrgy value affiliated with the group entry
        /// </summary>
        public double Energy { get; }

        /// <summary>
        /// Create new group energy entry from center particle, surrounding occupation and energy value
        /// </summary>
        /// <param name="centerParticle"></param>
        /// <param name="groupOccupation"></param>
        /// <param name="energy"></param>
        public GroupEnergyEntry(IParticle centerParticle, IOccupationState groupOccupation, double energy) : this()
        {
            CenterParticle = centerParticle;
            GroupOccupation = groupOccupation;
            Energy = energy;
        }

        /// <summary>
        /// Creates a new group energy entry with the same identifier info but a changed energy value
        /// </summary>
        /// <param name="energy"></param>
        /// <returns></returns>
        public GroupEnergyEntry ChangeEnergy(double energy)
        {
            return new GroupEnergyEntry(CenterParticle, GroupOccupation, energy);
        }

        /// <summary>
        /// Get a hash code for the identifier system of the group energy entry
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return CenterParticle.GetHashCode() ^ GroupOccupation.GetHashCode();
        }

        /// <summary>
        /// Checks for equivalence of the identifier objects with other groups information (Does not compare energy value)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(GroupEnergyEntry other)
        {
            return CenterParticle.Equals(other.CenterParticle) && GroupOccupation.Equals(other.GroupOccupation);
        }
    }
}
