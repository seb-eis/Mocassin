using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     The reference model data object for the transition manager
    /// </summary>
    [Serializable]
    [DataContract]
    public class TransitionModelData : ModelData<ITransitionDataPort>
    {
        /// <summary>
        ///     The list of state exchange pairs
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IStateExchangePair))]
        public List<StateExchangePair> StateExchangePairs { get; set; }

        /// <summary>
        ///     The list of state exchange groups
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IStateExchangeGroup))]
        public List<StateExchangeGroup> StateExchangeGroups { get; set; }

        /// <summary>
        ///     The list of existing abstract transitions for KMC and MMC
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IAbstractTransition))]
        public List<AbstractTransition> AbstractTransitions { get; set; }

        /// <summary>
        ///     The list of kinetic model transitions
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IKineticTransition))]
        public List<KineticTransition> KineticTransitions { get; set; }

        /// <summary>
        ///     The list of metropolis model transitions
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IMetropolisTransition))]
        public List<MetropolisTransition> MetropolisTransitions { get; set; }

        /// <summary>
        ///     The list of metropolis transition rules (Auto-managed by model)
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IMetropolisRule), IsAutoManaged = true)]
        public List<MetropolisRule> MetropolisRules { get; set; }

        /// <summary>
        ///     The list of kinetic transition rules (Auto-managed by model)
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IKineticRule), IsAutoManaged = true)]
        public List<KineticRule> KineticRules { get; set; }

        /// <inheritdoc />
        public override ITransitionDataPort AsReadOnly()
        {
            return new TransitionDataManager(this);
        }

        /// <inheritdoc />
        public override void ResetToDefault()
        {
            ResetAllIndexedData();
        }

        /// <summary>
        ///     Creates new transition model data with default settings
        /// </summary>
        /// <returns></returns>
        public static TransitionModelData CreateNew()
        {
            var data = new TransitionModelData();
            data.ResetToDefault();
            return data;
        }
    }
}