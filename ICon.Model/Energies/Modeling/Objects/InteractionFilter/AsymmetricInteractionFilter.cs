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
        public IUnitCellPosition CenterUnitCellPosition => null;

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
            var result = partnerUnitCellPosition == PartnerUnitCellPosition;
            result &= distance > StartRadius && distance < EndRadius
                      || distance.IsAlmostEqualByRange(StartRadius)
                      || distance.IsAlmostEqualByRange(EndRadius);

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
                   && PartnerUnitCellPosition == other.PartnerUnitCellPosition
                   && EndRadius.IsAlmostEqualByRange(other.EndRadius)
                   && StartRadius.IsAlmostEqualByRange(other.StartRadius);
        }

        /// <summary>
        ///     Creates new interaction filter that matches the passed filter interface
        /// </summary>
        /// <param name="interactionFilter"></param>
        /// <returns></returns>
        public static AsymmetricInteractionFilter FromInterface(IInteractionFilter interactionFilter)
        {
            return new AsymmetricInteractionFilter
            {
                StartRadius = interactionFilter.StartRadius,
                EndRadius = interactionFilter.EndRadius,
                PartnerUnitCellPosition = interactionFilter.PartnerUnitCellPosition
            };
        }
    }
}