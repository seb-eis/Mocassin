using System.Collections.Generic;
using System.Runtime.Serialization;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     The reference model data object for the energy manager
    /// </summary>
    [DataContract(Name = "EnergyModelData")]
    public class EnergyModelData : ModelData<IEnergyDataPort>
    {
        /// <summary>
        ///     The environment info parameter that describes the basic restrictions for all regular position environments
        /// </summary>
        [DataMember]
        [ModelParameter(typeof(IStableEnvironmentInfo))]
        public StableEnvironmentInfo StableEnvironmentInfo { get; set; }

        /// <summary>
        ///     The list of defined group interactions that can be used in environment descriptions
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IGroupInteraction))]
        public List<GroupInteraction> GroupInteractions { get; set; }

        /// <summary>
        ///     The list of unstable environment definitions that are defined to contain actual information and are non zero
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IUnstableEnvironment))]
        public List<UnstableEnvironment> UnstableEnvironmentInfos { get; set; }

        /// <summary>
        ///     The list of pair information that describe all existing pair interactions for stable positions (Auto-managed, no
        ///     input support)
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(ISymmetricPairInteraction), IsAutoManaged = true)]
        public List<SymmetricPairInteraction> StablePairInteractions { get; set; }

        /// <summary>
        ///     The list of asymmetric pair interactions that describe transition pair interactions for unstable positions
        ///     (Auto-managed, no input support)
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IAsymmetricPairInteraction), IsAutoManaged = true)]
        public List<AsymmetricPairInteraction> UnstablePairInteractions { get; set; }

        /// <inheritdoc />
        public override IEnergyDataPort AsReadOnly()
        {
            return new EnergyDataManager(this);
        }

        /// <inheritdoc />
        public override void ResetToDefault()
        {
            StableEnvironmentInfo = StableEnvironmentInfo.CreateDefault();
            ResetAllIndexedData();
        }

        /// <summary>
        ///     Creates a new energy model data object with default settings
        /// </summary>
        /// <returns></returns>
        public static EnergyModelData CreateNew()
        {
            var data = new EnergyModelData();
            data.ResetToDefault();
            return data;
        }
    }
}