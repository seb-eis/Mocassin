using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Mocassin.Mathematics.Extensions;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <inheritdoc cref="IStableEnvironmentInfo"/>
    [DataContract(Name = "StableEnvironmentInfo")]
    public class StableEnvironmentInfo : ModelParameter, IStableEnvironmentInfo
    {
        /// <inheritdoc />
        [DataMember]
        public double MaxInteractionRange { get; set; }

        //ToDo: Change the ignore system to enable (Wyckoff 0, Wyckoff 1, RadiusMin, RadiusMax) ignore definitions
        /// <summary>
        ///     The list of ignored pair interactions during environment sampling
        /// </summary>
        [DataMember]
        [UseTrackedReferences(ReferenceLevel = ReferenceLevel.Content)]
        public List<SymmetricParticlePair> IgnoredPairInteractions { get; set; }

        /// <inheritdoc />
        public IEnumerable<SymmetricParticlePair> GetIgnoredPairs()
        {
            return IgnoredPairInteractions.AsEnumerable();
        }

        /// <inheritdoc />
        public override string GetParameterName()
        {
            return "Stable Environment Info";
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