using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.ValueTypes;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Mapping for a kinetic transition that describes a 4D encoded transition path belonging to a specific kinetic reference transition
    /// </summary>
    public readonly struct KineticMapping
    {
        /// <summary>
        /// The interface of the transition the mapping is valid for
        /// </summary>
        public IKineticTransition Transition { get; }

        /// <summary>
        /// The transition path encoded as a set of 4D vectors (Cannot be null)
        /// </summary>
        public CrystalVector4D[] EncodedPath { get; }

        /// <summary>
        /// The transition path in fractional coordinates (Can be null)
        /// </summary>
        public Fractional3D[] FractionalPath { get; }

        /// <summary>
        /// Create a new kinetic mapping from transition interface, 4D encoded transition path and fractional path
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="encodedPath"></param>
        public KineticMapping(IKineticTransition transition, CrystalVector4D[] encodedPath, Fractional3D[] fractionalPath) : this()
        {
            Transition = transition;
            EncodedPath = encodedPath ?? throw new ArgumentNullException(nameof(encodedPath));
            FractionalPath = fractionalPath;
        }

        /// <summary>
        /// Create a new kinetic mapping from transition, 4D encoded transition path (The fractional path is set to null)
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="encodedPath"></param>
        public KineticMapping(IKineticTransition transition, CrystalVector4D[] encodedPath) : this(transition, encodedPath, null)
        {

        }
    }
}
