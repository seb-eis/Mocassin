using System;
using System.Collections.Generic;
using Mocassin.Model.Energies;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IGroupEnergyModel" />
    public class GroupEnergyModel : ModelComponentBase, IGroupEnergyModel
    {
        /// <inheritdoc />
        public IGroupInteraction GroupInteraction { get; set; }

        /// <inheritdoc />
        public IPositionGroupInfo PositionGroupInfo { get; set; }

        /// <inheritdoc />
        public IList<IOccupationState> OccupationStates { get; set; }

        /// <inheritdoc />
        public IList<GroupEnergyEntry> EnergyEntries { get; set; }

        /// <inheritdoc />
        public IList<long> GroupLookupCodes { get; set; }

        /// <inheritdoc />
        public IDictionary<IParticle, int> ParticleIndexToTableMapping { get; set; }

        /// <inheritdoc />
        public double[,] EnergyTable { get; set; }

        /// <summary>
        ///     Create new group energy model for the passed group interaction
        /// </summary>
        /// <param name="groupInteraction"></param>
        public GroupEnergyModel(IGroupInteraction groupInteraction)
        {
            GroupInteraction = groupInteraction ?? throw new ArgumentNullException(nameof(groupInteraction));
        }
    }
}