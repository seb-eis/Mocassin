using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Energies;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IEnvironmentModelBuilder" />
    public class EnvironmentModelBuilder : ModelBuilderBase, IEnvironmentModelBuilder
    {
        /// <summary>
        ///     Get or set the position pair interaction dictionary
        ///     that assigns each unit cell position its existing pair interactions
        /// </summary>
        protected IReadOnlyDictionary<ICellSite, IReadOnlyList<IPairInteraction>> PositionPairInteractions { get; set; }

        /// <summary>
        ///     Get or set the position group interaction dictionary
        ///     that assigns each unit cell position its existing group interactions
        /// </summary>
        protected IReadOnlyDictionary<ICellSite, IReadOnlyList<IGroupInteraction>> PositionGroupInteractions { get; set; }

        /// <summary>
        ///     The position group infos for all existing group interactions
        /// </summary>
        protected IReadOnlyList<IPositionGroupInfo> PositionGroupInfos { get; set; }

        /// <summary>
        ///     The unit cell vector encoder to transform cell vectors
        /// </summary>
        protected IUnitCellVectorEncoder VectorEncoder { get; set; }

        /// <inheritdoc />
        public EnvironmentModelBuilder(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <inheritdoc />
        public IList<IEnvironmentModel> BuildModels(IEnumerable<ICellSite> cellReferencePositions)
        {
            LoadBuildDataFromProject();

            var index = 0;
            return cellReferencePositions.Select(BuildEnvironmentModel)
                .Action(a => a.ModelId = index++)
                .ToList();
        }

        /// <summary>
        ///     Loads the required shared build data from the reference project managers
        /// </summary>
        protected void LoadBuildDataFromProject()
        {
            var manager = ModelProject.Manager<IEnergyManager>();
            VectorEncoder = ModelProject.Manager<IStructureManager>().DataAccess.Query(port => port.GetVectorEncoder());
            PositionPairInteractions = manager.DataAccess.Query(port => port.GetPositionPairInteractions());
            PositionGroupInteractions = manager.DataAccess.Query(port => port.GetPositionGroupInteractions());
            PositionGroupInfos = manager.DataAccess.Query(port => port.GetPositionGroupInfos());
        }

        /// <summary>
        ///     Builds a single environment model
        /// </summary>
        /// <param name="cellSite"></param>
        /// <returns></returns>
        protected IEnvironmentModel BuildEnvironmentModel(ICellSite cellSite)
        {
            var environmentModel = new EnvironmentModel
            {
                CellSite = cellSite,
                TransformOperations = ModelProject.SpaceGroupService.GetOperationDictionary(cellSite.Vector)
            };

            AddPairInteractionModels(environmentModel);
            AddGroupInteractionModels(environmentModel);
            return environmentModel;
        }

        /// <summary>
        ///     Creates and adds all existing pair interaction models to the environment model
        /// </summary>
        /// <param name="environmentModel"></param>
        protected void AddPairInteractionModels(IEnvironmentModel environmentModel)
        {
            if (!PositionPairInteractions.TryGetValue(environmentModel.CellSite, out var interactions))
                throw new InvalidOperationException("Cannot resolve pair interactions for the environment model");

            var multiplicityOperations = ModelProject.SpaceGroupService
                .GetOperationsNotShiftingOrigin(environmentModel.CellSite.Vector, true);

            var index = 0;
            var pairInteractionModels = interactions
                .Select(a => CreatePairInteractionModel(a, environmentModel))
                .SelectMany(a => ExtendPairInteractionModel(a, multiplicityOperations))
                .Action(a => a.ModelId = index++)
                .ToList();

            environmentModel.PairInteractionModels = pairInteractionModels;
        }

        /// <summary>
        ///     Creates a single pair interaction model from the passed interaction and the environment model it belongs to
        /// </summary>
        /// <param name="pairInteraction"></param>
        /// <param name="environmentModel"></param>
        /// <returns></returns>
        protected IPairInteractionModel CreatePairInteractionModel(IPairInteraction pairInteraction, IEnvironmentModel environmentModel)
        {
            var pairModel = new PairInteractionModel
            {
                EnvironmentModel = environmentModel,
                PairEnergyModel = new PairEnergyModel(pairInteraction),
                TargetPositionInfo = new TargetPositionInfo()
            };
            pairModel.TargetPositionInfo.PairInteractionModel = pairModel;

            if (pairInteraction.Position0 != environmentModel.CellSite)
                SetInteractionDataAsInverted(pairModel, pairInteraction);

            else
                SetInteractionData(pairModel, pairInteraction);

            return pairModel;
        }

        /// <summary>
        ///     Sets the pair interaction model info in normal forward cases from the passed pair interaction
        /// </summary>
        /// <param name="pairModel"></param>
        /// <param name="pairInteraction"></param>
        protected void SetInteractionData(IPairInteractionModel pairModel, IPairInteraction pairInteraction)
        {
            var targetInfo = pairModel.TargetPositionInfo;

            targetInfo.Distance = pairInteraction.Distance;
            targetInfo.CellSite = pairInteraction.Position1;
            targetInfo.AbsoluteFractional = pairInteraction.SecondPositionVector;
            targetInfo.RelativeFractional = targetInfo.AbsoluteFractional - pairInteraction.Position0.Vector;
            targetInfo.AbsoluteCartesian = VectorEncoder.Transformer.ToCartesian(targetInfo.AbsoluteFractional);

            if (!VectorEncoder.TryEncodeAsRelative(pairInteraction.Position0.Vector, targetInfo.RelativeFractional, out var relative4D))
                throw new InvalidOperationException("Could not create valid relative 4D interaction target");

            targetInfo.RelativeCrystalVector = relative4D;
        }

        /// <summary>
        ///     Sets the pair interaction model info in inverted cases where the interaction was created from the target cell
        ///     position
        /// </summary>
        /// <param name="pairModel"></param>
        /// <param name="pairInteraction"></param>
        protected void SetInteractionDataAsInverted(IPairInteractionModel pairModel, IPairInteraction pairInteraction)
        {
            var targetInfo = pairModel.TargetPositionInfo;
            var positionPair = new[] {pairInteraction.SecondPositionVector, pairInteraction.Position0.Vector};
            var invertedPair = ModelProject.SpaceGroupService
                .ShiftFirstToOriginCell(positionPair, ModelProject.GeometryNumeric.ComparisonRange)
                .ToList();

            var operation = ModelProject.SpaceGroupService
                .GetOperationToTarget(invertedPair[0], pairInteraction.Position1.Vector);

            targetInfo.Distance = pairInteraction.Distance;
            targetInfo.CellSite = pairInteraction.Position0;
            targetInfo.AbsoluteFractional = operation.Transform(invertedPair[1]);
            targetInfo.RelativeFractional = targetInfo.AbsoluteFractional - pairInteraction.Position1.Vector;
            targetInfo.AbsoluteCartesian = VectorEncoder.Transformer.ToCartesian(targetInfo.AbsoluteFractional);

            if (!VectorEncoder.TryEncodeAsRelative(pairInteraction.Position1.Vector, targetInfo.RelativeFractional, out var relative4D))
                throw new InvalidOperationException("Could not create valid relative 4D interaction target");

            targetInfo.RelativeCrystalVector = relative4D;
        }

        /// <summary>
        ///     Takes a single reference pair interaction model an creates the extended set of all equivalent interaction models
        ///     including the original using the provided multiplicity operations
        /// </summary>
        /// <param name="pairInteractionModel"></param>
        /// <param name="multiplicityOperations"></param>
        /// <returns></returns>
        protected IEnumerable<IPairInteractionModel> ExtendPairInteractionModel(IPairInteractionModel pairInteractionModel,
            IList<ISymmetryOperation> multiplicityOperations)
        {
            var absoluteVectors = new SetList<Fractional3D>(ModelProject.SpaceGroupService.Comparer, multiplicityOperations.Count);
            foreach (var operation in multiplicityOperations)
            {
                var vector = operation.Transform(pairInteractionModel.TargetPositionInfo.AbsoluteFractional);
                absoluteVectors.Add(vector);
            }

            var extended = absoluteVectors
                .Select(vector => CreateEquivalentModelByVector(pairInteractionModel, vector))
                .ToList();

            return extended.Action(model => model.EquivalentModels = extended);
        }

        /// <summary>
        ///     Creates a new pair interaction model from an original pair interaction model and a new absolute 3D target vector
        /// </summary>
        /// <param name="originalModel"></param>
        /// <param name="absoluteVector"></param>
        /// <returns></returns>
        protected IPairInteractionModel CreateEquivalentModelByVector(IPairInteractionModel originalModel, in Fractional3D absoluteVector)
        {
            var startVector = originalModel.EnvironmentModel.CellSite.Vector;
            var relativeVector = absoluteVector - startVector;
            var absoluteCartesian = VectorEncoder.Transformer.ToCartesian(absoluteVector);

            if (!VectorEncoder.TryEncodeAsRelative(startVector, relativeVector, out var relativeVector4D))
                throw new InvalidOperationException("Could not create valid relative 4D interaction target");

            var result = new PairInteractionModel
            {
                PairEnergyModel = originalModel.PairEnergyModel,
                EnvironmentModel = originalModel.EnvironmentModel,
                TargetPositionInfo = new TargetPositionInfo
                {
                    AbsoluteFractional = absoluteVector,
                    AbsoluteCartesian = absoluteCartesian,
                    RelativeFractional = relativeVector,
                    RelativeCrystalVector = relativeVector4D,
                    CellSite = originalModel.TargetPositionInfo.CellSite,
                    Distance = originalModel.TargetPositionInfo.Distance
                }
            };

            result.TargetPositionInfo.PairInteractionModel = result;

            return result;
        }

        /// <summary>
        ///     Creates and adds all existing group interaction models to the environment model
        /// </summary>
        /// <param name="environmentModel"></param>
        protected void AddGroupInteractionModels(IEnvironmentModel environmentModel)
        {
            if (!PositionGroupInteractions.TryGetValue(environmentModel.CellSite, out var groupInteractions))
                groupInteractions = new List<IGroupInteraction>();

            var index = 0;
            var groupModels = groupInteractions
                .Select(a => CreateGroupInteractionModel(a, environmentModel))
                .SelectMany(ExtendGroupInteractionModel)
                .Action(a => a.ModelId = index++)
                .ToList();

            environmentModel.GroupInteractionModels = groupModels;
        }

        /// <summary>
        ///     Creates a group interaction model for the passed group interaction that is linked to its parent environment model
        /// </summary>
        /// <param name="groupInteraction"></param>
        /// <param name="environmentModel"></param>
        /// <returns></returns>
        protected IGroupInteractionModel CreateGroupInteractionModel(IGroupInteraction groupInteraction, IEnvironmentModel environmentModel)
        {
            var groupModel = new GroupInteractionModel
            {
                EnvironmentModel = environmentModel,
                GroupEnergyModel = new GroupEnergyModel(groupInteraction),
                PositionGroupInfo = PositionGroupInfos.Single(a => a.GroupInteraction == groupInteraction)
            };

            return groupModel;
        }

        /// <summary>
        ///     Extend the passed group interaction model into its set of symmetry equivalents and looks up the required pair
        ///     interaction information
        /// </summary>
        /// <param name="groupModel"></param>
        /// <returns></returns>
        protected IEnumerable<IGroupInteractionModel> ExtendGroupInteractionModel(IGroupInteractionModel groupModel)
        {
            var comparer = ModelProject.SpaceGroupService.Comparer;
            var groupVectors = groupModel.PositionGroupInfo.PointOperationGroup.GetUniqueSequencesWithoutPreservedPointOrder();

            var pairInteractions = groupVectors
                .Select(vectorSet => vectorSet
                    .Select(vector => groupModel.EnvironmentModel.PairInteractionModels
                        .Single(a => comparer.Compare(a.TargetPositionInfo.AbsoluteFractional, vector) == 0))
                    .ToList())
                .ToList();

            var groupModels = pairInteractions.Select(a => CreateExtendedGroupModel(groupModel, a)).ToList();
            return groupModels.Action(a => a.EquivalentModels = groupModels);
        }

        /// <summary>
        ///     Creates an extended group interaction model from the original model and the passed list of pair interaction models
        /// </summary>
        /// <param name="originalModel"></param>
        /// <param name="pairModels"></param>
        /// <returns></returns>
        protected IGroupInteractionModel CreateExtendedGroupModel(IGroupInteractionModel originalModel,
            IList<IPairInteractionModel> pairModels)
        {
            var groupModel = new GroupInteractionModel
            {
                EnvironmentModel = originalModel.EnvironmentModel,
                PairInteractionModels = pairModels,
                GroupEnergyModel = originalModel.GroupEnergyModel,
                PositionGroupInfo = originalModel.PositionGroupInfo,
                PairIndexCoding = CreatePairIndexCoding(pairModels)
            };
            return groupModel;
        }

        /// <summary>
        ///     Creates a pair index coding for a group interaction model
        /// </summary>
        /// <param name="pairModels"></param>
        /// <returns></returns>
        protected int[] CreatePairIndexCoding(IList<IPairInteractionModel> pairModels)
        {
            var result = new int[8];
            for (var i = 0; i < 8; i++)
            {
                if (i < pairModels.Count)
                    result[i] = pairModels[i].ModelId;
                else
                    result[i] = -1;
            }

            return result;
        }
    }
}