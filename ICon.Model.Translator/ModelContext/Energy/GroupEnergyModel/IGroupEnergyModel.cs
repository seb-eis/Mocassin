using ICon.Model.Energies;
using ICon.Model.Particles;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Describes a single group energy model with an energy table and refernce geometry info
    /// </summary>
    public interface IGroupEnergyModel : IModelComponent
    {
        /// <summary>
        /// The group interaction the interaction model is based upon
        /// </summary>
        IGroupInteraction GroupInteraction { get; set; }

        /// <summary>
        /// The list of existing occupation states for the group
        /// </summary>
        IList<IOccupationState> OccupationStates { get; set; }

        /// <summary>
        /// Energy dictionary that assign each ceter particle a dictionary with the occupation state energy pairs
        /// </summary>
        IReadOnlyDictionary<IParticle, IReadOnlyDictionary<OccupationState, double>> EnergyDictionary { get; set; }

        /// <summary>
        /// The sorted list of all existing group lookup codes
        /// </summary>
        IList<long> GroupLookupCodes { get; set; }

        /// <summary>
        /// Dictionary that assigns each possible center particle a redirection index to reduce the table size
        /// </summary>
        IDictionary<IParticle, int> CenterParticleIndexing { get; set; }

        /// <summary>
        /// The energy table for the group interaction that enbales energy lookup by redirected center particle index and lookup code index
        /// </summary>
        /// <returns></returns>
        double[,] EnergyTable { get; set; }
    }
}
