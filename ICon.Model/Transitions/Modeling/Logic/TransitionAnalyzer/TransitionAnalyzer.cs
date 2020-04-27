using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Bundles analysis functions for both kinetic and metropolis transitions
    /// </summary>
    public class TransitionAnalyzer
    {
        /// <summary>
        ///     Checks if a transition geometry described by 4D vectors contains a cycle or ring transition
        /// </summary>
        /// <param name="geometryVectors"></param>
        /// <returns></returns>
        public bool ContainsRingTransition(IEnumerable<Vector4I> geometryVectors)
        {
            var positionList = geometryVectors.AsList();
            for (var i = 0; i < positionList.Count; i++)
            {
                for (var j = i + 1; j < positionList.Count; j++)
                {
                    if (positionList[i].Equals(positionList[j]))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Checks if a sequence of position vectors describe or contain a ring transition with the provided vector comparer
        /// </summary>
        /// <param name="positionGeometry"></param>
        /// <param name="equalityComparer"></param>
        /// <returns></returns>
        public bool ContainsRingTransition(IEnumerable<Fractional3D> positionGeometry, IComparer<Fractional3D> equalityComparer)
        {
            var positionList = positionGeometry.AsList();
            for (var i = 0; i < positionList.Count; i++)
            {
                for (var j = i + 1; j < positionList.Count; j++)
                {
                    if (equalityComparer.Compare(positionList[i], positionList[j]) == 0)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Takes a set of fractional reference positions and creates a 2D map of all symmetry equivalent intermediate
        ///     positions
        ///     between each two consecutive vectors
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="spaceGroupService"></param>
        /// <returns></returns>
        public IList<SetList<Fractional3D>> GetEquivalentIntermediatePositions(IEnumerable<Fractional3D> geometry,
            ISpaceGroupService spaceGroupService)
        {
            return geometry
                .SelectConsecutivePairs((first, second) => Fractional3D.CalculateMiddle(second, first))
                .Select(spaceGroupService.GetUnitCellP1PositionExtension)
                .ToList();
        }

        /// <summary>
        ///     Check if two rules form a symmetric rule pair meaning one rule is exactly the other but for the inverted case of
        ///     the underlying abstract transition
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <remarks> Rules are symmetric when the abstract is symmetric and the following is true: S_0 == E_1 and S_1 == E_0 </remarks>
        /// <returns></returns>
        public bool IsSymmetricRulePair(ITransitionRule lhs, ITransitionRule rhs)
        {
            if (lhs.MovementFlags != rhs.MovementFlags) return false;
            return lhs.GetStartStateOccupation().Select(a => a.Index).SequenceEqual(rhs.GetFinalStateOccupation().Select(a => a.Index))
                   && lhs.GetFinalStateOccupation().Select(a => a.Index).SequenceEqual(rhs.GetStartStateOccupation().Select(a => a.Index));
        }

        /// <summary>
        ///     Check if two rules describe a back-jump rule pair meaning that invoking one rule can cancel the invocation of the
        ///     other out
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <remarks> Rules form back-jump pair when the following is true. S_0 == S_1.Reverse and E_0 == E_1.Reverse </remarks>
        /// <returns></returns>
        public bool IsBackjumpRulePair(ITransitionRule lhs, ITransitionRule rhs)
        {
            if (lhs.MovementFlags != rhs.MovementFlags)
                return false;

            return lhs.GetStartStateOccupation().Select(a => a.Index)
                       .SequenceEqual(rhs.GetStartStateOccupation().Reverse().Select(a => a.Index))
                   && lhs.GetFinalStateOccupation().Select(a => a.Index)
                       .SequenceEqual(rhs.GetFinalStateOccupation().Reverse().Select(a => a.Index));
        }

        /// <summary>
        ///     Check if the rule pair form a twisted rule pair where they describe the twisted (Both states reversed) version of
        ///     the symmetric rule pair
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public bool IsTwistedRulePair(ITransitionRule lhs, ITransitionRule rhs)
        {
            if (lhs.MovementFlags != rhs.MovementFlags)
                return false;

            return lhs.GetStartStateOccupation().Select(a => a.Index)
                       .SequenceEqual(rhs.GetFinalStateOccupation().Reverse().Select(a => a.Index))
                   && lhs.GetFinalStateOccupation().Select(a => a.Index)
                       .SequenceEqual(rhs.GetStartStateOccupation().Reverse().Select(a => a.Index));
        }

        /// <summary>
        ///     Checks if the passed abstract transition is symmetric meaning reading the transition in reverse does not change its
        ///     meaning
        /// </summary>
        /// <param name="abstractTransition"></param>
        /// <returns></returns>
        public bool IsSymmetricTransition(IAbstractTransition abstractTransition)
        {
            return abstractTransition.GetStateExchangeGroups().SequenceEqual(abstractTransition.GetStateExchangeGroups().Reverse());
        }

        /// <summary>
        ///     Calculates the abstract transitions charge transport chain that assigns each state position a charge change value
        ///     form donor to acceptor state,
        ///     or NaN if a position has multiple possible values
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="comparer"></param>
        /// <remarks> Conductivity calculations based on the tracker property alone require this chain to have no Nan values </remarks>
        /// <returns></returns>
        public IList<double> GetChargeTransportChain(IAbstractTransition transition, IComparer<double> comparer)
        {
            var transportChain = new List<double>(transition.StateCount);
            foreach (var propertyGroup in transition.GetStateExchangeGroups())
            {
                var chargeTransport = GetStatePairChargeTransport(propertyGroup.GetStateExchangePairs().First());

                foreach (var statePair in propertyGroup.GetStateExchangePairs().Skip(1))
                {
                    chargeTransport = comparer.Compare(chargeTransport, GetStatePairChargeTransport(statePair)) == 0
                        ? chargeTransport
                        : double.NaN;
                }

                transportChain.Add(chargeTransport);
            }

            return transportChain;
        }

        /// <summary>
        ///     Calculates the charge transport value of a single state exchange pair
        /// </summary>
        /// <param name="statePair"></param>
        /// <returns></returns>
        public double GetStatePairChargeTransport(IStateExchangePair statePair)
        {
            return statePair.DonorParticle.Charge - statePair.AcceptorParticle.Charge;
        }
    }
}