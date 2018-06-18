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
            var particlePort = ProjectServices.GetManager<IParticleManager>().QueryPort;
            var transitionPort = ProjectServices.GetManager<ITransitionManager>().QueryPort;

            var particles = particlePort.Query(port => port.GetParticles().ToArray());
            var transitions = transitionPort.Query(port => port.GetKineticTransitions().Select(a => a.AbstractTransition));
            var ruleGenerator = new QuickRuleGenerator<KineticRule>(particles);

            return ruleGenerator.MakeUniqueRules(transitions).Select(result => (IList<KineticRule>)result.ToList()).ToList();
        }
    }
}
