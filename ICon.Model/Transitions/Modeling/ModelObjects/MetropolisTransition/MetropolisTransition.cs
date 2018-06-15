using System;
using System.Collections.Generic;
using System.Text;
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
        [IndexResolvable]
        public IUnitCellPosition CellPosition0 { get; set; }

        /// <summary>
        /// The unit cell position of the second sub-lattice
        /// </summary>
        [DataMember]
        [IndexResolvable]
        public IUnitCellPosition CellPosition1 { get; set; }

        /// <summary>
        /// Checks for equality to another metropolis transition (Also checks inverted case)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IMetropolisTransition other)
        {
            if (CellPosition0.Index == other.CellPosition0.Index && CellPosition1.Index == other.CellPosition1.Index)
            {
                return true;
            }
            return CellPosition0.Index == other.CellPosition1.Index && CellPosition1.Index == other.CellPosition0.Index;
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
        public override ModelObject PopulateObject(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IMetropolisTransition>(obj) is var transition)
            {
                CellPosition0 = transition.CellPosition0;
                CellPosition1 = transition.CellPosition1;
                return this;
            }
            return null;
        }
    }
}
