using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Comparer;
using Mocassin.Mathematics.Permutation;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Transitions
{
    /// <inheritdoc />
    public class TransitionRuleGenerator<TRule> : ITransitionRuleGenerator<TRule> where TRule : TransitionRule, new()
    {
        protected IParticle[] Particles { get; }

        /// <summary>
        ///     Creates a new rule generator that uses the passed array of index mapped <see cref="IParticle"/> instances
        /// </summary>
        /// <param name="particles"></param>
        public TransitionRuleGenerator(IParticle[] particles)
        {
            Particles = particles ?? throw new ArgumentNullException(nameof(particles));
            if (particles.Select((x, i) => (x, i)).Any(y => y.x.Index != y.i))
                throw new InvalidOperationException("Particles are not mapped according to their indices.");
        }

        /// <inheritdoc />
        public IEnumerable<IEnumerable<TRule>> MakeUniqueRules(IEnumerable<IAbstractTransition> abstractTransitions, bool onlySupported)
        {
            foreach (var abstractTransition in abstractTransitions)
            {
                if (abstractTransition.IsMetropolis)
                {
                    var rules = GenerateMmcTypeRules(abstractTransition);
                    yield return FlagAndLinkMmcTypeRules(rules);
                }
                else
                {
                    var rules = GenerateKmcTypeRules(abstractTransition);
                    yield return FlagAndLinkKmcTypeRules(rules);
                }
            }
        }

        /// <summary>
        ///     Generates a <see cref="IList{T}"/> of MMC type rules for the provided <see cref="IAbstractTransition"/>
        /// </summary>
        /// <param name="abstractTransition"></param>
        /// <returns></returns>
        protected IList<TRule> GenerateMmcTypeRules(IAbstractTransition abstractTransition)
        {
            var result = new List<TRule>();
            var (connectors, exchangeGroups) = GetConnectorsAndExchangeGroups(abstractTransition);
            var startStatePermutationSource = GetStartStatePermutationSource(exchangeGroups);
            var possibleStartStates = GetStartStatePermutations(exchangeGroups);
            var movementCode = GetMovementCode(connectors, exchangeGroups);

            foreach (var startState in startStatePermutationSource)
            {
                if (!TryGetMmcEndState(startState, exchangeGroups, out var endState)) continue;

                var rule = CreateMmcTypeRule(startState, endState, abstractTransition, movementCode);
                result.Add(rule);
            }

            AddMissingInversesToRules(result);
            return result;
        }

        /// <summary>
        ///     Factory method to create a new rule of the MMC type with the provided data
        /// </summary>
        /// <param name="startState"></param>
        /// <param name="endState"></param>
        /// <param name="abstractTransition"></param>
        /// <param name="movementCode"></param>
        /// <returns></returns>
        protected TRule CreateMmcTypeRule(IParticle[] startState, IParticle[] endState, IAbstractTransition abstractTransition,
            MovementCode movementCode)
        {
            var rule = new TRule
            {
                StartState = new OccupationState {Particles = startState.ToList()},
                FinalState = new OccupationState {Particles = endState.ToList()},
                TransitionState = new OccupationState {Particles = new List<IParticle>().Populate(() => Particles[0], 2)},
                AbstractTransition = abstractTransition,
                MovementCode = movementCode
            };
            return rule;
        }

        /// <summary>
        ///     Generates a <see cref="IList{T}"/> of KMC type rules for the provided <see cref="IAbstractTransition"/>
        /// </summary>
        /// <param name="abstractTransition"></param>
        /// <returns></returns>
        protected IList<TRule> GenerateKmcTypeRules(IAbstractTransition abstractTransition)
        {
            var result = new List<TRule>();
            var (connectors, exchangeGroups) = GetConnectorsAndExchangeGroups(abstractTransition);
            var startStatePermutationSource = GetStartStatePermutationSource(exchangeGroups);
            var possibleStartStates = GetStartStatePermutations(exchangeGroups);
            var movementCode = GetMovementCode(connectors, exchangeGroups);

            foreach (var startState in startStatePermutationSource)
            {
                if (!TryGetKmcEndState(startState, exchangeGroups, connectors, out var endState)) continue;

                if (!TryGetKmcTransitionState(startState, endState, exchangeGroups, connectors, out var transitionState)) continue;

                var rule = CreateKmcTypeRule(startState, transitionState, endState, abstractTransition, movementCode);
                result.Add(rule);
            }

            AddMissingInversesToRules(result);
            return result;
        }

        /// <summary>
        ///     Factory method to create a new rule of the KMC type with the provided data
        /// </summary>
        /// <param name="startState"></param>
        /// <param name="transitionState"></param>
        /// <param name="endState"></param>
        /// <param name="abstractTransition"></param>
        /// <param name="movementCode"></param>
        /// <returns></returns>
        protected TRule CreateKmcTypeRule(IParticle[] startState, IParticle[] transitionState, IParticle[] endState,
            IAbstractTransition abstractTransition,
            MovementCode movementCode)
        {
            var rule = new TRule
            {
                StartState = new OccupationState {Particles = startState.ToList()},
                FinalState = new OccupationState {Particles = endState.ToList()},
                TransitionState = new OccupationState {Particles = transitionState.ToList()},
                AbstractTransition = abstractTransition,
                MovementCode = movementCode
            };
            return rule;
        }

        /// <summary>
        ///     Adds all missing inverse transition rules to a <see cref="IList{T}"/> of rules
        /// </summary>
        /// <param name="rules"></param>
        protected void AddMissingInversesToRules(IList<TRule> rules)
        {
            var rulesWithoutInversion = new List<TRule>();
            for (var i = 0; i < rules.Count; i++)
            {
                var hasInversion = false;
                for (var j = i + 1; j < rules.Count; j++)
                {
                    if (rules[i].StartState.LexicographicCompare(rules[j].FinalState) != 0) continue;
                    hasInversion = true;
                    break;
                }

                if (!hasInversion) rulesWithoutInversion.Add(rules[i]);
            }

            rules.AddRange(rulesWithoutInversion.Select(CreateInverseRule));
        }

        /// <summary>
        ///     Factory method that creates the inverse of a transition rule
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected TRule CreateInverseRule(TRule rule)
        {
            var result = new TRule
            {
                TransitionRuleFlags = rule.TransitionRuleFlags,
                AbstractTransition = rule.AbstractTransition,
                StartState = rule.FinalState.DeepCopy(),
                TransitionState = rule.TransitionState.DeepCopy(),
                FinalState = rule.StartState.DeepCopy(),
                MovementCode = rule.MovementCode.GetInverse()
            };
            return result;
        }

        /// <summary>
        ///     Flags and links a set of completely build MMC type transition rules
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        protected IList<TRule> FlagAndLinkMmcTypeRules(IList<TRule> rules)
        {
            rules.Action(x => x.MovementFlags = GetMovementFlags(x)).Load();
            MoveDependentRulesToParentRules(rules);
            return rules;
        }

        /// <summary>
        ///     Flags and links a set of completely build KMC type rules
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        protected IList<TRule> FlagAndLinkKmcTypeRules(IList<TRule> rules)
        {
            rules.Action(x => x.MovementFlags = GetMovementFlags(x)).Load();
            MoveDependentRulesToParentRules(rules);
            return rules;
        }

        /// <summary>
        ///     Tries to create the KMC transition state based on the initial and final state. Returns false if the method fails to create a transition state
        /// </summary>
        /// <param name="startState"></param>
        /// <param name="endState"></param>
        /// <param name="exchangeGroups"></param>
        /// <param name="connectors"></param>
        /// <param name="transitionState"></param>
        /// <returns></returns>
        protected bool TryGetKmcTransitionState(IParticle[] startState, IParticle[] endState, IStateExchangeGroup[] exchangeGroups, ConnectorType[] connectors, out IParticle[] transitionState)
        {
            transitionState = CreateRawKmcTransitionState(startState);

            for (var i = 0; i < exchangeGroups.Length; i++)
            {
                if (!exchangeGroups[i].IsUnstablePositionGroup) continue;

                if (connectors[i - 1] == ConnectorType.Static && connectors[i] == ConnectorType.Static)
                {
                    if (exchangeGroups[i].StatePairCount != 1)
                        throw new InvalidOperationException("Combined transition sites between two static connectors require one unique non-void particle option.");
                    var exchange = exchangeGroups[i].GetStateExchangePairs().First();
                    transitionState[i] = GetExchangeParticle(Particles[0], exchange);
                    continue;
                }

                if (connectors[i - 1] != ConnectorType.Dynamic || connectors[i] != ConnectorType.Dynamic) throw new InvalidOperationException("A unexpected lhs-rhs connector pair was encountered.");

                if (startState[i - 1].Symbol == endState[i - 1].Symbol && startState[i + 1].Symbol == endState[i + 1].Symbol)
                {
                    var chargeTransfer = Math.Abs(endState[i - 1].Charge - startState[i - 1].Charge);
                    var exchangeMatch = exchangeGroups[i]
                                        .GetStateExchangePairs()
                                        .SingleOrDefault(x => NumericComparer.Default().Equals(Math.Abs(GetExchangeParticle(Particles[0], x).Charge), chargeTransfer))
                                        ?? exchangeGroups[i]
                                           .GetStateExchangePairs()
                                           .SingleOrDefault(x => GetExchangeParticle(Particles[0], x).Equals(startState[i - 1]));
                    if (exchangeMatch == null) throw new InvalidOperationException("Failed to find transition state for either charge transfer or push-like mechanism type.");
                    transitionState[i] = GetExchangeParticle(Particles[0], exchangeMatch);
                }
                else
                {
                    var exchangeMatch = exchangeGroups[i]
                                        .GetStateExchangePairs()
                                        .SingleOrDefault(x => GetExchangeParticle(Particles[0], x).Equals(startState[i - 1]));
                    if (exchangeMatch == null) throw new InvalidOperationException("The ion move process has an ambiguous transition state definition.");
                    transitionState[i] = GetExchangeParticle(Particles[0], exchangeMatch);
                }
            }

            return transitionState.All(x => x != null);
        }

        /// <summary>
        ///     Tries to determine a valid KMC end state based on a start state, exchange groups, and connector types
        /// </summary>
        /// <param name="startState"></param>
        /// <param name="exchangeGroups"></param>
        /// <param name="connectors"></param>
        /// <param name="endState"></param>
        /// <returns></returns>
        protected bool TryGetKmcEndState(IParticle[] startState, IStateExchangeGroup[] exchangeGroups, ConnectorType[] connectors, out IParticle[] endState)
        {
            endState = CreateRawKmcEndState(startState);

            for (var lhsId = 0; lhsId < startState.Length - 1;)
            {
                if (connectors[lhsId] == ConnectorType.Static)
                {
                    lhsId++;
                    continue;
                }

                var rhsId = lhsId + 1;
                while (connectors[rhsId - 1] == ConnectorType.Dynamic && exchangeGroups[rhsId].IsUnstablePositionGroup) rhsId++;

                var lhsExchanges = GetApplicableStateExchanges(startState[lhsId], exchangeGroups[lhsId]);
                var rhsExchanges = GetApplicableStateExchanges(startState[rhsId], exchangeGroups[rhsId]);

                foreach (var lhsExchange in lhsExchanges)
                {
                    foreach (var rhsExchange in rhsExchanges)
                    {
                        var conDoIonMove = CanDoKmcTypeIonMove(startState[lhsId], startState[rhsId], lhsExchange, rhsExchange);
                        var canDoChargeMove = CanDoKmcTypeChargeMove(startState[lhsId], startState[rhsId], lhsExchange, rhsExchange);
                        var canDoIonPush = lhsId >= 2
                                           && connectors[lhsId - 1] == ConnectorType.Dynamic
                                           && CanDoKmcTypeIonPush(startState[lhsId], startState[rhsId], lhsExchange, rhsExchange,
                                               startState[lhsId - 2]);

                        if (!conDoIonMove && !canDoChargeMove && !canDoIonPush) continue;
                        endState[lhsId] = GetExchangeParticle(startState[lhsId], lhsExchange);
                        endState[rhsId] = GetExchangeParticle(startState[rhsId], rhsExchange);
                    }
                }

                lhsId = rhsId;
            }

            return endState.All(x => x != null) && StatesPreserveMass(startState, endState);
        }

        /// <summary>
        ///     Checks if the mass between two states is conserved
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        protected bool StatesPreserveMass(IParticle[] lhs, IParticle[] rhs)
        {
            if (lhs.Length != rhs.Length) return false;

            var counters = new int[Particles.Length];
            var vacancyCount = 0;

            for (var i = 0; i < lhs.Length; i++)
            {
                if (lhs[i].IsVacancy)
                    vacancyCount++;
                else
                    counters[lhs[i].Index]++;

                if (rhs[i].IsVacancy)
                    vacancyCount--;
                else
                    counters[rhs[i].Index]--;
            }

            return vacancyCount == 0 && counters.All(x => x == 0);
        }

        /// <summary>
        ///     Creates an <see cref="IParticle"/> array that represents a raw KMC end state that matches the start state
        /// </summary>
        /// <param name="startState"></param>
        /// <returns></returns>
        protected IParticle[] CreateRawKmcEndState(IParticle[] startState)
        {
            var result = new IParticle[startState.Length];
            for (var i = 0; i < result.Length; i++)
            {
                if (startState[i].IsVoid)
                    result[i] = Particles[0];
            }

            return result;
        }

        /// <summary>
        ///     Creates an <see cref="IParticle"/> array that represents a raw KMC transition state that matches the start state
        /// </summary>
        /// <param name="startState"></param>
        /// <returns></returns>
        protected IParticle[] CreateRawKmcTransitionState(IParticle[] startState)
        {
            var result = new IParticle[startState.Length];
            for (var i = 0; i < result.Length; i++)
            {
                if (!startState[i].IsVoid)
                    result[i] = Particles[0];
            }

            return result;
        }

        /// <summary>
        ///     Tries to determine the end state of an MMC type rule by start state and exchange groups. Returns false if no valid end state is found
        /// </summary>
        /// <param name="startState"></param>
        /// <param name="exchangeGroups"></param>
        /// <param name="endState"></param>
        /// <returns></returns>
        protected bool TryGetMmcEndState(IParticle[] startState, IStateExchangeGroup[] exchangeGroups, out IParticle[] endState)
        {
            endState = null;
            var lhsExchanges = GetApplicableStateExchanges(startState[0], exchangeGroups[0]);
            var rhsExchanges = GetApplicableStateExchanges(startState[1], exchangeGroups[1]);

            foreach (var lhsExchange in lhsExchanges)
            {
                foreach (var rhsExchange in rhsExchanges)
                {
                    if (!CanDoMmcTypeExchange(startState[0], startState[1], lhsExchange, rhsExchange)) continue;
                    if (endState != null) throw new InvalidOperationException("More than one end state matched the start state.");
                    var lhsParticleNew = GetExchangeParticle(startState[0], lhsExchange);
                    var rhsParticleNew = GetExchangeParticle(startState[1], rhsExchange);
                    endState = new[] {lhsParticleNew, rhsParticleNew};
                }
            }

            return endState != null;
        }

        /// <summary>
        ///     Checks if the passed <see cref="IStateExchangePair"/> instances support an MMC type exchange based on the passed original <see cref="IParticle"/> objects
        /// </summary>
        /// <param name="lhsParticleOld"></param>
        /// <param name="rhsParticleOld"></param>
        /// <param name="lhsExchange"></param>
        /// <param name="rhsExchange"></param>
        /// <returns></returns>
        protected bool CanDoMmcTypeExchange(IParticle lhsParticleOld, IParticle rhsParticleOld, IStateExchangePair lhsExchange, IStateExchangePair rhsExchange)
        {
            if (lhsParticleOld.Equals(rhsParticleOld)) return false;
            var lhsParticleNew = GetExchangeParticle(lhsParticleOld, lhsExchange);
            var rhsParticleNew = GetExchangeParticle(rhsParticleOld, rhsExchange);
            var massIsOk = !ExchangeViolatesMassConservation(lhsParticleOld, lhsParticleNew, rhsParticleOld, rhsParticleNew);
            var chargeIsOk = !ExchangeViolatesChargeConservation(lhsParticleOld, lhsParticleNew, rhsParticleOld, rhsParticleNew);
            var isNoHybrid = !ExchangesViolatesMmcHybridRule(lhsParticleOld, lhsParticleNew, rhsParticleOld, rhsParticleNew);
            return massIsOk && chargeIsOk && isNoHybrid;
        }

        /// <summary>
        ///     Checks if the passed <see cref="IStateExchangePair"/> instances support a KMC type ion move based on the passed original <see cref="IParticle"/> objects
        /// </summary>
        /// <param name="lhsParticleOld"></param>
        /// <param name="rhsParticleOld"></param>
        /// <param name="lhsExchange"></param>
        /// <param name="rhsExchange"></param>
        protected bool CanDoKmcTypeIonMove(IParticle lhsParticleOld, IParticle rhsParticleOld, IStateExchangePair lhsExchange, IStateExchangePair rhsExchange)
        {
            var lhsParticleNew = GetExchangeParticle(lhsParticleOld, lhsExchange);
            var rhsParticleNew = GetExchangeParticle(rhsParticleOld, rhsExchange);
            var lhsMovesToRhs = rhsParticleNew.Equals(lhsParticleOld);
            var lhsToVacancy = lhsParticleNew.IsVacancy;
            return lhsToVacancy && lhsMovesToRhs;
        }

        /// <summary>
        ///     Checks if the passed <see cref="IStateExchangePair"/> instances support a KMC type charge move based on the passed original <see cref="IParticle"/> objects
        /// </summary>
        /// <param name="lhsParticleOld"></param>
        /// <param name="rhsParticleOld"></param>
        /// <param name="lhsExchange"></param>
        /// <param name="rhsExchange"></param>
        protected bool CanDoKmcTypeChargeMove(IParticle lhsParticleOld, IParticle rhsParticleOld, IStateExchangePair lhsExchange, IStateExchangePair rhsExchange)
        {
            if (lhsParticleOld.Equals(rhsParticleOld)) return false;
            var lhsParticleNew = GetExchangeParticle(lhsParticleOld, lhsExchange);
            var rhsParticleNew = GetExchangeParticle(rhsParticleOld, rhsExchange);
            var lhsIsDonor = lhsParticleNew.Equals(lhsExchange.DonorParticle);
            var chargeIsOk = !ExchangeViolatesChargeConservation(lhsParticleOld, lhsParticleNew, rhsParticleOld, rhsParticleNew);
            var noMovement = lhsParticleOld.Symbol == lhsParticleNew.Symbol && rhsParticleOld.Symbol == rhsParticleNew.Symbol;
            return lhsIsDonor && chargeIsOk && noMovement;
        }

        /// <summary>
        ///     Checks if the passed <see cref="IStateExchangePair"/> instances support a KMC type ion push based on the passed original <see cref="IParticle"/> objects
        /// </summary>
        /// <param name="lhsParticleOld"></param>
        /// <param name="rhsParticleOld"></param>
        /// <param name="lhsExchange"></param>
        /// <param name="rhsExchange"></param>
        protected bool CanDoKmcTypeIonPush(IParticle lhsParticleOld, IParticle rhsParticleOld, IStateExchangePair lhsExchange, IStateExchangePair rhsExchange, IParticle pushingParticle)
        {
            var lhsParticleNew = GetExchangeParticle(lhsParticleOld, lhsExchange);
            var rhsParticleNew = GetExchangeParticle(rhsParticleOld, rhsExchange);
            var lhsMovesToRhs = rhsParticleNew.Equals(lhsParticleOld);
            var lhsToPushParticle = lhsParticleNew.Equals(pushingParticle);
            return lhsMovesToRhs && lhsToPushParticle;
        }

        /// <summary>
        ///     Gets all <see cref="IStateExchangePair"/> objects that can be applied to the passed start <see cref="IParticle"/>
        /// </summary>
        /// <param name="startParticle"></param>
        /// <param name="exchangeGroup"></param>
        /// <returns></returns>
        protected IList<IStateExchangePair> GetApplicableStateExchanges(IParticle startParticle, IStateExchangeGroup exchangeGroup)
        {
            return exchangeGroup.GetStateExchangePairs()
                                .Where(x => x.DonorParticle.Equals(startParticle) || x.AcceptorParticle.Equals(startParticle))
                                .ToList();
        }

        /// <summary>
        ///     Get the <see cref="ConnectorType"/> and <see cref="IStateExchangeGroup"/> arrays from an <see cref="IAbstractTransition"/>
        /// </summary>
        /// <param name="abstractTransition"></param>
        /// <returns></returns>
        protected (ConnectorType[], IStateExchangeGroup[]) GetConnectorsAndExchangeGroups(IAbstractTransition abstractTransition)
        {
            var connectors = abstractTransition.GetConnectorSequence().ToArray(abstractTransition.ConnectorCount);
            var exchangeGroups = abstractTransition.GetStateExchangeGroups().ToArray(abstractTransition.StateCount);
            return (connectors, exchangeGroups);
        }

        /// <summary>
        ///     Gets a <see cref="IList{T}"/> of <see cref="IParticle"/> arrays that represent all possible start state permutations possible in the passed <see cref="IStateExchangeGroup"/> array
        /// </summary>
        /// <param name="exchangeGroups"></param>
        /// <returns></returns>
        protected IList<IParticle[]> GetStartStatePermutations(IStateExchangeGroup[] exchangeGroups)
        {
            var startStates = GetStartStatePermutationSource(exchangeGroups).ToList();
            startStates.RemoveDuplicates((a, b) => a.LexicographicCompare(b) == 0);
            return startStates;
        }

        /// <summary>
        ///     Creates a <see cref="IPermutationSource{T1}"/> of <see cref="IParticle"/> arrays that represent all possible start state permutations possible in the passed <see cref="IStateExchangeGroup"/> array
        /// </summary>
        /// <param name="exchangeGroups"></param>
        /// <remarks> The permutation source can yields duplicates </remarks>
        /// <returns></returns>
        protected IPermutationSource<IParticle> GetStartStatePermutationSource(IStateExchangeGroup[] exchangeGroups)
        {
            var optionSets = new IEnumerable<IParticle>[exchangeGroups.Length];
            for (var i = 0; i < exchangeGroups.Length; i++)
            {
                if (exchangeGroups[i].IsUnstablePositionGroup)
                {
                    optionSets[i] = Particles[0].AsSingleton();
                    continue;
                }

                var donors = exchangeGroups[i].GetStateExchangePairs().Select(x => x.DonorParticle);
                var acceptors = exchangeGroups[i].GetStateExchangePairs().Select(x => x.AcceptorParticle);
                var union = donors.Union(acceptors);
                optionSets[i] = union;
            }

            return new PermutationSlotMachine<IParticle>(optionSets);
        }

        /// <summary>
        ///     Converts an <see cref="ConnectorType"/> and <see cref="IStateExchangeGroup"/> array pair to a <see cref="MovementCode"/>
        /// </summary>
        /// <param name="connectors"></param>
        /// <param name="exchangeGroups"></param>
        /// <returns></returns>
        protected MovementCode GetMovementCode(ConnectorType[] connectors, IStateExchangeGroup[] exchangeGroups)
        {
            var exchangeList = new List<int>();
            for (var i = 0; i < connectors.Length; i++)
            {
                var current = i;
                if (connectors[current] != ConnectorType.Dynamic)
                    continue;

                while (exchangeGroups[i + 1].IsUnstablePositionGroup)
                    i++;

                exchangeList.Add(current);
                exchangeList.Add(i + 1);
            }

            return new MovementCode {CodeValues = exchangeList.ToArray()};
        }

        /// <summary>
        ///     Get the affiliated exchange <see cref="IParticle"/> if an <see cref="IStateExchangePair"/> is applied to the provided <see cref="IParticle"/>
        /// </summary>
        /// <param name="oldParticle"></param>
        /// <param name="stateExchangePair"></param>
        /// <returns></returns>
        protected IParticle GetExchangeParticle(IParticle oldParticle, IStateExchangePair stateExchangePair)
        {
            if (oldParticle.Equals(stateExchangePair.DonorParticle)) return stateExchangePair.AcceptorParticle;
            if (oldParticle.Equals(stateExchangePair.AcceptorParticle)) return stateExchangePair.DonorParticle;
            throw new InvalidOperationException("Particle and state exchange pair do not match");
        }

        /// <summary>
        ///     Checks if the four <see cref="IParticle"/> objects describe an not supported hybrid MMC exchange, that is, both charge transfer and physical ion transfer happen simultaneously
        /// </summary>
        /// <param name="lhsOld"></param>
        /// <param name="lhsNew"></param>
        /// <param name="rhsOld"></param>
        /// <param name="rhsNew"></param>
        /// <returns></returns>
        protected bool ExchangesViolatesMmcHybridRule(IParticle lhsOld, IParticle lhsNew, IParticle rhsOld, IParticle rhsNew)
        {
            if (lhsOld.IsVoid || rhsOld.IsVoid) return false;
            var condition0 = lhsNew.Symbol != lhsOld.Symbol && rhsNew.Symbol != rhsOld.Symbol;
            var condition1 = lhsNew.Symbol == rhsOld.Symbol && rhsNew.Symbol == lhsOld.Symbol;
            var condition2 = !NumericComparer.Default().Equals(lhsNew.Charge, lhsOld.Charge);
            var condition3 = !NumericComparer.Default().Equals(rhsNew.Charge, rhsOld.Charge);
            return condition0 && condition1 && condition2 && condition3;
        }

        /// <summary>
        ///     Checks if the four <see cref="IParticle"/> objects describe an exchange that violates mass conservation
        /// </summary>
        /// <param name="lhsOld"></param>
        /// <param name="lhsNew"></param>
        /// <param name="rhsOld"></param>
        /// <param name="rhsNew"></param>
        /// <returns></returns>
        protected bool ExchangeViolatesMassConservation(IParticle lhsOld, IParticle lhsNew, IParticle rhsOld, IParticle rhsNew)
        {
            var option0 = lhsOld.Symbol == rhsNew.Symbol && lhsNew.Symbol == rhsOld.Symbol;
            var option1 = lhsOld.Symbol == lhsNew.Symbol && rhsOld.Symbol == rhsNew.Symbol;
            return !option0 && !option1;
        }

        /// <summary>
        ///     Checks if the four <see cref="IParticle"/> objects describe an exchange that violates charge conservation
        /// </summary>
        /// <param name="lhsOld"></param>
        /// <param name="lhsNew"></param>
        /// <param name="rhsOld"></param>
        /// <param name="rhsNew"></param>
        protected bool ExchangeViolatesChargeConservation(IParticle lhsOld, IParticle lhsNew, IParticle rhsOld, IParticle rhsNew) =>
            !NumericComparer.Default().Equals(lhsNew.Charge - lhsOld.Charge, rhsOld.Charge - rhsNew.Charge);

        /// <summary>
        ///     Determines one parent rule for all dependent rules and moves the dependent rules from the main list to the affiliated list on the parent rule
        /// </summary>
        /// <param name="rules"></param>
        protected void MoveDependentRulesToParentRules(IList<TRule> rules)
        {
            var analyzer = new TransitionAnalyzer();
            for (var i = 0; i < rules.Count - 1; i++)
            {
                for (var j = i + 1; j < rules.Count;)
                {
                    if (analyzer.RulePairIsDependentPair(rules[i], rules[j]))
                    {
                        rules[i].AddDependentRule(rules[j]);
                        rules.RemoveAt(j);
                        continue;
                    }

                    j++;
                }
            }
        }

        /// <summary>
        ///     Gets all <see cref="RuleMovementFlags"/> that describe the passed rule
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected RuleMovementFlags GetMovementFlags(TRule rule)
        {
            var flags = rule.PathLength == 2 ? RuleMovementFlags.IsExchange : RuleMovementFlags.IsMigration;

            if (rule.AbstractTransition.GetConnectorSequence().Any(x => x == ConnectorType.Static))
                flags |= RuleMovementFlags.HasVehicleMovement;

            if (ContainsPushChain(rule.AbstractTransition)) flags |= RuleMovementFlags.HasChainedMovement;

            for (var i = 0; i < rule.PathLength; i++)
            {
                var (particle0, particle1) = (rule.StartState[i], rule.FinalState[i]);
                if (particle0.Symbol != particle1.Symbol) flags |= RuleMovementFlags.HasPhysicalMovement;

                if (particle0.Symbol == particle1.Symbol && !NumericComparer.Default().Equals(particle0.Charge, particle1.Charge))
                    flags |= RuleMovementFlags.HasPropertyMovement;

                if (particle0.IsVacancy ^ particle1.IsVacancy) flags |= RuleMovementFlags.HasVacancyMovement;
            }

            return flags;
        }

        /// <summary>
        ///     Checks if an <see cref="IAbstractTransition"/> contains a push chain (interstitialcy mechanism)
        /// </summary>
        /// <param name="abstractTransition"></param>
        /// <returns></returns>
        protected bool ContainsPushChain(IAbstractTransition abstractTransition)
        {
            var dynamicSequenceLength = 0;
            foreach (var connectorType in abstractTransition.GetConnectorSequence())
            {
                if (connectorType == ConnectorType.Dynamic)
                {
                    dynamicSequenceLength++;
                    if (dynamicSequenceLength > 2) return true;
                    continue;
                }

                dynamicSequenceLength = 0;
            }

            return false;
        }
    }
}