using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Framework.Extensions;
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
            var particlePort = ProjectServices.GetManager<IParticleManager>().QueryPort;
            var transitionPort = ProjectServices.GetManager<ITransitionManager>().QueryPort;

            var particles = particlePort.Query(port => port.GetParticles().ToArray());
            var transitions = transitionPort.Query(port => port.GetMetropolisTransitions().ToArray());
            var ruleGenerator = new QuickRuleGenerator<MetropolisRule>(particles);

            int index = -1;
            return ruleGenerator
                .MakeUniqueRules(transitions.Select(a => a.AbstractTransition))
                .Select(result =>
                {
                    ++index; return (IList<MetropolisRule>)result.Change(value => value.Transition = transitions[index]).ToList();
                }).ToList();
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
            var transitions = transitionPort.Query(port => port.GetKineticTransitions().ToArray());
            var ruleGenerator = new QuickRuleGenerator<KineticRule>(particles);

            int index = -1;
            return ruleGenerator
                .MakeUniqueRules(transitions.Select(a => a.AbstractTransition))
                .Select(result =>
                {
                    ++index; return (IList<KineticRule>)result.Change(value => value.Transition = transitions[index]).ToList();
                }).ToList();
        }
    }
}
