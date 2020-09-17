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
        public IList<IList<KineticMapping>> GetAllKineticMappingLists() => GetResultFromCache(CreateAllKineticMappings);

        /// <inheritdoc />
        public IList<IList<KineticRule>> GetAllKineticRuleLists() => GetResultFromCache(CreateAllKineticRules);

        /// <inheritdoc />
        public IList<IList<MetropolisMapping>> GetAllMetropolisMappingLists() => GetResultFromCache(CreateAllMetropolisMappings);

        /// <inheritdoc />
        public IList<IList<MetropolisRule>> GetAllMetropolisRuleLists() => GetResultFromCache(CreateAllMetropolisRules);

        /// <inheritdoc />
        public IList<KineticMapping> GetKineticMappingList(int index) => GetAllKineticMappingLists()[index];

        /// <inheritdoc />
        public IList<KineticRule> GetKineticRuleList(int index) => GetAllKineticRuleLists()[index];

        /// <inheritdoc />
        public IList<MetropolisMapping> GetMetropolisMappingList(int index) => GetAllMetropolisMappingLists()[index];

        /// <inheritdoc />
        public IList<MetropolisRule> GetMetropolisRuleList(int index) => GetAllMetropolisRuleLists()[index];

        /// <inheritdoc />
        public IDictionary<ICellSite, HashSet<IKineticTransition>> GetKineticTransitionPositionDictionary() =>
            GetResultFromCache(CreateKineticTransitionPositionDictionary);

        /// <inheritdoc />
        public IDictionary<ICellSite, HashSet<IMetropolisTransition>> GetMetropolisTransitionPositionDictionary() =>
            GetResultFromCache(CreateMetropolisTransitionPositionDictionary);

        /// <inheritdoc />
        public IList<double> GetAbstractChargeTransportChain(int index) => GetAbstractChargeTransportChains()[index];

        /// <inheritdoc />
        public IList<IList<double>> GetAbstractChargeTransportChains() => GetResultFromCache(CreateAbstractChargeTransportChains);

        /// <inheritdoc />
        public IRuleSetterProvider GetRuleSetterProvider() => GetResultFromCache(CreateRuleSetterProvider);

        /// <summary>
        ///     Creates a dictionary that assigns each unit cell position its possible list of metropolis transitions
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IDictionary<ICellSite, HashSet<IMetropolisTransition>> CreateMetropolisTransitionPositionDictionary()
        {
            var structureManager = ModelProject.Manager<IStructureManager>();
            var transitionManager = ModelProject.Manager<ITransitionManager>();
            var result = new Dictionary<ICellSite, HashSet<IMetropolisTransition>>();

            foreach (var position in structureManager.DataAccess.Query(port => port.GetCellReferencePositions()))
                result.Add(position, new HashSet<IMetropolisTransition>());

            foreach (var transition in transitionManager.DataAccess.Query(port => port.GetMetropolisTransitions()))
            {
                if (transition.IsDeprecated)
                    continue;

                result[transition.FirstCellSite].Add(transition);
                result[transition.SecondCellSite].Add(transition);
            }

            return result;
        }

        /// <summary>
        ///     Creates a dictionary that assigns each unit cell position its possible list of kinetic transitions
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IDictionary<ICellSite, HashSet<IKineticTransition>> CreateKineticTransitionPositionDictionary()
        {
            var structureManager = ModelProject.Manager<IStructureManager>();
            var result = new Dictionary<ICellSite, HashSet<IKineticTransition>>();

            foreach (var position in structureManager.DataAccess.Query(port => port.GetCellReferencePositions()))
                result.Add(position, new HashSet<IKineticTransition>());

            foreach (var mappingList in GetAllKineticMappingLists())
            {
                if (mappingList.Count == 0)
                    continue;

                var mapping = mappingList[0];
                result[mapping.StartCellSite].Add(mapping.Transition);
                result[mapping.EndCellSite].Add(mapping.Transition);
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
            var positionSets = ModelProject.Manager<IStructureManager>().DataAccess.Query(port => port.GetEncodedExtendedPositionLists());
            var result = new List<IList<MetropolisMapping>>();

            foreach (var transition in ModelProject.Manager<ITransitionManager>().DataAccess
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
            var transitionManager = ModelProject.Manager<ITransitionManager>();
            var structureManager = ModelProject.Manager<IStructureManager>();
            var encoder = structureManager.DataAccess.Query(port => port.GetVectorEncoder());
            var uniCellProvider = structureManager.DataAccess.Query(port => port.GetFullUnitCellProvider());
            var mapper = new KineticTransitionMapper(ModelProject.SpaceGroupService, encoder, uniCellProvider);

            var result = new List<IList<KineticMapping>>();
            foreach (var transition in transitionManager.DataAccess.Query(port => port.GetKineticTransitions()))
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
            var particlePort = ModelProject.Manager<IParticleManager>().DataAccess;
            var transitionPort = ModelProject.Manager<ITransitionManager>().DataAccess;

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
            var particlePort = ModelProject.Manager<IParticleManager>().DataAccess;
            var transitionPort = ModelProject.Manager<ITransitionManager>().DataAccess;

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
            var abstracts = ModelProject.Manager<ITransitionManager>().DataAccess.Query(port => port.GetAbstractTransitions());
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
            return ModelProject.Manager<ITransitionManager>().DataAccess.Query(port => port.GetRuleSetterProvider(ModelProject.Settings));
        }
    }
}