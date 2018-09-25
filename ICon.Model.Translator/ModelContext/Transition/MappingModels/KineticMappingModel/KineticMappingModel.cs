using ICon.Mathematics.ValueTypes;
using ICon.Model.Transitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class KineticMappingModel : IKineticMappingModel
    {
        /// <inheritdoc />
        public KineticMapping Mapping { get; set; }

        /// <inheritdoc />
        public IKineticMappingModel InverseMapping { get; set; }

        /// <inheritdoc />
        public IList<CrystalVector4D> TransitionSequence4D { get; set; }

        /// <inheritdoc />
        public IList<Fractional3D> TransitionSequence3D { get; set; }

        /// <inheritdoc />
        public IList<Fractional3D> StepWeightingVectors { get; set; }

        /// <inheritdoc />
        public Fractional3D EffectiveTransitionVector { get; set; }

        /// <inheritdoc />
        public Matrix2D PositionMovementMatrix { get; set; }
    }
}
