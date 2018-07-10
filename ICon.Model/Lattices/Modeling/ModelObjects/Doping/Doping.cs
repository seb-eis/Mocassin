using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ICon.Model.Basic;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Doping information that describes the element, concentration, sublattice which is substituted. 
    /// May also contain information about counter doping which is described in the same manner.
    /// </summary>
    [DataContract(Name = "Doping")]
    public class Doping : ModelObject, IDoping
    {
        /// <summary>
        /// Specifies the doping concentration
        /// </summary>
        [DataMember]
        public double Concentration { set; get; }

        /// <summary>
        /// Information about the doping (particles and sublattice)
        /// </summary>
        [DataMember]
        [IndexResolvable]
        public IDopingCombination DopingInfo { set; get; }

        /// <summary>
        /// Information about the counter doping (particles and sublattice)
        /// </summary>
        [DataMember]
        [IndexResolvable]
        public IDopingCombination CounterDopingInfo { set; get; }

        /// <summary>
        /// Counter doping multiplier
        /// </summary>
        [DataMember]
        [IndexResolvable]
        public double CounterDopingMultiplier { get; set; }

        /// <summary>
        /// Flag that indicates whether the custom CounterDopingMultiplier is used
        /// </summary>
        [DataMember]
        [IndexResolvable]
        public bool UseCustomMultiplier { get; set; }

        /// <summary>
        /// Flag to indicate whether a counter doping should be applied
        /// </summary>
        [DataMember]
        [IndexResolvable]
        public bool UseCounterDoping { get; set; }

        /// <summary>
        /// Doping Group for simutaneous doping
        /// </summary>
        [DataMember]
        [IndexResolvable]
        public int DopingGroup { get; set; }

        /// <summary>
        /// Get the type name string
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Doping'";
        }

        /// <summary>
        /// Copies the information from the provided model object interface and returns the object (Retruns null if type mismatch)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateObject(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IDoping>(obj) is var doping)
            {
                Concentration = doping.Concentration;
                DopingInfo = doping.DopingInfo;
                CounterDopingInfo = doping.CounterDopingInfo;
                return this;
            }
            return null;
        }
    }
}
