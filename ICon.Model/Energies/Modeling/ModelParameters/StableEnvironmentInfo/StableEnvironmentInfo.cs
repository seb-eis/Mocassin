using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ICon.Mathematics.Extensions;
using ICon.Model.Basic;

namespace ICon.Model.Energies
{
    /// <inheritdoc cref="ICon.Model.Energies.IStableEnvironmentInfo"/>
    [DataContract(Name = "StableEnvironmentInfo")]
    public class StableEnvironmentInfo : ModelParameter, IStableEnvironmentInfo
    {
        /// <inheritdoc />
        [DataMember]
        public double MaxInteractionRange { get; set; }

        /// <summary>
        ///     The list of ignored pair interactions during environment sampling
        /// </summary>
        [DataMember]
        public List<SymmetricParticlePair> IgnoredPairInteractions { get; set; }

        /// <inheritdoc />
        public IEnumerable<SymmetricParticlePair> GetIgnoredPairs()
        {
            return IgnoredPairInteractions.AsEnumerable();
        }

        /// <inheritdoc />
        public override string GetParameterName()
        {
            return "'Stable Environment Info'";
        }

        /// <inheritdoc />
        public override ModelParameter PopulateObject(IModelParameter modelParameter)
        {
            if (!(modelParameter is IStableEnvironmentInfo info)) 
                return null;

            MaxInteractionRange = info.MaxInteractionRange;
            IgnoredPairInteractions = info.GetIgnoredPairs().ToList();
            return this;

        }

        /// <inheritdoc />
        public override bool Equals(IModelParameter other)
        {
            if (!(other is IStableEnvironmentInfo info))
                return false;

            return MaxInteractionRange.IsAlmostEqualByRange(info.MaxInteractionRange, 1.0e-10) 
                   && info.GetIgnoredPairs().All(pairCode => IgnoredPairInteractions.Contains(pairCode));
        }

        /// <summary>
        ///     Create a default environment info parameter (Range of one internal unit and no ignored interactions)
        /// </summary>
        /// <returns></returns>
        public static StableEnvironmentInfo CreateDefault()
        {
            return new StableEnvironmentInfo {IgnoredPairInteractions = new List<SymmetricParticlePair>(0), MaxInteractionRange = 1.0};
        }
    }
}