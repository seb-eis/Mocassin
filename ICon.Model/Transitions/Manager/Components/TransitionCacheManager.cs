using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Framework.Collections;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Structures;
using ICon.Model.Particles;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Basic implementation of the transition cache manager that provides read only access to the extended 'on demand' transition data
    /// </summary>
    internal class TransitionCacheManager : ModelCacheManager<TransitionDataCache, ITransitionCachePort>, ITransitionCachePort
    {
        /// <summary>
        /// Create new transition cache manager for the provided data cache and project services
        /// </summary>
        /// <param name="dataCache"></param>
        /// <param name="projectServices"></param>
        public TransitionCacheManager(TransitionDataCache dataCache, IProjectServices projectServices) : base(dataCache, projectServices)
        {

        }

        /// <summary>
        /// Get a list interface for all kinetic mappings lists (Listss for deprecated entries are empty)
        /// </summary>
        /// <returns></returns>
        public IList<IList<KineticMapping>> GetAllKineticMappingLists()
        {
            return AccessCacheableDataEntry(CreateAllKineticMappings);
        }

        /// <summary>
        /// Get a list interface for all kinetic transition rule lists (Lists for deprecated entries are empty)
        /// </summary>
        /// <returns></returns>
        public IList<IList<KineticRule>> GetAllKineticRuleLists()
        {
            return AccessCacheableDataEntry(CreateAllKineticRules);
        }

        /// <summary>
        /// Get a list interface for all metropolis mapping lists (Lists for deprecated entries are empty)
        /// </summary>
        /// <returns></returns>
        public IList<IList<MetropolisMapping>> GetAllMetropolisMappingLists()
        {
            return AccessCacheableDataEntry(CreateAllMetropolisMappings);
        }

        /// <summary>
        /// Get a list interface for all metropolis rule lists (Lists for deprecated entries are empty)
        /// </summary>
        /// <returns></returns>
        public IList<IList<MetropolisRule>> GetAllMetropolisRuleLists()
        {
            return AccessCacheableDataEntry(CreateAllMetropolisRules);
        }

        /// <summary>
        /// Get a list interface for the kinetic mapping of the transition at the specfified index (Lists for deprecated entries are empty)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IList<KineticMapping> GetKineticMappingList(int index)
        {
            return GetAllKineticMappingLists()[index];
        }

        /// <summary>
        /// Get a list interface for the transition rules of the kinetic transition at the specified index (Lists for deprecated entries are empty)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IList<KineticRule> GetKineticRuleList(int index)
        {
            return GetAllKineticRuleLists()[index];
        }

        /// <summary>
        /// Get a list interface for the transition mapping of the metropolis transition at the specified index (Lists for deprecated entries are empty)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IList<MetropolisMapping> GetMetropolisMappingList(int index)
        {
            return GetAllMetropolisMappingLists()[index];
        }

        /// <summary>
        /// Get a list interface for the transition rules of the metropolis transition at the specified index (Lists for deprecated entries are empty)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IList<MetropolisRule> GetMetropolisRuleList(int index)
        {
            return GetAllMetropolisRuleLists()[index];
        }

        /// <summary>
        /// Get a list of state pair groups for each refernce position of the unit cell. Groups contain all possible state pairs for these positions
        /// </summary>
        /// <returns></returns>
        public IList<StatePairGroup> GetPossibleStatePairsForAllPositions()
        {
            return AccessCacheableDataEntry(CreatePossibleStatePairsForAllPositions);
        }

        /// <summary>
        /// Get the state pair groups with undefined status for all property groups (Deprecate groups have empty state pair groups)
        /// </summary>
        /// <returns></returns>
        public IList<StatePairGroup> GetAllStatePairGroups()
        {
            return AccessCacheableDataEntry(CreateAllStatePairGroups);
        }

        /// <summary>
        /// Get the state pair group for the property group at the specfified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public StatePairGroup GetStatePairGroup(int index)
        {
            return GetAllStatePairGroups()[index];
        }

        /// <summary>
        /// Get all symmetry equivalent intermediate positions for all kinetic transitions
        /// </summary>
        /// <returns></returns>
        IList<IList<SetList<Fractional3D>>> GetKineticIntermediateEquivalencyMaps()
        {
            return AccessCacheableDataEntry(CreateKineticIntermediateEquivalencyMaps);
        }

        /// <summary>
        /// Get the symmetry equivalent intermediate positions for the transition at the specfified index
        /// </summary>
        /// <returns></returns>
        IList<SetList<Fractional3D>> GetKineticIntermediateEquivalencyMap(int index)
        {
            return GetKineticIntermediateEquivalencyMaps()[index];
        }

        /// <summary>
        /// Creates the list of symmetry equivalent intermediate positions for all kinetic reference transitions
        /// </summary>
        /// <returns></returns>
        [CacheableMethod]
        protected IList<IList<SetList<Fractional3D>>> CreateKineticIntermediateEquivalencyMaps()
        {
            var transitionManager = ProjectServices.GetManager<ITransitionManager>();
            var structureManager = ProjectServices.GetManager<IStructureManager>();
            var analyzer = new TransitionAnalyzer();
            var encoder = structureManager.QueryPort.Query(port => port.GetVectorEncoder());

            var result = new List<IList<SetList<Fractional3D>>>();
            foreach (var transition in transitionManager.QueryPort.Query((ITransitionDataPort port) => port.GetKineticTransitions()))
            {
                if (transition.IsDeprecated)
                {
                    result.Add(new List<SetList<Fractional3D>>());
                }
                result.Add(analyzer.GetEquivalentIntermediatePositions(transition.GetGeometrySequence(), ProjectServices.SpaceGroupService));
            }
            return result;
        }

        /// <summary>
        /// Create all state pair groups for all property groups. The groups have an undefined position status
        /// </summary>
        /// <returns></returns>
        [CacheableMethod]
        protected IList<StatePairGroup> CreateAllStatePairGroups()
        {
            var transitionManager = ProjectServices.GetManager<ITransitionManager>();
            var statePairs = transitionManager.QueryPort.Query((ITransitionDataPort port) => port.GetPropertyStatePairs());
            var groupCreator = new StatePairGroupCreator();
            var result = new List<StatePairGroup>();

            foreach (var propertyGroup in transitionManager.QueryPort.Query((ITransitionDataPort port) => port.GetPropertyGroups()))
            {
                if (propertyGroup.IsDeprecated)
                {
                    result.Add(StatePairGroup.CreateEmpty());
                    continue;
                }
                result.Add(groupCreator.MakeGroup(propertyGroup, statePairs));
            }
            return result;
        }

        /// <summary>
        /// Creates a state pair group for each sublattice position that contains all possible options required for metropolis transitions
        /// </summary>
        /// <returns></returns>
        [CacheableMethod]
        protected IList<StatePairGroup> CreatePossibleStatePairsForAllPositions()
        {
            var structureDataAccess = ProjectServices.GetManager<IStructureManager>().QueryPort;
            var transitionDataAccess = ProjectServices.GetManager<ITransitionManager>().QueryPort;
            var result = new List<StatePairGroup>();

            var groupCreator = new StatePairGroupCreator();
            var statePairs = transitionDataAccess.Query(port => port.GetPropertyStatePairs());
            var propertyGroups = transitionDataAccess.Query(port => port.GetPropertyGroups());
            var cellPositions = structureDataAccess.Query(port => port.GetUnitCellPositions());

            foreach (var particleSet in cellPositions.Select(a => a.OccupationSet))
            {
                result.Add(groupCreator.MakeMergedGroup(propertyGroups, statePairs, particleSet.GetParticles().Select(a => a.Index)));
            }
            return result;
        }

        /// <summary>
        /// Creates all metropolis transition mappings and supplies it as a 2D list interface system
        /// </summary>
        /// <returns></returns>
        [CacheableMethod]
        protected IList<IList<MetropolisMapping>> CreateAllMetropolisMappings()
        {
            var mapper = new MetropolisTransitionMapper();
            var positionSets = ProjectServices.GetManager<IStructureManager>().QueryPort.Query(port => port.GetEncodedExtendedPositionLists());
            var result = new List<IList<MetropolisMapping>>();

            foreach (var transition in ProjectServices.GetManager<ITransitionManager>().QueryPort.Query((ITransitionDataPort port) => port.GetMetropolisTransitions()))
            {
                if (transition.IsDeprecated)
                {
                    result.Add(new List<MetropolisMapping>());
                    continue;
                }
                result.Add(mapper.MapTransition(transition, positionSets).ToList());
            }
            return result;
        }

        /// <summary>
        /// Creates all kinetic transition mappings and supplies it as a 2D list interface system
        /// </summary>
        /// <returns></returns>
        [CacheableMethod]
        protected IList<IList<KineticMapping>> CreateAllKineticMappings()
        {
            var transitionManager = ProjectServices.GetManager<ITransitionManager>();
            var encoder = ProjectServices.GetManager<IStructureManager>().QueryPort.Query(port => port.GetVectorEncoder());
            var mapper = new KineticTransitionQuickMapper(ProjectServices.SpaceGroupService, encoder);

            var result = new List<IList<KineticMapping>>();
            foreach (var transition in transitionManager.QueryPort.Query((ITransitionDataPort port) => port.GetKineticTransitions()))
            {
                if (transition.IsDeprecated)
                {
                    result.Add(new List<KineticMapping>());
                    continue;
                }
                result.Add(mapper.GetMappings(transition).ToList());
            }
            return result;
        }

        /// <summary>
        /// Creates all metropolis transition rules and supplies it as a 2D list interface system
        /// </summary>
        /// <returns></returns>
        [CacheableMethod]
        protected IList<IList<MetropolisRule>> CreateAllMetropolisRules()
        {
            var transitionDataAccess = ProjectServices.GetManager<ITransitionManager>().QueryPort;
            var particleDataAccess = ProjectServices.GetManager<IParticleManager>().QueryPort;
            var statePairGroups = GetPossibleStatePairsForAllPositions();
            var connections = new ConnectorType[] { ConnectorType.Dynamic };
            var result = new List<IList<MetropolisRule>>();

            var particles = particleDataAccess.Query(port => port.GetParticles().ToArray());
            var ruleGenerator = new QuickRuleGenerator<MetropolisRule>(particles);

            foreach (var transition in transitionDataAccess.Query(port => port.GetMetropolisTransitions()))
            {
                if (transition.IsDeprecated)
                {
                    result.Add(new List<MetropolisRule>());
                    continue;
                }

                var stateDescription = new StatePairGroup[]
                {
                    statePairGroups[transition.CellPosition0.Index],
                    statePairGroups[transition.CellPosition1.Index]
                };

                var rules = ruleGenerator
                    .MakeUniqueRules(stateDescription, connections)
                    .Select(a => { a.Transition = transition; return a; });
                result.Add(rules.ToList());
            }
            return result;
        }

        /// <summary>
        /// Creates all kinetic transition rules and supplies it as a 2D list interface system
        /// </summary>
        /// <returns></returns>
        [CacheableMethod]
        protected IList<IList<KineticRule>> CreateAllKineticRules()
        {
            var transitionPort = ProjectServices.GetManager<ITransitionManager>().QueryPort;
            var particlePort = ProjectServices.GetManager<IParticleManager>().QueryPort;

            var statePairGroups = GetAllStatePairGroups();
            var result = new List<IList<KineticRule>>();

            var particles = particlePort.Query(port => port.GetParticles().ToArray());
            var ruleGenerator = new QuickRuleGenerator<KineticRule>(particles);

            foreach (var transition in transitionPort.Query(port => port.GetKineticTransitions()))
            {
                if (transition.IsDeprecated)
                {
                    result.Add(new List<KineticRule>());
                    continue;
                }
                var rules = ruleGenerator
                    .MakeUniqueRules(transition.AbstractTransition, statePairGroups)
                    .Select(rule => { rule.Transition = transition; return rule; });
                result.Add(rules.ToList());
            }

            return result;
        }
    }
}
