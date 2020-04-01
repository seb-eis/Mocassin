using System;
using Mocassin.Mathematics.Extensions;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Unstable pair interaction filter to customize ignored interaction in <see cref="IPairInteractionFinder" /> search routines of unstable interactions
    /// </summary>
    public class UnstableInteractionFilter : IInteractionFilter, IEquatable<UnstableInteractionFilter>
    {
        /// <inheritdoc />
        public ICellSite CenterCellSite => null;

        /// <inheritdoc />
        [UseTrackedData]
        public ICellSite PartnerCellSite { get; set; }

        /// <inheritdoc />
        public double StartRadius { get; set; }

        /// <inheritdoc />
        public double EndRadius { get; set; }

        /// <inheritdoc />
        public bool Equals(UnstableInteractionFilter other)
        {
            return other != null
                   && PartnerCellSite == other.PartnerCellSite
                   && EndRadius.AlmostEqualByRange(other.EndRadius)
                   && StartRadius.AlmostEqualByRange(other.StartRadius);
        }

        /// <inheritdoc />
        public bool IsApplicable(double distance, ICellSite centerCellSite, ICellSite partnerCellSite)
        {
            var result = partnerCellSite == PartnerCellSite;
            result &= distance > StartRadius && distance < EndRadius
                      || distance.AlmostEqualByRange(StartRadius)
                      || distance.AlmostEqualByRange(EndRadius);

            return result;
        }

        /// <inheritdoc />
        public bool IsApplicable(IPairInteraction pairInteraction)
        {
            if (!(pairInteraction is IUnstablePairInteraction asymmetricPair))
                return false;

            return IsApplicable(asymmetricPair.Distance, asymmetricPair.Position0, asymmetricPair.Position1);
        }

        /// <inheritdoc />
        public bool IsEqualFilter(IInteractionFilter other)
        {
            return Equals(FromInterface(other));
        }

        /// <summary>
        ///     Creates new interaction filter that matches the passed filter interface
        /// </summary>
        /// <param name="interactionFilter"></param>
        /// <returns></returns>
        public static UnstableInteractionFilter FromInterface(IInteractionFilter interactionFilter)
        {
            if (interactionFilter == null) throw new ArgumentNullException(nameof(interactionFilter));
            return new UnstableInteractionFilter
            {
                StartRadius = interactionFilter.StartRadius,
                EndRadius = interactionFilter.EndRadius,
                PartnerCellSite = interactionFilter.PartnerCellSite
            };
        }
    }
}