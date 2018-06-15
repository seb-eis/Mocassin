using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;
using ICon.Model.Structures;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Represents an actual kinetic transition that includes a refernce geometry and abstract description
    /// </summary>
    public interface IKineticTransition : IModelObject
    {
        /// <summary>
        /// The number of geometry steps
        /// </summary>
        int GeometryStepCount { get; }

        /// <summary>
        /// The abstract transition that describes the general transition behavior
        /// </summary>
        IAbstractTransition AbstractTransition { get; }

        /// <summary>
        /// Get the geometry of the transition as a sequence of 3D fractional vectors
        /// </summary>
        /// <returns></returns>
        IEnumerable<Fractional3D> GetGeometrySequence();
    }
}
