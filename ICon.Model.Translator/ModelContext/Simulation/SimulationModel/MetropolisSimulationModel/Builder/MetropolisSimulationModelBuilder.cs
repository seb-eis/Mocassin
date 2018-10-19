using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.Model.Simulations;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IMetropolisSimulationModelBuilder" />
    public class MetropolisSimulationModelBuilder : ModelBuilderBase, IMetropolisSimulationModelBuilder
    {
        /// <inheritdoc />
        public MetropolisSimulationModelBuilder(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <inheritdoc />
        public IList<IMetropolisSimulationModel> BuildModels(IEnumerable<IMetropolisSimulation> metropolisSimulations)
        {
            return metropolisSimulations
                .Select(a => new MetropolisSimulationModel {Simulation = a})
                .Action(a => a.TransitionModels = CreateTransitionModelPlaceholders(a.Simulation.Transitions))
                .Cast<IMetropolisSimulationModel>()
                .ToList();
        }

        /// <inheritdoc />
        public void BuildLinkingDependentComponents(IEnumerable<IMetropolisSimulationModel> simulationModels)
        {
            foreach (var simulationModel in simulationModels)
                simulationModel.MappingAssignMatrix = CreateMappingAssignMatrix(simulationModel.TransitionModels);
        }

        /// <summary>
        ///     Takes a sequence of metropolis transitions and creates placeholder transition models for the later linking process
        /// </summary>
        /// <param name="transitions"></param>
        /// <returns></returns>
        protected IList<IMetropolisTransitionModel> CreateTransitionModelPlaceholders(IEnumerable<IMetropolisTransition> transitions)
        {
            return transitions
                .Select(a => new MetropolisTransitionModel {Transition = a})
                .Cast<IMetropolisTransitionModel>()
                .ToList();
        }

        /// <summary>
        ///     Creates the mapping assign matrix for the passed list of metropolis transition models
        /// </summary>
        /// <param name="transitionModels"></param>
        /// <returns></returns>
        protected IMetropolisMappingModel[,,] CreateMappingAssignMatrix(IList<IMetropolisTransitionModel> transitionModels)
        {
            var listResult = new List<IList<IList<IMetropolisMappingModel>>>();

            foreach (var transitionModel in transitionModels)
            {
                foreach (var particle in transitionModel.MobileParticles)
                    InsertMappingModelsIntoRawMatrix(listResult, transitionModel.MappingModels, particle);
            }

            return ConvertRawAssignListsToMatrix(listResult);
        }

        /// <summary>
        ///     Inserts the passed list of mapping models into the list based raw mapping matrix
        /// </summary>
        /// <param name="rawMatrix"></param>
        /// <param name="mappingModels"></param>
        /// <param name="particle"></param>
        protected void InsertMappingModelsIntoRawMatrix(IList<IList<IList<IMetropolisMappingModel>>> rawMatrix,
            IList<IMetropolisMappingModel> mappingModels, IParticle particle)
        {
            var maxP = mappingModels.Select(a => a.StartVector4D.P).Max();
            while (rawMatrix.Count <= maxP)
                rawMatrix.Add(new List<IList<IMetropolisMappingModel>>(particle.Index + 1));

            foreach (var list in rawMatrix.Where(a => a.Count <= particle.Index))
            {
                while (list.Count <= particle.Index)
                    list.Add(new List<IMetropolisMappingModel>());
            }

            foreach (var mappingModel in mappingModels) rawMatrix[mappingModel.StartVector4D.P][particle.Index].Add(mappingModel);
        }

        /// <summary>
        ///     Converts the raw list based mapping assign matrix into a fixed size 3D array and fills the placeholder spots with
        ///     an invalid mapping
        /// </summary>
        /// <param name="rawMatrix"></param>
        /// <returns></returns>
        protected IMetropolisMappingModel[,,] ConvertRawAssignListsToMatrix(IList<IList<IList<IMetropolisMappingModel>>> rawMatrix)
        {
            var (sizeA, sizeB, sizeC) = GetRawAssignListSizeInfo(rawMatrix);
            var matrix = new IMetropolisMappingModel[sizeA, sizeB, sizeC];

            for (var a = 0; a < rawMatrix.Count; a++)
            {
                for (var b = 0; b < rawMatrix[a].Count; b++)
                {
                    for (var c = 0; c < rawMatrix[a][b].Count; c++)
                        matrix[a, b, c] = rawMatrix[a][b][c];
                }
            }

            return matrix;
        }

        /// <summary>
        ///     Get the size information in three dimensions for the list based mapping assign matrix
        /// </summary>
        /// <param name="rawMatrix"></param>
        /// <returns></returns>
        protected (int, int, int) GetRawAssignListSizeInfo(IList<IList<IList<IMetropolisMappingModel>>> rawMatrix)
        {
            return (rawMatrix.Count, rawMatrix[0].Count, rawMatrix.SelectMany(a => a.Select(b => b).Select(c => c.Count)).Max());
        }
    }
}