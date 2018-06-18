using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ICon.Model.Basic;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// The reference model data object for the transition manager
    /// </summary>
    [Serializable]
    [DataContract]
    public class TransitionModelData : ModelData<ITransitionDataPort>
    {
        /// <summary>
        /// The list of property state pairs
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IPropertyStatePair))]
        public List<PropertyStatePair> PropertyStatePairs { get; set; }

        /// <summary>
        /// The list of property groups
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IPropertyGroup))]
        public List<PropertyGroup> PropertyGroups { get; set; }

        /// <summary>
        /// The list of existing abstract transitions for KMC and MMC
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IAbstractTransition))]
        public List<AbstractTransition> AbstractTransitions { get; set; }

        /// <summary>
        /// The list of kinetic model transitions
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IKineticTransition))]
        public List<KineticTransition> KineticTransitions { get; set; }

        /// <summary>
        /// The list of metropolis model transitions
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IMetropolisTransition))]
        public List<MetropolisTransition> MetropolisTransitions { get; set; }

        /// <summary>
        /// The list of metropolis transition rules (Automanaged by model)
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IMetropolisRule), IsAutoManaged =true)]
        public List<MetropolisRule> MetropolisRules { get; set; }

        /// <summary>
        /// The list of kinetic transition rules (Automanaged by model)
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IKineticRule), IsAutoManaged =true)]
        public List<KineticRule> KineticRules { get; set; }

        /// <summary>
        /// Get a new read only access port for this data object
        /// </summary>
        /// <returns></returns>
        public override ITransitionDataPort AsReadOnly()
        {
            return new TransitionDataManager(this);
        }

        /// <summary>
        /// Reset the data object to default values
        /// </summary>
        public override void ResetToDefault()
        {
            ResetAllIndexedData();
        }

        /// <summary>
        /// Creates new transition model data with default settings
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
