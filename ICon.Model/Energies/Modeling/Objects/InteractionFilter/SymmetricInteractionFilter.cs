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
        [UseTrackedReferences]
        public IUnitCellPosition CenterUnitCellPosition { get; set; }

        /// <inheritdoc />
        [UseTrackedReferences]
        public IUnitCellPosition PartnerUnitCellPosition { get; set; }

        /// <inheritdoc />
        public double StartRadius { get; set; }

        /// <inheritdoc />
        public double EndRadius { get; set; }

        /// <inheritdoc />
        public bool IsApplicable(double distance, IUnitCellPosition centerUnitCellPosition, IUnitCellPosition partnerUnitCellPosition)
        {
            var result = partnerUnitCellPosition == PartnerUnitCellPosition 
                         && centerUnitCellPosition == CenterUnitCellPosition;

            result |= partnerUnitCellPosition == CenterUnitCellPosition 
                      && centerUnitCellPosition == PartnerUnitCellPosition;

            result &= distance > StartRadius && distance < EndRadius 
                      || distance.IsAlmostEqualByRange(StartRadius)
                      || distance.IsAlmostEqualByRange(EndRadius);

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
                   && PartnerUnitCellPosition == other.PartnerUnitCellPosition
                   && CenterUnitCellPosition == other.CenterUnitCellPosition
                   && EndRadius.IsAlmostEqualByRange(other.EndRadius)
                   && StartRadius.IsAlmostEqualByRange(other.StartRadius);
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
                PartnerUnitCellPosition = interactionFilter.PartnerUnitCellPosition,
                CenterUnitCellPosition = interactionFilter.CenterUnitCellPosition
            };
        }
    }
}