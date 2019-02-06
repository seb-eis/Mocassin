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

        /// <summary>
        ///     The list of interaction filters for stable environments
        /// </summary>
        [DataMember]
        [UseTrackedReferences(ReferenceLevel = ReferenceLevel.Content)]
        public List<SymmetricInteractionFilter> InteractionFilters { get; set; }

        /// <inheritdoc />
        public IEnumerable<IInteractionFilter> GetInteractionFilters()
        {
            return (InteractionFilters ?? new List<SymmetricInteractionFilter>()).AsEnumerable();
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
            InteractionFilters = info.GetInteractionFilters().Select(SymmetricInteractionFilter.FromInterface).ToList();
            return this;

        }

        /// <inheritdoc />
        public override bool Equals(IModelParameter other)
        {
            if (!(other is IStableEnvironmentInfo info))
                return false;

            foreach (var interactionFilter in InteractionFilters)
            {
                if (!info.GetInteractionFilters().Contains(interactionFilter))
                    return false;
            }
            return MaxInteractionRange.IsAlmostEqualByRange(info.MaxInteractionRange);
        }

        /// <summary>
        ///     Create a default environment info parameter (Range of one internal unit and no interaction filters)
        /// </summary>
        /// <returns></returns>
        public static StableEnvironmentInfo CreateDefault()
        {
            return new StableEnvironmentInfo {InteractionFilters = new List<SymmetricInteractionFilter>(0), MaxInteractionRange = 1.0};
        }
    }
}