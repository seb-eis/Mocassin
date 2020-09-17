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
            var positionModels = environmentModels
                                 .SelectMany(CreatePositionModels)
                                 .ToList();

            IndexAndSortPositionModels(positionModels);
            return positionModels;
        }

        /// <summary>
        ///     Loads all required reference build data from the current project
        /// </summary>
        protected void LoadBuildDataFromProject()
        {
            var manager = ModelProject.Manager<IStructureManager>();
            VectorEncoder = manager.DataAccess.Query(port => port.GetVectorEncoder());
        }

        /// <summary>
        ///     Synchronizes the <see cref="IPositionModel" /> list indexing and sorting with the extended wyckoff position list
        /// </summary>
        /// <param name="positionModels"></param>
        protected void IndexAndSortPositionModels(List<IPositionModel> positionModels)
        {
            foreach (var positionModel in positionModels)
                positionModel.ModelId = VectorEncoder.PositionList.IndexOf(positionModel.CenterVector);

            positionModels.Sort(Comparer<IPositionModel>.Create((a, b) => a.ModelId.CompareTo(b.ModelId)));
        }

        /// <summary>
        ///     Creates the set of position models that results from the passed environment model and the space group of the
        ///     project
        /// </summary>
        /// <param name="environmentModel"></param>
        /// <returns></returns>
        protected IEnumerable<IPositionModel> CreatePositionModels(IEnvironmentModel environmentModel)
        {
            var sourceVector = environmentModel.CellSite.Vector;
            var targetInfos = environmentModel.PairInteractionModels
                                              .Select(a => a.TargetPositionInfo)
                                              .ToList();

            var targetVectors = ModelProject.SpaceGroupService.GetUnitCellP1PositionExtension(environmentModel.CellSite.Vector);
            var positionModels = targetVectors
                                 .Select(target => ModelProject.SpaceGroupService.GetOperationToTarget(sourceVector, target))
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
                CellSite = environmentModel.CellSite,
                EnvironmentModel = environmentModel,
                TransformOperation = operation,
                CenterVector = operation.Transform(environmentModel.CellSite.Vector)
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
                    CellSite = positionInfo.CellSite,
                    Distance = positionInfo.Distance,
                    AbsoluteFractional = operation.Transform(positionInfo.AbsoluteFractional),
                    PairInteractionModel = positionInfo.PairInteractionModel
                };

                targetInfo.RelativeFractional = targetInfo.AbsoluteFractional - centerVector;
                targetInfo.AbsoluteCartesian = VectorEncoder.Transformer.ToCartesian(targetInfo.AbsoluteFractional);

                if (!VectorEncoder.TryEncodeAsRelative(centerVector, targetInfo.RelativeFractional, out var relative4D))
                    throw new InvalidOperationException("Failed to encode target info into 4D relative vector");

                targetInfo.RelativeCrystalVector = relative4D;
                result.Add(targetInfo);
            }

            return result;
        }

        /// <summary>
        ///     Creates a new <see cref="ITargetPositionInfo" /> list that ensures the correct sorting
        /// </summary>
        /// <param name="capacity"></param>
        /// <returns></returns>
        protected SetList<ITargetPositionInfo> CreateNewTargetInfoList(int capacity)
        {
            int Compare(ITargetPositionInfo lhs, ITargetPositionInfo rhs) => lhs.PairInteractionModel.ModelId.CompareTo(rhs.PairInteractionModel.ModelId);
            //return lhs.RelativeVector4D.CompareTo(rhs.RelativeVector4D);
            return new SetList<ITargetPositionInfo>(Comparer<ITargetPositionInfo>.Create(Compare), capacity);
        }
    }
}