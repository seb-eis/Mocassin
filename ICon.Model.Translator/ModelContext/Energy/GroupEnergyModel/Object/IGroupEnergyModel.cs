using System.Collections.Generic;
using Mocassin.Model.Energies;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Describes a single group energy model with an energy table and reference geometry info
    /// </summary>
    public interface IGroupEnergyModel : IModelComponent
    {
        /// <summary>
        ///     The group interaction the interaction model is based upon
        /// </summary>
        IGroupInteraction GroupInteraction { get; set; }

        /// <summary>
        ///     The list of existing occupation states for the group
        /// </summary>
        IList<IOccupationState> OccupationStates { get; set; }

        /// <summary>
        ///     Energy entry list that contains all possible combinations of center and surrounding occupation with energy value
        /// </summary>
        IList<GroupEnergyEntry> EnergyEntries { get; set; }

        /// <summary>
        ///     The position group info that describes extended symmetry information
        /// </summary>
        IPositionGroupInfo PositionGroupInfo { get; set; }

        /// <summary>
        ///     The sorted list of all existing group lookup codes
        /// </summary>
        IList<long> GroupLookupCodes { get; set; }

        /// <summary>
        ///     Dictionary that assigns each possible center particle a redirection index to reduce the table size
        /// </summary>
        IDictionary<IParticle, int> ParticleIndexToTableMapping { get; set; }

        /// <summary>
        ///     Assigns each group lookup code its affiliated <see cref="IOccupationState"/>
        /// </summary>
        IDictionary<long, IOccupationState> GroupCodeToOccupationStateMapping { get; set; }

        /// <summary>
        ///     The energy table for the group interaction that enables energy lookup by redirected center particle
        ///     index and lookup code index
        /// </summary>
        /// <returns></returns>
        double[,] EnergyTable { get; set; }
    }
}