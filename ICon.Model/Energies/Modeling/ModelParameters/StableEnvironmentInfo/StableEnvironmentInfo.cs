using System.Collections.Generic;
using System.Linq;
using Mocassin.Mathematics.Extensions;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <inheritdoc cref="IStableEnvironmentInfo" />
    public class StableEnvironmentInfo : ModelParameter, IStableEnvironmentInfo
    {
        /// <inheritdoc />
        public double MaxInteractionRange { get; set; }

        /// <summary>
        ///     The list of interaction filters for stable environments
        /// </summary>
        [UseTrackedData(ReferenceCorrectionLevel = ReferenceCorrectionLevel.IgnoreTopLevel)]
        public List<StableInteractionFilter> InteractionFilters { get; set; }

        /// <summary>
        ///     Get or set the list of defect energy entries
        /// </summary>
        [UseTrackedData(ReferenceCorrectionLevel = ReferenceCorrectionLevel.IgnoreTopLevel)]
        public List<DefectEnergy> DefectEnergies { get; set; }

        /// <inheritdoc />
        public IEnumerable<IInteractionFilter> GetInteractionFilters() => (InteractionFilters ?? new List<StableInteractionFilter>()).AsEnumerable();

        /// <inheritdoc />
        public IEnumerable<IDefectEnergy> GetDefectEnergies() => (DefectEnergies ?? new List<DefectEnergy>()).AsEnumerable();

        /// <inheritdoc />
        public override string GetParameterName() => "Stable Environment Info";

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

            foreach (var defectEnergy in DefectEnergies)
            {
                if (!info.GetDefectEnergies().Contains(defectEnergy))
                    return false;
            }

            return MaxInteractionRange.AlmostEqualByRange(info.MaxInteractionRange);
        }

        /// <inheritdoc />
        public override ModelParameter PopulateObject(IModelParameter modelParameter)
        {
            if (!(modelParameter is IStableEnvironmentInfo info))
                return null;

            MaxInteractionRange = info.MaxInteractionRange;
            InteractionFilters = info.GetInteractionFilters().Select(StableInteractionFilter.FromInterface).ToList();
            DefectEnergies = info.GetDefectEnergies().Select(DefectEnergy.FromInterface).ToList();
            return this;
        }

        /// <summary>
        ///     Create a default environment info parameter (Range of one internal unit and no interaction filters)
        /// </summary>
        /// <returns></returns>
        public static StableEnvironmentInfo CreateDefault() =>
            new StableEnvironmentInfo
            {
                InteractionFilters = new List<StableInteractionFilter>(0),
                DefectEnergies = new List<DefectEnergy>(0),
                MaxInteractionRange = 1.0
            };
    }
}