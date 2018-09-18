using System;
using System.Collections.Generic;
using System.Text;
using ICon.Model.Energies;
using ICon.Model.Particles;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Basic implementation of the group energy model. Stores the model data context of group interactions
    /// </summary>
    public class GroupEnergyModel : ModelComponentBase, IGroupEnergyModel
    {
        /// <summary>
        /// The group interaction the interaction model is based upon
        /// </summary>
        public IGroupInteraction GroupInteraction { get; set; }

        /// <summary>
        /// The position group info that describes extended symmetry infromation
        /// </summary>
        public IPositionGroupInfo PositionGroupInfo { get; set; }

        /// <summary>
        /// The list of existing occupation states for the group
        /// </summary>
        public IList<IOccupationState> OccupationStates { get; set; }

        /// <summary>
        /// Energy entry list that contains all possible combinations of center and surrounding occupation with energy value
        /// </summary>
        public IList<GroupEnergyEntry> EnergyEntries { get; set; }

        /// <summary>
        /// The sorted list of all existing group lookup codes
        /// </summary>
        public IList<long> GroupLookupCodes { get; set; }

        /// <summary>
        /// Dictionary that assigns each possible center particle a redirection index to reduce the table size
        /// </summary>
        public IDictionary<IParticle, int> CenterParticleIndexing { get; set; }

        /// <summary>
        /// The energy table for the group interaction that enbales energy lookup by redirected center particle index and lookup code index
        /// </summary>
        /// <returns></returns>
        public double[,] EnergyTable { get; set; }

        /// <summary>
        /// Create new group energy model for the passed group interaction
        /// </summary>
        /// <param name="groupInteraction"></param>
        public GroupEnergyModel(IGroupInteraction groupInteraction)
        {
            GroupInteraction = groupInteraction ?? throw new ArgumentNullException(nameof(groupInteraction));
        }
    }
}
