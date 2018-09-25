using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ICon.Model.ProjectServices;
using ICon.Model.Transitions;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Structures;
using ICon.Mathematics.Coordinates;
using ICon.Framework.Extensions;
using ICon.Model.Particles;
using ICon.Framework.Collections;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Transition model context builder. Extends the reference transition model information into a full data context
    /// </summary>
    public class TransitionModelContextBuilder : ModelContextBuilderBase<ITransitionModelContext>
    {
        /// <inheritdoc />
        public TransitionModelContextBuilder(IProjectModelContextBuilder projectModelContextBuilder) : base(projectModelContextBuilder)
        {

        }

        /// <inheritdoc />
        protected override void PopulateContext()
        {
            var kineticTask = Task.Run(BuildKineticTransitionModels);
            var metropolisTask = Task.Run(BuildMetropolisTransitionModels);

            ModelContext.KineticTransitionModels = kineticTask.Result;
            ModelContext.MetropolisTransitionModels = metropolisTask.Result;

            ModelContext.PositionTransitionModels = BuildPositionTransitionModels();
        }

        /// <summary>
        /// Creates all kinetic transition models and inverse models where required
        /// </summary>
        /// <returns></returns>
        protected IList<IKineticTransitionModel> BuildKineticTransitionModels()
        {
            var transitionManager = ProjectServices.GetManager<ITransitionManager>();
            var resultModels = new List<IKineticTransitionModel>();
            var inverseModels = new List<IKineticTransitionModel>();

            int index = 0;
            foreach (var transition in transitionManager.QueryPort.Query(port => port.GetKineticTransitions()))
            {
                var model = CreateTransitionModel(transition);
                model.ModelId = index++;
                resultModels.Add(model);
            }

            foreach (var transitionModel in resultModels)
            {
                var inverseModel = CreateTransitionModelInversion(transitionModel);
                if (transitionModel != inverseModel)
                {
                    inverseModel.ModelId = index++;
                    inverseModels.Add(transitionModel.InverseTransitionModel);
                }
                transitionModel.InverseTransitionModel = inverseModel;
            }

            resultModels.AddRange(inverseModels);
            resultModels.ForEach(a => a.MobileParticles = CreateMobileParticleSet(a.RuleModels));
            return resultModels;
        }

        /// <summary>
        /// Build all metropolis transition models and inverse models where required
        /// </summary>
        /// <returns></returns>
        protected IList<IMetropolisTransitionModel> BuildMetropolisTransitionModels()
        {
            var transitionManager = ProjectServices.GetManager<ITransitionManager>();
            var resultModels = new List<IMetropolisTransitionModel>();
            var inverseModels = new List<IMetropolisTransitionModel>();

            int index = 0;
            foreach (var transition in transitionManager.QueryPort.Query(port => port.GetMetropolisTransitions()))
            {
                var model = CreateTransitionModel(transition);
                model.ModelId = index++;
                resultModels.Add(model);
            }

            foreach (var transitionModel in resultModels)
            {
                var inverseModel = CreateTransitionModelInversion(transitionModel);
                if (transitionModel != inverseModel)
                {
                    inverseModel.ModelId = index++;
                    inverseModels.Add(transitionModel.InverseTransitionModel);
                }
                transitionModel.InverseTransitionModel = inverseModel;
            }

            resultModels.AddRange(inverseModels);
            resultModels.ForEach(a => a.MobileParticles = CreateMobileParticleSet(a.RuleModels));
            return resultModels;
        }

        /// <summary>
        /// Creates a single metropolis transition model with rule models and mapping models
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        protected IMetropolisTransitionModel CreateTransitionModel(IMetropolisTransition transition)
        {
            var transitionModel = new MetropolisTransitionModel()
            {
                Transition = transition
            };

            CreateAndAddMappingModels(transitionModel);
            CreateAndAddRuleModels(transitionModel);

            return transitionModel;
        }

        /// <summary>
        /// Creates a single kinetic transition model with rule models and mapping models
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        protected IKineticTransitionModel CreateTransitionModel(IKineticTransition transition)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an inverse metropolis transition model if required or if the mappings already contain
        /// the inversion the original is returned
        /// </summary>
        /// <param name="transitionModel"></param>
        /// <returns></returns>
        protected IMetropolisTransitionModel CreateTransitionModelInversion(IMetropolisTransitionModel transitionModel)
        {
            if (transitionModel.Transition.MappingsContainInversion())
            {
                return transitionModel;
            }

            var inverseModel = new MetropolisTransitionModel()
            {
                Transition = transitionModel.Transition,
                InverseTransitionModel = transitionModel
            };

            CreateAndAddMappingModelInversions(transitionModel, inverseModel);
            CreateAndAddRuleModelInversions(transitionModel, inverseModel);

            return null;
        }

        /// <summary>
        /// Creates an inverse kinetic transition model if required or if the mappings already contain
        /// the inversion the original is returned
        /// </summary>
        /// <param name="transitionModel"></param>
        /// <returns></returns>
        protected IKineticTransitionModel CreateTransitionModelInversion(IKineticTransitionModel transitionModel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates all mapping models for a metropolis transition model and links the mappings together if they contain
        /// their own inversions
        /// </summary>
        /// <param name="transitionModel"></param>
        protected void CreateAndAddMappingModels(IMetropolisTransitionModel transitionModel)
        {
            var transitionManager = ProjectServices.GetManager<ITransitionManager>();
            var mappings = transitionManager.QueryPort.Query(port => port.GetMetropolisMappingList(transitionModel.Transition.Index));

            transitionModel.MappingModels = mappings.Select(CreateMappingModel).ToList();

            if (transitionModel.Transition.MappingsContainInversion())
            {
                LinkMappingModelsToInversions(transitionModel.MappingModels);
            }
        }

        /// <summary>
        /// Creates a single mapping model for a metropolis transition mapping object
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        protected IMetropolisMappingModel CreateMappingModel(MetropolisMapping mapping)
        {
            var vectorEncoder = ProjectServices.GetManager<IStructureManager>().QueryPort.Query(port => port.GetVectorEncoder());
            var mappingModel = new MetropolisMappingModel()
            {
                Mapping = mapping,
                StartVector4D = new CrystalVector4D(0, 0, 0, mapping.PositionIndex0),
                EndVector4D = new CrystalVector4D(0, 0, 0, mapping.PositionIndex1)
            };
            
            if (!vectorEncoder.TryDecode(mappingModel.StartVector4D, out Fractional3D startVector3D))
            {
                throw new InvalidOperationException("Data inconsistency during model generation. 4D to 3D vector conversion failed");
            }
            if (!vectorEncoder.TryDecode(mappingModel.EndVector4D, out Fractional3D endVector3D))
            {
                throw new InvalidOperationException("Data inconsistency during model generation. 4D to 3D vector conversion failed");
            }

            mappingModel.StartVector3D = startVector3D;
            mappingModel.EndVector3D = endVector3D;
            return mappingModel;
        }

        /// <summary>
        /// Creates inverted mappings models for the original transition models, links them to the originals
        /// and adds the inversions to the list of the inverse model
        /// </summary>
        /// <param name="originalModel"></param>
        /// <param name="inverseModel"></param>
        protected void CreateAndAddMappingModelInversions(IMetropolisTransitionModel originalModel, IMetropolisTransitionModel inverseModel)
        {
            var inverseMappings = originalModel.MappingModels.Select(CreateAndLinkInverseModel).ToList();
            inverseModel.MappingModels = inverseMappings;
        }

        /// <summary>
        /// Creates a single inverted mapping model and sets both inverted mappings to the appropriate value
        /// </summary>
        /// <param name="mappingModel"></param>
        /// <returns></returns>
        protected IMetropolisMappingModel CreateAndLinkInverseModel(IMetropolisMappingModel mappingModel)
        {
            var inverseModel = mappingModel.CreateInverse();
            mappingModel.InverseMapping = inverseModel;
            return inverseModel;
        }

        /// <summary>
        /// Links all metropolis mapping models contained in the list together with their inverted mapping model.
        /// For self contained transitions only!
        /// </summary>
        /// <param name="mappingModels"></param>
        protected void LinkMappingModelsToInversions(IList<IMetropolisMappingModel> mappingModels)
        {
            for (var i = 0; i < mappingModels.Count; i++)
            {
                if (mappingModels[i].InverseIsSet) continue;

                for (var j = i; j < mappingModels.Count; j++)
                {
                    if (mappingModels[j].InverseIsSet) continue;
                    if (mappingModels[i].LinkIfInverseMatch(mappingModels[j])) break;
                }
            }
        }

        /// <summary>
        /// Creates and adds the rule models of both parent and dependent rules on the passed transition model
        /// </summary>
        /// <param name="transitionModel"></param>
        protected void CreateAndAddRuleModels(IMetropolisTransitionModel transitionModel)
        {
            var ruleModels = new List<IMetropolisRuleModel>();

            foreach (var rule in transitionModel.Transition.GetTransitionRules())
            {
                var ruleModel = CreateRuleModelBasis<MetropolisRuleModel>(rule);
                ruleModel.MetropolisRule = rule;
                ruleModels.Add(ruleModel);

                foreach (var dependentRule in rule.GetDependentRules())
                {
                    ruleModel = CreateRuleModelBasis<MetropolisRuleModel>(dependentRule);
                    ruleModel.MetropolisRule = dependentRule;
                    ruleModels.Add(ruleModel);
                }
            }

            CreateCodesAndLinkInverseRuleModels(ruleModels);

            transitionModel.RuleModels = ruleModels;
        }

        /// <summary>
        /// Creates the rule model of the specififed type and sets the basis information
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transitionRule"></param>
        /// <returns></returns>
        protected T CreateRuleModelBasis<T>(ITransitionRule transitionRule) where T : TransitionRuleModel, new()
        {
            var ruleModel = new T
            {
                StartState = transitionRule.GetStartStateOccupation().ToList(),
                FinalState = transitionRule.GetFinalStateOccupation().ToList(),
                EndIndexingDeltas = CreateEndIndexingDeltas(transitionRule),
            };
            return ruleModel;
        }

        /// <summary>
        /// Creates all inversion links and simulation codes for a prepared list of rule models
        /// </summary>
        /// <param name="ruleModels"></param>
        protected void CreateCodesAndLinkInverseRuleModels(IList<IMetropolisRuleModel> ruleModels)
        {
            var buffer = new byte[8];
            foreach (var ruleModel in ruleModels)
            {
                ruleModel.StartStateCode = Create64BitIndexCode(ruleModel.StartState.Select(a => a.Index), buffer);
                ruleModel.FinalStateCode = Create64BitIndexCode(ruleModel.FinalState.Select(a => a.Index), buffer);
                ruleModel.FinalTrackerOrderCode = CreateFinalTrackerOrderCode(ruleModel.EndIndexingDeltas, buffer);
            }

            for (int i = 0; i < ruleModels.Count; i++)
            {
                if (ruleModels[i].InverseIsSet) continue;

                for (int j = i; j < ruleModels.Count; j++)
                {
                    if (ruleModels[j].InverseIsSet) continue;
                    if (ruleModels[i].LinkIfInverseMatch(ruleModels[j])) break;
                }
            }
        }

        /// <summary>
        /// Creates a 64 long code for the simulation from the passed set of int values
        /// </summary>
        /// <param name="values"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected long Create64BitIndexCode(IEnumerable<int> values, byte[] buffer)
        {
            int index = -1;
            foreach (var item in values)
            {
                buffer[++index] = (byte) item;
            }

            long code = BitConverter.ToInt64(buffer, 0);
            for (; index >= 0; index--)
            {
                buffer[index] = 0;
            }
            return code;
        }

        /// <summary>
        /// Translates the end indexing deltas into the final tracker 64 bit order code for the simulation
        /// </summary>
        /// <param name="endIndexingDeltas"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected long CreateFinalTrackerOrderCode(IList<int> endIndexingDeltas, byte[] buffer)
        {
            var orderIndexing = new List<int>(endIndexingDeltas.Count);

            for (int i = 0; i < orderIndexing.Count; i++)
            {
                orderIndexing.Add(i + endIndexingDeltas[i]);
            }

            return Create64BitIndexCode(orderIndexing, buffer);
        }

        /// <summary>
        /// Converts the movement description of a transition rule into the end indexing delta value list
        /// </summary>
        /// <param name="transitionRule"></param>
        /// <returns></returns>
        protected IList<int> CreateEndIndexingDeltas(ITransitionRule transitionRule)
        {
            var index = 0;
            var result = new List<int>(transitionRule.PathLength).Populate(() => index++, transitionRule.PathLength);

            foreach (var (start, end) in transitionRule.GetMovementDescription().ConsecutivePairSelect((a,b) => (a,b)))
            {
                result.Swap(start, end);
            }

            for (int i = 0; i < result.Count; i++)
            {
                result[i] -= i;
            }

            return result;
        }

        /// <summary>
        /// Creates the set of rule inversions and sets the on the passed inverse metropolis transition model
        /// </summary>
        /// <param name="originalModel"></param>
        /// <param name="inverseModel"></param>
        protected void CreateAndAddRuleModelInversions(IMetropolisTransitionModel originalModel, IMetropolisTransitionModel inverseModel)
        {
            var inverseModels = originalModel.RuleModels.Select(CreateAndLinkInverseModel).ToList();
            inverseModel.RuleModels = inverseModels;
        }

        /// <summary>
        /// Creates a single inverted metropolis rule model and sets both inverted mappings to the appropriate value
        /// </summary>
        /// <param name="ruleModel"></param>
        /// <returns></returns>
        protected IMetropolisRuleModel CreateAndLinkInverseModel(IMetropolisRuleModel ruleModel)
        {
            var inverseModel = ruleModel.CreateInverse();
            ruleModel.InverseRuleModel = inverseModel;
            return inverseModel;
        }

        /// <summary>
        /// Takes a set of rule models and creates the mobile particle set
        /// </summary>
        /// <param name="ruleModels"></param>
        /// <returns></returns>
        protected IParticleSet CreateMobileParticleSet(IEnumerable<ITransitionRuleModel> ruleModels)
        {
            var uniqueParticles = ruleModels.Select(a => a.SelectableParticle).ToSetList();
            return new ParticleSet() { Index = -1, Particles = uniqueParticles.ToList() };
        }

        protected IList<IPositionTransitionModel> BuildPositionTransitionModels()
        {
            return null;
        }
    }
}
