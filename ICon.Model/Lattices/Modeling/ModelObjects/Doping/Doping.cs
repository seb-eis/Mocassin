using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    /// Doping information that describes the element, concentration, sublattice which is substituted. 
    /// May also contain information about counter doping which is described in the same manner.
    /// </summary>
    [DataContract(Name = "Doping")]
    public class Doping : ModelObject, IDoping
    {
        /// <summary>
        /// Information about the doping (particles and sublattice)
        /// </summary>
        [DataMember]
        [IndexResolved]
        public IDopingCombination DopingInfo { set; get; }

        /// <summary>
        /// Information about the counter doping (particles and sublattice)
        /// </summary>
        [DataMember]
        [IndexResolved]
        public IDopingCombination CounterDopingInfo { set; get; }

        /// <summary>
        /// Counter doping multiplier
        /// </summary>
<<<<<<< HEAD
        [DataMember]
        public double CounterDopingMultiplier { get; set; }

        /// <summary>
        /// Flag that indicates whether the custom CounterDopingMultiplier is used
        /// </summary>
        [DataMember]
        public bool UseCustomMultiplier { get; set; }
=======
        /// <returns></returns>
        public override string GetObjectName()
        {
            return "Doping";
        }
>>>>>>> origin/s.eisele@dev

        /// <summary>
        /// Flag to indicate whether a counter doping should be applied
        /// </summary>
        [DataMember]
        public bool UseCounterDoping { get; set; }

        /// <summary>
        /// Doping Group for simutaneous doping
        /// </summary>
        [DataMember]
        public int DopingGroup { get; set; }

        /// <summary>
        /// Get the type name string
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
<<<<<<< HEAD
            return "'Doping'";
=======
            return $"{GetObjectName()} ({DopingInfo.ToString()}, {CounterDopingInfo.ToString()})";
>>>>>>> origin/s.eisele@dev
        }

        /// <summary>
        /// Copies the information from the provided model object interface and returns the object (Retruns null if type mismatch)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastIfNotDeprecated<IDoping>(obj) is var doping)
            {
                DopingInfo = doping.DopingInfo;
                CounterDopingInfo = doping.CounterDopingInfo;
                CounterDopingMultiplier = doping.CounterDopingMultiplier;
                UseCustomMultiplier = doping.UseCustomMultiplier;
                UseCounterDoping = doping.UseCounterDoping;
                DopingGroup = doping.DopingGroup;
                return this;
            }
            return null;
        }
    }
}
