using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Represents a metropolis transition rule that describes a static state exchange
    /// </summary>
    public interface IMetropolisRule : ITransitionRule
    {
        /// <summary>
        /// Get the metropolis parent transition
        /// </summary>
        IMetropolisTransition Transition { get; }
    }
}
