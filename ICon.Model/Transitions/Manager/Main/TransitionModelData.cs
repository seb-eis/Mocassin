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
    [DataContract(Name ="TransitionData")]
    public class TransitionModelData : ModelData<ITransitionDataPort>
    {
        /// <summary>
        /// The list of property state pairs
        /// </summary>
        [DataMember(Name ="StatePairs")]
        [IndexedModelData(typeof(IPropertyStatePair))]
        public List<PropertyStatePair> PropertyStatePairs { get; set; }

        /// <summary>
        /// The list of property groups
        /// </summary>
        [DataMember(Name ="PropertyGroups")]
        [IndexedModelData(typeof(IPropertyGroup))]
        public List<PropertyGroup> PropertyGroups { get; set; }

        /// <summary>
        /// The list of abstract kinetic transitions
        /// </summary>
        [DataMember(Name ="KineticAbstracts")]
        [IndexedModelData(typeof(IAbstractTransition))]
        public List<AbstractTransition> AbstractKineticTransitions { get; set; }

        /// <summary>
        /// The list of kinetic model transitions
        /// </summary>
        [DataMember(Name ="KineticTransitions")]
        [IndexedModelData(typeof(IKineticTransition))]
        public List<KineticTransition> KineticTransitions { get; set; }

        /// <summary>
        /// The list of metropolis model transitions
        /// </summary>
        [DataMember(Name ="MetropolisTransitions")]
        [IndexedModelData(typeof(IMetropolisTransition))]
        public List<MetropolisTransition> MetropolisTransitions { get; set; }

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
