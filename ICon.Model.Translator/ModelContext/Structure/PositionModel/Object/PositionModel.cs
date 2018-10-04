using ICon.Mathematics.ValueTypes;
using ICon.Model.Structures;
using ICon.Symmetry.SpaceGroups;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.IPositionModel"/>
    public class PositionModel : ModelComponentBase, IPositionModel
    {
        /// <inheritdoc />
        public IUnitCellPosition UnitCellPosition { get; set; }

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
