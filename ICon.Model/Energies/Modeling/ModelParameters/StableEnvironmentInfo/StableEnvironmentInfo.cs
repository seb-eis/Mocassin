using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ICon.Framework.Collections;
using ICon.Model.Basic;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Environement info parameter that defines the basic restrictions and behavior for all stable position environments
    /// </summary>
    [DataContract(Name ="StableEnvironmentInfo")]
    public class StableEnvironmentInfo : ModelParameter, IStableEnvironmentInfo
    {
        /// <summary>
        /// The maximum interaction range in internal unit
        /// </summary>
        [DataMember]
        public double MaxInteractionRange { get; set; }

        /// <summary>
        /// The list of ignored pair interactions during environment sampling
        /// </summary>
        [DataMember]
        public List<SymmetricParticlePair> IgnoredPairInteractions { get; set; }

        /// <summary>
        /// Get the ignroed pair interactions as an enumerable
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SymmetricParticlePair> GetIgnoredPairs()
        {
            return IgnoredPairInteractions.AsEnumerable();
        }

        /// <summary>
        /// Get the parameter literal name
        /// </summary>
        /// <returns></returns>
        public override string GetParameterName()
        {
            return "'Stable Environment Info'";
        }

        /// <summary>
        /// Consumes a model parameter interface and returns this object on success. Returns null if the consume failed
        /// </summary>
        /// <param name="modelParameter"></param>
        /// <returns></returns>
        public override ModelParameter PopulateObject(IModelParameter modelParameter)
        {
            if (modelParameter is IStableEnvironmentInfo info)
            {
                MaxInteractionRange = info.MaxInteractionRange;
                IgnoredPairInteractions = info.GetIgnoredPairs().ToList();
                return this;
            }
            return null;
        }

        /// <summary>
        /// Checks parameter for content equility to another model parameter interface
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(IModelParameter other)
        {
            if (other is IStableEnvironmentInfo info)
            {
                if (MaxInteractionRange == info.MaxInteractionRange)
                {
                    foreach (var pairCode in info.GetIgnoredPairs())
                    {
                        if (!IgnoredPairInteractions.Contains(pairCode))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Create a default environment info parameter (Range of one internal unit and no ignored interactions)
        /// </summary>
        /// <returns></returns>
        public static StableEnvironmentInfo CreateDefault()
        {
            return new StableEnvironmentInfo() { IgnoredPairInteractions = new List<SymmetricParticlePair>(0), MaxInteractionRange = 1.0 };
        }
    }
}
