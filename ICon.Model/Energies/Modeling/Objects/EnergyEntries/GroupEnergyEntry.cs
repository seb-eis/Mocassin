using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Represents a group energy entry that is fully described by center particle, a surrounding occupation state and an
    ///     energy value
    /// </summary>
    public readonly struct GroupEnergyEntry : IEquatable<GroupEnergyEntry>, IComparable<GroupEnergyEntry>
    {
        /// <summary>
        ///     The center particle of the group energy entry
        /// </summary>
        public IParticle CenterParticle { get; }

        /// <summary>
        ///     The occupation state of the surrounding positions
        /// </summary>
        public IOccupationState GroupOccupation { get; }

        /// <summary>
        ///     The energy value affiliated with the group entry
        /// </summary>
        public double Energy { get; }

        /// <summary>
        ///     Create new group energy entry from center particle, surrounding occupation and energy value
        /// </summary>
        /// <param name="centerParticle"></param>
        /// <param name="groupOccupation"></param>
        /// <param name="energy"></param>
        public GroupEnergyEntry(IParticle centerParticle, IOccupationState groupOccupation, double energy)
            : this()
        {
            CenterParticle = centerParticle;
            GroupOccupation = groupOccupation;
            Energy = energy;
        }

        /// <summary>
        ///     Creates a new group energy entry with the same identifier info but a changed energy value
        /// </summary>
        /// <param name="energy"></param>
        /// <returns></returns>
        public GroupEnergyEntry ChangeEnergy(double energy)
        {
            return new GroupEnergyEntry(CenterParticle, GroupOccupation, energy);
        }

        /// <summary>
        ///     Get a hash code for the identifier system of the group energy entry
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return CenterParticle.GetHashCode() ^ GroupOccupation.GetHashCode();
        }

        /// <inheritdoc />
        public bool Equals(GroupEnergyEntry other)
        {
            return CenterParticle.Equals(other.CenterParticle) && GroupOccupation.Equals(other.GroupOccupation);
        }

        /// <summary>
        ///     Compares to other energy entry in order of particle, than occupation
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(GroupEnergyEntry other)
        {
            var particleCompare = CenterParticle.CompareTo(other.CenterParticle);
            return particleCompare == 0 
                ? GroupOccupation.CompareTo(other.GroupOccupation) 
                : particleCompare;
        }

        /// <summary>
        ///     Creates a group energy entry with reordered state code
        /// </summary>
        /// <param name="newOrder"></param>
        /// <returns></returns>
        public GroupEnergyEntry CreateReordered(IList<int> newOrder)
        {
            if (newOrder.Count != GroupOccupation.StateLength)
                throw new ArgumentException("Order instruction is of wrong size", nameof(newOrder));

            var newOccupation = new OccupationState
            {
                Particles = GroupOccupation.Particles.SelectByIndexing(newOrder).ToList()
            };

            return new GroupEnergyEntry(CenterParticle, newOccupation, Energy);
        }
    }
}