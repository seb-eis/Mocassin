using ICon.Mathematics.ValueTypes;
using ICon.Model.Structures;
using ICon.Symmetry.SpaceGroups;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Implementation of a position model that fully describes the behavior of a single extended wyckoff position in the model context
    /// </summary>
    public class PositionModel : ModelComponentBase, IPositionModel
    {
        /// <summary>
        /// The unit cell position that is valid on this position
        /// </summary>
        public IUnitCellPosition UnitCellPosition { get; set; }

        /// <summary>
        /// The actual center vector of the position
        /// </summary>
        public Fractional3D CenterVector { get; set; }

        /// <summary>
        /// The environment model that belongs to the position model
        /// </summary>
        public IEnvironmentModel EnvironmentModel { get; set; }

        /// <summary>
        /// The position transition model that belongs to this position model
        /// </summary>
        public IPositionTransitionModel PositionTransitionModel { get; set; }

        /// <summary>
        /// The transform operation that was used to transfrom the surrounding from the original unit cell position
        /// </summary>
        public ISymmetryOperation TransformOperation { get; set; }

        /// <summary>
        /// The list of relative surrounding positions as 3D fractional vectors
        /// </summary>
        public IList<Fractional3D> RelativeSurroundingVectors3D { get; set; }

        /// <summary>
        /// The list of relative surrounding positions as 4D encoded vectors
        /// </summary>
        public IList<CrystalVector4D> RelativeSurroundingVectors4D { get; set; }
    }
}
