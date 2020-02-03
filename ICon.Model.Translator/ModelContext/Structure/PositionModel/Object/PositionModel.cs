using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IPositionModel" />
    public class PositionModel : ModelComponentBase, IPositionModel
    {
        /// <inheritdoc />
        public ICellReferencePosition CellReferencePosition { get; set; }

        /// <inheritdoc />
        public Fractional3D CenterVector { get; set; }

        /// <inheritdoc />
        public IEnvironmentModel EnvironmentModel { get; set; }

        /// <inheritdoc />
        public IPositionTransitionModel PositionTransitionModel { get; set; }

        /// <inheritdoc />
        public ISymmetryOperation TransformOperation { get; set; }

        /// <inheritdoc />
        public IList<ITargetPositionInfo> TargetPositionInfos { get; set; }
    }
}