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
        /// Specifies the doping concentration
        /// </summary>
        [DataMember]
        public double Concentration { set; get; }

        /// <summary>
        /// Information about the doping (particles and sublattice)
        /// </summary>
        [DataMember]
        [UseTrackedReferences]
        public IDopingCombination DopingInfo { set; get; }

        /// <summary>
        /// Information about the counter doping (particles and sublattice)
        /// </summary>
        [DataMember]
        [UseTrackedReferences]
        public IDopingCombination CounterDopingInfo { set; get; }

        /// <summary>
        /// Get the type name string
        /// </summary>
        /// <returns></returns>
        public override string GetObjectName()
        {
            return "Doping";
        }

        /// <summary>
        /// creates a string that contains the model object information
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{GetObjectName()} ({DopingInfo.ToString()}, {CounterDopingInfo.ToString()})";
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
                Concentration = doping.Concentration;
                DopingInfo = doping.DopingInfo;
                CounterDopingInfo = doping.CounterDopingInfo;
                return this;
            }
            return null;
        }
    }
}
