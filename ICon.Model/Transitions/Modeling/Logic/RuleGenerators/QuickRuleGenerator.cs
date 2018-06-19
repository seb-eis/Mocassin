using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Framework.Extensions;
using ICon.Framework.Collections;
using ICon.Mathematics.Permutation;
using ICon.Model.Particles;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Generic rule generator to create tha basic information of new transition rules
    /// </summary>
    public class QuickRuleGenerator<TRule> where TRule : TransitionRule, new()
    {
        /// <summary>
        /// The particle pool dictionary to translate the indexing into actual particle references
        /// </summary>
        protected SortedDictionary<int, IParticle> ParticleDictionary { get; set; }

        /// <summary>
        /// Create new quick rule generator that uses the provided set of particles
        /// </summary>
        /// <param name="particlePool"></param>
        public QuickRuleGenerator(IEnumerable<IParticle> particlePool)
        {
            ParticleDictionary = new SortedDictionary<int, IParticle>();
            foreach (var item in particlePool)
            {
                ParticleDictionary.Add(item.Index, item);
            }
        }

        /// <summary>
        /// Makes the unique rule sequence for each passed abstract transition
        /// </summary>
        /// <param name="abstractTransitions"></param>
        /// <returns></returns>
        public IEnumerable<IEnumerable<TRule>> MakeUniqueRules(IEnumerable<IAbstractTransition> abstractTransitions)
        {
            var statePairGroups = new StatePairGroupCreator().MakeGroupsWithBlanks(abstractTransitions.SelectMany(value => value.GetPropertyGroupSequence()));
            return MakeUniqueRules(abstractTransitions, statePairGroups);
        }

        /// <summary>
        /// Creates the unique for a seqeunce of abstract transitions and a general list of state pair groups
        /// </summary>
        /// <param name="abstractTransitions"></param>
        /// <param name="statePairGroups"></param>
        /// <returns></returns>
        public IEnumerable<IEnumerable<TRule>> MakeUniqueRules(IEnumerable<IAbstractTransition> abstractTransitions, IList<StatePairGroup> statePairGroups)
        {
            return abstractTransitions.Select(transition => MakeUniqueRules(transition, statePairGroups));
        }

        /// <summary>
        /// Creates the unqiue rules for an abstract transition with the provided statePairGroup list
        /// </summary>
        /// <param name="abstractTransition"></param>
        /// <param name="statePairGroups"></param>
        /// <returns></returns>
        public IEnumerable<TRule> MakeUniqueRules(IAbstractTransition abstractTransition, IList<StatePairGroup> statePairGroups)
        {
            var stateDescription = abstractTransition.GetPropertyGroupSequence().Select(value => statePairGroups[value.Index].AutoChangeStatus());
            foreach (var rule in MakeUniqueRules(stateDescription, abstractTransition.GetConnectorSequence()))
            {
                rule.AbstractTransition = abstractTransition;
                yield return rule;
            }
        }

        /// <summary>
        /// Creates the unique rules that can be derived from the passes state group and pattern description of a transition path
        /// (No abstract transition is set on the rules)
        /// </summary>
        /// <param name="pairGroups"></param>
        /// <param name="connectorTypes"></param>
        /// <returns></returns>
        public IEnumerable<TRule> MakeUniqueRules(IEnumerable<StatePairGroup> pairGroups, IEnumerable<ConnectorType> connectorTypes)
        {
            return GetValidTransitionRules(pairGroups.ToArray(), connectorTypes.ToArray());
        }

        /// <summary>
        /// Creates a state pair permuter for the possible state pair configurations of the transition sequence
        /// </summary>
        /// <param name="statePairGroups"></param>
        /// <returns></returns>
        protected SlotMachinePermuter<StatePair> MakeStateSetPermuter(StatePairGroup[] statePairGroups)
        {
            var permuterSet = statePairGroups
                .Select(group => group.StatePairs.Select(value => StatePair.CreateForStatus(value.Donor, value.Acceptor, group.PositionStatus)))
                .ToArray();

            return new SlotMachinePermuter<StatePair>(permuterSet);
        }

        /// <summary>
        /// Takes a state pair permutation and creates a permuter that can create all possible states of the path allowed by this state pair permutation
        /// </summary>
        /// <param name="statePairs"></param>
        /// <returns></returns>
        protected SlotMachinePermuter<int> MakePathStatePermuter(StatePair[] statePairs)
        {
            return new SlotMachinePermuter<int>(statePairs.Select(pair => new List<int> { pair.DonorIndex, pair.AcceptorIndex }).ToArray());
        }

        /// <summary>
        /// Creates a set of transition rules which are filtered for inversions and physically impossible state changes
        /// </summary>
        /// <param name="statePairGroups"></param>
        /// <param name="connectorTypes"></param>
        /// <returns></returns>
        protected IEnumerable<TRule> GetValidTransitionRules(StatePairGroup[] statePairGroups, ConnectorType[] connectorTypes)
        {
            var movementCode = GetMovementCode(connectorTypes, statePairGroups);
            var results = new SetList<TRule>(Comparer<TRule>.Create((a, b) => a.CompareToWithCodeInversion(b)));

            foreach (var stateSet in MakeStateSetPermuter(statePairGroups))
            {
                foreach (var pathState in MakePathStatePermuter(stateSet))
                {
                    TRule rule = new TRule
                    {
                        StartState = new OccupationState() { Particles = pathState.Select(a => ParticleDictionary[a]).ToList() }
                    };

                    if (!TrySetFinalState(rule, stateSet, connectorTypes))
                    {
                        continue;
                    }

                    SetTransitionState(rule, stateSet, connectorTypes);
                    rule.MovementCode = movementCode;
                    results.Add(rule);
                }
            }
            return results;
        }

        /// <summary>
        /// Sets the transition state of the rule. Default state is 0 for all stables and donor on all unstables
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="statePairs"></param>
        /// <param name="connectorTypes"></param>
        protected void SetTransitionState(TRule rule, StatePair[] statePairs, ConnectorType[] connectorTypes)
        {
            rule.TransitionState = new OccupationState()
            {
                Particles = new List<IParticle>(statePairs.Length)
            };
            foreach (var pair in statePairs)
            {
                int index = (pair.PositionStatus == Structures.PositionStatus.Unstable) ? pair.DonorIndex : 0;
                rule.TransitionState.Particles.Add(ParticleDictionary[index]);
            }
        }

        /// <summary>
        /// Tries to set the final state of a rule. Aborts if a physically meaningless state change is detected and returns false in this case
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="statePairs"></param>
        /// <param name="connectorTypes"></param>
        /// <returns></returns>
        protected bool TrySetFinalState(TRule rule, StatePair[] statePairs, ConnectorType[] connectorTypes)
        {
            if (!CanHaveValidEndState(rule, statePairs))
            {
                rule.RuleFlags |= RuleFlags.PhysicallyInvalid;
                return false;
            }

            rule.FinalState = new OccupationState() { Particles = new List<IParticle>(statePairs.Length) };
            rule.FinalState = rule.StartState.DeepCopy();

            for (int i = 0; i < connectorTypes.Length; i++)
            {
                int lastIndex = i;
                while (statePairs[i + 1].PositionStatus == Structures.PositionStatus.Unstable)
                {
                    i++;
                }
                var states = (rule.FinalState[lastIndex].Index, rule.FinalState[i + 1].Index);
                if (!TryChangeStateStep(ref states, (statePairs[lastIndex], statePairs[i + 1]), connectorTypes[lastIndex]))
                {
                    rule.RuleFlags |= RuleFlags.PhysicallyInvalid;
                    return false;
                }
                rule.FinalState[lastIndex] = ParticleDictionary[states.Item1];
                rule.FinalState[i + 1] = ParticleDictionary[states.Item2];
            }
            return true;
        }

        /// <summary>
        /// Checks if a combination of rule start occupation and state pairs can result in a meaningfull transition
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="statePairs"></param>
        /// <returns></returns>
        protected bool CanHaveValidEndState(TRule rule, StatePair[] statePairs)
        {
            var (donors, stables) = (0, rule.StartState.StateLength);
            for (int i = 0; i < rule.PathLength; i++)
            {
                if (statePairs[i].PositionStatus == Structures.PositionStatus.Unstable)
                {
                    if (rule.StartState[i].Index != 0)
                    {
                        return false;
                    }
                    stables--;
                }
                if (statePairs[i].PositionStatus != Structures.PositionStatus.Unstable)
                {
                    donors += (rule.StartState[i].Index == statePairs[i].DonorIndex) ? 1 : 0;
                }
            }
            return donors >= stables - donors;
        }

        /// <summary>
        /// Creates the tracker reorder instruction pairs for the connector pattern
        /// </summary>
        /// <param name="connectorTypes"></param>
        /// <param name="pairGroups"></param>
        protected MovementCode GetMovementCode(ConnectorType[] connectorTypes, StatePairGroup[] pairGroups)
        {
            var exchangeList = new List<int>();
            for (int i = 0; i < connectorTypes.Length; i++)
            {
                int current = i;
                if (connectorTypes[current] == ConnectorType.Dynamic)
                {
                    while (pairGroups[i + 1].PositionStatus == Structures.PositionStatus.Unstable)
                    {
                        i++;
                    }
                    exchangeList.Add(current);
                    exchangeList.Add(i + 1);
                }
            }
            return new MovementCode() { CodeValues = exchangeList.ToArray() };
        }

        /// <summary>
        /// Tries to change the state of the passed states with the state pairs according to the operation define by the connectr type
        /// </summary>
        /// <param name="states"></param>
        /// <param name="statePair"></param>
        /// <param name="connectorType"></param>
        /// <returns></returns>
        protected bool TryChangeStateStep(ref (int, int) states, in (StatePair, StatePair) statePairs, ConnectorType connectorType)
        {
            switch (connectorType)
            {
                case ConnectorType.Dynamic:
                    return TryChangeStepDynamic(ref states, statePairs);
                case ConnectorType.Static:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Tries to change the state 
        /// </summary>
        /// <param name="states"></param>
        /// <param name="statePairs"></param>
        /// <returns></returns>
        protected bool TryChangeStepDynamic(ref (int, int) states, in (StatePair, StatePair) statePairs)
        {
            var (value0, direction0) = ChangeState(states.Item1, statePairs.Item1);
            var (value1, direction1) = ChangeState(states.Item2, statePairs.Item2);
            if (direction0 + direction1 == 0)
            {
                states = (value0, value1);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Changes the state using the provided state and retuns value and change direction (+1 for D-A, -1 for A-D)
        /// </summary>
        /// <param name="current"></param>
        /// <param name="statePair"></param>
        /// <returns></returns>
        protected (int value, int direction) ChangeState(int current, in StatePair statePair)
        {
            if (current == statePair.AcceptorIndex)
            {
                return (statePair.DonorIndex, 1);
            }
            if (current == statePair.DonorIndex)
            {
                return (statePair.AcceptorIndex, -1);
            }
            throw new ArgumentException("Index not found in state pair", nameof(current));
        }

        /// <summary>
        /// Applies the rules of matter conservation to a rule set based upon the currently known particle set and their symbols
        /// </summary>
        /// <typeparam name="TRule"></typeparam>
        /// <param name="unfiltered"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public IEnumerable<TRule> FilterbyConservationRule(IEnumerable<TRule> unfiltered)
        {
            foreach (var rule in unfiltered)
            {
                bool isValid = true;
                var zipped = rule.GetStartStateOccupation().Zip(rule.GetFinalStateOccupation(), (a, b) => (a, b)).ToArray();
                var movement = rule.GetMovementDescription().ToArray();
                for (int i = 0; i < movement.Length - 1; i = i+2)
                {
                    if (!IsValidVacancyExchange(zipped[i], zipped[i+1]) && !IsExclusiveStateExchange(zipped[i], zipped[i + 1], rule.AbstractTransition.IsMetropolis))
                    {
                        isValid = false;
                        break;
                    }
                }
                if (isValid)
                {
                    yield return rule;
                }
            }
        }

        /// <summary>
        /// Checks if two particle tuples describing the start and end states of two dynamically exchanging positions describe a valid vacancy exchange
        /// that does not violated matter conservation
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns> True if the exchange is valid in the sense of a vacancy mechansim step </returns>
        public bool IsValidVacancyExchange(in (IParticle start, IParticle end) lhs, in (IParticle start, IParticle end) rhs)
        {
            if ((!lhs.start.IsVacancy && !lhs.end.IsVacancy) || (lhs.start.IsVacancy && lhs.end.IsVacancy))
            {
                return false;
            }
            if ((!rhs.start.IsVacancy && !rhs.end.IsVacancy) || (rhs.start.IsVacancy && rhs.end.IsVacancy))
            {
                return false;
            }
            return lhs.start.Index == rhs.end.Index && lhs.end.Index == rhs.start.Index;
        }

        /// <summary>
        /// Checks if two particle tuples describing the start and end states of two dynamically exchanging positions describe a valid property exchange
        /// that does not violated matter conservation
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="isMetropolis"></param>
        /// <returns> True only if the exchange is either property exchange (KMC and MMC) or physical exchange (MMC) but never both </returns>
        /// <remarks> Theoretically nothing is wrong with both happening in the MMC case and maybe supporting it could be benefical to the MMC </remarks>
        public bool IsExclusiveStateExchange(in (IParticle start, IParticle end) lhs, in (IParticle start, IParticle end) rhs, bool isMetropolis)
        {
            bool isPropertyExchange = lhs.start.Symbol == lhs.end.Symbol && rhs.start.Symbol == rhs.end.Symbol;
            bool isPhysicalExchange = lhs.start.Index == rhs.end.Index && lhs.end.Index == rhs.start.Index;

            if (isMetropolis)
            {
                return isPropertyExchange ^ isPhysicalExchange;
            }
            return isPropertyExchange;
        }
    }
}
