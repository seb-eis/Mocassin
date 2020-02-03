using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Basic implementation of the transition cache manager that provides read only access to the extended 'on demand'
    ///     transition data
    /// </summary>
    internal class TransitionCacheManager : ModelCacheManager<TransitionModelCache, ITransitionCachePort>, ITransitionCachePort
    {
        /// <inheritdoc />
        public TransitionCacheManager(TransitionModelCache modelCache, IModelProject modelProject)
            : base(modelCache, modelProject)
        {
        }

        /// <inheritdoc />
        public IList<IList<KineticMapping>> GetAllKineticMappingLists()
        {
            return GetResultFromCache(CreateAllKineticMappings);
        }

        /// <inheritdoc />
        public IList<IList<KineticRule>> GetAllKineticRuleLists()
        {
            return GetResultFromCache(CreateAllKineticRules);
        }

        /// <inheritdoc />
        public IList<IList<MetropolisMapping>> GetAllMetropolisMappingLists()
        {
            return GetResultFromCache(CreateAllMetropolisMappings);
        }

        /// <inheritdoc />
        public IList<IList<MetropolisRule>> GetAllMetropolisRuleLists()
        {
            return GetResultFromCache(CreateAllMetropolisRules);
        }

        /// <inheritdoc />
        public IList<KineticMapping> GetKineticMappingList(int index)
        {
            return GetAllKineticMappingLists()[index];
        }

        /// <inheritdoc />
        public IList<KineticRule> GetKineticRuleList(int index)
        {
            return GetAllKineticRuleLists()[index];
        }

        /// <inheritdoc />
        public IList<MetropolisMapping> GetMetropolisMappingList(int index)
        {
            return GetAllMetropolisMappingLists()[index];
        }

        /// <inheritdoc />
        public IList<MetropolisRule> GetMetropolisRuleList(int index)
        {
            return GetAllMetropolisRuleLists()[index];
        }

        /// <inheritdoc />
        public IDictionary<ICellReferencePosition, HashSet<IKineticTransition>> GetKineticTransitionPositionDictionary()
        {
            return GetResultFromCache(CreateKineticTransitionPositionDictionary);
        }

        /// <inheritdoc />
        public IDictionary<ICellReferencePosition, HashSet<IMetropolisTransition>> GetMetropolisTransitionPositionDictionary()
        {
            return GetResultFromCache(CreateMetropolisTransitionPositionDictionary);
        }

        /// <inheritdoc />
        public IList<double> GetAbstractChargeTransportChain(int index)
        {
            return GetAbstractChargeTransportChains()[index];
        }

        /// <inheritdoc />
        public IList<IList<double>> GetAbstractChargeTransportChains()
        {
            return GetResultFromCache(CreateAbstractChargeTransportChains);
        }

        /// <inheritdoc />
        public IRuleSetterProvider GetRuleSetterProvider()
        {
            return GetResultFromCache(CreateRuleSetterProvider);
        }

        /// <summary>
        ///     Creates a dictionary that assigns each unit cell position its possible list of metropolis transitions
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IDictionary<ICellReferencePosition, HashSet<IMetropolisTransition>> CreateMetropolisTransitionPositionDictionary()
        {
            var structureManager = ModelProject.GetManager<IStructureManager>();
            var transitionManager = ModelProject.GetManager<ITransitionManager>();
            var result = new Dictionary<ICellReferencePosition, HashSet<IMetropolisTransition>>();

            foreach (var position in structureManager.QueryPort.Query(port => port.GetCellReferencePositions()))
                result.Add(position, new HashSet<IMetropolisTransition>());

            foreach (var transition in transitionManager.QueryPort.Query(port => port.GetMetropolisTransitions()))
            {
                if (transition.IsDeprecated)
                    continue;

                result[transition.FirstCellReferencePosition].Add(transition);
                result[transition.SecondCellReferencePosition].Add(transition);
            }

            return result;
        }

        /// <summary>
        ///     Creates a dictionary that assigns each unit cell position its possible list of kinetic transitions
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IDictionary<ICellReferencePosition, HashSet<IKineticTransition>> CreateKineticTransitionPositionDictionary()
        {
            var structureManager = ModelProject.GetManager<IStructureManager>();
            var result = new Dictionary<ICellReferencePosition, HashSet<IKineticTransition>>();

            foreach (var position in structureManager.QueryPort.Query(port => port.GetCellReferencePositions()))
                result.Add(position, new HashSet<IKineticTransition>());

            foreach (var mappingList in GetAllKineticMappingLists())
            {
                if (mappingList.Count == 0)
                    continue;

                var mapping = mappingList[0];
                result[mapping.StartCellReferencePosition].Add(mapping.Transition);
                result[mapping.EndCellReferencePosition].Add(mapping.Transition);
            }

            return result;
        }

        /// <summary>
        ///     Creates all metropolis transition mappings and supplies it as a 2D list interface system
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IList<IList<MetropolisMapping>> CreateAllMetropolisMappings()
        {
            var mapper = new MetropolisTransitionMapper();
            var positionSets = ModelProject.GetManager<IStructureManager>().QueryPort.Query(port => port.GetEncodedExtendedPositionLists());
            var result = new List<IList<MetropolisMapping>>();

            foreach (var transition in ModelProject.GetManager<ITransitionManager>().QueryPort
                .Query(port => port.GetMetropolisTransitions()))
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
        ///     Creates all kinetic transition mappings and supplies it as a 2D list interface system
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IList<IList<KineticMapping>> CreateAllKineticMappings()
        {
            var transitionManager = ModelProject.GetManager<ITransitionManager>();
            var structureManager = ModelProject.GetManager<IStructureManager>();
            var encoder = structureManager.QueryPort.Query(port => port.GetVectorEncoder());
            var uniCellProvider = structureManager.QueryPort.Query(port => port.GetFullUnitCellProvider());
            var mapper = new KineticTransitionMapper(ModelProject.SpaceGroupService, encoder, uniCellProvider);

            var result = new List<IList<KineticMapping>>();
            foreach (var transition in transitionManager.QueryPort.Query(port => port.GetKineticTransitions()))
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
        ///     Creates all metropolis transition rules and supplies it as a 2D list interface system
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IList<IList<MetropolisRule>> CreateAllMetropolisRules()
        {
            var particlePort = ModelProject.GetManager<IParticleManager>().QueryPort;
            var transitionPort = ModelProject.GetManager<ITransitionManager>().QueryPort;

            var particles = particlePort.Query(port => port.GetParticles().ToArray());
            var transitions = transitionPort.Query(port => port.GetMetropolisTransitions().ToArray());
            var ruleGenerator = new TransitionRuleGenerator<MetropolisRule>(particles);

            var index = -1;
            return ruleGenerator
                .MakeUniqueRules(transitions.Select(a => a.AbstractTransition), true)
                .Select(result =>
                {
                    ++index;
                    return (IList<MetropolisRule>) result.Action(value => value.Transition = transitions[index]).ToList();
                }).ToList();
        }

        /// <summary>
        ///     Creates all kinetic transition rules and supplies it as a 2D list interface system
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IList<IList<KineticRule>> CreateAllKineticRules()
        {
            var particlePort = ModelProject.GetManager<IParticleManager>().QueryPort;
            var transitionPort = ModelProject.GetManager<ITransitionManager>().QueryPort;

            var particles = particlePort.Query(port => port.GetParticles().ToArray());
            var transitions = transitionPort.Query(port => port.GetKineticTransitions().ToArray());
            var ruleGenerator = new TransitionRuleGenerator<KineticRule>(particles);

            var index = -1;
            return ruleGenerator
                .MakeUniqueRules(transitions.Select(a => a.AbstractTransition), true)
                .Select(result =>
                {
                    ++index;
                    return (IList<KineticRule>) result.Action(value => value.Transition = transitions[index]).ToList();
                }).ToList();
        }

        /// <summary>
        ///     Creates the abstract charge transport chains for all abstract transitions as a 2D list interface
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IList<IList<double>> CreateAbstractChargeTransportChains()
        {
            var analyzer = new TransitionAnalyzer();
            var comparer = ModelProject.CommonNumeric.RangeComparer;
            var abstracts = ModelProject.GetManager<ITransitionManager>().QueryPort.Query(port => port.GetAbstractTransitions());
            var result = new List<IList<double>>();

            foreach (var abstractTransition in abstracts) result.Add(analyzer.GetChargeTransportChain(abstractTransition, comparer));

            return result;
        }

        /// <summary>
        ///     Creates the <see cref="IRuleSetterProvider" /> for the current state of the <see cref="ProjectSettings" /> data
        ///     object
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IRuleSetterProvider CreateRuleSetterProvider()
        {
            return ModelProject.GetManager<ITransitionManager>().QueryPort.Query(port => port.GetRuleSetterProvider(ModelProject.Settings));
        }
    }
}