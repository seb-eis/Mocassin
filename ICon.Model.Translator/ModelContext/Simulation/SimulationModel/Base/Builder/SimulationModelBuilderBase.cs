using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Base class for the simulation model builder that supplies shared generic simulation model build functionality
    /// </summary>
    public abstract class SimulationModelBuilderBase : ModelBuilderBase
    {
        /// <inheritdoc />
        protected SimulationModelBuilderBase(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <summary>
        ///     Creates the mapping assign matrix for the passed set of transition models using the set generic type
        ///     specializations
        /// </summary>
        /// <param name="transitionModels"></param>
        /// <returns></returns>
        protected TMappingModel[,,] CreateMappingAssignMatrix<TMappingModel, TTransitionModel>(
            IEnumerable<TTransitionModel> transitionModels)
            where TMappingModel : ITransitionMappingModel
            where TTransitionModel : ITransitionModel
        {
            var listResult = new List<IList<IList<TMappingModel>>>();

            foreach (var transitionModel in transitionModels)
            {
                var mappingModels = transitionModel.GetMappingModels()
                    .Cast<TMappingModel>()
                    .ToList();

                foreach (var particle in transitionModel.SelectableParticles)
                    InsertMappingModelsIntoRawMatrix(listResult, mappingModels, particle);
            }

            return ConvertRawAssignListsToMatrix(listResult);
        }

        /// <summary>
        ///     Inserts the passed list of mapping models into the list based raw mapping matrix
        /// </summary>
        /// <param name="rawMatrix"></param>
        /// <param name="mappingModels"></param>
        /// <param name="particle"></param>
        protected void InsertMappingModelsIntoRawMatrix<TMappingModel>(IList<IList<IList<TMappingModel>>> rawMatrix,
            IList<TMappingModel> mappingModels, IParticle particle)
            where TMappingModel : ITransitionMappingModel
        {
            var maxP = mappingModels.Select(a => a.StartVector4D.P).Max();
            while (rawMatrix.Count <= maxP)
                rawMatrix.Add(new List<IList<TMappingModel>>(particle.Index + 1));

            foreach (var list in rawMatrix.Where(a => a.Count <= particle.Index))
            {
                while (list.Count <= particle.Index)
                    list.Add(new List<TMappingModel>());
            }

            foreach (var mappingModel in mappingModels)
                rawMatrix[mappingModel.StartVector4D.P][particle.Index].Add(mappingModel);
        }

        /// <summary>
        ///     Converts the raw list based mapping assign matrix into a fixed size 3D array and fills the placeholder spots with
        ///     an invalid mapping
        /// </summary>
        /// <param name="rawMatrix"></param>
        /// <returns></returns>
        protected TMappingModel[,,] ConvertRawAssignListsToMatrix<TMappingModel>(IList<IList<IList<TMappingModel>>> rawMatrix)
            where TMappingModel : ITransitionMappingModel
        {
            var (sizeA, sizeB, sizeC) = GetRawAssignListSizeInfo(rawMatrix);
            var matrix = new TMappingModel[sizeA, sizeB, sizeC];

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
        protected (int, int, int) GetRawAssignListSizeInfo<TMappingModel>(IList<IList<IList<TMappingModel>>> rawMatrix)
            where TMappingModel : ITransitionMappingModel
        {
            return (rawMatrix.Count, rawMatrix[0].Count, rawMatrix.SelectMany(a => a.Select(b => b).Select(c => c.Count)).Max());
        }

        /// <summary>
        ///     Creates the indexing dictionary for the mapping models of the passed transition model sequence
        /// </summary>
        /// <param name="transitionModels"></param>
        /// <returns></returns>
        protected IDictionary<ITransitionMappingModel, int> GetTransitionMappingIndexing(IEnumerable<ITransitionModel> transitionModels)
        {
            var index = 0;
            return transitionModels.SelectMany(x => x.GetMappingModels()).ToDictionary(model => model, model => index++);
        }

        /// <summary>
        ///     Creates the indexing dictionary for the passed sequence of kinetic transition models
        /// </summary>
        /// <param name="transitionModels"></param>
        /// <returns></returns>
        protected IDictionary<ITransitionModel, int> GetTransitionModelIndexing(IEnumerable<ITransitionModel> transitionModels)
        {
            var index = 0;
            return transitionModels.ToDictionary(model => model, model => index++);
        }

        /// <summary>
        ///     Translates a 3D transition mapping assign matrix into the 2D jump count table and checks if position id + particle
        ///     id without jumps have a passive involvement in jumps
        /// </summary>
        /// <param name="mappingAssignMatrix"></param>
        /// <param name="jumpModels"></param>
        /// <returns></returns>
        protected int[,] GetJumpCountTable<TMappingModel, TJumpModel>(TMappingModel[,,] mappingAssignMatrix, IList<TJumpModel> jumpModels)
            where TMappingModel : ITransitionMappingModel
            where TJumpModel : ILocalJumpModel
        {
            var (rows, cols) = (mappingAssignMatrix.GetLength(0), mappingAssignMatrix.GetLength(1));
            var jumpCountTable = new int[rows, cols];

            for (var positionId = 0; positionId < rows; positionId++)
            {
                for (var particleId = 0; particleId < cols; particleId++)
                {
                    for (var i = 0; i < mappingAssignMatrix.GetLength(2); i++)
                    {
                        if (mappingAssignMatrix[positionId, particleId, i] != null)
                        {
                            jumpCountTable[positionId, particleId]++;
                            continue;
                        }

                        if (jumpCountTable[positionId, particleId] == 0)
                        {
                            jumpCountTable[positionId, particleId] =
                                jumpModels.Any(x => x.GetMobilityType(positionId, particleId) != MobilityType.Immobile)
                                    ? SimulationConstants.JumpCountIfPassivelyMobile
                                    : SimulationConstants.JumpCountIfNotMobile;
                        }

                        break;
                    }
                }
            }

            return jumpCountTable;
        }


        /// <summary>
        ///     Creates the jump index assign table on the passed kinetic indexing model using the provided kinetic mapping assign
        ///     matrix
        /// </summary>
        /// <param name="encodingModel"></param>
        /// <param name="mappingAssignMatrix"></param>
        protected void AddJumpIndexAssignTable<TMappingModel>(ISimulationEncodingModel encodingModel, TMappingModel[,,] mappingAssignMatrix)
            where TMappingModel : ITransitionMappingModel
        {
            var (dim0, dim1, dim2) = (mappingAssignMatrix.GetLength(0), mappingAssignMatrix.GetLength(1), mappingAssignMatrix.GetLength(2));
            var result = new int[dim0, dim1, dim2];
            for (var positionId = 0; positionId < dim0; positionId++)
            {
                for (var particleId = 0; particleId < dim1; particleId++)
                {
                    for (var i = 0; i < dim2; i++)
                    {
                        var mappingModel = mappingAssignMatrix[positionId, particleId, i];
                        if (mappingModel == null)
                            result[positionId, particleId, i] = SimulationConstants.InvalidId;
                        else
                            result[positionId, particleId, i] = encodingModel.TransitionMappingToJumpDirectionId[mappingModel];
                    }
                }
            }

            encodingModel.JumpIndexAssignTable = result;
        }

        /// <summary>
        ///     Adds the filed influence mappings from the passed set of local jump models to the simulation encoding model
        /// </summary>
        /// <param name="encodingModel"></param>
        /// <param name="jumpModels"></param>
        protected void AddElectricFieldInfluenceMappings(ISimulationEncodingModel encodingModel, IEnumerable<ILocalJumpModel> jumpModels)
        {
            if (!(jumpModels is IReadOnlyCollection<ILocalJumpModel> jumpModelList))
                jumpModelList = jumpModels.ToList();

            var directionFactors = GetUniqueDirectionFieldFactors(jumpModelList);
            var ruleFactors = GetUniqueRuleFieldFactors(jumpModelList);

            encodingModel.TransitionRuleToElectricFieldFactors = ruleFactors;
            encodingModel.TransitionMappingToElectricFieldFactors = directionFactors;
        }

        /// <summary>
        ///     Filters the passed set of local jump models for the unique jump direction field influences and returns them as a
        ///     dictionary
        /// </summary>
        /// <param name="jumpModels"></param>
        /// <returns></returns>
        /// <remarks>
        ///     Ensures that the model context building system did not produce any inconsistent jump field influence
        ///     information
        /// </remarks>
        protected IDictionary<ITransitionMappingModel, double> GetUniqueDirectionFieldFactors(IEnumerable<ILocalJumpModel> jumpModels)
        {
            var comparer = ModelProject.CommonNumeric.RangeComparer;
            var result = new Dictionary<ITransitionMappingModel, double>();

            foreach (var jumpModel in jumpModels)
            {
                if (!result.TryGetValue(jumpModel.MappingModelBase, out var currentValue))
                {
                    result.Add(jumpModel.MappingModelBase, jumpModel.ElectricFieldMappingFactor);
                    continue;
                }

                var currentIsZero = comparer.Compare(currentValue, 0.0) == 0;
                var otherIsZero = comparer.Compare(jumpModel.ElectricFieldMappingFactor, 0.0) == 0;

                if (currentIsZero && !otherIsZero)
                {
                    result[jumpModel.MappingModelBase] = jumpModel.ElectricFieldMappingFactor;
                    continue;
                }

                if (!currentIsZero && !otherIsZero && comparer.Compare(currentValue, jumpModel.ElectricFieldMappingFactor) != 0)
                    throw new InvalidOperationException("The jump model set contains inconsistent mapping field influence factors");
            }

            return result;
        }

        /// <summary>
        ///     Filters the passed set of local jump models for the unique jump rule field influences and returns them as a
        ///     dictionary
        /// </summary>
        /// <param name="jumpModels"></param>
        /// <returns></returns>
        /// <remarks>
        ///     Ensures that the model context building system did not produce any inconsistent jump field influence
        ///     information
        /// </remarks>
        protected IDictionary<ITransitionRuleModel, double> GetUniqueRuleFieldFactors(IEnumerable<ILocalJumpModel> jumpModels)
        {
            var comparer = ModelProject.CommonNumeric.RangeComparer;
            var result = new Dictionary<ITransitionRuleModel, double>();

            foreach (var jumpModel in jumpModels)
            {
                if (!result.TryGetValue(jumpModel.RuleModelBase, out var currentValue))
                {
                    result.Add(jumpModel.RuleModelBase, jumpModel.ElectricFieldRuleInfluence);
                    continue;
                }

                var currentIsZero = comparer.Compare(currentValue, 0.0) == 0;
                var otherIsZero = comparer.Compare(jumpModel.ElectricFieldRuleInfluence, 0.0) == 0;

                if (currentIsZero && !otherIsZero)
                {
                    result[jumpModel.RuleModelBase] = jumpModel.ElectricFieldRuleInfluence;
                    continue;
                }

                if (!currentIsZero && !otherIsZero && comparer.Compare(currentValue, jumpModel.ElectricFieldRuleInfluence) != 0)
                    throw new InvalidOperationException("The jump model set contains inconsistent rule field influence factors");
            }

            return result;
        }

        /// <summary>
        /// Creates the mapping dictionary that assigns each possible position index its set of mobility information
        /// </summary>
        /// <param name="jumpModels"></param>
        /// <returns></returns>
        protected IDictionary<int, MobilityType[]> GetPositionIndexToMobilitySetMapping(IEnumerable<ILocalJumpModel> jumpModels)
        {
            var result = new Dictionary<int, MobilityType[]>();

            var particleCount = ModelProject.GetManager<IParticleManager>().QueryPort
                .Query(port => port.GetParticles().Count);

            var positionCount = ModelProject.GetManager<IStructureManager>().QueryPort
                .Query(port => port.GetLinearizedExtendedPositionCount());

            for (var i = 0; i < positionCount; i++)
            {
                result.Add(i, new MobilityType[particleCount]);
            }

            foreach (var jumpModel in jumpModels)
            {
                for (var positionId = 0; positionId < positionCount; positionId++)
                {
                    var mobilitySet = result[positionId];
                    for (var particleId = 0; particleId < particleCount; particleId++)
                    {
                        var mobilityType = jumpModel.GetMobilityType(positionId, particleId);
                        if (mobilityType > mobilitySet[particleId])
                            mobilitySet[particleId] = mobilityType;
                    }
                }
            }

            return result;
        }
    }
}