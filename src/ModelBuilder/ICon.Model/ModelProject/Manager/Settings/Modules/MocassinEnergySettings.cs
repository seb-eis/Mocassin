using System.Runtime.Serialization;
using Mocassin.Model.Energies;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Settings data object for the energy managing module
    /// </summary>
    [DataContract, ModuleSettings(typeof(IEnergyManager))]
    public class MocassinEnergySettings : MocassinModuleSettings
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

        /// <inheritdoc />
        public override void InitAsDefault()
        {
            EnforceGroupConsistency = true;
            AtomsPerGroup = new ValueSetting<int>("Particles per Group", 2, 8);
            GroupsPerPosition = new ValueSetting<int>("Groups per Position", 0, 0, 4, 10);
            PermutationsPerGroup = new ValueSetting<long>("Permutations per Group", 1, 1, 500, 5000);
            PositionsPerStable = new ValueSetting<long>("Positions per Stable Environment", 0, 1, 500, 5000);
            PositionsPerUnstable = new ValueSetting<long>("Positions per Unstable Environment", 0, 1, 100, 1000);
            PairEnergies = new ValueSetting<double>("Pair Energy", -100, 100);
            GroupEnergies = new ValueSetting<double>("Group Energy", -100, 100);
        }
    }
}