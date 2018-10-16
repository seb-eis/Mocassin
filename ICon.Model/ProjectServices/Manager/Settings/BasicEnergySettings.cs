using System.Runtime.Serialization;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    ///     Settings data object for the energy managing module
    /// </summary>
    [DataContract]
    public class BasicEnergySettings
    {
        /// <summary>
        ///     Boolean flag for the group consistency enforcement. If true, groups are automatically translated and added to all
        ///     positions they include
        /// </summary>
        [DataMember]
        public bool EnforceGroupConsistency { get; set; }

        /// <summary>
        ///     Value restriction setting for the number of atoms per group
        /// </summary>
        [DataMember]
        public ValueSetting<int> AtomsPerGroup { get; set; }

        /// <summary>
        ///     Value restriction setting for the number of groups per unit cell position
        /// </summary>
        [DataMember]
        public ValueSetting<int> GroupsPerPosition { get; set; }

        /// <summary>
        ///     Value restriction setting for the number of non-unique permutations per group
        /// </summary>
        [DataMember]
        public ValueSetting<long> PermutationsPerGroup { get; set; }

        /// <summary>
        ///     Value restriction setting for the number of positions per stable environment
        /// </summary>
        [DataMember]
        public ValueSetting<long> PositionsPerStable { get; set; }

        /// <summary>
        ///     Value restriction setting for the number of positions per unstable environment
        /// </summary>
        [DataMember]
        public ValueSetting<long> PositionsPerUnstable { get; set; }

        /// <summary>
        ///     Value restriction setting for the energy values of pair interactions
        /// </summary>
        [DataMember]
        public ValueSetting<double> PairEnergies { get; set; }

        /// <summary>
        ///     Value restriction setting for the energy values of group interactions
        /// </summary>
        [DataMember]
        public ValueSetting<double> GroupEnergies { get; set; }
    }
}