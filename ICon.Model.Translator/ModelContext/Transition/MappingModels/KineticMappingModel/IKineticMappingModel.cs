using System;
using System.Collections.Generic;
using System.Text;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Particles;
using ICon.Model.Transitions;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a kinetic mapping model that fully describes a transitions geometric properties on a specific position
    /// </summary>
    public interface IKineticMappingModel
    {
        /// <summary>
        /// The kinetic mapping object the model is based upon
        /// </summary>
        KineticMapping Mapping { get; set; }

        /// <summary>
        /// The inverse mapping model that describes the neutralizing transition
        /// </summary>
        IKineticMappingModel InverseMapping { get; set; }

        /// <summary>
        /// The encoded 4D transition sequence where each vector is relative to the start position
        /// </summary>
        IList<CrystalVector4D> TransitionSequence4D { get; set; }

        /// <summary>
        /// The fractional 3D transition sequence where each vector is relative to the start position
        /// </summary>
        IList<Fractional3D> TransitionSequence3D { get; set; }

        /// <summary>
        /// The step weighting vectors that describe the field weighting for (A,B,C) direction of each transition step
        /// </summary>
        IList<Fractional3D> StepWeightingVectors { get; set; }

        /// <summary>
        /// The effective transition vector from start to end. Used for global movement tracking
        /// </summary>
        Fractional3D EffectiveTransitionVector { get; set; }

        /// <summary>
        /// The position movement matrix. Describes how each involved position moves on transition in fractional coordinates
        /// </summary>
        Matrix2D PositionMovementMatrix { get; set; }

        
    }
}
