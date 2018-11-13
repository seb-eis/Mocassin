using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IKineticTransitionModelBuilder" />
    public class KineticTransitionModelBuilder : TransitionModelBuilder, IKineticTransitionModelBuilder
    {
        /// <inheritdoc />
        public KineticTransitionModelBuilder(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <inheritdoc />
        public IList<IKineticTransitionModel> BuildModels(IList<IKineticTransition> transitions)
        {
            var resultModels = new List<IKineticTransitionModel>();
            var inverseModels = new List<IKineticTransitionModel>();

            var index = 0;
            foreach (var transition in transitions)
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
            foreach (var transitionModel in resultModels)
            {
                transitionModel.MobileParticles = CreateMobileParticleSet(transitionModel.RuleModels);
                transitionModel.EffectiveParticle = CreateEffectiveMobileParticle(transitionModel.MobileParticles);
            }

            return resultModels;
        }


        /// <summary>
        ///     Creates a single kinetic transition model with rule models and mapping models
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        protected IKineticTransitionModel CreateTransitionModel(IKineticTransition transition)
        {
            var transitionModel = new KineticTransitionModel
            {
                Transition = transition
            };

            CreateAndAddAbstractMovement(transitionModel);
            CreateAndAddMappingModels(transitionModel);
            CreateAndAddRuleModels(transitionModel);

            return transitionModel;
        }

        /// <summary>
        ///     Determines and creates an effective particle from the passed mobile particle set
        /// </summary>
        /// <param name="mobileParticles"></param>
        /// <remarks> Particle is for calculation purposes only and not part of the actual model data </remarks>
        protected IParticle CreateEffectiveMobileParticle(IParticleSet mobileParticles)
        {
            var comparer = ModelProject.CommonNumeric.RangeComparer;
            var first = mobileParticles.First();

            var effectiveParticle = new Particle {Index = -1, Name = first.Name, Symbol = first.Symbol, Charge = first.Charge};

            foreach (var particle in mobileParticles.Skip(1))
            {
                if (particle.IsEmpty)
                    continue;

                if (particle.IsVacancy && comparer.Compare(particle.Charge, 0.0) == 0)
                    continue;

                effectiveParticle.Charge += particle.Charge;
                effectiveParticle.Symbol += $"-{particle.Symbol}";
                effectiveParticle.Name += $"-{particle.Name}";
            }

            return SearchEffectiveParticleInModel(effectiveParticle, comparer) ?? effectiveParticle;
        }

        /// <summary>
        ///     Searches if the effective particle can be found in the model data. Returns null if none is found
        /// </summary>
        /// <param name="effectiveParticle"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        protected IParticle SearchEffectiveParticleInModel(IParticle effectiveParticle, IComparer<double> comparer)
        {
            var particles = ModelProject.GetManager<IParticleManager>().QueryPort.Query(port => port.GetParticles());
            foreach (var particle in particles)
            {
                if (particle.EqualsInModelProperties(effectiveParticle, comparer))
                    return particle;
            }

            return null;
        }

        /// <summary>
        ///     Creates and adds the abstract movement information to the transition model
        /// </summary>
        /// <param name="transitionModel"></param>
        protected void CreateAndAddAbstractMovement(IKineticTransitionModel transitionModel)
        {
            var unitCellProvider = ModelProject.GetManager<IStructureManager>()
                .QueryPort.Query(port => port.GetFullUnitCellProvider());

            var positions = transitionModel.Transition.GetGeometrySequence()
                .Select(vector => unitCellProvider.GetEntryValueAt(vector))
                .ToList();

            var connectors = transitionModel.Transition.AbstractTransition
                .GetConnectorSequence()
                .ToList();

            transitionModel.AbstractMovement = CalculateAbstractMovement(connectors, positions);
        }

        /// <summary>
        ///     Creates and adds the rule models and affiliated inversions to a kinetic transition model
        /// </summary>
        /// <param name="transitionModel"></param>
        protected void CreateAndAddRuleModels(IKineticTransitionModel transitionModel)
        {
            var comparer = ModelProject.CommonNumeric.RangeComparer;
            var ruleModels = transitionModel.Transition.GetExtendedTransitionRules()
                .Select(kineticRule => CreateRuleModel(kineticRule, comparer))
                .Action(ruleModel => ruleModel.TransitionModel = transitionModel)
                .ToList();

            CreateCodesAndLinkInverseRuleModels(ruleModels);

            transitionModel.RuleModels = ruleModels;
        }

        /// <summary>
        ///     Creates a new rule model from a transition rule ith the provided comparer
        /// </summary>
        /// <param name="kineticRule"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        protected IKineticRuleModel CreateRuleModel(IKineticRule kineticRule, IComparer<double> comparer)
        {
            var ruleModel = CreateRuleModelBasis<KineticRuleModel>(kineticRule);
            ruleModel.ChargeTransportMatrix = CreateChargeTransportMatrix(ruleModel.StartState, comparer);
            ruleModel.TransitionState = kineticRule.GetTransitionStateOccupation().ToList();
            ruleModel.KineticRule = kineticRule;
            return ruleModel;
        }

        /// <summary>
        ///     Create all contained kinetic transition mapping models an adds them to the transition model
        /// </summary>
        /// <param name="transitionModel"></param>
        protected void CreateAndAddMappingModels(IKineticTransitionModel transitionModel)
        {
            var manager = ModelProject.GetManager<ITransitionManager>();

            // Avoid implicit captured 'this' through local variable
            var index = transitionModel.Transition.Index;
            var mappings = manager.QueryPort.Query(port => port.GetKineticMappingList(index));

            transitionModel.MappingModels = mappings
                .Select(a => CreateMappingModel(a, transitionModel))
                .ToList();

            if (!transitionModel.MappingsContainInversion())
                return;

            LinkSelfConsistentMappingModels(transitionModel.MappingModels);
        }

        /// <summary>
        ///     Creates the mapping model for the passed kinetic mapping and kinetic transition model
        /// </summary>
        /// <param name="kineticMapping"></param>
        /// <param name="transitionModel"></param>
        /// <returns></returns>
        protected IKineticMappingModel CreateMappingModel(KineticMapping kineticMapping, IKineticTransitionModel transitionModel)
        {
            var mappingModel = new KineticMappingModel
            {
                Mapping = kineticMapping,
                TransitionModel = transitionModel
            };

            AddTransitionSequences(mappingModel);
            AddMovementMatrix(mappingModel, transitionModel.AbstractMovement);

            return mappingModel;
        }

        /// <summary>
        ///     Adds the 3D and 4D transition sequences to the passed mapping model
        /// </summary>
        /// <param name="mappingModel"></param>
        protected void AddTransitionSequences(IKineticMappingModel mappingModel)
        {
            mappingModel.TransitionSequence3D = new List<Fractional3D>(mappingModel.Mapping.PathLength - 1);
            mappingModel.TransitionSequence4D = new List<CrystalVector4D>(mappingModel.Mapping.PathLength - 1);

            for (var i = 1; i < mappingModel.Mapping.PathLength; i++)
            {
                var fractional = mappingModel.PositionSequence3D[i] - mappingModel.PositionSequence3D[0];
                mappingModel.TransitionSequence3D.Add(fractional);

                var crystal = mappingModel.PositionSequence4D[i] - mappingModel.PositionSequence4D[0];
                mappingModel.TransitionSequence4D.Add(crystal);
            }
        }

        /// <summary>
        ///     Adds the movement matrix to the passed mapping model using the provided abstract movement description
        /// </summary>
        /// <param name="mappingModel"></param>
        /// <param name="abstractMovement"></param>
        protected void AddMovementMatrix(IKineticMappingModel mappingModel, IList<int> abstractMovement)
        {
            var matrix = new Matrix2D(3, mappingModel.Mapping.PathLength, ModelProject.CommonNumeric.RangeComparer);
            for (var i = 0; i < matrix.Cols; i++)
            {
                var moveIndex = abstractMovement[i];
                var vector = mappingModel.PositionSequence3D[i + moveIndex] - mappingModel.PositionSequence3D[i];

                matrix[0, i] = vector.A;
                matrix[1, i] = vector.B;
                matrix[2, i] = vector.C;
            }

            mappingModel.PositionMovementMatrix = matrix;
        }

        /// <summary>
        ///     Calculates the abstract movement from a list of connector types and the list of affiliated unit cell positions
        /// </summary>
        /// <param name="connectorTypes"></param>
        /// <param name="positions"></param>
        /// <returns></returns>
        protected IList<int> CalculateAbstractMovement(IList<ConnectorType> connectorTypes, IList<IUnitCellPosition> positions)
        {
            var result = new List<int>(connectorTypes.Count);

            for (var i = 0; i < connectorTypes.Count;)
            {
                var currentSum = 0;
                while (i++ < connectorTypes.Count && connectorTypes[i - 1] == ConnectorType.Dynamic)
                {
                    currentSum++;
                    result.Add(1);
                }

                result.Add(-currentSum);
            }

            for (var i = 1; i < result.Count; i++)
            {
                if (positions[i].Status != PositionStatus.Unstable || connectorTypes[i - 1] != ConnectorType.Dynamic)
                    continue;

                result[i - 1]++;
                result[i] = 0;
            }

            return result;
        }

        /// <summary>
        ///     Links a self consistent set of mapping models (Mappings that contain their own inversions)
        /// </summary>
        /// <param name="mappingModels"></param>
        protected void LinkSelfConsistentMappingModels(IList<IKineticMappingModel> mappingModels)
        {
            for (var i = 0; i < mappingModels.Count; i++)
            {
                if (mappingModels[i].InverseIsSet)
                    continue;

                for (var j = i; j < mappingModels.Count; j++)
                {
                    if (mappingModels[j].InverseIsSet)
                        continue;

                    if (mappingModels[i].LinkIfInverseMatch(mappingModels[j]))
                        break;
                }
            }
        }

        /// <summary>
        ///     Creates an inverse kinetic transition model if required or if the mappings already contain
        ///     the inversion the original is returned
        /// </summary>
        /// <param name="transitionModel"></param>
        /// <returns></returns>
        protected IKineticTransitionModel CreateTransitionModelInversion(IKineticTransitionModel transitionModel)
        {
            if (transitionModel.MappingsContainInversion())
                return transitionModel;

            var inverseModel = transitionModel.CreateInverse();
            CreateAndAddMappingModelInversions(transitionModel, inverseModel);
            CreateAndAddRuleModelInversions(transitionModel, inverseModel);

            return inverseModel;
        }

        /// <summary>
        ///     Create and add the inverted rules from the source model to the inverted target model
        /// </summary>
        /// <param name="transitionModel"></param>
        /// <param name="inverseModel"></param>
        protected void CreateAndAddRuleModelInversions(IKineticTransitionModel transitionModel, IKineticTransitionModel inverseModel)
        {
            var inverseModels = transitionModel.RuleModels.Select(CreateAndLinkInverseModel).ToList();
            inverseModel.RuleModels = inverseModels;
        }

        /// <summary>
        ///     Create and add the inverted rules from the source model to the inverted target model
        /// </summary>
        /// <param name="transitionModel"></param>
        /// <param name="inverseModel"></param>
        protected void CreateAndAddMappingModelInversions(IKineticTransitionModel transitionModel, IKineticTransitionModel inverseModel)
        {
            var inverseModels = transitionModel.MappingModels.Select(CreateAndLinkInverseModel).ToList();
            inverseModel.MappingModels = inverseModels;
        }

        /// <summary>
        ///     Creates the inverse kinetic transition rule model and links it to the source model
        /// </summary>
        /// <param name="ruleModel"></param>
        /// <returns></returns>
        protected IKineticRuleModel CreateAndLinkInverseModel(IKineticRuleModel ruleModel)
        {
            var inverseModel = ruleModel.CreateInverse();
            ruleModel.InverseRuleModel = inverseModel;
            return inverseModel;
        }

        /// <summary>
        ///     Creates and the inverse kinetic transition mapping and links it to the source model
        /// </summary>
        /// <param name="mappingModel"></param>
        /// <returns></returns>
        protected IKineticMappingModel CreateAndLinkInverseModel(IKineticMappingModel mappingModel)
        {
            var inverseModel = mappingModel.CreateInverse();
            mappingModel.InverseMapping = inverseModel;
            return inverseModel;
        }
    }
}