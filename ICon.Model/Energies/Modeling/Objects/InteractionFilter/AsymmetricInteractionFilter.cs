using System;
using Mocassin.Mathematics.Extensions;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Asymmetric pair interaction filter to customize ignored interaction in <see cref="IPairInteractionFinder" /> search
    ///     routines of asymmetric interactions
    /// </summary>
    public class AsymmetricInteractionFilter : IInteractionFilter, IEquatable<AsymmetricInteractionFilter>
    {
        /// <inheritdoc />
        public ICellReferencePosition CenterCellReferencePosition => null;

        /// <inheritdoc />
        [UseTrackedReferences]
        public ICellReferencePosition PartnerCellReferencePosition { get; set; }

        /// <inheritdoc />
        public double StartRadius { get; set; }

        /// <inheritdoc />
        public double EndRadius { get; set; }

        /// <inheritdoc />
        public bool IsApplicable(double distance, ICellReferencePosition centerCellReferencePosition, ICellReferencePosition partnerCellReferencePosition)
        {
            var result = partnerCellReferencePosition == PartnerCellReferencePosition;
            result &= distance > StartRadius && distance < EndRadius
                      || distance.AlmostEqualByRange(StartRadius)
                      || distance.AlmostEqualByRange(EndRadius);

            return result;
        }

        /// <inheritdoc />
        public bool IsApplicable(IPairInteraction pairInteraction)
        {
            if (!(pairInteraction is IAsymmetricPairInteraction asymmetricPair))
                return false;

            return IsApplicable(asymmetricPair.Distance, asymmetricPair.Position0, asymmetricPair.Position1);
        }

        /// <inheritdoc />
        public bool IsEqualFilter(IInteractionFilter other)
        {
            return Equals(FromInterface(other));
        }

        /// <inheritdoc />
        public bool Equals(AsymmetricInteractionFilter other)
        {
            return other != null
                   && PartnerCellReferencePosition == other.PartnerCellReferencePosition
                   && EndRadius.AlmostEqualByRange(other.EndRadius)
                   && StartRadius.AlmostEqualByRange(other.StartRadius);
        }

        /// <summary>
        ///     Creates new interaction filter that matches the passed filter interface
        /// </summary>
        /// <param name="interactionFilter"></param>
        /// <returns></returns>
        public static AsymmetricInteractionFilter FromInterface(IInteractionFilter interactionFilter)
        {
            if (interactionFilter == null) throw new ArgumentNullException(nameof(interactionFilter));
            return new AsymmetricInteractionFilter
            {
                StartRadius = interactionFilter.StartRadius,
                EndRadius = interactionFilter.EndRadius,
                PartnerCellReferencePosition = interactionFilter.PartnerCellReferencePosition
            };
        }
    }
}