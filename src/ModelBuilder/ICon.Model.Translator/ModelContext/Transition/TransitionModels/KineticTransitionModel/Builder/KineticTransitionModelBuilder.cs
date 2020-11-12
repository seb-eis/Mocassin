using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.Solver;
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
                if (TryLinkTransitionModelToExistingSets(transitionModel, resultModels)) continue;
                if (transitionModel.RuleModels.Any(x => (x.KineticRule.MovementFlags & RuleMovementFlags.HasChainedMovement) != 0)) continue;
                var inverseModel = CreateGeometricModelInversion(transitionModel);

                inverseModel.ModelId = index++;
                inverseModels.Add(inverseModel);
            }

            resultModels.AddRange(inverseModels);
            foreach (var transitionModel in resultModels)
            {
                AddBasicMobilityInformation(transitionModel);
                transitionModel.EffectiveParticle = CreateOrFindEffectiveMobileParticle(transitionModel.GetMobileParticles());
            }

            return resultModels;
        }

        /// <summary>
        ///     Checks if the user has defined a <see cref="IKineticTransitionModel" /> that can be used as the inverse
        /// </summary>
        /// <param name="transitionModel"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        protected bool TryLinkTransitionModelToExistingSets(IKineticTransitionModel transitionModel, IList<IKineticTransitionModel> models)
        {
            if (models.FirstOrDefault(x => x.InverseTransitionModel == transitionModel) != null) return true;
            if (transitionModel.MappingsContainInversion())
            {
                transitionModel.InverseTransitionModel = transitionModel;
                return true;
            }

            foreach (var otherModel in models.Where(x => x.InverseTransitionModel == null && x != transitionModel && x.Transition.GeometryStepCount == transitionModel.Transition.GeometryStepCount))
            {
                var pseudoSet = transitionModel.MappingModels.Concat(otherModel.MappingModels).ToList();
                var linkSuccess = TryLinkSelfConsistentMappingModels(pseudoSet);
                if (!linkSuccess) continue;

                transitionModel.InverseTransitionModel = otherModel;
                otherModel.InverseTransitionModel = transitionModel;
                return true;
            }

            return false;
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
        protected IParticle CreateOrFindEffectiveMobileParticle(IEnumerable<IParticle> mobileParticles)
        {
            var comparer = ModelProject.CommonNumeric.RangeComparer;
            var particleList = mobileParticles.AsCollection();
            var first = particleList.First();

            var effectiveParticle = new Particle {Index = -1, Name = first.Name, Symbol = first.Symbol, Charge = first.Charge};

            foreach (var particle in particleList.Skip(1))
            {
                if (particle.IsVoid)
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
            var particles = ModelProject.Manager<IParticleManager>().DataAccess.Query(port => port.GetParticles());
            return particles.FirstOrDefault(particle => particle.EqualsInModelProperties(effectiveParticle, comparer));
        }

        /// <summary>
        ///     Creates and adds the abstract movement information to the transition model
        /// </summary>
        /// <param name="transitionModel"></param>
        protected void CreateAndAddAbstractMovement(IKineticTransitionModel transitionModel)
        {
            var unitCellProvider = ModelProject.Manager<IStructureManager>()
                                               .DataAccess.Query(port => port.GetFullUnitCellProvider());

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

            CreateCodesAndLinkLogicRuleInversions(ruleModels);
            AddRuleDirectionInformation(ruleModels);
            transitionModel.RuleModels = ruleModels;
        }

        /// <summary>
        ///     Determine and add the rule model direction information to the passed rule models in the context of their set
        ///     transition model
        /// </summary>
        /// <param name="ruleModels"></param>
        protected void AddRuleDirectionInformation(IEnumerable<IKineticRuleModel> ruleModels)
        {
            var solver = new PointMechanicsSolver();
            var comparer = ModelProject.GeometryNumeric.RangeComparer;
            var vectorEncoder = ModelProject.Manager<IStructureManager>().DataAccess.Query(port => port.GetVectorEncoder());

            // @ToDo: Use a more stable way of determining the rule direction, can currently fail on chained transitions
            foreach (var ruleModel in ruleModels)
            {
                var distanceShift = GetChargeFocalPointDistanceShift(ruleModel, solver, vectorEncoder.Transformer);

                if (distanceShift > 0)
                    ruleModel.RuleDirectionValue = SimulationConstants.PositiveRuleDirectionFactor;

                if (distanceShift < 0)
                    ruleModel.RuleDirectionValue = SimulationConstants.NegativeRuleDirectionFactor;

                if (comparer.Compare(distanceShift, 0.0) == 0)
                    ruleModel.RuleDirectionValue = SimulationConstants.UndefinableRuleDirectionFactor;
            }
        }

        /// <summary>
        ///     Determines the absolute distance shift of the charge focal point from the transition origin in the context of the
        ///     given transition rule model
        /// </summary>
        /// <param name="ruleModel"></param>
        /// <param name="solver"></param>
        /// <param name="transformer"></param>
        /// <returns></returns>
        protected double GetChargeFocalPointDistanceShift(IKineticRuleModel ruleModel, PointMechanicsSolver solver,
            IVectorTransformer transformer)
        {
            var geometry = transformer.ToCartesian(ruleModel.TransitionModel.Transition.GetGeometrySequence())
                                      .ToList();

            var origin = geometry.First();

            var startChargeMassPoints = geometry
                .Zip(ruleModel.StartState.Select(x => x.Index), (vector, mass) => new CartesianMassPoint3D(mass, vector));

            var finalChargeMassPoints = geometry
                .Zip(ruleModel.FinalState.Select(x => x.Index), (vector, mass) => new CartesianMassPoint3D(mass, vector));

            var startFocalPoint = solver.GetMassCenter(startChargeMassPoints, ModelProject.GeometryNumeric.RangeComparer);
            var endFocalPoint = solver.GetMassCenter(finalChargeMassPoints, ModelProject.GeometryNumeric.RangeComparer);

            return (endFocalPoint - origin).GetLength() - (startFocalPoint - origin).GetLength();
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
            var manager = ModelProject.Manager<ITransitionManager>();

            // Avoid implicit captured 'this' through local variable
            var index = transitionModel.Transition.Index;
            var mappings = manager.DataAccess.Query(port => port.GetKineticMappingList(index));

            transitionModel.MappingModels = mappings
                                            .Select(a => CreateMappingModel(a, transitionModel))
                                            .ToList();

            if (!transitionModel.MappingsContainInversion()) return;

            var linkSuccess = TryLinkSelfConsistentMappingModels(transitionModel.MappingModels);
            if (!linkSuccess) throw new InvalidOperationException("Failed to link a set of kinetic mapping models previously marked as self consistent.");
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
            mappingModel.TransitionSequence4D = new List<Vector4I>(mappingModel.Mapping.PathLength - 1);

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
        protected IList<int> CalculateAbstractMovement(IList<ConnectorType> connectorTypes, IList<ICellSite> positions)
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
                if (positions[i].Stability != PositionStability.Unstable || connectorTypes[i - 1] != ConnectorType.Dynamic)
                    continue;

                result[i - 1]++;
                result[i] = 0;
            }

            return result;
        }

        /// <summary>
        ///     Links a self consistent set of mapping models (Mappings that contain their own inversions) with an optional option
        ///     to restore the original linking on failure
        /// </summary>
        /// <param name="mappingModels"></param>
        /// <param name="restoreOriginalOnFailure"></param>
        protected bool TryLinkSelfConsistentMappingModels(IList<IKineticMappingModel> mappingModels, bool restoreOriginalOnFailure = true)
        {
            var remaining = mappingModels.Count;
            var backup = restoreOriginalOnFailure ? mappingModels.Select(x => x.InverseMapping).ToList() : null;
            for (var i = 0; i < mappingModels.Count; i++)
            {
                if (mappingModels[i].InverseIsSet)
                    continue;

                for (var j = i; j < mappingModels.Count; j++)
                {
                    if (mappingModels[j].InverseIsSet)
                        continue;

                    if (!mappingModels[i].LinkIfGeometricInversion(mappingModels[j])) continue;
                    remaining -= 2;
                    break;
                }
            }

            if (remaining == 0) return true;
            if (!restoreOriginalOnFailure) return false;
            for (var i = 0; i < mappingModels.Count; i++) mappingModels[i].InverseMapping = backup[i];
            return false;
        }

        /// <summary>
        ///     Creates an inverse kinetic transition model for the passed one
        /// </summary>
        /// <param name="transitionModel"></param>
        /// <returns></returns>
        protected IKineticTransitionModel CreateGeometricModelInversion(IKineticTransitionModel transitionModel)
        {
            var inverseModel = transitionModel.CreateGeometricInverse();
            transitionModel.InverseTransitionModel = inverseModel;

            AddGeometricMappingModelInversions(transitionModel, inverseModel);
            AddGeometricRuleModelInversions(transitionModel, inverseModel);

            return inverseModel;
        }

        /// <summary>
        ///     Create and add the inverted rules from the source model to the inverted target model
        /// </summary>
        /// <param name="transitionModel"></param>
        /// <param name="inverseModel"></param>
        protected void AddGeometricRuleModelInversions(IKineticTransitionModel transitionModel, IKineticTransitionModel inverseModel)
        {
            var inverseModels = transitionModel.RuleModels.Select(CreateGeometricInversion).ToList();
            CreateCodesAndLinkLogicRuleInversions(inverseModels);
            inverseModel.RuleModels = inverseModels;
        }

        /// <summary>
        ///     Create and add the inverted rules from the source model to the inverted target model
        /// </summary>
        /// <param name="transitionModel"></param>
        /// <param name="inverseModel"></param>
        protected void AddGeometricMappingModelInversions(IKineticTransitionModel transitionModel, IKineticTransitionModel inverseModel)
        {
            var inverseModels = transitionModel.MappingModels.Select(CreateAndLinkGeometricInversion).ToList();
            inverseModel.MappingModels = inverseModels;
        }

        /// <summary>
        ///     Creates the geometric inversion of the passed kinetic transition rule model
        /// </summary>
        /// <param name="ruleModel"></param>
        /// <returns></returns>
        protected IKineticRuleModel CreateGeometricInversion(IKineticRuleModel ruleModel)
        {
            var inverseModel = ruleModel.CreateGeometricInversion();
            return inverseModel;
        }

        /// <summary>
        ///     Creates and the inverse kinetic transition mapping and links it to the source model
        /// </summary>
        /// <param name="mappingModel"></param>
        /// <returns></returns>
        protected IKineticMappingModel CreateAndLinkGeometricInversion(IKineticMappingModel mappingModel)
        {
            var inverseModel = mappingModel.CreateGeometricInversion();
            mappingModel.InverseMapping = inverseModel;
            return inverseModel;
        }
    }
}