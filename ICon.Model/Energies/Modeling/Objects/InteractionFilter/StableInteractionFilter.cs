using System;
using Mocassin.Mathematics.Extensions;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Stable pair interaction filter to customize ignored interaction in <see cref="IPairInteractionFinder" /> search
    ///     routines of stable interactions
    /// </summary>
    public class StableInteractionFilter : IInteractionFilter, IEquatable<StableInteractionFilter>
    {
        /// <inheritdoc />
        [UseTrackedData]
        public ICellSite CenterCellSite { get; set; }

        /// <inheritdoc />
        [UseTrackedData]
        public ICellSite PartnerCellSite { get; set; }

        /// <inheritdoc />
        public double StartRadius { get; set; }

        /// <inheritdoc />
        public double EndRadius { get; set; }

        /// <inheritdoc />
        public bool Equals(StableInteractionFilter other) =>
            other != null
            && PartnerCellSite == other.PartnerCellSite
            && CenterCellSite == other.CenterCellSite
            && EndRadius.AlmostEqualByRange(other.EndRadius)
            && StartRadius.AlmostEqualByRange(other.StartRadius);

        /// <inheritdoc />
        public bool IsApplicable(double distance, ICellSite centerCellSite, ICellSite partnerCellSite)
        {
            var result = partnerCellSite == PartnerCellSite
                         && centerCellSite == CenterCellSite;

            result |= partnerCellSite == CenterCellSite
                      && centerCellSite == PartnerCellSite;

            result &= distance > StartRadius && distance < EndRadius
                      || distance.AlmostEqualByRange(StartRadius)
                      || distance.AlmostEqualByRange(EndRadius);

            return result;
        }

        /// <inheritdoc />
        public bool IsApplicable(IPairInteraction pairInteraction)
        {
            if (!(pairInteraction is IStablePairInteraction symmetricPair))
                return false;

            return IsApplicable(symmetricPair.Distance, symmetricPair.Position0, symmetricPair.Position1);
        }

        /// <inheritdoc />
        public bool IsEqualFilter(IInteractionFilter other) => Equals(FromInterface(other));

        /// <summary>
        ///     Creates new interaction filter that matches the passed filter interface
        /// </summary>
        /// <param name="interactionFilter"></param>
        /// <returns></returns>
        public static StableInteractionFilter FromInterface(IInteractionFilter interactionFilter)
        {
            if (interactionFilter == null) throw new ArgumentNullException(nameof(interactionFilter));
            return new StableInteractionFilter
            {
                StartRadius = interactionFilter.StartRadius,
                EndRadius = interactionFilter.EndRadius,
                PartnerCellSite = interactionFilter.PartnerCellSite,
                CenterCellSite = interactionFilter.CenterCellSite
            };
        }
    }
}