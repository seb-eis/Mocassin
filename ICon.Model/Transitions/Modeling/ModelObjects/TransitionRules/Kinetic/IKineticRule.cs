using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Represents a kinetic transition rule that describes a dynamic state change process and allows for setting of boundary flags and attempt frequency
    /// </summary>
    public interface IKineticRule : ITransitionRule
    {
        /// <summary>
        /// Get the parent kinetic transition
        /// </summary>
        IKineticTransition Transition { get; }

        /// <summary>
        /// The cell boundary flags of the rule
        /// </summary>
        CellBoundaryFlags BoundaryFlags { get; }

        /// <summary>
        /// The attempt frequency of this rule
        /// </summary>
        double AttemptFrequency { get; }

        /// <summary>
        /// Set the cell boundary flags of the rule
        /// </summary>
        /// <param name="flags"></param>
        void SetCellBoundaryFlags(CellBoundaryFlags flags);

        /// <summary>
        /// Set the attempt frequency of the rule
        /// </summary>
        /// <param name="value"></param>
        void SetAttemptFrequency(double value);
    }
}
