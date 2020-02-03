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
    public class SymmetricInteractionFilter : IInteractionFilter, IEquatable<SymmetricInteractionFilter>
    {
        /// <inheritdoc />
        [UseTrackedData]
        public ICellReferencePosition CenterCellReferencePosition { get; set; }

        /// <inheritdoc />
        [UseTrackedData]
        public ICellReferencePosition PartnerCellReferencePosition { get; set; }

        /// <inheritdoc />
        public double StartRadius { get; set; }

        /// <inheritdoc />
        public double EndRadius { get; set; }

        /// <inheritdoc />
        public bool IsApplicable(double distance, ICellReferencePosition centerCellReferencePosition, ICellReferencePosition partnerCellReferencePosition)
        {
            var result = partnerCellReferencePosition == PartnerCellReferencePosition 
                         && centerCellReferencePosition == CenterCellReferencePosition;

            result |= partnerCellReferencePosition == CenterCellReferencePosition 
                      && centerCellReferencePosition == PartnerCellReferencePosition;

            result &= distance > StartRadius && distance < EndRadius 
                      || distance.AlmostEqualByRange(StartRadius)
                      || distance.AlmostEqualByRange(EndRadius);

            return result;
        }

        /// <inheritdoc />
        public bool IsApplicable(IPairInteraction pairInteraction)
        {
            if (!(pairInteraction is ISymmetricPairInteraction symmetricPair))
                return false;

            return IsApplicable(symmetricPair.Distance, symmetricPair.Position0, symmetricPair.Position1);
        }

        /// <inheritdoc />
        public bool IsEqualFilter(IInteractionFilter other)
        {
            return Equals(FromInterface(other));
        }

        /// <inheritdoc />
        public bool Equals(SymmetricInteractionFilter other)
        {
            return other != null
                   && PartnerCellReferencePosition == other.PartnerCellReferencePosition
                   && CenterCellReferencePosition == other.CenterCellReferencePosition
                   && EndRadius.AlmostEqualByRange(other.EndRadius)
                   && StartRadius.AlmostEqualByRange(other.StartRadius);
        }

        /// <summary>
        ///     Creates new interaction filter that matches the passed filter interface
        /// </summary>
        /// <param name="interactionFilter"></param>
        /// <returns></returns>
        public static SymmetricInteractionFilter FromInterface(IInteractionFilter interactionFilter)
        {
            if (interactionFilter == null) throw new ArgumentNullException(nameof(interactionFilter));
            return new SymmetricInteractionFilter
            {
                StartRadius = interactionFilter.StartRadius,
                EndRadius = interactionFilter.EndRadius,
                PartnerCellReferencePosition = interactionFilter.PartnerCellReferencePosition,
                CenterCellReferencePosition = interactionFilter.CenterCellReferencePosition
            };
        }
    }
}