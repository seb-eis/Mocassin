using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mocassin.Framework.Collections;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Xml;
using Mocassin.Mathematics.Permutation;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Generic rule generator to create tha basic information of new transition rules
    /// </summary>
    public class TransitionRuleGenerator<TRule> where TRule : TransitionRule, new()
    {
        /// <summary>
        ///     The particle pool dictionary to translate the indexing into actual particle references
        /// </summary>
        protected SortedDictionary<int, IParticle> ParticleDictionary { get; set; }

        /// <summary>
        ///     Create new quick rule generator that uses the provided set of particles
        /// </summary>
        /// <param name="particlePool"></param>
        public TransitionRuleGenerator(IEnumerable<IParticle> particlePool)
        {
            ParticleDictionary = new SortedDictionary<int, IParticle>();
            foreach (var item in particlePool)
                ParticleDictionary.Add(item.Index, item);
        }

        /// <summary>
        ///     Makes the unique rule sequence for each passed abstract transition. With option flag to control if the system
        ///     should automatically filter out rules
        ///     that are not supported
        /// </summary>
        /// <param name="abstractTransitions"></param>
        /// <param name="onlySupported"></param>
        /// <returns></returns>
        /// <remarks> Unsupported rules are typically physically meaningless e.g. they violate matter conservation rules </remarks>
        public IEnumerable<IEnumerable<TRule>> MakeUniqueRules(IEnumerable<IAbstractTransition> abstractTransitions, bool onlySupported)
        {
            var abstractCollection = abstractTransitions.ToCollection();
            var statePairGroups =
                new StatePairGroupCreator().MakeGroupsWithBlanks(abstractCollection.SelectMany(value => value.GetStateExchangeGroups()));

            var results = MakeUniqueRules(abstractCollection, statePairGroups);
            return onlySupported
                ? results.Select(FilterByCommonBehaviorAndInversionRules)
                : results;
        }

        /// <summary>
        ///     Creates the unique for a sequence of abstract transitions and a general list of state pair groups
        /// </summary>
        /// <param name="abstractTransitions"></param>
        /// <param name="statePairGroups"></param>
        /// <returns></returns>
        public IEnumerable<IEnumerable<TRule>> MakeUniqueRules(IEnumerable<IAbstractTransition> abstractTransitions,
            IList<StatePairGroup> statePairGroups)
        {
            return abstractTransitions.Select(transition => MakeUniqueRules(transition, statePairGroups));
        }

        /// <summary>
        ///     Creates the unique rules for an abstract transition with the provided statePairGroup list
        /// </summary>
        /// <param name="abstractTransition"></param>
        /// <param name="statePairGroups"></param>
        /// <returns></returns>
        public IEnumerable<TRule> MakeUniqueRules(IAbstractTransition abstractTransition, IList<StatePairGroup> statePairGroups)
        {
            var stateDescription = abstractTransition.GetStateExchangeGroups()
                .Select(value => statePairGroups[value.Index].AutoChangeStatus());

            foreach (var rule in MakeUniqueRules(stateDescription, abstractTransition.GetConnectorSequence()))
            {
                rule.AbstractTransition = abstractTransition;
                yield return rule;
            }
        }

        /// <summary>
        ///     Creates the unique rules that can be derived from the passes state group and pattern description of a transition
        ///     path
        ///     (No abstract transition is set on the rules)
        /// </summary>
        /// <param name="pairGroups"></param>
        /// <param name="connectorTypes"></param>
        /// <returns></returns>
        public IEnumerable<TRule> MakeUniqueRules(IEnumerable<StatePairGroup> pairGroups, IEnumerable<ConnectorType> connectorTypes)
        {
            return GetValidTransitionRules(pairGroups.ToArray(), connectorTypes.ToArray());
        }

        /// <summary>
        ///     Creates a state pair permutation source for the possible state pair configurations of the transition sequence
        /// </summary>
        /// <param name="statePairGroups"></param>
        /// <returns></returns>
        protected IPermutationSource<StatePair> MakeStateSetPermutationSource(StatePairGroup[] statePairGroups)
        {
            var objectSet = statePairGroups
                .Select(group =>
                    group.StatePairs.Select(value => StatePair.CreateForStatus(value.Donor, value.Acceptor, group.PositionStatus)))
                .ToArray();

            return new PermutationSlotMachine<StatePair>(objectSet);
        }

        /// <summary>
        ///     Takes a state pair permutation and creates a permutation source that can create all possible states of the path
        ///     allowed by
        ///     this state pair permutation
        /// </summary>
        /// <param name="statePairs"></param>
        /// <returns></returns>
        protected IPermutationSource<int> MakePathStatePermutationSource(StatePair[] statePairs)
        {
            return new PermutationSlotMachine<int>(statePairs.Select(pair => new List<int> {pair.DonorIndex, pair.AcceptorIndex}).ToList());
        }

        /// <summary>
        ///     Creates a set of transition rules which are filtered for inversions and physically impossible state changes
        /// </summary>
        /// <param name="statePairGroups"></param>
        /// <param name="connectorTypes"></param>
        /// <returns></returns>
        protected IEnumerable<TRule> GetValidTransitionRules(StatePairGroup[] statePairGroups, ConnectorType[] connectorTypes)
        {
            var movementCode = GetMovementCode(connectorTypes, statePairGroups);
            var results = new SetList<TRule>(Comparer<TRule>.Create((a, b) => a.CompareTo(b)));

            foreach (var stateSet in MakeStateSetPermutationSource(statePairGroups))
            {
                foreach (var pathState in MakePathStatePermutationSource(stateSet))
                {
                    var rule = new TRule
                    {
                        StartState = new OccupationState {Particles = pathState.Select(a => ParticleDictionary[a]).ToList()}
                    };

                    if (!TrySetFinalState(rule, stateSet, connectorTypes))
                        continue;

                    SetTransitionState(rule, stateSet, connectorTypes);
                    rule.MovementCode = movementCode;
                    results.Add(rule);
                }
            }

            return results;
        }

        /// <summary>
        ///     Takes a list interface of transition rules and filters out both inverted and reversed duplicate rules. The filtered
        ///     rules are added
        ///     as dependent rules to a parent rule
        /// </summary>
        /// <remarks>
        ///     Possibly requires changes in the future since correct definition of "meaningful and duplicate" in all cases
        ///     is tricky
        /// </remarks>
        /// <param name="rules"></param>
        protected void FilterInvertedAndReversedDuplicateRules(IList<TRule> rules)
        {
            var analyzer = new TransitionAnalyzer();
            for (var i = 0; i < rules.Count - 1; i++)
            {
                var parentRule = rules[i];
                for (var j = i + 1; j < rules.Count;)
                {
                    var checkRule = rules[j];
                    var isRemovable = analyzer.IsSymmetricRulePair(parentRule, checkRule)
                                      || analyzer.IsBackjumpRulePair(parentRule, checkRule)
                                      || analyzer.IsTwistedRulePair(parentRule, checkRule);

                    if (isRemovable)
                    {
                        parentRule.AddDependentRule(checkRule);
                        rules.RemoveAt(j);
                        continue;
                    }

                    j++;
                }
            }
        }

        /// <summary>
        ///     Sets the transition state of the rule. Default state is 0 for all stables and donor on all unstable positions
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="statePairs"></param>
        /// <param name="connectorTypes"></param>
        protected void SetTransitionState(TRule rule, StatePair[] statePairs, ConnectorType[] connectorTypes)
        {
            rule.TransitionState = new OccupationState
            {
                Particles = new List<IParticle>(statePairs.Length)
            };

            foreach (var pair in statePairs)
            {
                var index = pair.PositionStatus == PositionStatus.Unstable ? pair.DonorIndex : 0;
                rule.TransitionState.Particles.Add(ParticleDictionary[index]);
            }
        }

        /// <summary>
        ///     Tries to set the final state of a rule. Aborts if a physically meaningless state change is detected and returns
        ///     false in this case
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="statePairs"></param>
        /// <param name="connectorTypes"></param>
        /// <returns></returns>
        protected bool TrySetFinalState(TRule rule, StatePair[] statePairs, ConnectorType[] connectorTypes)
        {
            if (!CanHaveValidEndState(rule, statePairs))
            {
                rule.RuleFlags |= RuleFlags.IsPhysicallyInvalid;
                return false;
            }

            rule.FinalState = new OccupationState {Particles = new List<IParticle>(statePairs.Length)};
            rule.FinalState = rule.StartState.DeepCopy();

            for (var i = 0; i < connectorTypes.Length; i++)
            {
                var lastIndex = i;
                while (statePairs[i + 1].PositionStatus == PositionStatus.Unstable)
                    i++;

                var states = (rule.FinalState[lastIndex].Index, rule.FinalState[i + 1].Index);
                if (!TryChangeStateStep(ref states, (statePairs[lastIndex], statePairs[i + 1]), connectorTypes[lastIndex]))
                {
                    rule.RuleFlags |= RuleFlags.IsPhysicallyInvalid;
                    return false;
                }

                rule.FinalState[lastIndex] = ParticleDictionary[states.Item1];
                rule.FinalState[i + 1] = ParticleDictionary[states.Item2];
            }

            return true;
        }

        /// <summary>
        ///     Checks if a combination of rule start occupation and state pairs can result in a meaningful transition end state
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="statePairs"></param>
        /// <returns></returns>
        protected bool CanHaveValidEndState(TRule rule, StatePair[] statePairs)
        {
            var (donors, stables) = (0, rule.StartState.StateLength);
            for (var i = 0; i < rule.PathLength; i++)
            {
                if (statePairs[i].PositionStatus == PositionStatus.Unstable)
                {
                    if (rule.StartState[i].Index != 0) return false;
                    stables--;
                }

                if (statePairs[i].PositionStatus != PositionStatus.Unstable)
                    donors += rule.StartState[i].Index == statePairs[i].DonorIndex ? 1 : 0;
            }

            return donors >= stables - donors;
        }

        /// <summary>
        ///     Creates the tracker reorder instruction pairs for the connector pattern
        /// </summary>
        /// <param name="connectorTypes"></param>
        /// <param name="pairGroups"></param>
        protected MovementCode GetMovementCode(ConnectorType[] connectorTypes, StatePairGroup[] pairGroups)
        {
            var exchangeList = new List<int>();
            for (var i = 0; i < connectorTypes.Length; i++)
            {
                var current = i;
                if (connectorTypes[current] != ConnectorType.Dynamic)
                    continue;

                while (pairGroups[i + 1].PositionStatus == PositionStatus.Unstable)
                    i++;

                exchangeList.Add(current);
                exchangeList.Add(i + 1);
            }

            return new MovementCode {CodeValues = exchangeList.ToArray()};
        }

        /// <summary>
        ///     Tries to change the state of the passed states with the state pairs according to the operation define by the
        ///     connector type
        /// </summary>
        /// <param name="states"></param>
        /// <param name="statePairs"></param>
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
        ///     Tries to change the state for a dynamically linked step
        /// </summary>
        /// <param name="states"></param>
        /// <param name="statePairs"></param>
        /// <returns></returns>
        protected bool TryChangeStepDynamic(ref (int, int) states, in (StatePair, StatePair) statePairs)
        {
            var (value0, direction0) = ChangeState(states.Item1, statePairs.Item1);
            var (value1, direction1) = ChangeState(states.Item2, statePairs.Item2);

            if (direction0 + direction1 != 0)
                return false;

            states = (value0, value1);
            return true;
        }

        /// <summary>
        ///     Changes the state using the provided state and returns value and change direction (+1 for D-A, -1 for A-D)
        /// </summary>
        /// <param name="current"></param>
        /// <param name="statePair"></param>
        /// <returns></returns>
        protected (int value, int direction) ChangeState(int current, in StatePair statePair)
        {
            if (current == statePair.AcceptorIndex)
                return (statePair.DonorIndex, 1);

            if (current == statePair.DonorIndex)
                return (statePair.AcceptorIndex, -1);

            throw new ArgumentException("Index not found in state pair", nameof(current));
        }

        /// <summary>
        ///     Filters a set of rules by assigning common movement tags and removing all rules that can be generated from each
        ///     other and are therefore equivalent
        /// </summary>
        /// <typeparam name="TRule"></typeparam>
        /// <param name="unfilteredRules"></param>
        /// <remarks> Equivalent rules are; Back-jump rules, Symmetric rules or twisted symmetric rules  </remarks>
        /// <returns></returns>
        protected IEnumerable<TRule> FilterByCommonBehaviorAndInversionRules(IEnumerable<TRule> unfilteredRules)
        {
            var results = new List<TRule>(10);
            foreach (var rule in DetermineAndSetRuleMovementTypes(unfilteredRules))
            {
                // If the rule determination detects not supported, try to handle the rule as a chained migration
                if (rule.MovementFlags.HasFlag(RuleMovementFlags.ContainsUnsupported))
                    continue;

                // Handle the push like case (intersticialcy) where the inverse rule was not created
                if (rule.MovementFlags.HasFlag(RuleMovementFlags.ContainsAtomPushing))
                {
                    HandleIntersticialcyLikeRule(rule);
                    results.Add(rule);
                    continue;
                }

                // Migration that contain physical migrations without a vacancy are possible but basically meaningless
                if (rule.MovementFlags.HasFlag(RuleMovementFlags.PhysicalMigration) &&
                    !rule.MovementFlags.HasFlag(RuleMovementFlags.ContainsVacancyMovement))
                    continue;

                // Handle association/dissociation case where one has to be selected
                if (!(!rule.AbstractTransition.IsAssociation ^ rule.MovementFlags.HasFlag(RuleMovementFlags.AssociationBehavior)))
                    continue;

                results.Add(rule);
            }

            FilterInvertedAndReversedDuplicateRules(results);
            return results;
        }

        /// <summary>
        ///     Tries to handle the passed rule as a chained/vehicle migration and creates the missing inverse rule if possible
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected void HandleIntersticialcyLikeRule(TRule rule)
        {
            if (!rule.MovementFlags.HasFlag(RuleMovementFlags.ContainsAtomPushing))
                throw new InvalidOperationException("Rule does not have the valid flags for this operation");

            var inverseRule = new TRule
            {
                RuleFlags = rule.RuleFlags,
                AbstractTransition = rule.AbstractTransition,
                StartState = rule.FinalState.DeepCopy(),
                TransitionState = rule.TransitionState.DeepCopy(),
                FinalState = rule.StartState.DeepCopy(),
                MovementCode = rule.MovementCode.GetInverse()
            };

            rule.AddDependentRule(inverseRule);
        }

        /// <summary>
        ///     Determines the combination of rule movement flags for each rule and sets the values
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        protected IEnumerable<TRule> DetermineAndSetRuleMovementTypes(IEnumerable<TRule> rules)
        {
            foreach (var rule in rules)
            {
                var moveType = rule.AbstractTransition.ConnectorCount == 1 ? RuleMovementFlags.HasExchangeBehavior : RuleMovementFlags.HasMigrationBehavior;
                moveType |= GetRequiredVehicleFlags(rule);

                var movement = rule.GetMovementDescription().ToList();
                var zipped = rule.GetStartStateOccupation().Zip(rule.GetFinalStateOccupation(), (a, b) => (a, b)).ToList();
                for (var i = 0; i < movement.Count - 1; i = i + 2)
                    moveType |= GetStepMovementType(zipped[movement[i]], zipped[movement[i + 1]]);

                if (IsIntersticialyLikeMovement(movement))
                {
                    moveType -= moveType & RuleMovementFlags.ContainsUnsupported;
                    moveType |= RuleMovementFlags.ContainsAtomPushing;
                }

                rule.MovementFlags = moveType;
                yield return rule;
            }
        }

        /// <summary>
        /// Checks if a rule movement description is like the intersticialcy type mechanism
        /// </summary>
        /// <param name="movement"></param>
        /// <returns></returns>
        protected bool IsIntersticialyLikeMovement(IList<int> movement)
        {
            if (movement.Count <= 2)
                return false;

            for (var i = 1; i < movement.Count - 2; i++)
            {
                if (movement[i] != movement[i + 1])
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Get the required vehicle flags for the passed transition rule or none if the rule does not describe a vehicle
        ///     mechanism
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected RuleMovementFlags GetRequiredVehicleFlags(TRule rule)
        {
            RuleMovementFlags result = 0;
            if (rule.PathLength <= 3)
                return result;

            result |= RuleMovementFlags.ContainsVehicleMovement;
            result |= MovementIsAssociationDissociation(rule) ? RuleMovementFlags.AssociationBehavior : 0;
            return result;
        }

        /// <summary>
        ///     Check if the movement description of the passed transition rule conforms to association/dissociation behavior
        ///     independent of what is defined in its set abstract transition
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected bool MovementIsAssociationDissociation(TRule rule)
        {
            if (rule.PathLength <= 3)
                return false;

            var (forwardCount, backwardCount) = (0, 0);
            var statePairMap = rule.AbstractTransition.GetStateExchangeGroups().Select(x => x.GetStateExchangePairs().ToList()).ToList();
            var movement = rule.MovementCode.AsExchangePairs().ToList();

            for (var i = 0; i < rule.PathLength; i++)
            {
                if (rule.StartState[i].IsEmpty) 
                    continue;

                if (statePairMap[i].Count(x => x.DonorParticle == rule.StartState[i]) == 0)
                    continue;

                var firstMovement = movement.FirstOrDefault(x => x.Item1 == i || x.Item2 == i);
                var direction = firstMovement.Item1 == i ? 1 : -1;

                if (direction < 0)
                    backwardCount++;
                if (direction > 0)
                    forwardCount++;
            }

            return forwardCount > 0 && backwardCount > 0;
        }

        /// <summary>
        ///     Determines the specific rule movement type of a single type step based upon the characteristics the involve
        ///     particles provide
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        protected RuleMovementFlags GetStepMovementType(in (IParticle start, IParticle end) lhs, in (IParticle start, IParticle end) rhs)
        {
            var isVacancyExchange = IsValidVacancyExchange(lhs, rhs);
            var isPropertyExchange = lhs.start.Symbol == lhs.end.Symbol && rhs.start.Symbol == rhs.end.Symbol;
            var isPhysicalExchange = lhs.start.Index == rhs.end.Index && lhs.end.Index == rhs.start.Index;
            var isSameTypeExchange = lhs.start.Symbol == rhs.start.Symbol;

            if (isVacancyExchange)
                return RuleMovementFlags.ContainsPhysicalMovement | RuleMovementFlags.ContainsVacancyMovement;

            if (isPropertyExchange && isPhysicalExchange && isSameTypeExchange)
                return RuleMovementFlags.ContainsPropertyMovement;

            if (isPhysicalExchange)
                return RuleMovementFlags.ContainsPhysicalMovement;

            return isPropertyExchange
                ? RuleMovementFlags.ContainsPropertyMovement
                : RuleMovementFlags.ContainsUnsupported;
        }

        /// <summary>
        ///     Checks if two particle tuples describing the start and end states of two dynamically exchanging positions describe
        ///     a valid vacancy exchange that does not violated matter conservation
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns> True if the exchange is valid in the sense of a vacancy mechanism step </returns>
        protected bool IsValidVacancyExchange(in (IParticle start, IParticle end) lhs, in (IParticle start, IParticle end) rhs)
        {
            if (!lhs.start.IsVacancy && !lhs.end.IsVacancy || lhs.start.IsVacancy && lhs.end.IsVacancy)
                return false;

            if (!rhs.start.IsVacancy && !rhs.end.IsVacancy || rhs.start.IsVacancy && rhs.end.IsVacancy)
                return false;

            return lhs.start.Index == rhs.end.Index && lhs.end.Index == rhs.start.Index;
        }
    }
}