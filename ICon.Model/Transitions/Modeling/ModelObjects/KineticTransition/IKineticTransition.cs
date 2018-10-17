using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Structures;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    /// Represents an actual kinetic transition that includes a reference geometry and abstract description
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
        /// Get the affiliated transition rules of the transition
        /// </summary>
        /// <returns></returns>
        IEnumerable<IKineticRule> GetTransitionRules();

        /// <summary>
        /// Get the affiliated transition rules of the transition including dependent rules
        /// </summary>
        /// <returns></returns>
        IEnumerable<IKineticRule> GetExtendedTransitionRules();

        /// <summary>
        /// Get the geometry of the transition as a sequence of 3D fractional vectors
        /// </summary>
        /// <returns></returns>
        IEnumerable<Fractional3D> GetGeometrySequence();
    }
}
