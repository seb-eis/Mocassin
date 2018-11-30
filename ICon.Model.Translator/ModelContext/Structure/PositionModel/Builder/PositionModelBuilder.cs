using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IPositionModelBuilder" />
    public class PositionModelBuilder : ModelBuilderBase, IPositionModelBuilder
    {
        /// <summary>
        ///     The vector encoder for vector transformation
        /// </summary>
        protected IUnitCellVectorEncoder VectorEncoder { get; set; }

        /// <inheritdoc />
        public PositionModelBuilder(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <inheritdoc />
        public IList<IPositionModel> BuildModels(IList<IEnvironmentModel> environmentModels)
        {
            LoadBuildDataFromProject();
            return environmentModels
                .SelectMany(CreatePositionModels)
                .ToList();
        }

        /// <summary>
        ///     Loads all required reference build data from the current project
        /// </summary>
        protected void LoadBuildDataFromProject()
        {
            var manager = ModelProject.GetManager<IStructureManager>();
            VectorEncoder = manager.QueryPort.Query(port => port.GetVectorEncoder());
        }

        /// <summary>
        ///     Creates the set of position models that results from the passed environment model and the space group of the
        ///     project
        /// </summary>
        /// <param name="environmentModel"></param>
        /// <returns></returns>
        protected IEnumerable<IPositionModel> CreatePositionModels(IEnvironmentModel environmentModel)
        {
            var sourceVector = environmentModel.UnitCellPosition.Vector;
            var targetInfos = environmentModel.PairInteractionModels
                .Select(a => a.TargetPositionInfo)
                .ToList();

            var targetVectors = ModelProject.SpaceGroupService.GetAllWyckoffPositions(environmentModel.UnitCellPosition.Vector);
            var positionModels = targetVectors
                .Select(target => ModelProject.SpaceGroupService.CreateOperationToTarget(sourceVector, target))
                .Select(operation => CreatePositionModel(environmentModel, operation, targetInfos));

            return positionModels;
        }

        /// <summary>
        ///     Creates a position model from the passed environment model, pair interaction target info
        ///     and transform symmetry operation
        /// </summary>
        /// <param name="environmentModel"></param>
        /// <param name="operation"></param>
        /// <param name="targetInfos"></param>
        /// <returns></returns>
        protected IPositionModel CreatePositionModel(IEnvironmentModel environmentModel, ISymmetryOperation operation,
            IList<ITargetPositionInfo> targetInfos)
        {
            var positionModel = new PositionModel
            {
                UnitCellPosition = environmentModel.UnitCellPosition,
                EnvironmentModel = environmentModel,
                TransformOperation = operation,
                CenterVector = operation.ApplyUntrimmed(environmentModel.UnitCellPosition.Vector)
            };

            positionModel.TargetPositionInfos = TransformTargetInfos(targetInfos, operation, positionModel.CenterVector);

            return positionModel;
        }

        /// <summary>
        ///     Transforms the passed list of target infos to a new center using the provided symmetry operation
        /// </summary>
        /// <param name="targetInfos"></param>
        /// <param name="operation"></param>
        /// <param name="centerVector"></param>
        /// <returns></returns>
        protected IList<ITargetPositionInfo> TransformTargetInfos(IList<ITargetPositionInfo> targetInfos, ISymmetryOperation operation,
            in Fractional3D centerVector)
        {
            var result = CreateNewTargetInfoList(targetInfos.Count);
            foreach (var positionInfo in targetInfos)
            {
                var targetInfo = new TargetPositionInfo
                {
                    UnitCellPosition = positionInfo.UnitCellPosition,
                    Distance = positionInfo.Distance,
                    AbsoluteFractional3D = operation.ApplyUntrimmed(positionInfo.AbsoluteFractional3D)
                };

                targetInfo.RelativeFractional3D = targetInfo.AbsoluteFractional3D - centerVector;
                targetInfo.AbsoluteCartesian3D = VectorEncoder.Transformer.ToCartesian(targetInfo.AbsoluteFractional3D);

                if (!VectorEncoder.TryEncodeAsRelative(centerVector, targetInfo.RelativeFractional3D, out var relative4D))
                    throw new InvalidOperationException("Failed to encode target info into 4D relative vector");

                targetInfo.RelativeVector4D = relative4D;
                result.Add(targetInfo);
            }

            return result;
        }

        /// <summary>
        ///     Creates a new target info list that ensures the correct sorting of the target position infos by the relative 4D
        ///     vector
        /// </summary>
        /// <param name="capacity"></param>
        /// <returns></returns>
        protected SetList<ITargetPositionInfo> CreateNewTargetInfoList(int capacity)
        {
            int Compare(ITargetPositionInfo lhs, ITargetPositionInfo rhs)
            {
                return lhs.RelativeVector4D.CompareTo(rhs.RelativeVector4D);
            }

            return new SetList<ITargetPositionInfo>(Comparer<ITargetPositionInfo>.Create(Compare), capacity);
        }
    }
}