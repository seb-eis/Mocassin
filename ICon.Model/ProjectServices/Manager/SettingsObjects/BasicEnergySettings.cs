using System;
using System.Runtime.Serialization;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    /// Settings data object for the energy managing modul
    /// </summary>
    [Serializable]
    [DataContract]
    public class BasicEnergySettings
    {
        /// <summary>
        /// Flags that enables or disables the grouping system
        /// </summary>
        public bool GroupingEnabled { get; set; }

        /// <summary>
        /// The maximum number of atoms within a single grouping
        /// </summary>
        public int MaxGroupingSize { get; set; }

        /// <summary>
        /// The maximum number of overlay groupings for each surrounding
        /// </summary>
        public int MaxGroupingCount { get; set; }

        /// <summary>
        /// The maximum number of non-unique permutations a grouping is allowed to support
        /// </summary>
        public int MaxGroupPermutationCount { get; set; }

        /// <summary>
        /// Flags that indicates if the grouping has to be compatible between all environments that would require to contain it for energy consitency reasons
        /// </summary>
        public bool EnforceStableGroupingConsistency { get; set; }

        /// <summary>
        /// Limits the maximum number of stable environment positions
        /// </summary>
        public int MaxStableEnvironmentPositionCount { get; set; }

        /// <summary>
        /// Limits the maximum number of unstable environment positions
        /// </summary>
        public int MaxUnstableEnvironmentPositionCount { get; set; }

        /// <summary>
        /// The limit of positions from which on the system returns a non-critical warning about the environment position count
        /// </summary>
        public int EnvironmentPositionWarningLimit { get; set; }

    }
}
