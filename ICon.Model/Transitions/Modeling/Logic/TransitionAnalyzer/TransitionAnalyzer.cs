using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Mathematics.ValueTypes;
using ICon.Framework.Operations;
using ICon.Framework.Collections;
using ICon.Symmetry.SpaceGroups;
using ICon.Framework.Extensions;

using ICon.Model.Basic;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Bundles analysis functions for both kinetic and metropolis transitions
    /// </summary>
    public class TransitionAnalyzer
    {
        /// <summary>
        /// Checks if a transition geometry described by 4D vectors contains a cycle or ring transition
        /// </summary>
        /// <param name="geometryVectors"></param>
        /// <returns></returns>
        public bool ContainsRingTransition(IEnumerable<CrystalVector4D> geometryVectors)
        {
            CrystalVector4D relative = new CrystalVector4D(0, 0, 0, 0), last = new CrystalVector4D(0, 0, 0, 0);
            foreach (var vector in geometryVectors)
            {
                relative += vector - last;
                if (relative.Equals(CrystalVector4D.NullVector))
                {
                    return true;
                }
                last = vector;
            }
            return false;
        }

        /// <summary>
        /// Cehcks if a seqeunce of position vectors describe or contain a ring transition with the provided vector comparer
        /// </summary>
        /// <param name="positionGeometry"></param>
        /// <param name="equalityComparer"></param>
        /// <returns></returns>
        public bool ContainsRingTransition(IEnumerable<Fractional3D> positionGeometry, IComparer<Fractional3D> equalityComparer)
        {
            var current = new Fractional3D(0, 0, 0);
            foreach (var vector in positionGeometry.ConsecutivePairSelect((a, b) => b - a))
            {
                current += vector;
                if (equalityComparer.Compare(current, Fractional3D.NullVector) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Takes a set of fractional refernce positions and creates a 2D map of all symmetry equivalent intermediate positions between each two consecutive vectors
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="spaceGroupService"></param>
        /// <returns></returns>
        public IList<SetList<Fractional3D>> GetEquivalentIntermediatePositions(IEnumerable<Fractional3D> geometry, ISpaceGroupService spaceGroupService)
        {
            var result = new List<SetList<Fractional3D>>();
            foreach (var item in geometry.ConsecutivePairSelect((first, second) => Fractional3D.GetMiddle(second, first)))
            {
                result.Add(spaceGroupService.GetAllWyckoffPositions(item));
            }
            return result;
        }

        /// <summary>
        /// Check if two rules form a symmetric rule pair meaning one rule is exactly the other but for the inverted case of the underlying abstract transition
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <remarks> Rules are symmetric when the abstract is symmtric and the following is true: S_0 == E_1 and S_1 == E_0 </remarks>
        /// <returns></returns>
        public bool IsSymmetricRulePair(ITransitionRule lhs, ITransitionRule rhs)
        {
            if (lhs.MovementType != rhs.MovementType)
            {
                return false;
            }
            return lhs.GetStartStateOccupation().Select(a => a.Index).SequenceEqual(rhs.GetFinalStateOccupation().Select(a => a.Index))
                && lhs.GetFinalStateOccupation().Select(a => a.Index).SequenceEqual(rhs.GetStartStateOccupation().Select(a => a.Index));
        }

        /// <summary>
        /// Check if two rules describe a backjump rule pair meaning that invoking one rule can cancel the invokation of the other out
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <remarks> Rules form backjump pair when the following is true. S_0 == S_1.Reverse && E_0 == E_1.Reverse </remarks>
        /// <returns></returns>
        public bool IsBackjumpRulePair(ITransitionRule lhs, ITransitionRule rhs)
        {
            if (lhs.MovementType != rhs.MovementType)
            {
                return false;
            }
            return lhs.GetStartStateOccupation().Select(a => a.Index).SequenceEqual(rhs.GetStartStateOccupation().Reverse().Select(a => a.Index))
                && lhs.GetFinalStateOccupation().Select(a => a.Index).SequenceEqual(rhs.GetFinalStateOccupation().Reverse().Select(a => a.Index));
        }

        /// <summary>
        /// Check if the rule pair form a twisted rule pair where they describe the twisted (Both states reversed) version of the symmtric rule pair
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public bool IsTwistedRulePair(ITransitionRule lhs, ITransitionRule rhs)
        {
            if (lhs.MovementType != rhs.MovementType)
            {
                return false;
            }
            return lhs.GetStartStateOccupation().Select(a => a.Index).SequenceEqual(rhs.GetFinalStateOccupation().Reverse().Select(a => a.Index))
                && lhs.GetFinalStateOccupation().Select(a => a.Index).SequenceEqual(rhs.GetStartStateOccupation().Reverse().Select(a => a.Index));
        }

        /// <summary>
        /// Ccheks if the passed abstract transition is symmetric meaning reading the transition in reverse does not change its meaning
        /// </summary>
        /// <param name="abstractTransition"></param>
        /// <returns></returns>
        public bool IsSymmetricTransition(IAbstractTransition abstractTransition)
        {
            return abstractTransition.GetStateExchangeGroups().SequenceEqual(abstractTransition.GetStateExchangeGroups().Reverse());
        }

        /// <summary>
        /// Calculates the abstract transitions charge transport chain that assigns each state position a charge change value form donor to acceptor state,
        /// or NaN if a position has multiple possible values
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
                double chargeTransport = GetStatePairChargeTransport(propertyGroup.GetStateExchangePairs().First());
                foreach (var statePair in propertyGroup.GetStateExchangePairs().Skip(1))
                {
                    chargeTransport = (comparer.Compare(chargeTransport, GetStatePairChargeTransport(statePair)) == 0) ? chargeTransport : double.NaN;
                }
                transportChain.Add(chargeTransport);
            }
            return transportChain;
        }

        /// <summary>
        /// Calculates the charge transport value of a single state exchange pair
        /// </summary>
        /// <param name="statePair"></param>
        /// <returns></returns>
        public double GetStatePairChargeTransport(IStateExchangePair statePair)
        {
            return statePair.DonorParticle.Charge - statePair.AcceptorParticle.Charge;
        }
    }
}
