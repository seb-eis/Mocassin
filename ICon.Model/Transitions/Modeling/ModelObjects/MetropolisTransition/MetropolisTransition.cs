using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using ICon.Model.Basic;
using ICon.Model.Structures;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// BAsic implementation of the metropolis transition (Simply defined by two exchanging sub-lattices)
    /// </summary>
    [Serializable]
    [DataContract(Name ="MetropolisTransition")]
    public class MetropolisTransition : ModelObject, IMetropolisTransition
    {
        /// <summary>
        /// The unit cell position of the first sub-lattice
        /// </summary>
        [DataMember]
        [LinkableByIndex]
        public IUnitCellPosition FirstUnitCellPosition { get; set; }

        /// <summary>
        /// The unit cell position of the second sub-lattice
        /// </summary>
        [DataMember]
        [LinkableByIndex]
        public IUnitCellPosition SecondUnitCellPosition { get; set; }

        /// <summary>
        /// The affiliated abstract transition
        /// </summary>
        [DataMember]
        [LinkableByIndex]
        public IAbstractTransition AbstractTransition { get; set; }

        /// <summary>
        /// The list of affiliated metropolis transition rules (Automanaged by model system)
        /// </summary>
        [DataMember]
        [LinkableByIndex]
        public List<MetropolisRule> TransitionRules { get; set; }
       
        /// <summary>
        /// Get the affiliated transition rules as an enumerable sequence
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMetropolisRule> GetTransitionRules()
        {
            return (TransitionRules ?? new List<MetropolisRule>()).AsEnumerable();
        }

        /// <summary>
        /// Checks for equality to another metropolis transition (Also checks inverted case)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IMetropolisTransition other)
        {
            if (FirstUnitCellPosition.Index == other.FirstUnitCellPosition.Index && SecondUnitCellPosition.Index == other.SecondUnitCellPosition.Index)
            {
                return AbstractTransition == other.AbstractTransition;
            }
            return FirstUnitCellPosition.Index == other.SecondUnitCellPosition.Index
                && SecondUnitCellPosition.Index == other.FirstUnitCellPosition.Index
                && AbstractTransition == other.AbstractTransition;
        }

        /// <summary>
        /// Ge the type name of the model object
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Metropolis Transition'";
        }

        /// <summary>
        /// Tries to create new metropolis transition object from model object interface (Returns null if wrong type or deprecated)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IMetropolisTransition>(obj) is var transition)
            {
                FirstUnitCellPosition = transition.FirstUnitCellPosition;
                SecondUnitCellPosition = transition.SecondUnitCellPosition;
                AbstractTransition = transition.AbstractTransition;
                return this;
            }
            return null;
        }
    }
}
