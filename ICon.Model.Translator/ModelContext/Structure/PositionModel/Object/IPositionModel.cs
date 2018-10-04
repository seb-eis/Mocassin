using System;
using System.Collections.Generic;
using System.Text;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Structures;
using ICon.Model.Transitions;
using ICon.Symmetry.SpaceGroups;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Describes the full model information for a single extended wyckoff position in the unit cell
    /// </summary>
    public interface IPositionModel : IModelComponent
    {
        /// <summary>
        /// The unit cell position that is valid on this position
        /// </summary>
        IUnitCellPosition UnitCellPosition { get; set; }

        /// <summary>
        /// The actual center vector of the position
        /// </summary>
        Fractional3D CenterVector { get; set; }

        /// <summary>
        /// The environment model that belongs to the position model
        /// </summary>
        IEnvironmentModel EnvironmentModel { get; set; }

        /// <summary>
        /// The position transition model that belongs to this position model
        /// </summary>
        IPositionTransitionModel PositionTransitionModel { get; set; }

        /// <summary>
        /// The transform operation that was used to transform the surrounding from the original unit cell position
        /// </summary>
        ISymmetryOperation TransformOperation { get; set; }

        /// <summary>
        /// The list of transformed target position infos of the environment in order of its pair interactions
        /// </summary>
        IList<ITargetPositionInfo> TargetPositionInfos { get; set; }
    }
}
